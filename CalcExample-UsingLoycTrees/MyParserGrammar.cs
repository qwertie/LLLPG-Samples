// Generated from MyParserGrammar.ecs by LLLPG custom tool. LLLPG version: 1.1.0.0
// Note: you can give command-line arguments to the tool via 'Custom Tool Namespace':
// --macros=FileName.dll Load macros from FileName.dll, path relative to this file 
// --verbose             Allow verbose messages (shown as 'warnings')
// --no-out-header       Suppress this message
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Loyc;
using Loyc.Collections;
using Loyc.Syntax.Lexing;
using Loyc.Syntax;
namespace MyLanguage
{
	using TT = TokenType;
	using S = CodeSymbols;
	public partial class MyParser : BaseParser<Token>
	{
		LNode BinOp(Symbol type, LNode lhs, LNode rhs)
		{
			return F.Call(type, lhs, rhs, lhs.Range.StartIndex, rhs.Range.EndIndex);
		}
		public LNode Start()
		{
			var e = Expr();
			Match((int) EOF);
			return e;
		}
		LNode Expr()
		{
			TokenType la0;
			var e = AddExpr();
			// Line 37: (TT.Set Expr)?
			la0 = LA0;
			if (la0 == TT.Set) {
				Skip();
				var rhs = Expr();
				e = BinOp(S.Set, e, rhs);
			}
			return e;
		}
		LNode AddExpr()
		{
			TokenType la0;
			var e = MulExpr();
			// Line 43: ((TT.Add|TT.Sub) MulExpr)*
			 for (;;) {
				la0 = LA0;
				if (la0 == TT.Add || la0 == TT.Sub) {
					var op = MatchAny();
					var rhs = MulExpr();
					e = BinOp((Symbol) op.Value, e, rhs);
				} else
					break;
			}
			return e;
		}
		LNode MulExpr()
		{
			TokenType la0;
			var e = PrefixExpr();
			// Line 49: ((TT.Mul|TT.Div) PrefixExpr)*
			 for (;;) {
				la0 = LA0;
				if (la0 == TT.Div || la0 == TT.Mul) {
					var op = MatchAny();
					var rhs = PrefixExpr();
					e = BinOp((Symbol) op.Value, e, rhs);
				} else
					break;
			}
			return e;
		}
		LNode PrefixExpr()
		{
			TokenType la0;
			// Line 56: (TT.Sub Term | Term)
			la0 = LA0;
			if (la0 == TT.Sub) {
				var minus = MatchAny();
				var e = Term();
				return F.Call(S.Sub, e, minus.StartIndex, e.Range.EndIndex);
			} else {
				var e = Term();
				return e;
			}
		}
		LNode Term()
		{
			TokenType la0;
			var e = Atom();
			// Line 63: (Atom)*
			 for (;;) {
				la0 = LA0;
				if (la0 == TT.Id || la0 == TT.LParen || la0 == TT.Num) {
					var rest = Atom();
					e = BinOp(S.Mul, e, rest);
				} else
					break;
			}
			return e;
		}
		LNode Atom()
		{
			TokenType la0, la1;
			LNode r;
			// Line 69: ( TT.Id | TT.Num | TT.LParen Expr TT.RParen )
			la0 = LA0;
			if (la0 == TT.Id) {
				var t = MatchAny();
				r = F.Id(t);
			} else if (la0 == TT.Num) {
				var t = MatchAny();
				r = F.Literal(t);
			} else if (la0 == TT.LParen) {
				Skip();
				r = Expr();
				Match((int) TT.RParen);
			} else {
				r = F._Missing;
				Error(0, "Expected identifer, number, or (parens)");
			}
			// Line 75: greedy(TT.Exp Atom)*
			 for (;;) {
				la0 = LA0;
				if (la0 == TT.Exp) {
					la1 = LA(1);
					if (la1 == TT.Id || la1 == TT.LParen || la1 == TT.Num) {
						Skip();
						var e = Atom();
						r = BinOp(S.XorBits, r, e);
					} else
						break;
				} else
					break;
			}
			return r;
		}
	}
}
