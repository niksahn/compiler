using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace compiler
{
    using System;
    using System.Collections.Generic;

    namespace compiler
    {
        public class SemanticAnalyzer
        {
            // Словарь для хранения переменных и их типов
            private Dictionary<string, string> symbolTable = new Dictionary<string, string>();

            // Основной метод для запуска семантического анализа
            public void Analyze(SyntaxTreeNode root)
            {
                TraverseNode(root);
            }

            private void TraverseNode(SyntaxTreeNode node)
            {
                // Если узел представляет нетерминал
                if (node.Symbol.IsNonTerminal())
                {
                    switch (node.Symbol.NonTerminal)
                    {
                        case NonTerminal.Программа:
                            TraverseChildren(node);
                            break;

                        case NonTerminal.Объявление_переменных:
                            AddSubIdents(node);
                            break;

                        case NonTerminal.Присваивание:
                            CheckSubTree(node);

                            // Рекурсивный обход для проверки выражения присваивания
                            TraverseNode(node.Children[2]); // Выражение справа от присваивания
                            break;

                        //case NonTerminal.Идент:
                        //    string id = node.FirstIdent().Symbol.Token.Value;

                        //    // Проверка на использование необъявленной переменной
                        //    if (!symbolTable.ContainsKey(id))
                        //    {
                        //        throw new Exception($"Ошибка: переменная '{id}' не объявлена.");
                        //    }
                        //    break;

                     

                        case NonTerminal.Бин_оп:
                            // Проверка типов для бинарных операций
                            TraverseChildren(node);
                            break;

                        case NonTerminal.Чтение:
                        case NonTerminal.Запись:
                            //// Проверка всех переменных, которые выводятся на консоль
                            //foreach (var child in node.Children)
                            //{
                            //    if (!child.Symbol.IsNonTerminal() && child.Symbol.Token.Type == TokenType.IDENT)
                            //    {
                            //        string outputVariable = child.Symbol.Token.Value;
                            //        if (!symbolTable.ContainsKey(outputVariable))
                            //        {
                            //            throw new Exception($"Ошибка: переменная '{outputVariable}' не объявлена.");
                            //        }
                            //    }
                            //}
                            CheckSubTree(node);
                            break;

                        case NonTerminal.Цикл:
                            // Проверка правильности цикла и инициализации переменных
                            CheckSubTree(node);

                            TraverseNode(node.Children[3]); // Начальное значение цикла
                            TraverseNode(node.Children[5]); // Конечное значение цикла
                            TraverseNode(node.Children[7]); // Список операторов в теле цикла
                            break;

                        default:
                            TraverseChildren(node);
                            break;
                    }
                }
            }

            private void CheckSubTree(SyntaxTreeNode node)
            {
                foreach (var child in node.Children)
                {
                    CheckIdent(child);
                    CheckSubTree(child);
                }
            }

            private void AddSubIdents(SyntaxTreeNode node)
            {
                foreach (var child in node.Children)
                {
                    if (!child.Symbol.IsNonTerminal() && child.Symbol.Token.Type == TokenType.IDENT)
                    {
                        string variableName = child.Symbol.Token.Value;

                        // Проверка на повторное объявление переменной
                        if (symbolTable.ContainsKey(variableName))
                        {
                            throw new Exception($"Ошибка: переменная '{variableName}' уже объявлена.");
                        }

                        // Добавляем переменную в таблицу символов
                        symbolTable[variableName] = "int"; // Здесь предполагаем, что тип всегда int для простоты
                    }
                    AddSubIdents(child);
                }
            }

            private void CheckIdent(SyntaxTreeNode node)
            {
                if (node.IsIdent())
                {
                    string loopVar = node.Symbol.Token.Value;
                    if (!symbolTable.ContainsKey(loopVar) && node.IsIdent())
                    {
                        throw new Exception($"Ошибка: переменная '{loopVar}' не объявлена.");
                    }
                }
            }

            private void TraverseChildren(SyntaxTreeNode node)
            {
                foreach (var child in node.Children)
                {
                    TraverseNode(child);
                }
            }
        }
    }
}
