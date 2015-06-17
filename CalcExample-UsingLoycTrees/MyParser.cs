using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Loyc;               // (for IMessageSink, Symbol, etc.)
using Loyc.Collections;   // (many handy interfaces & classes)
using Loyc.Syntax.Lexing; // (for BaseLexer)
using Loyc.Syntax;        // (for BaseParser<Token>, LNode)

namespace MyLanguage
{
	using TT = TokenType;  // Abbreviate TokenType as TT
	using S = CodeSymbols;
	using System.Diagnostics; // To access symbols commonly used in Loyc trees

	public partial class MyParser : BaseParser<Token>
 	{
		List<Token> _tokens;
		LNodeFactory F;
		public IMessageSink ErrorSink { get; set; }

		public MyParser(string text, string fileName, IMessageSink errorSink = null) : this((StringSlice)text, fileName, errorSink) { }
		public MyParser(ICharSource text, string fileName, IMessageSink errorSink = null)
		{
			// Grab all tokens from the lexer and ignore spaces
			var lexer = new MyLexer(text, fileName);
			_tokens = new List<Token>();
			Token t;
			while ((t = lexer.NextToken()).Type() != TT.EOF) {
				if (t.Kind != TokenKind.Spaces)
					_tokens.Add(t);
			}

			ErrorSink = errorSink ?? MessageSink.Console;
			F = new LNodeFactory(lexer.SourceFile);
			InputPosition = 0; // Causes BaseParser to request & cache LT0
		}

		#region Methods & properties required by BaseParser and LLLPG
		// Here are a couple of things required by LLLPG itself (EOF, LA0, 
		// LA(i)) followed by the helper methods required by BaseParser. 
		// The difference between "LA" and "LT" is that "LA" refers to the 
		// lookahead token type (e.g. TT.Num, TT.Add, etc.), while "LT" 
		// refers to the entire token (that's the Token structure, in this 
		// example.) LLLPG itself only requires LA, but BaseParser assumes 
		// that there is also a "Token" struct or class, which is the type 
		// returned by its Match() methods.

		const TokenType EOF = TT.EOF;
		TokenType LA0 { [DebuggerStepThrough] get { return LT0.Type(); } }
		TokenType LA(int offset) { return LT(offset).Type(); }

		protected override int EofInt() { return (int) EOF; }
		protected override int LA0Int { get { return (int) LT0.Type(); } }
		protected override Token LT(int i)
		{
			i += InputPosition;
			if (i < _tokens.Count) {
				return _tokens[i];
			} else {
				return new Token((int)TT.EOF, F.File.Text.Count, 0);
			}
		}
		protected override void Error(int lookahead, string message)
		{
			int charIndex = LT(lookahead).StartIndex;
			SourcePos location = F.File.IndexToLine(charIndex);
			ErrorSink.Write(Severity.Error, location, message);
		}
		// BaseParser.Match() uses this for constructing error messages.
		protected override string ToString(int tokenType)
		{
			switch ((TT) tokenType) {
			case TT.Id:     return "identifier";
			case TT.Num:    return "number";
			case TT.Set:    return "':='";
			case TT.LParen: return "'('";
			case TT.RParen: return "')'";
			default:        return ((TokenType) tokenType).ToString();
			}
		}

		#endregion
	}
}
