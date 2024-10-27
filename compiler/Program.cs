using compiler;
    
string input = @"
VAR x,y : INTEGER
BEGIN x = 5 + 10; 
    y = 10;
    WRITE(x,y);
    FOR x = 0 TO 10 DO WRITE(x); ENDFOR
END";
Lexer lexer = new Lexer(input);

 Token token;
var tokens = new List<Token>();
 do
 {
     token = lexer.GetNextToken();
     tokens.Add(token);
 } while (token.Type != TokenType.EOF);

var synt = new Syntacsys(tokens);
synt.ParseProgram().PrintTree();