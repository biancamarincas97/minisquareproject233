using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
