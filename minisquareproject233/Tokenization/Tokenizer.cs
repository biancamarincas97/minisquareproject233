using Compiler.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Tokenization
{

    public class Tokenizer
    {

        public ErrorReporter Reporter { get; }
        private IFileReader Reader { get; }
        private StringBuilder TokenSpelling { get; } = new StringBuilder();

        public Tokenizer(IFileReader reader, ErrorReporter reporter)
        {
            Reader = reader;
            Reporter = reporter;
        }

        public List<Token> GetAllTokens()
        {
            List<Token> tokens = new List<Token>();
            Token token = GetNextToken();
            while (token.Type != TokenType.EndOfText)
            {
                tokens.Add(token);
                token = GetNextToken();
            }
            tokens.Add(token);
            Reader.Close();
            return tokens;
        }

        private Token GetNextToken()
        {
            // Skip forward over any white space and comments
            SkipSeparators();

            // Remember the starting position of the token
            Position tokenStartPosition = Reader.CurrentPosition;

            // Scan the token and work out its type
            TokenType tokenType = ScanToken();

            // Create the token
            Token token = new Token(tokenType, TokenSpelling.ToString(), tokenStartPosition);
            Debugger.Write($"Scanned {token}");

            // Report an error if necessary
            if (tokenType == TokenType.Error)
            {
                // Report the error here
            }
            return token;
        }

        private void SkipSeparators()
        {
            while (IsComment(Reader.Current) || IsWhiteSpace(Reader.Current))
            {
                if (IsComment(Reader.Current))
                {
                    Reader.SkipRestOfLine();
                }
                else
                {
                    Reader.MoveNext();
                }
                char previousChar = Reader.Current;
                Debugger.Write($" current reader chara: {previousChar}");
            }
        }

       

        private TokenType ScanToken()
        {
            TokenSpelling.Clear();
            if (char.IsLetter(Reader.Current))      // IDENTIFIER
            {
                TakeIt();
                while (IsLetterDashDigit(Reader.Current))
                {
                    TakeIt();           
                }

                if (TokenTypes.IsKeyword(TokenSpelling))
                    return TokenTypes.GetTokenForKeyword(TokenSpelling);
                else
                    return TokenType.Identifier;
            }

            else if (char.IsDigit(Reader.Current))   //INTLITERAL
            {

                if (Reader.Current == '0')
                {
                    TakeIt();
                    return TokenType.IntLiteral;
                }

                else if (IsNonZeroDigit(Reader.Current))
                {
                    TakeIt();
                    while (char.IsDigit(Reader.Current))
                    {
                        TakeIt();
                    }
                    return TokenType.IntLiteral;
                }

            }


            else if (Reader.Current == '\'')          // CHARLITERAL
            {
                char startingQuote = Reader.Current;

                TakeIt();

                if (IsLetterOrSpace(Reader.Current))
                {
                    TakeIt();
                }

                if (Reader.Current == startingQuote)
                {
                    TakeIt();
                    return TokenType.CharLiteral;
                }
                else
                {
                    return TokenType.Error;
                }

            }

            else if (IsOperator(Reader.Current))  // OPERATOR  
            {
                TakeIt();
                return TokenType.Operator;
            }

            else if (Reader.Current == '=')             
            {
                TakeIt();
                if (Reader.Current == '=')          // if the following Reader reads the second = then return an Operator token type
                {
                    TakeIt();
                    return TokenType.Operator;
                }
                else
                {
                    return TokenType.Error;         // however, if the reader only reads one = then return an Error token type
                }

            }

            else if (Reader.Current == ';') 
            {
                TakeIt();
                return TokenType.Semicolon;
            }

            else if (Reader.Current == '.')
            {
                TakeIt();
                return TokenType.FullStop;
            }

            else if (Reader.Current == '~')
            {
                TakeIt();
                return TokenType.Is;
            }

            else if (Reader.Current == '(')
            {
                TakeIt();
                return TokenType.LeftBracket;
            }

            else if (Reader.Current == ')')
            {
                TakeIt();
                return TokenType.RightBracket;
            }

            else if (Reader.Current == '[')
            {
                TakeIt();
                return TokenType.LeftSquareBracket;
            }

            else if (Reader.Current == ']')
            {
                TakeIt();
                return TokenType.RightSquareBracket;
            }

            else if (IsNop())
            {
                TakeIt();
                return TokenType.Nop;
            }

            else if (Reader.Current == default(char))
            {
                TakeIt();
                return TokenType.EndOfText;
            }          

            else
            {
                TakeIt();
                return TokenType.Error;
            }

            return TokenType.Error;

        }

        private void TakeIt()
        {
            TokenSpelling.Append(Reader.Current);
            Reader.MoveNext();
        }

        private static bool IsComment(char currentChar)
        {
            return currentChar == '@';
        }

        private static bool IsWhiteSpace(char currentChar)
        {
            return IsSpace(currentChar) || currentChar == '\t' || currentChar == '\n';
        }

        private static bool IsSpace(char currentChar)
        {
            return currentChar == ' ';
        }

        private bool IsNop()
        {
            int currentState = 0;
            char[] sequenceToMatch = { 'n', 'o', 'p' };

            while (currentState < sequenceToMatch.Length)
            {
                if (Reader.Current == sequenceToMatch[currentState])
                {
                    currentState++;
                }
                else
                {
                    currentState = 0;
                }

                Reader.MoveNext();

                if (Reader.Current == '\0')
                {
                    break;
                }
            }
            return currentState == sequenceToMatch.Length;
        }

        private static bool IsOperator(char currentChar)
        {
            switch (currentChar)
            {

                case '+':
                case '-':
                case '*':
                case '/':
                case '<':
                case '>':
                case '\\':
                    return true;
                default:
                    return false;

            }

        }

        private static bool IsNonZeroDigit(char currentChar)
        {
            return currentChar >= '1' && currentChar <= '9';
        }

        private static bool IsLetterDashDigit(char currentChar)
        {
            return char.IsLetter(currentChar) || currentChar == '-' || char.IsDigit(currentChar);
        }

        private static bool IsLetterOrSpace(char currentChar)
        {
            return char.IsLetter(currentChar) || IsSpace(currentChar);
        }




        
    }
}
