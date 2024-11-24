using compiler;
using compiler.compiler;
using compiler.compiler.compiler;

class Program
{
    async static Task Main(string[] args)
    {
        var originalOut = Console.Out;  // Сохраняем стандартный вывод
        var stringWriter = new StringWriter();
       

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
        Console.WriteLine("\nкод программы C#:\n");
        new SemanticAnalyzer().Analyze(tree);
        var gener = new Translator().Translate(tree).ToString();
        Console.WriteLine(gener);
       // Console.WriteLine("\nВведите название файла в который хотите сохранить скомпилированный код:\n");
      //  var compileFileName = Console.ReadLine();
        Console.WriteLine("\nход работы прогрраммы C#:\n");
       CodeRunner.RunCode(gener, null);
    }
}
