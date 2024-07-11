using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compiler.Tokenization.TokenType;

namespace Compiler.Tokenization
{
    public enum TokenType
    {

        // non-terminals tokens
        IntLiteral, Identifier, CharLiteral, Operator,

        //reserved words tokens - terminals
        //If, Unless, Else, While, Do, Let, In, With, Done, Out,
        If, While, Do, Let, In, Else, Repeat, Const, Var, 

        // punctuation tokens - terminals
        Semicolon, Is, LeftBracket, RightBracket,
        LeftSquareBracket, RightSquareBracket, FullStop, 

        //special token NOP could perhaps be in the reserved words section as well, otherwise I considered it as a NOP word 
        EndOfText, Error, Nop,


    }


    public static class TokenTypes
    {

        public static ImmutableDictionary<string, TokenType> Keywords { get; } = new Dictionary<string, TokenType>()
        {
            {"if", If },
            {"while", While },
            {"do", Do },
            {"let", Let },
            {"in", In },
            {"else", Else },
            {"repeat", Repeat },
            {"const", Const },
            {"var", Var },
            //{"nop", Nop },

        }.ToImmutableDictionary();

        public static bool IsKeyword(StringBuilder word)
        {
            return Keywords.ContainsKey(word.ToString());
        }

        public static TokenType GetTokenForKeyword(StringBuilder word)
        {
            if (!IsKeyword(word)) throw new ArgumentException("Word is not a keyword");
            return Keywords[word.ToString()];
        }


    }

}
