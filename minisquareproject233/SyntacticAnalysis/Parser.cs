using Compiler.IO;
using Compiler.Tokenization;
using System.Collections.Generic;
using static Compiler.Tokenization.TokenType;
using Compiler.Nodes;

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


        public ProgramNode Parse(List<Token> tokens)
        {
            this.tokens = tokens;
            ProgramNode program = ParseProgram();
            return program;
        }


        private ProgramNode ParseProgram()
        {
            Debugger.Write("Parsing program");
            ICommandNode command = ParseCommand();
            ProgramNode program = new ProgramNode(command);
            return program;
        }


        private ICommandNode ParseCommand()
        {
            Debugger.Write("Parsing command");
            List<ICommandNode> commands = new List<ICommandNode>();
            commands.Add(ParseSingleCommand());
            
            while (CurrentToken.Type == Semicolon || CurrentToken.Type == FullStop)
            {
                Accept(CurrentToken.Type);
                commands.Add(ParseSingleCommand());
            }

            if(commands.Count == 1)
                return commands[0];
            else 
                return new SequentialCommandNode(commands);

        }


        private ICommandNode ParseSingleCommand()
        {
            Debugger.Write("Parsing single command");
            switch (CurrentToken.Type)
            {                              
                case Identifier:
                    return ParseAssignmentOrCallCommand();                    
                case If:
                    return ParseIfCommand();                    
                case While:
                    return ParseWhileCommand();                    
                case Let:
                    return ParseLetCommand();                    
                case Do:
                    return ParseDoIfCommand();                    
                case Repeat:
                    return ParseRepeatCommand();                    
                case LeftSquareBracket:
                    return ParseBeginCommand();                    
                default:
                    return ParseNOPCommand();
            }

        }

        private ICommandNode ParseAssignmentOrCallCommand()
        {
            Position startPosition = CurrentToken.Position;
            IdentifierNode identifier = ParseIdentifier();
            if (CurrentToken.Type == LeftBracket)
            {
                Debugger.Write("Parsing CALL command");
                Accept(LeftBracket);
                IParameterNode parameter = ParseParameter();
                Accept(RightBracket);
                return new CallCommandNode(identifier, parameter);
            }
            else if (CurrentToken.Type == Is)
            {
                Debugger.Write("Parsing ASSIGNMENT command");
                Accept(Is);
                IExpressionNode expression = ParseExpression();
                return new AssignCommandNode(identifier, expression);
            }
            else
            {
                return new ErrorNode(startPosition);
            }
        }


        private ICommandNode ParseIfCommand()
        {
            Debugger.Write("Parsing IF command");
            Position startPosition = CurrentToken.Position;
            Accept(If);
            IExpressionNode expression = ParseExpression();
            ICommandNode command = ParseSingleCommand();
            Accept(Else);
            ICommandNode elseCommand = ParseSingleCommand();
            
            return new IfCommandNode(expression, command, elseCommand, startPosition);
        }


        private ICommandNode ParseWhileCommand()
        {
            Debugger.Write("Parsing WHILE command");
            Position startPosition = CurrentToken.Position;
            Accept(While);
            IExpressionNode expression = ParseExpression();
            Accept(Do);
            ICommandNode command =  ParseSingleCommand();

            return new WhileCommandNode(expression, command, startPosition);
        }


        private ICommandNode ParseLetCommand()
        {
            Debugger.Write("Parsing LET command");
            Position startPosition = CurrentToken.Position;
            Accept(Let);
            IDeclarationNode declaration = ParseDeclaration();
            Accept(In);
            ICommandNode command = ParseSingleCommand();

            return new LetCommandNode(declaration, command, startPosition);
        }


        private ICommandNode ParseDoIfCommand()
        {
            Debugger.Write("Parsing DO IF command");
            Position startPosition = CurrentToken.Position;
            Accept(Do);
            ICommandNode doSingleCommand = ParseSingleCommand();
            Accept(If);
            IExpressionNode expression = ParseExpression();
            Accept(Else);
            ICommandNode elseSingleCommand = ParseSingleCommand();

            return new DoIfCommandNode(doSingleCommand, expression, elseSingleCommand, startPosition);      // for the DoIfCommand a new Node class has been created to represent the correct order of arguments. Although the IfCommand node could have been reused (as the logic regarding the order of arguments lies within the function itself) 
        }


        private ICommandNode ParseRepeatCommand()
        {
            Debugger.Write("Parsing REPEAT command");
            Position startPosition = CurrentToken.Position;
            Accept(Repeat);
            ICommandNode repeatCommand = ParseSingleCommand();
            Accept(While);
            IExpressionNode expression = ParseExpression();

            return new RepeatCommandNode(repeatCommand, expression, startPosition);     // new Repeat command node created 
        }

        private ICommandNode ParseBeginCommand()
        {
            Debugger.Write("Parsing BEGIN command");
            Accept(LeftSquareBracket);
            ICommandNode command = ParseCommand();
            Accept(RightSquareBracket);

            return command; 
        }

        private ICommandNode ParseNOPCommand()
        {
            Debugger.Write("Parsing NOP command");
            Position startPosition = CurrentToken.Position;
            Accept(Nop);
            return new BlankCommandNode(startPosition);
        }


        private IDeclarationNode ParseDeclaration()
        {
            Debugger.Write("Parsing declaration");
            List<IDeclarationNode> declarations = new List<IDeclarationNode>();
            declarations.Add(ParseSingleDeclaration());
            
            while (CurrentToken.Type == TokenType.Semicolon || CurrentToken.Type == TokenType.FullStop)
            {
                Accept(CurrentToken.Type);
                declarations.Add(ParseSingleDeclaration());
            }

            if (declarations.Count == 1)
                return declarations[0];
            else
                return new SequentialDeclarationNode(declarations);

        }


        private IDeclarationNode ParseSingleDeclaration()
        {
            Debugger.Write("Parsing single declaration");
            switch (CurrentToken.Type)
            {
                case Const:
                    return ParseConstDeclaration();                   
                case Var:
                    return ParseVarDeclaration();                    
                default:
                    return new ErrorNode(CurrentToken.Position);
            }

        }


        private IDeclarationNode ParseConstDeclaration()
        {
            Debugger.Write("Parsing CONST declaration");
            Position startPosition = CurrentToken.Position;
            Accept(Const);
            TypeDenoterNode typeDenoter = ParseTypeDenoter();
            IdentifierNode identifier = ParseIdentifier();
            Accept(Is);
            IExpressionNode expression = ParseExpression();

            return new ConstDeclarationNode(typeDenoter, identifier, expression, startPosition);        // Const declaration node edited to include the typeDenoter as an argument
        }


        private IDeclarationNode ParseVarDeclaration()
        {
            Debugger.Write("Parsing VAR declaration");
            Position startPosition = CurrentToken.Position;
            Accept(Var);
            TypeDenoterNode typeDenoter = ParseTypeDenoter();
            IdentifierNode identifier = ParseIdentifier();

            return new VarDeclarationNode(identifier, typeDenoter, startPosition);
        }


        private IParameterNode ParseParameter()
        {
            Debugger.Write("Parsing parameter");
            switch(CurrentToken.Type)
            {
                case IntLiteral:
                case Identifier:
                case CharLiteral:
                case Operator:
                case LeftBracket:
                    return ParseExpressionParameter();
                case Var:
                    return ParseVarParameter();
                case RightBracket:
                    return new BlankParameterNode(CurrentToken.Position);       // BLANK PARAMETER
                default:
                    return new ErrorNode(CurrentToken.Position);
            }

        }


        private IParameterNode ParseExpressionParameter()
        {
            Debugger.Write("Parsing VALUE parameter");
            IExpressionNode expression = ParseExpression();
            return new ExpressionParameterNode(expression);     // VALUE PARAMETER
        }


        private IParameterNode ParseVarParameter()
        {
            Debugger.Write("Parsing VAR parameter");
            Position startPosition = CurrentToken.Position;
            Accept(Var);
            IdentifierNode identifier = ParseIdentifier();

            return new VarParameterNode(identifier, startPosition);     // VAR PARAMTER
        }


        private TypeDenoterNode ParseTypeDenoter()
        {
            Debugger.Write("Parsing type denoter");
            IdentifierNode identifier = ParseIdentifier();

            return new TypeDenoterNode(identifier);         // TYPE NAME
        }


        private IExpressionNode ParseExpression()
        {
            Debugger.Write("Parsing expression");
            IExpressionNode leftExpression =  ParsePrimaryExpression();
            while (CurrentToken.Type == Operator)
            {
                OperatorNode operation = ParseOperator();
                IExpressionNode rightExpression =  ParsePrimaryExpression();
                leftExpression = new BinaryExpressionNode(leftExpression, operation, rightExpression);
            }
            return leftExpression;
        }


        private IExpressionNode ParsePrimaryExpression()
        {
            Debugger.Write("Parsing primary expression");
            switch (CurrentToken.Type)
            {
                case IntLiteral:
                    return ParseIntegerExpression();
                case CharLiteral:
                    return ParseCharExpression();
                case Identifier:
                    return ParseIdOrCallExpression();
                case Operator:
                    return ParseUnaryExpression();
                case LeftBracket:
                    return ParseBracketExpression();
                default:
                    return new ErrorNode(CurrentToken.Position);
            }

        }


        private IExpressionNode ParseIntegerExpression() {
            Debugger.Write("Parsing integer expression");
            IntegerLiteralNode intLiteral = ParseIntegerLiteral();

            return new IntegerExpressionNode(intLiteral);                
        }

        private IExpressionNode ParseCharExpression() {
            Debugger.Write("Parsing character expression");
            CharacterLiteralNode charLiteral = ParseCharLiteral();

            return new CharacterExpressionNode(charLiteral);
        }

        private IExpressionNode ParseIdOrCallExpression() {

            Debugger.Write("Parsing character expression");
            IdentifierNode identifier = ParseIdentifier();


            if (CurrentToken.Type == LeftBracket)
            {
                Accept(LeftBracket);
                IParameterNode parameter = ParseParameter();
                Accept(RightBracket);

                return new CallExpressionNode(identifier, parameter);   // new CALL expression node that contains both the identifier and parameter
            }

            else return new IdExpressionNode(identifier);
        }



        private IExpressionNode ParseUnaryExpression() {

            Debugger.Write("Parsing unary expression");
            OperatorNode operation = ParseOperator();
            IExpressionNode expression = ParsePrimaryExpression();
            
            return new UnaryExpressionNode(operation, expression);

        }

        private IExpressionNode ParseBracketExpression() {
            Debugger.Write("Parsing bracket expression");
            Accept(LeftBracket);
            IExpressionNode expression = ParseExpression();
            Accept(RightBracket);
            
            return expression;

        }



        private IntegerLiteralNode ParseIntegerLiteral()
        {
            Debugger.Write("Parsing integer literal");
            Token integerLiteralToken = CurrentToken;
            Accept(IntLiteral);

            return new IntegerLiteralNode(integerLiteralToken);
        }



        private CharacterLiteralNode ParseCharLiteral()
        {
            Debugger.Write("Parsing character literal");
            Token characterLiteralToken = CurrentToken;
            Accept(CharLiteral);

            return new CharacterLiteralNode(characterLiteralToken);
        }


        private IdentifierNode ParseIdentifier()
        {
            Debugger.Write("Parsing identifier");
            Token identifierToken = CurrentToken;
            Accept(Identifier);

            return new IdentifierNode(identifierToken);
        }


        private OperatorNode ParseOperator()
        {
            Debugger.Write("Parsing operator");
            Token operatorToken = CurrentToken;
            Accept(Operator);

            return new OperatorNode(operatorToken);
        }


    }
}
