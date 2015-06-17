using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loyc;
using Loyc.Collections;
using Loyc.Syntax.Lexing;
using Loyc.Syntax;

namespace MyLanguage
{
	using TT = TokenType;
	using System.Diagnostics;  // Abbreviate TokenType as TT

	public enum TokenType
	{
		EOF        = 0,
		Spaces     = TokenKind.Spaces + 1,
		Newline    = TokenKind.Spaces + 2,
		Id         = TokenKind.Id,
		Num        = TokenKind.Number,
		Set        = TokenKind.Assignment,
		Mul        = TokenKind.Operator + 1,
		Div        = TokenKind.Operator + 2,
		Add        = TokenKind.Operator + 3,
		Sub        = TokenKind.Operator + 4,
		Exp        = TokenKind.Operator + 5,
		LParen     = TokenKind.LParen,
		RParen     = TokenKind.RParen,
		Unknown    = TokenKind.Other,
		Semicolon  = TokenKind.Separator,
	}
	public static class TokenExt
	{
		[DebuggerStepThrough]
		public static TokenType Type(this Token t) { return (TokenType)t.TypeInt; }
	}

	partial class MyLexer : BaseLexer
	{
		public MyLexer(string text, string fileName = "")
			: this((StringSlice)text, fileName) { }
		public MyLexer(ICharSource text, string fileName = "")
			: base(text, fileName) { }

		public new ISourceFile SourceFile { get { return base.SourceFile; } }

		protected override void Error(int lookahead, string message)
		{
			Console.WriteLine("At index {0}: {1}", InputPosition + lookahead, message);
		}
	}
}
