using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loyc.Syntax.Lexing;
using System.Diagnostics;

namespace Ecs.Parser
{
	using TT = TokenType;
	using Loyc;

	public enum TokenType
	{
		EOF       = 0,
		Spaces    = TokenKind.Spaces + 1,
		Newline   = TokenKind.Spaces + 2,
		SLComment = TokenKind.Comment,
		MLComment = TokenKind.Comment + 1,
		Shebang   = TokenKind.Comment + 2,
		Id        = TokenKind.Id,
		// var, dynamic, trait, alias, where, assembly, module.
		// Does not include partial, because any Id can be a word attribute.
		ContextualKeyword = TokenKind.Id + 1,
		@base     = TokenKind.Id + 2,
		@this     = TokenKind.Id + 3,
		Number    = TokenKind.Number,
		String    = TokenKind.String,
		SQString  = TokenKind.String + 1,
		OtherLit  = TokenKind.OtherLit,
		Symbol    = TokenKind.OtherLit + 1,
		Comma     = TokenKind.Separator,
		Semicolon = TokenKind.Separator + 1,
		LParen    = TokenKind.LParen,
		RParen    = TokenKind.RParen,
		LBrack    = TokenKind.LBrack,
		RBrack    = TokenKind.RBrack,
		LBrace    = TokenKind.LBrace,
		RBrace    = TokenKind.RBrace,
		AttrKeyword = TokenKind.AttrKeyword,
		TypeKeyword = TokenKind.TypeKeyword,
		
		@break    = TokenKind.OtherKeyword + 1,
		@case     = TokenKind.OtherKeyword + 2,
		@checked  = TokenKind.OtherKeyword + 3,
		@class    = TokenKind.OtherKeyword + 4,
		@continue = TokenKind.OtherKeyword + 5,
		@default  = TokenKind.OtherKeyword + 6,
		@delegate = TokenKind.OtherKeyword + 7,
		@do       = TokenKind.OtherKeyword + 8,
		@enum     = TokenKind.OtherKeyword + 9,
		@event    = TokenKind.OtherKeyword + 10,
		@fixed    = TokenKind.OtherKeyword + 11,
		@for      = TokenKind.OtherKeyword + 12,
		@foreach  = TokenKind.OtherKeyword + 13,
		@goto     = TokenKind.OtherKeyword + 14,
		@if       = TokenKind.OtherKeyword + 15,
		@interface= TokenKind.OtherKeyword + 16,
		@lock     = TokenKind.OtherKeyword + 17,
		@namespace= TokenKind.OtherKeyword + 18,
		@return   = TokenKind.OtherKeyword + 19,
		@struct   = TokenKind.OtherKeyword + 20,
		@switch   = TokenKind.OtherKeyword + 21,
		@throw    = TokenKind.OtherKeyword + 22,
		@try      = TokenKind.OtherKeyword + 23,
		@unchecked= TokenKind.OtherKeyword + 24,
		@using    = TokenKind.OtherKeyword + 25,
		@while    = TokenKind.OtherKeyword + 26,

		@operator = TokenKind.OtherKeyword + 32,
		@sizeof   = TokenKind.OtherKeyword + 33,
		@typeof   = TokenKind.OtherKeyword + 34,

		@else     = TokenKind.OtherKeyword + 40,
		@catch    = TokenKind.OtherKeyword + 41,
		@finally  = TokenKind.OtherKeyword + 42,

		@in       = TokenKind.OtherKeyword + 48,
		@as       = TokenKind.OtherKeyword + 49,
		@is       = TokenKind.OtherKeyword + 50,

		@new       = TokenKind.OtherKeyword + 56,
		@out       = TokenKind.OtherKeyword + 57,
		@stackalloc= TokenKind.OtherKeyword + 58,

		PPif       = TokenKind.Other + 64,
		PPelse     = TokenKind.Other + 65,
		PPelif     = TokenKind.Other + 66,
		PPendif    = TokenKind.Other + 67,
		PPdefine   = TokenKind.Other + 68,
		PPundef    = TokenKind.Other + 69,
		PPwarning  = TokenKind.Other + 70,
		PPerror    = TokenKind.Other + 71,
		PPnote     = TokenKind.Other + 72,
		PPline     = TokenKind.Other + 73,
		PPregion   = TokenKind.Other + 74,
		PPendregion= TokenKind.Other + 75,
		PPpragma   = TokenKind.Other + 76,
		PPignored  = TokenKind.Other + 77, // covers one or more lines ignored by #if/#elif/#else.

		Dot          = TokenKind.Dot,     // .
		PtrArrow     = TokenKind.Dot + 1, // ->
		ColonColon   = TokenKind.Dot + 2, // ::
		NullDot      = TokenKind.Dot + 3, // ?.

		Set         = TokenKind.Assignment, // =
		CompoundSet = TokenKind.Assignment + 1, // +=, *=, >>=, etc.
		QuickBindSet = TokenKind.Assignment + 2, // :=

		// Operators: Different operators that are used in the same way and have
		// the same precence may be grouped into a single TokenType. There is
		// no token type for >> or << because these are formed from two > or <
		// tokens.
		Colon     = TokenKind.Operator,     // :
		At        = TokenKind.Operator + 1, // @
		BQString  = TokenKind.Operator + 2, // `...`
		Backslash = TokenKind.Operator + 4, // \
		Mul       = TokenKind.Operator + 5, // *
		DivMod       = TokenKind.Operator + 6, // / %
		Power     = TokenKind.Operator + 7, // **   (can also represent double-deref: (**x))
		Add       = TokenKind.Operator + 8, // +
		Sub       = TokenKind.Operator + 9, // -
		IncDec    = TokenKind.Operator + 10, // ++ --
		And       = TokenKind.Operator + 11, // &&
		OrXor     = TokenKind.Operator + 12, // || ^^
		Not       = TokenKind.Operator + 14, // !
		AndBits   = TokenKind.Operator + 15, // &
		OrBits    = TokenKind.Operator + 16, // |
		XorBits   = TokenKind.Operator + 17, // ^
		NotBits   = TokenKind.Operator + 18, // ~
		EqNeq     = TokenKind.Operator + 19, // == !=
		LT        = TokenKind.Operator + 20, // <
		GT        = TokenKind.Operator + 21, // >
		LEGE      = TokenKind.Operator + 22, // <= >=
		DotDot       = TokenKind.Operator + 23, // ..
		QuestionMark = TokenKind.Operator + 24, // ?
		NullCoalesce = TokenKind.Operator + 25, // ??
		QuickBind    = TokenKind.Operator + 26, // =:
		Forward      = TokenKind.Operator + 27, // ==>
		Substitute   = TokenKind.Operator + 28, // $
		LambdaArrow  = TokenKind.Operator + 29, // =>
		
		Unknown = TokenKind.Other, // invalid input
		//Indent = TokenKind.LBrace + 1,
		//Dedent = TokenKind.RBrace + 1,
	}

	/// <summary>Provides the <c>Type()</c> extension method required by 
	/// <see cref="Token"/> and the ToString(Token) method to express an EC# token
	/// as a string, for tokens that contain sufficient information to do so.</summary>
	public static class TokenExt
	{
		/// <summary>Converts <c>t.TypeInt</c> to <see cref="TokenType"/>.</summary>
		[DebuggerStepThrough]
		public static TokenType Type(this Token t) { return (TokenType)t.TypeInt; }
	}
}
