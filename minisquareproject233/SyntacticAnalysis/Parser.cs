using Compiler.IO;
using Compiler.Tokenization;
using System.Collections.Generic;
using static Compiler.Tokenization.TokenType;

namespace Compiler.SyntacticAnalysis
{
    /// <summary>
    /// A recursive descent parser
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// The error reporter
        /// </summary>
        public ErrorReporter Reporter { get; }

        /// <summary>
        /// The tokens to be parsed
        /// </summary>
        private List<Token> tokens;

        /// <summary>
        /// The index of the current token in tokens
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// The current token
        /// </summary>
        private Token CurrentToken { get { return tokens[currentIndex]; } }

        /// <summary>
        /// Advances the current token to the next one to be parsed
        /// </summary>
        private void MoveNext()
        {
            if (currentIndex < tokens.Count - 1)
                currentIndex += 1;
        }

        /// <summary>
        /// Creates a new parser
        /// </summary>
        /// <param name="reporter">The error reporter to use</param>
        public Parser(ErrorReporter reporter)
        {
            Reporter = reporter;
        }

        /// <summary>
        /// Checks the current token is the expected kind and moves to the next token
        /// </summary>
        /// <param name="expectedType">The expected token type</param>
        private void Accept(TokenType expectedType)
        {
            if (CurrentToken.Type == expectedType)
            {
                Debugger.Write($"Accepted {CurrentToken}");
                MoveNext();
            }
        }

        
        public void Parse(List<Token> tokens)
        {
            this.tokens = tokens;
            ParseProgram();
        }



        private void ParseProgram()
        {
            Debugger.Write("Parsing program");
            ParseSingleCommand();
        }


        private void ParseCommand() 
        {
            Debugger.Write("Parsing command");
            ParseSingleCommand();

            while (CurrentToken.Type == Semicolon || CurrentToken.Type == FullStop)
            {
                Accept(CurrentToken.Type);
                ParseSingleCommand();
            }

        }


        private void ParseSingleCommand()
        {
            Debugger.Write("Parsing single command");
            switch (CurrentToken.Type)
            {
                case Nop:
                    Accept(Nop);
                    break;
                case Identifier:
                    ParseIdentifier();
                    if (CurrentToken.Type == Is)
                    {
                        Accept(Is);
                        ParseExpression();
                    }
                    else if (CurrentToken.Type == LeftBracket)
                    {
                        Accept(LeftBracket);
                        ParseParameter();
                        Accept(RightBracket);
                    }
                    break;
                case If:
                    ParseIfCommand();
                    break;
                case While:
                    ParseWhileCommand();
                    break;
                case Let:
                    ParseLetCommand();
                    break;
                case Do:
                    ParseDoCommand();
                    break;
                case Repeat:
                    ParseRepeatCommand();
                    break;
                case LeftSquareBracket:
                    Accept(LeftSquareBracket);
                    ParseCommand();
                    Accept(RightSquareBracket);
                    break;
                default:
                    Debugger.Write("Could not parse single command");
                    break;

            }

        }

        private void ParseIfCommand() 
        {
            Debugger.Write("Parsing IF command");
            Accept(If);
            ParseExpression();
            ParseSingleCommand();
            Accept(Else);
            ParseSingleCommand();

        }
        private void ParseWhileCommand() 
        {
            Debugger.Write("Parsing WHILE command");
            Accept(While);
            ParseExpression();
            Accept(Do);
            ParseSingleCommand();

        }
        private void ParseLetCommand() 
        {
            Debugger.Write("Parsing LET command");
            Accept(Let);
            ParseDeclaration();
            Accept(In);
            ParseSingleCommand();


        }
        private void ParseDoCommand() 
        {
            Debugger.Write("Parsing DO command");
            Accept(Do);
            ParseSingleCommand();
            Accept(If);
            ParseExpression();
            Accept(Else);
            ParseSingleCommand();

        }
        private void ParseRepeatCommand() 
        {
            Debugger.Write("Parsing REPEAT command");
            Accept(Repeat);
            ParseSingleCommand();
            Accept(While);
            ParseExpression();
        }

         

        private void ParseDeclaration()
        {
            Debugger.Write("Parsing declaration");
            ParseSingleDeclaration();
            while(CurrentToken.Type == TokenType.Semicolon || CurrentToken.Type == TokenType.FullStop)
            {
                Accept(CurrentToken.Type);
                ParseSingleDeclaration();
            }

        }

        private void ParseSingleDeclaration() 
        {
            Debugger.Write("Parsing single declaration");
            switch (CurrentToken.Type)
            {
                case Const:
                    ParseConstDeclaration();
                    break;
                case Var:
                    ParseVarDeclaration();
                    break;
                default:
                    Debugger.Write("Could not parse single declaration");
                    break;
            }
                
        }

        private void ParseConstDeclaration() 
        {
            Debugger.Write("Parsing CONST declaration");
            Accept(Const);
            ParseTypeDenoter();
            ParseIdentifier();
            Accept(Is);
            ParseExpression();

        }

        private void ParseVarDeclaration() 
        {
            Debugger.Write("Parsing VAR declaration");
            Accept(Var);
            ParseTypeDenoter();
            ParseIdentifier();

        }

        private void ParseParameter()
        {
            Debugger.Write("Parsing parameter");
            if (CurrentToken.Type == Var)
            {
                ParseVarParameter();
            }
            else if (CurrentToken.Type != RightBracket)
            {
                ParseExpression();
            }
        }

        private void ParseVarParameter()
        {
            Debugger.Write("Parsing VAR parameter");
            Accept(Var);
            ParseIdentifier();
        }


        private void ParseTypeDenoter()
        {
            Debugger.Write("Parsing type denoter");
            ParseIdentifier();

        }

        private void ParseExpression()
        {
            Debugger.Write("Parsing expression");
            ParsePrimaryExpression();
            while(CurrentToken.Type == Operator)
            {
                ParseOperator();
                ParsePrimaryExpression();
            }

        }


        private void ParsePrimaryExpression()
        {
            Debugger.Write("Parsing primary expression");
            switch (CurrentToken.Type)
            {
                case IntLiteral:
                    ParseIntegerLiteral();
                    break;
                case CharLiteral:
                    ParseCharLiteral();
                    break;
                case Identifier:
                    ParseIdentifier();
                    if(CurrentToken.Type == LeftBracket)
                    {
                        Accept(LeftBracket);
                        ParseParameter();   
                        Accept(RightBracket);
                    }
                    break;
                case Operator:
                    ParseOperator();
                    ParsePrimaryExpression();
                    break;
                case LeftBracket:
                    Accept(LeftBracket);
                    ParseExpression();
                    Accept(RightBracket);
                    break;

                default:
                    Debugger.Write("Could not parse primary expression");
                    break;
            }

        }

        private void ParseIntegerLiteral()
        {
            Debugger.Write("Parsing integer literal");
            Accept(IntLiteral);
        }

        private void ParseCharLiteral() {
            Debugger.Write("Parsing character literal");
            Accept(CharLiteral);
        }

        private void ParseIdentifier()
        {
            Debugger.Write("Parsing identifier");
            Accept(Identifier);
        }


        private void ParseOperator()
        {
            Debugger.Write("Parsing operator");
            Accept(Operator);
        }
  

        



       
    }
}
