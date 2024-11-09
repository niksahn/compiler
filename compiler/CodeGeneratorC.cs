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
                    switch (node.Token.Value)
                    {
                        case "<Программа>":
                            outputCode.AppendLine("public class Program");
                            outputCode.AppendLine("{");
                            outputCode.AppendLine("    public static void Main()");
                            outputCode.AppendLine("    {");
                            TraverseChildren(node);
                            outputCode.AppendLine("    }");
                            outputCode.AppendLine("}");
                            break;

                        case "<Объявление_переменных>":
                            outputCode.Append("int ");
                            TraverseChildren(node);
                            outputCode.AppendLine(";");
                            break;

                        case "<Список_переменных>":
                            TraverseChildren(node);
                            break;

                        case "<Доп_переменные>":
                            outputCode.Append(", ");
                            TraverseChildren(node);
                            break;

                        case "<Описание_вычислений>":
                        case "<Список_операторов>":
                        case "<Список_операторов_хвост>":
                            TraverseChildren(node);
                            break;

                        case "<Присваивание>":
                            TraverseNode(node.Children[0]); // Идентификатор
                            outputCode.Append(" = ");
                            TraverseNode(node.Children[2]); // Выражение
                            outputCode.AppendLine(";");
                            break;

                        case "<Идент>":
                            outputCode.Append(node.Children[0].Token.Value);
                            break;

                        case "<Запись>":
                            outputCode.Append("Console.WriteLine(");
                            TraverseNode(node.Children[2]); // Список идентификаторов
                            outputCode.AppendLine(");");
                            break;

                        case "<Список_идентификаторов>":
                            TraverseChildren(node);
                            break;

                        case "<Доп_идентификаторы>":
                            outputCode.Append(", ");
                            TraverseChildren(node);
                            break;

                        case "<Выражение>":
                            TraverseChildren(node);
                            break;

                        case "<Подвыражение>":
                            // Проверка на наличие открывающей и закрывающей скобки в подвыражении
                            if (node.Children.Count > 0 && node.Children[0].Token.Value == "(")
                            {
                                outputCode.Append("(");
                                TraverseChildren(node);
                            }
                            else
                            {
                                TraverseChildren(node); // Обычное подвыражение
                            }
                            break;


                        case "<Подвыражение_хвост>":
                            if (node.Token.Type == TokenType.RPAREN)
                            {
                                outputCode.Append(")");
                            }
                            if (node.Children.Count > 0)
                            {
                                outputCode.Append(" "); 
                                TraverseChildren(node);
                            }
                            break;

                        case "<Бин_оп>":
                        case "<Ун_оп>":
                            outputCode.Append(node.Children[0].Token.Value);
                            break;

                        case "<Конст>":
                            outputCode.Append(node.Children[0].Token.Value);
                            break;

                        case "<Цикл>":
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
}
