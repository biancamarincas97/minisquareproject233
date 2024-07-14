using System.Collections.Immutable;


namespace Compiler.CodeGeneration
{
    public interface IRuntimeEntity { }

    public class SquareAbstractMachine
    {
        public enum Primitive { }
        public enum Type { }
        public static ImmutableDictionary<Type, byte> TypeSize { get; }

    }
}
