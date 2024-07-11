using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Tokenization
{

    public class Token
    {

        public TokenType Type { get; }

        public string Spelling { get; }

        public Position Position { get; }

        public Token(TokenType type, string spelling, Position position)
        {
            Spelling = spelling;
            Type = type;
            Position = position;
        }

        public override string ToString()
        {
            return $"type={Type}, spelling=\"{Spelling}\", position={Position}";
        }

    }

}
