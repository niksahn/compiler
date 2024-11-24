using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace compiler
{
    public class CodeRunner
    {
        public static void SaveAssemblyToFile(string filePath, byte[] assemblyBytes)
        {
            try
            {
                File.WriteAllBytes(filePath, assemblyBytes);
                Console.WriteLine($"Сборка успешно сохранена в файл: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении сборки: {ex.Message}");
            }
        }

        public static void RunCode(string code, string? fileName)
        {
            // Создаем синтаксическое дерево из кода
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

            // Получаем ссылки на необходимые сборки
            var references = new List<MetadataReference>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var location = assembly.Location;
                if (!string.IsNullOrEmpty(location))
                {
                    references.Add(MetadataReference.CreateFromFile(location));
                }
            }

            // Настраиваем параметры компиляции
            var compilationOptions = new CSharpCompilationOptions(OutputKind.ConsoleApplication)
            .WithPlatform(Platform.AnyCpu)
            .WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic>
            {
                ["CS1701"] = ReportDiagnostic.Suppress // Подавить предупреждения о несовместимых версиях
            }
            );

            var compilation = CSharpCompilation.Create(
            assemblyName: Path.GetRandomFileName(),
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: compilationOptions
            );
            using (var ms = new MemoryStream())
            {
                // Компилируем код в сборку в памяти
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    // Если есть ошибки компиляции, выводим их
                    var failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                    Console.WriteLine("Ошибки компиляции:");

                    foreach (var diagnostic in failures)
                    {
                        Console.Error.WriteLine(diagnostic.ToString());
                    }
                }
                else
                {
                    // Если компиляция успешна, сохраняем сборку в файл
                    ms.Seek(0, SeekOrigin.Begin);
                    if(fileName!=null) SaveAssemblyToFile(fileName, ms.ToArray());

                    Assembly assembly = Assembly.Load(ms.ToArray());

                    // Перенаправляем Console.Out и Console.In, если необходимо
                    var originalOut = Console.Out;
                    var originalIn = Console.In;

                    try
                    {
                        // Захватываем вывод в StringWriter
                        using (var writer = new StringWriter())
                        {
                            Console.SetOut(writer);
                            // Ищем тип Program и метод Main
                            var type = assembly.GetType("Program");
                            var method = type.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);

                            // Если метод существует, вызываем его
                            if (method != null)
                            {
                                method.Invoke(null, null);
                            }

                            // Получаем вывод
                            string output = writer.ToString();
                            Console.SetOut(originalOut);
                            Console.WriteLine(output);
                        }
                    }
                    finally
                    {
                        // Восстанавливаем потоки Console
                        Console.SetOut(originalOut);
                        Console.SetIn(originalIn);
                    }

                }
            }
        }
    }
}
