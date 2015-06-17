// Generated from MyParserGrammar.ecs by LeMP custom tool. LLLPG version: 1.3.1.0
// Note: you can give command-line arguments to the tool via 'Custom Tool Namespace':
// --no-out-header       Suppress this message
// --verbose             Allow verbose messages (shown by VS as 'warnings')
// --macros=FileName.dll Load macros from FileName.dll, path relative to this file 
// Use #importMacros to use macros in a given namespace, e.g. #importMacros(Loyc.LLPG);
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
			#line 33 "MyParserGrammar.ecs"
			return e;
			#line default
		}
		LNode Expr()
		{
			TokenType la0;
			var e = AddExpr();
			// Line 37: (TT.Set Expr)?
			la0 = (TokenType) LA0;
			if (la0 == TT.Set) {
				Skip();
				var rhs = Expr();
				#line 37 "MyParserGrammar.ecs"
				e = BinOp(S.Assign, e, rhs);
				#line default
			}
			#line 38 "MyParserGrammar.ecs"
			return e;
			#line default
		}
		LNode AddExpr()
		{
			TokenType la0;
			var e = MulExpr();
			// Line 43: ((TT.Add|TT.Sub) MulExpr)*
			for (;;) {
				la0 = (TokenType) LA0;
				if (la0 == TT.Add || la0 == TT.Sub) {
					var op = MatchAny();
					var rhs = MulExpr();
					#line 43 "MyParserGrammar.ecs"
					e = BinOp((Symbol) op.Value, e, rhs);
					#line default
				} else
					break;
			}
			#line 44 "MyParserGrammar.ecs"
			return e;
			#line default
		}
		LNode MulExpr()
		{
			TokenType la0;
			var e = PrefixExpr();
			// Line 49: ((TT.Div|TT.Mul) PrefixExpr)*
			for (;;) {
				la0 = (TokenType) LA0;
				if (la0 == TT.Div || la0 == TT.Mul) {
					var op = MatchAny();
					var rhs = PrefixExpr();
					#line 50 "MyParserGrammar.ecs"
					e = BinOp((Symbol) op.Value, e, rhs);
					#line default
				} else
					break;
			}
			#line 52 "MyParserGrammar.ecs"
			return e;
			#line default
		}
		LNode PrefixExpr()
		{
			TokenType la0;
			// Line 56: (TT.Sub Term | Term)
			la0 = (TokenType) LA0;
			if (la0 == TT.Sub) {
				var minus = MatchAny();
				var e = Term();
				#line 56 "MyParserGrammar.ecs"
				return F.Call(S.Sub, e, minus.StartIndex, e.Range.EndIndex);
				#line default
			} else {
				var e = Term();
				#line 57 "MyParserGrammar.ecs"
				return e;
				#line default
			}
		}
		LNode Term()
		{
			TokenType la0;
			var e = Atom();
			// Line 63: (Atom)*
			for (;;) {
				la0 = (TokenType) LA0;
				if (la0 == TT.Id || la0 == TT.LParen || la0 == TT.Num) {
					var rest = Atom();
					#line 63 "MyParserGrammar.ecs"
					e = BinOp(S.Mul, e, rest);
					#line default
				} else
					break;
			}
			#line 64 "MyParserGrammar.ecs"
			return e;
			#line default
		}
		LNode Atom()
		{
			TokenType la0, la1;
			#line 68 "MyParserGrammar.ecs"
			LNode r;
			#line default
			// Line 69: ( TT.Id | TT.Num | TT.LParen Expr TT.RParen )
			la0 = (TokenType) LA0;
			if (la0 == TT.Id) {
				var t = MatchAny();
				#line 69 "MyParserGrammar.ecs"
				r = F.Id(t);
				#line default
			} else if (la0 == TT.Num) {
				var t = MatchAny();
				#line 70 "MyParserGrammar.ecs"
				r = F.Literal(t);
				#line default
			} else if (la0 == TT.LParen) {
				Skip();
				r = Expr();
				Match((int) TT.RParen);
			} else {
				#line 72 "MyParserGrammar.ecs"
				r = F._Missing;
				#line 72 "MyParserGrammar.ecs"
				Error(0, "Expected identifer, number, or (parens)");
				#line default
			}
			// Line 75: greedy(TT.Exp Atom)*
			for (;;) {
				la0 = (TokenType) LA0;
				if (la0 == TT.Exp) {
					la1 = (TokenType) LA(1);
					if (la1 == TT.Id || la1 == TT.LParen || la1 == TT.Num) {
						Skip();
						var e = Atom();
						#line 76 "MyParserGrammar.ecs"
						r = BinOp(S.XorBits, r, e);
						#line default
					} else
						break;
				} else
					break;
			}
			#line 78 "MyParserGrammar.ecs"
			return r;
			#line default
		}
	}
}
