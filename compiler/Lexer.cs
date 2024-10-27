using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace compiler
{
    public enum TokenType
    {
        BEGIN, END, VAR, INTEGER, READ, WRITE, FOR, TO, DO, END_FOR,
        SEMICOLON, COLON, EQUALS, PLUS, MINUS, MULT, LPAREN, RPAREN, COMMA,
        IDENT, CONST, EOF // Конец файла
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type}: {Value}";
        }
    }
    public class Lexer
    {
        private string _input;
        private int _position;
        private Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>
        {
        { "BEGIN", TokenType.BEGIN },
        { "END", TokenType.END },
        { "VAR", TokenType.VAR },
        { "INTEGER", TokenType.INTEGER },
        { "READ", TokenType.READ },
        { "WRITE", TokenType.WRITE },
        { "FOR", TokenType.FOR },
        { "TO", TokenType.TO },
        { "DO", TokenType.DO },
        { "ENDFOR", TokenType.END_FOR }
        };

        private Dictionary<string, TokenType> _additional_keywords = new Dictionary<string, TokenType>
        {
        { ";", TokenType.SEMICOLON },
        { ":", TokenType.COLON },
        { "=", TokenType.EQUALS },
        { "+", TokenType.PLUS },
        { "-", TokenType.MINUS },
        { "*", TokenType.MULT },
        { "(", TokenType.LPAREN },
        { ")", TokenType.RPAREN },
        { ",", TokenType.COMMA }
        };

        public Lexer(string input)
        {
            _input = input;
            _position = 0;
        }

        private char CurrentChar => _position >= _input.Length ? '\0' : _input[_position];

        private void Advance() => _position++;

        private void SkipWhitespace()
        {
            while (char.IsWhiteSpace(CurrentChar))
            {
                Advance();
            }
        }

        private Token GetNumber()
        {
            string result = "";
            while (char.IsDigit(CurrentChar))
            {
                result += CurrentChar;
                Advance();
            }
            return new Token(TokenType.CONST, result);
        }

        private Token GetIdentifier()
        {
            string result = "";
            while (char.IsLetterOrDigit(CurrentChar))
            {
                result += CurrentChar;
                Advance();
            }

            if (_keywords.ContainsKey(result))
            {
                return new Token(_keywords[result], result);
            }

            return new Token(TokenType.IDENT, result);
        }

        public Token GetNextToken()
        {
            while (CurrentChar != '\0')
            {
                if (char.IsWhiteSpace(CurrentChar))
                {
                    SkipWhitespace();
                    continue;
                }

                if (char.IsLetter(CurrentChar))
                {
                    return GetIdentifier();
                }

                if (char.IsDigit(CurrentChar))
                {
                    return GetNumber();
                }
                if (_additional_keywords.ContainsKey(CurrentChar.ToString()))
                {
                    var rez = CurrentChar.ToString();
                    Advance();
                    return new Token(_additional_keywords[rez], rez);
                }

                throw new Exception($"Unknown character: {CurrentChar}");
            }

            return new Token(TokenType.EOF, null);
        }
    }

}
