using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler
{
    using System;
    using System.Collections.Generic;

    public class Symbol
    {
        public Token Token { get; }
        public NonTerminal? NonTerminal { get; }

        // Конструктор для терминала (токена)
        public Symbol(Token token)
        {
            Token = token;
            NonTerminal = null;
        }

        // Конструктор для нетерминала
        public Symbol(NonTerminal nonTerminal)
        {
            Token = null;
            NonTerminal = nonTerminal;
        }

        // Метод для получения значения символа (токена или нетерминала)
        public string GetValue()
        {
            return Token != null ? ( Token.Value + " " + Token.Type ) :( NonTerminal.ToString());
        }

        public bool IsNonTerminal() { return NonTerminal != null; }
    }

    public enum NonTerminal
    {
        START_FILE,
        Программа,
        Объявление_переменных,
        Список_переменных,
        Доп_переменные,
        Описание_вычислений,
        Список_операторов,
        Список_операторов_хвост,
        Оператор,
        Присваивание,
        Выражение,
        Подвыражение,
        Подвыражение_хвост,
        Ун_оп,
        Бин_оп,
        Операнд,
        Идент,
        Конст,
        Чтение,
        Запись,
        Список_идентификаторов,
        Доп_идентификаторы,
        Цикл
    }

    public class SyntaxTreeNode
    {
        public Symbol Symbol { get; }
        public List<SyntaxTreeNode> Children { get; }

        // Конструктор, принимающий объект Symbol (терминал или нетерминал)
        public SyntaxTreeNode(Symbol symbol)
        {
            Symbol = symbol;
            Children = new List<SyntaxTreeNode>();
        }

        public void AddChild(SyntaxTreeNode child)
        {
            Children.Add(child);
        }

        public void PrintTree(int indent = 0)
        {
            string indentString = new string('\t', indent);
            Console.WriteLine($"{indentString}- {Symbol.GetValue()}");

            foreach (var child in Children)
            {
                child.PrintTree(indent + 1);
            }
        }

        public Boolean IsIdent()
        {
            return Symbol.Token != null && Symbol.Token.Type == TokenType.IDENT;
        }
    }


    internal class Syntacsys
    {
        private readonly List<Token> tokens;
        private int currentPosition;
        private Token CurrentToken => tokens[currentPosition];

        public Syntacsys(List<Token> tokens)
        {
            this.tokens = tokens;
            currentPosition = 0;
        }

        private void Advance()
        {
            if (currentPosition < tokens.Count - 1)
                currentPosition++;
        }

        public SyntaxTreeNode ParseProgram()
        {
            // Создаем корневой узел для дерева с нетерминалом "Программа"
            SyntaxTreeNode root = new SyntaxTreeNode(new Symbol(NonTerminal.START_FILE));
            Parse(NonTerminal.START_FILE, root);
            return root;
        }

        private void Parse(NonTerminal nonTerminal, SyntaxTreeNode root)
        {
            // Получаем продукцию из таблицы предсказаний
            if (predictionTable.TryGetValue((nonTerminal, CurrentToken.Type), out string production))
            {
                // Разбиваем строку продукции на отдельные символы
                var symbols = production.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                SyntaxTreeNode parentNode = new SyntaxTreeNode(new Symbol(nonTerminal));
                root.AddChild(parentNode);

                foreach (var symbol in symbols)
                {
                    if (Enum.TryParse(symbol, out NonTerminal parsedNonTerminal)) // Проверка, является ли символ нетерминалом
                    {
                        // Рекурсивный вызов Parse для нетерминала
                        Parse(parsedNonTerminal, parentNode);
                    }
                    else if (symbol == "ε") // Пустое правило
                    {
                        continue;
                    }
                    else // Терминал
                    {
                        // Создаем новый узел дерева для терминала и добавляем его
                        SyntaxTreeNode terminalNode = new SyntaxTreeNode(new Symbol(CurrentToken));
                        parentNode.AddChild(terminalNode);
                        Advance(); // Переходим к следующему токену
                    }
                }
            }
            else
            {
                root.PrintTree();
                throw new Exception($"Синтаксическая ошибка: текущий токен '{CurrentToken}' не соответствует ни одному правилу из '{nonTerminal}'");
            }
        }

        private static Dictionary<(NonTerminal, TokenType), string> predictionTable = new Dictionary<(NonTerminal, TokenType), string>
{
            
    { (NonTerminal.START_FILE, TokenType.VAR), "Программа" },

    // Программа

    { (NonTerminal.Программа, TokenType.VAR), "Объявление_переменных Описание_вычислений" },

    // Объявление переменных
    { (NonTerminal.Объявление_переменных, TokenType.VAR), "VAR Список_переменных : INTEGER ;" },

    // Список переменных
    { (NonTerminal.Список_переменных, TokenType.IDENT), "Идент Доп_переменные" },

    // Дополнительные переменные
    { (NonTerminal.Доп_переменные, TokenType.COMMA), ", Список_переменных" },
    { (NonTerminal.Доп_переменные, TokenType.COLON), "ε" }, // конец списка

    // Описание вычислений
    { (NonTerminal.Описание_вычислений, TokenType.BEGIN), "BEGIN Список_операторов END" },

    // Список операторов
    { (NonTerminal.Список_операторов, TokenType.IDENT), "Оператор Список_операторов_хвост" },
    { (NonTerminal.Список_операторов, TokenType.READ), "Оператор Список_операторов_хвост" },
    { (NonTerminal.Список_операторов, TokenType.WRITE), "Оператор Список_операторов_хвост" },
    { (NonTerminal.Список_операторов, TokenType.FOR), "Оператор Список_операторов_хвост" },
    { (NonTerminal.Список_операторов, TokenType.END), "ε" },
    { (NonTerminal.Список_операторов, TokenType.END_FOR), "ε" },

    // Список операторов хвост
    { (NonTerminal.Список_операторов_хвост, TokenType.SEMICOLON), "; Список_операторов" },
    { (NonTerminal.Список_операторов_хвост, TokenType.END), "ε" },
    { (NonTerminal.Список_операторов_хвост, TokenType.END_FOR), "ε" },
    { (NonTerminal.Список_операторов_хвост, TokenType.CONST), "Конст" },

    // Оператор
    { (NonTerminal.Оператор, TokenType.IDENT), "Присваивание" },
    { (NonTerminal.Оператор, TokenType.READ), "Чтение" },
    { (NonTerminal.Оператор, TokenType.WRITE), "Запись" },
    { (NonTerminal.Оператор, TokenType.FOR), "Цикл" },

    // Присваивание
    { (NonTerminal.Присваивание, TokenType.IDENT), "Идент = Выражение" },

    // Выражение
    { (NonTerminal.Выражение, TokenType.MINUS), "Ун_оп Подвыражение" },
    { (NonTerminal.Выражение, TokenType.LPAREN), "Подвыражение" },
    { (NonTerminal.Выражение, TokenType.IDENT), "Подвыражение" },
    { (NonTerminal.Выражение, TokenType.CONST), "Подвыражение" },

    // Подвыражение
    { (NonTerminal.Подвыражение, TokenType.LPAREN), "( Выражение ) Подвыражение_хвост" },
    { (NonTerminal.Подвыражение, TokenType.IDENT), "Операнд Подвыражение_хвост" },
    { (NonTerminal.Подвыражение, TokenType.CONST), "Операнд Подвыражение_хвост" },

    // Подвыражение хвост
    { (NonTerminal.Подвыражение_хвост, TokenType.PLUS), "Бин_оп Подвыражение" },
    { (NonTerminal.Подвыражение_хвост, TokenType.MINUS), "Бин_оп Подвыражение" },
    { (NonTerminal.Подвыражение_хвост, TokenType.MULT), "Бин_оп Подвыражение" },
    { (NonTerminal.Подвыражение_хвост, TokenType.SEMICOLON), "ε" },
    { (NonTerminal.Подвыражение_хвост, TokenType.RPAREN), "ε" },
    { (NonTerminal.Подвыражение_хвост, TokenType.TO), "ε" },
    { (NonTerminal.Подвыражение_хвост, TokenType.DO), "ε" },

    // Ун_оп
    { (NonTerminal.Ун_оп, TokenType.MINUS), "-" },

    // Бин_оп
    { (NonTerminal.Бин_оп, TokenType.PLUS), "+" },
    { (NonTerminal.Бин_оп, TokenType.MINUS), "-" },
    { (NonTerminal.Бин_оп, TokenType.MULT), "*" },

    // Операнд
    { (NonTerminal.Операнд, TokenType.IDENT), "Идент" },
    { (NonTerminal.Операнд, TokenType.CONST), "Конст" },

    // Идент
    { (NonTerminal.Идент, TokenType.IDENT), "Идентификатор" },

    // Конст
    { (NonTerminal.Конст, TokenType.CONST), "Константа" },

    // Чтение
    { (NonTerminal.Чтение, TokenType.READ), "READ ( Список_идентификаторов )" },

    // Запись
    { (NonTerminal.Запись, TokenType.WRITE), "WRITE ( Список_идентификаторов )" },

    // Список идентификаторов
    { (NonTerminal.Список_идентификаторов, TokenType.IDENT), "Идент Доп_идентификаторы" },

    // Дополнительные идентификаторы
    { (NonTerminal.Доп_идентификаторы, TokenType.COMMA), ", Список_идентификаторов" },
    { (NonTerminal.Доп_идентификаторы, TokenType.RPAREN), "ε" },

    // Цикл
    { (NonTerminal.Цикл, TokenType.FOR), "FOR Идент = Выражение TO Выражение DO Список_операторов ENDFOR" },
};
    }
}