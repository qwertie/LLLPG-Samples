// Currently, no IntelliSense (code completion) is available in .ecs files,
// so it can be useful to split your Lexer and Parser classes between two 
// files. In this file (the .cs file) IntelliSense will be available and 
// the other file (the .ecs file) contains your grammar code.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loyc;               // optional (for IMessageSink, Symbol, etc.)
using Loyc.Collections;   // optional (many handy interfaces & classes)
using Loyc.Syntax.Lexing; // For BaseLexer
using Loyc.Syntax;        // For BaseParser<Token> and LNode

namespace MyLanguage
{
    // You can define your own `Token` structure, or use the default `Token` in 
    // the `Loyc.Syntax.Lexing` namespace; in the latter case, we should define 
    // this extension method on it because the default `Token` just uses a raw 
    // `int` as the token type.
    public static class TokenExt {
        public static TokenType Type(this Token t) { return (TokenType)t.TypeInt; }
    }

    partial class Lexer : BaseILexer<ICharSource, Token>
    {
        // When using the Loyc libraries, `BaseLexer` and `BaseILexer` read character 
        // data from an `ICharSource`, which the string wrapper `UString` implements.
        public Lexer(string text, string fileName = "") 
            : this((UString)text, fileName) { }
        public Lexer(ICharSource text, string fileName = "") 
            : base(text, fileName) { }

        private int _startIndex;
    
        // Creates a Token
        private Token T(TokenType type, object value)
        {
            return new Token((int)type, _startIndex, InputPosition - _startIndex, value);
        }
    
        // Gets the text of the current token that has been parsed so far
        private UString Text()
        {
            return CharSource.Slice(_startIndex, InputPosition - _startIndex);
        }
    }
    
    partial class Parser : BaseParserForList<Token, int>
    {
        public static List<double> Parse(string text, string fileName = "")
        {
            var lexer = new Lexer(text, fileName);
            // Lexer is derived from BaseILexer, which implements IEnumerator<Token>.
            // Buffered() is an extension method that gathers the output of the 
            // enumerator into a list so that the parser can consume it.
            var parser = new Parser(lexer.Buffered(), lexer.SourceFile);
            return parser.Numbers();
        }
        
        protected Parser(IList<Token> list, ISourceFile file, int startIndex = 0) 
            : base(list, default(Token), file, startIndex) {}
        
        // Used for error reporting
        protected override string ToString(int tokenType) { 
            return ((TokenType)tokenType).ToString();
        }
    }
}
