namespace Compiler.Nodes
{
    /// <summary>
    /// A node corresponding to a repeat command
    /// </summary>
    public class RepeatCommandNode : ICommandNode
    {
        /// <summary>
        /// The repeat branch command
        /// </summary>
        public ICommandNode RepeatCommand { get; }


        /// <summary>
        /// The condition expression
        /// </summary>
        public IExpressionNode Expression { get; }


        /// <summary>
        /// The position in the code where the content associated with the node begins
        /// </summary>
        public Position Position { get; }


        /// <summary>
        /// Creates a new repeat command node
        /// </summary>
        /// <param name="repeatCommand">The do branch command</param>
        /// <param name="expression">The condition expression</param>
        /// <param name="position">The position in the code where the content associated with the node begins</param>

        public RepeatCommandNode(ICommandNode repeatCommand, IExpressionNode expression, Position position)
        {
            RepeatCommand = repeatCommand;
            Expression = expression;
            Position = position;
        }

    }
}
