namespace Compiler.Nodes
{
    /// <summary>
    /// A node corresponding to a do if command
    /// </summary>
    public class DoIfCommandNode : ICommandNode
    {
        /// <summary>
        /// The do branch command
        /// </summary>
        public ICommandNode DoCommand { get; }


        /// <summary>
        /// The condition expression
        /// </summary>
        public IExpressionNode Expression { get; }

        /// <summary>
        /// The else branch command
        /// </summary>
        public ICommandNode ElseCommand { get; }


        /// <summary>
        /// The position in the code where the content associated with the node begins
        /// </summary>
        public Position Position { get; }


        /// <summary>
        /// Creates a new if command node
        /// </summary>
        /// <param name="doCommand">The do branch command</param>
        /// <param name="expression">The condition expression</param>
        /// <param name="elseCommand">The else branch command</param>
        /// <param name="position">The position in the code where the content associated with the node begins</param>

        public DoIfCommandNode(ICommandNode doCommand, IExpressionNode expression, ICommandNode elseCommand, Position position)
        {
            DoCommand = doCommand;
            Expression = expression;
            ElseCommand = elseCommand;
            Position = position;
        }

    }
}
