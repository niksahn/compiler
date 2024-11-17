using compiler;
using compiler.compiler;
using compiler.compiler.compiler;

using System;
using System.IO;

class Program
{

    static void Main(string[] args)
    {
        string input = "";

        if (args.Length > 0)
        {
            // Проверяем, указан ли файл в аргументах
            string fileName = args[0];

            if (File.Exists(fileName))
            {
                // Читаем содержимое файла, если он существует
                input = File.ReadAllText(fileName);
                Console.WriteLine($"Содержимое файла '{fileName}' загружено.");
            }
            else
            {
                Console.WriteLine($"Файл '{fileName}' не найден.");
                return;
            }
        }
        else
        {
            // Если файл не указан, запрашиваем ввод у пользователя
            Console.WriteLine("Введите код программы:");

            string cur = "";
            while (cur != "END")
            {
                cur = Console.ReadLine();
                input += "\n" + cur;
            }
        }

        Lexer lexer = new Lexer(input);

        var tokens = lexer.GetTokens();
        var synt = new Syntacsys(tokens);
        var tree = synt.ParseProgram();
        Console.WriteLine("\nкод прогрраммы C#:\n");
        new SemanticAnalyzer().Analyze(tree);
        var gener = new Translator().Translate(tree);
        Console.WriteLine(gener.ToString());
    }
}
