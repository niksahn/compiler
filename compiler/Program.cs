using compiler;
using compiler.compiler;
using compiler.compiler.compiler;

string input = @"
VAR x,y,i,z : INTEGER;
BEGIN x = 5 + 10; 
    y = 10;
    z = -(-9 + (10 - 10) * y);
    WRITE(x,y);
    FOR i = 0 TO 10 DO WRITE(i); ENDFOR
END";


//string input = @"
//VAR x : INTEGER;
//BEGIN x = 5 + 10; 
//    WRITE(x);
//END";

Lexer lexer = new Lexer(input);

 Token token;
var tokens = new List<Token>();
 do
 {
     token = lexer.GetNextToken();
     tokens.Add(token);
 } while (token.Type != TokenType.EOF);

var synt = new Syntacsys(tokens);
var tree = synt.ParseProgram();

tree.PrintTree();
new SemanticAnalyzer().Analyze(tree);
var gener = new Translator().Translate(tree);
Console.WriteLine(gener.ToString());