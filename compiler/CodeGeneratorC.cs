namespace compiler
{
    using System.Text;

    namespace compiler
    {
        using System;
        using System.Collections.Generic;
        using System.Text;

        namespace compiler
        {
           
                public class Translator
                {
                    private StringBuilder outputCode = new StringBuilder();

                    public string Translate(SyntaxTreeNode root)
                    {
                        TraverseNode(root);
                        return outputCode.ToString();
                    }

                    private void TraverseNode(SyntaxTreeNode node)
                    {
                        // Проверка на тип символа (нетерминал или терминал) через Symbol
                        if (node.Symbol.IsNonTerminal())
                        {
                            switch (node.Symbol.NonTerminal)
                            {
                                case NonTerminal.Программа:
                                    outputCode.AppendLine("public class Program");
                                    outputCode.AppendLine("{");
                                    outputCode.AppendLine("    public static void Main()");
                                    outputCode.AppendLine("    {");
                                    TraverseChildren(node);
                                    outputCode.AppendLine("    }");
                                    outputCode.AppendLine("}");
                                    break;

                                case NonTerminal.Объявление_переменных:
                                    outputCode.Append("int ");
                                    TraverseChildren(node);
                                    outputCode.AppendLine(";");
                                    break;

                                case NonTerminal.Список_переменных:
                                case NonTerminal.Доп_переменные:
                                case NonTerminal.Описание_вычислений:
                                case NonTerminal.Список_операторов:
                                case NonTerminal.Список_операторов_хвост:
                                case NonTerminal.Список_идентификаторов:
                                case NonTerminal.Доп_идентификаторы:
                                case NonTerminal.Выражение:
                                    TraverseChildren(node);
                                    break;

                                case NonTerminal.Присваивание:
                                    TraverseNode(node.Children[0]); // Идентификатор
                                    outputCode.Append(" = ");
                                    TraverseNode(node.Children[2]); // Выражение
                                    outputCode.AppendLine(";");
                                    break;

                                case NonTerminal.Идент:
                                    outputCode.Append(node.Children[0].Symbol.Token.Value);
                                    break;

                                case NonTerminal.Запись:
                                    outputCode.Append("Console.WriteLine(");
                                    TraverseNode(node.Children[2]); // Список идентификаторов
                                    outputCode.AppendLine(");");
                                    break;

                                case NonTerminal.Подвыражение:
                                 
                                        TraverseChildren(node);
                                   
                                    break;

                                case NonTerminal.Подвыражение_хвост:
                                        outputCode.Append(" ");
                                        TraverseChildren(node);
                                 
                                    break;

                                case NonTerminal.Бин_оп:
                                case NonTerminal.Ун_оп:
                                case NonTerminal.Конст:
                                    outputCode.Append(node.Children[0].Symbol.Token.Value);
                                    break;

                                case NonTerminal.Цикл:
                                    outputCode.Append("for (int ");
                                    TraverseNode(node.Children[1]); // Идентификатор
                                    outputCode.Append(" = ");
                                    TraverseNode(node.Children[3]); // Начальное значение
                                    outputCode.Append("; ");
                                    TraverseNode(node.Children[1]);
                                    outputCode.Append(" <= ");
                                    TraverseNode(node.Children[5]); // Конечное значение
                                    outputCode.Append("; ");
                                    TraverseNode(node.Children[1]);
                                    outputCode.Append("++)");
                                    outputCode.AppendLine("{");
                                    TraverseNode(node.Children[7]); // Список операторов
                                    outputCode.AppendLine("}");
                                    break;

                                default:
                                    TraverseChildren(node);
                                    break;
                            }
                        }
                        else
                        {
                            // Обработка терминала: фильтрация ненужных токенов
                            if (ShouldIncludeToken(node.Symbol.Token))
                            {
                                outputCode.Append(node.Symbol.Token.Value);
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

                    // Функция для фильтрации терминалов, которые не нужно добавлять в код C#
                    private bool ShouldIncludeToken(Token token)
                    {
                        switch (token.Type)
                        {
                            case TokenType.BEGIN:
                            case TokenType.END:
                            case TokenType.VAR:
                            case TokenType.INTEGER:
                            case TokenType.COLON:

                            case TokenType.SEMICOLON: // Можно пропустить ';', если он не обязателен в коде
                                return false;
                            default:
                                return true;
                        }
                    }
                }
            }

        }
    }
   
