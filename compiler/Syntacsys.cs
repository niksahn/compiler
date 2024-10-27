using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler
{
    public class SyntaxTreeNode
    {
        public Token Token { get; }
        public List<SyntaxTreeNode> Children { get; }

        public SyntaxTreeNode(Token token)
        {
            Token = token;
            Children = new List<SyntaxTreeNode>();
        }

        public void AddChild(SyntaxTreeNode child)
        {
            Children.Add(child);
        }

        public void PrintTree(int indent = 0)
        {
            // Generate a string of tabs for indentation
            string indentString = new string('\t', indent);

            // Print the current node's token
            Console.WriteLine($"{indentString}- {Token.Value}");

            // Recursively print each child node, increasing the indent level
            foreach (var child in Children)
            {
                child.PrintTree(indent + 1);
            }
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
            SyntaxTreeNode root = new SyntaxTreeNode(new Token(TokenType.VAR, "E")); // Корень дерева
            Parse("<Программа>", root);
            return root;
        }

        private void Parse(string nonTerminal, SyntaxTreeNode root)
        {
            if (predictionTable.TryGetValue((nonTerminal, CurrentToken.Type), out string production))
            {
                var symbols = production.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                SyntaxTreeNode parentNode = new SyntaxTreeNode(new Token(CurrentToken.Type, nonTerminal));
                root.AddChild(parentNode);

                foreach (var symbol in symbols)
                {
                    if (symbol.StartsWith("<") && symbol.EndsWith(">")) // Нетерминал
                    {
                        Parse(symbol, parentNode); // Изменён корень на parentNode
                    }
                    else if (symbol == "ε") // Пустое правило
                    {
                        continue;
                    }
                    else // Терминал
                    {
                           // Console.WriteLine(symbol);
                            parentNode.AddChild(new SyntaxTreeNode(CurrentToken)); // Изменён корень на parentNode
                            Advance();     
                    }
                }
            }
            else
            {
                root.PrintTree();
                throw new Exception($"Синтаксическая ошибка: текущий токен '{CurrentToken}' не соответствует ни одному правилу из '{nonTerminal}'");
               
            }
        }

        private static Dictionary<(string, TokenType), string> predictionTable = new Dictionary<(string, TokenType), string>
        {
            // Программа
            { ("<Программа>", TokenType.VAR), "<Объявление_переменных> <Описание_вычислений>" },

            // Объявление переменных  
            { ("<Объявление_переменных>", TokenType.VAR), "VAR <Список_переменных> : INTEGER;" },

            // Список переменных
            { ("<Список_переменных>", TokenType.IDENT), "<Идент> <Доп_переменные>" },

            // Дополнительные переменные
            { ("<Доп_переменные>", TokenType.COMMA), ", <Список_переменных>" },
            { ("<Доп_переменные>", TokenType.COLON), "ε" }, // конец списка

            // Описание вычислений
            { ("<Описание_вычислений>", TokenType.BEGIN), "BEGIN <Список_операторов> END" },

            // Список операторов
            { ("<Список_операторов>", TokenType.IDENT), "<Оператор> <Список_операторов_хвост>" },
            { ("<Список_операторов>", TokenType.READ), "<Оператор> <Список_операторов_хвост>" },
            { ("<Список_операторов>", TokenType.WRITE), "<Оператор> <Список_операторов_хвост>" },
            { ("<Список_операторов>", TokenType.FOR), "<Оператор> <Список_операторов_хвост>" },
            { ("<Список_операторов>", TokenType.END), "ε" },
            { ("<Список_операторов>", TokenType.END_FOR), "ε" },

            // Список операторов хвост
            { ("<Список_операторов_хвост>", TokenType.SEMICOLON), "; <Список_операторов>" },
            { ("<Список_операторов_хвост>", TokenType.END), "ε" },
            { ("<Список_операторов_хвост>", TokenType.END_FOR), "ε" },


            // Оператор
            { ("<Оператор>", TokenType.IDENT), "<Присваивание>" },
            { ("<Оператор>", TokenType.READ), "<Чтение>" },
            { ("<Оператор>", TokenType.WRITE), "<Запись>" },
            { ("<Оператор>", TokenType.FOR), "<Цикл>" },

            // Присваивание
            { ("<Присваивание>", TokenType.IDENT), "<Идент> = <Выражение>" },

            // Выражение
            { ("<Выражение>", TokenType.MINUS), "<Ун_оп> <Подвыражение>" },
            { ("<Выражение>", TokenType.LPAREN), "<Подвыражение>" },
            { ("<Выражение>", TokenType.IDENT), "<Подвыражение>" },
            { ("<Выражение>", TokenType.CONST), "<Подвыражение>" },

            // Подвыражение
            { ("<Подвыражение>", TokenType.LPAREN), "( <Выражение> )" },
            { ("<Подвыражение>", TokenType.IDENT), "<Операнд> <Подвыражение_хвост>" },
            { ("<Подвыражение>", TokenType.CONST), "<Операнд> <Подвыражение_хвост>" },

            // Подвыражение хвост
            { ("<Подвыражение_хвост>", TokenType.PLUS), "<Бин_оп> <Операнд> <Подвыражение_хвост>" },
            { ("<Подвыражение_хвост>", TokenType.MINUS), "<Бин_оп> <Операнд> <Подвыражение_хвост>" },
            { ("<Подвыражение_хвост>", TokenType.MULT), "<Бин_оп> <Операнд> <Подвыражение_хвост>" },
            { ("<Подвыражение_хвост>", TokenType.SEMICOLON), "ε" },
            { ("<Подвыражение_хвост>", TokenType.RPAREN), "ε" },
            { ("<Подвыражение_хвост>", TokenType.TO), "ε" },
            { ("<Подвыражение_хвост>", TokenType.DO), "ε" }, // Завершаем, если встречаем TO

            // Ун.оп.
            { ("<Ун_оп>", TokenType.MINUS), "-" },

            // Бин.оп.
            { ("<Бин_оп>", TokenType.PLUS), "+" },
            { ("<Бин_оп>", TokenType.MINUS), "-" },
            { ("<Бин_оп>", TokenType.MULT), "*" },

            // Операнд
            { ("<Операнд>", TokenType.IDENT), "<Идент>" },
            { ("<Операнд>", TokenType.CONST), "<Конст>" },

            // Идент
            { ("<Идент>", TokenType.IDENT), "Идентификатор" },

            // Конст
            { ("<Конст>", TokenType.CONST), "Константа" },

            // Чтение
            { ("<Чтение>", TokenType.READ), "READ ( <Список_идентификаторов> )" },

            // Запись
            { ("<Запись>", TokenType.WRITE), "WRITE ( <Список_идентификаторов> )" },

            // Список идентификаторов
            { ("<Список_идентификаторов>", TokenType.IDENT), "<Идент> <Доп_идентификаторы>" },

            // Дополнительные идентификаторы
            { ("<Доп_идентификаторы>", TokenType.COMMA), ", <Список_идентификаторов>" },
            { ("<Доп_идентификаторы>", TokenType.RPAREN), "ε" },

            { ("<Цикл>", TokenType.FOR), "FOR <Идент> = <Выражение> TO <Выражение> DO <Список_операторов> ENDFOR" },
        };
    }
}
