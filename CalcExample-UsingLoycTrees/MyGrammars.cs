// Generated from MyGrammars.ecs by LeMP custom tool. LLLPG version: 1.3.2.0
// Note: you can give command-line arguments to the tool via 'Custom Tool Namespace':
// --no-out-header       Suppress this message
// --verbose             Allow verbose messages (shown by VS as 'warnings')
// --macros=FileName.dll Load macros from FileName.dll, path relative to this file 
// Use #importMacros to use macros in a given namespace, e.g. #importMacros(Loyc.LLPG);
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Loyc;
using Loyc.Collections;
using Loyc.Syntax.Lexing;
using Loyc.Syntax;
namespace MyLanguage
{
	using TT = TokenType;
	using S = CodeSymbols;
	public enum TokenType
	{
		EOF = 0, Spaces = TokenKind.Spaces + 1, Newline = TokenKind.Spaces + 2, Id = TokenKind.Id, Num = TokenKind.Number, Shr = TokenKind.Operator + 1, Shl = TokenKind.Operator + 2, LE = TokenKind.Operator + 3, GE = TokenKind.Operator + 4, Eq = TokenKind.Operator + 5, Neq = TokenKind.Operator + 6, GT = TokenKind.Operator + 7, LT = TokenKind.Operator + 8, AndBits = TokenKind.Operator + 14, OrBits = TokenKind.Operator + 15, Assign = TokenKind.Assignment, Exp = TokenKind.Operator + 9, Mul = TokenKind.Operator + 10, Div = TokenKind.Operator + 11, Add = TokenKind.Operator + 12, Sub = TokenKind.Operator + 13, LParen = TokenKind.LParen, RParen = TokenKind.RParen, Semicolon = TokenKind.Separator, Unknown
	}
	public static class TokenExt
	{
		[DebuggerStepThrough] public static TokenType Type(this Token t)
		{
			return (TokenType) t.TypeInt;
		}
	}
	partial class MyLexer : BaseILexer<ICharSource,Token>
	{
		public MyLexer(UString text, string fileName = "") : this((ICharSource) text, fileName)
		{
		}
		public MyLexer(ICharSource text, string fileName = "") : base(text, fileName)
		{
		}
		public new ISourceFile SourceFile
		{
			get {
				return base.SourceFile;
			}
		}
		TokenType _type;
		object _value;
		int _startIndex;
		public override Maybe<Token> NextToken()
		{
			int la0, la1;
			_startIndex = InputPosition;
			_value = null;
			// Line 101: ( (Num | Id | Newline | [\t ] ([\t ])*) | ([>] [>] / [<] [<] / [<] [=] / [>] [=] / [=] [=] / [!] [=] / [>] / [<] / [&] / [|] / [=] / [\^] / [*] / [/] / [+] / [\-] / [(] / [)] / [;]) )
			la0 = LA0;
			switch (la0) {
			case '.':
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				{
					#line 101 "MyGrammars.ecs"
					_type = TT.Num;
					#line default
					Num();
				}
				break;
			case '\n':
			case '\r':
				{
					#line 103 "MyGrammars.ecs"
					_type = TT.Newline;
					#line default
					Newline();
				}
				break;
			case '\t':
			case ' ':
				{
					#line 104 "MyGrammars.ecs"
					_type = TT.Spaces;
					#line default
					Skip();
					// Line 104: ([\t ])*
					for (;;) {
						la0 = LA0;
						if (la0 == '\t' || la0 == ' ')
							Skip();
						else
							break;
					}
				}
				break;
			case '>':
				{
					la1 = LA(1);
					if (la1 == '>') {
						Skip();
						Skip();
						_type = TT.Shr;
						_value = S.Shr;
					} else if (la1 == '=') {
						Skip();
						Skip();
						_type = TT.GE;
						_value = S.GE;
					} else {
						Skip();
						_type = TT.GT;
						_value = S.GT;
					}
				}
				break;
			case '<':
				{
					la1 = LA(1);
					if (la1 == '<') {
						Skip();
						Skip();
						_type = TT.Shl;
						_value = S.Shl;
					} else if (la1 == '=') {
						Skip();
						Skip();
						_type = TT.LE;
						_value = S.LE;
					} else {
						Skip();
						_type = TT.LT;
						_value = S.LT;
					}
				}
				break;
			case '=':
				{
					la1 = LA(1);
					if (la1 == '=') {
						Skip();
						Skip();
						_type = TT.Eq;
						_value = S.Eq;
					} else {
						Skip();
						_type = TT.Assign;
						_value = S.Assign;
					}
				}
				break;
			case '!':
				{
					Skip();
					Match('=');
					_type = TT.Neq;
					_value = S.Neq;
				}
				break;
			case '&':
				{
					Skip();
					_type = TT.AndBits;
					_value = S.AndBits;
				}
				break;
			case '|':
				{
					Skip();
					_type = TT.OrBits;
					_value = S.OrBits;
				}
				break;
			case '^':
				{
					Skip();
					_type = TT.Exp;
					_value = S.Exp;
				}
				break;
			case '*':
				{
					Skip();
					_type = TT.Mul;
					_value = S.Mul;
				}
				break;
			case '/':
				{
					Skip();
					_type = TT.Div;
					_value = S.Div;
				}
				break;
			case '+':
				{
					Skip();
					_type = TT.Add;
					_value = S.Add;
				}
				break;
			case '-':
				{
					Skip();
					_type = TT.Sub;
					_value = S.Sub;
				}
				break;
			case '(':
				{
					Skip();
					_type = TT.LParen;
					_value = null;
				}
				break;
			case ')':
				{
					Skip();
					_type = TT.RParen;
					_value = null;
				}
				break;
			case ';':
				{
					Skip();
					_type = TT.Semicolon;
					_value = S.Semicolon;
				}
				break;
			default:
				if (la0 >= 'A' && la0 <= 'Z' || la0 == '_' || la0 >= 'a' && la0 <= 'z') {
					#line 102 "MyGrammars.ecs"
					_type = TT.Id;
					#line default
					Id();
				} else {
					#line 106 "MyGrammars.ecs"
					return NoValue.Value;
					#line default
					// Line 107: ([^\$])?
					la0 = LA0;
					if (la0 != -1) {
						Skip();
						#line 107 "MyGrammars.ecs"
						_type = TT.Unknown;
						#line default
					}
				}
				break;
			}
			#line 109 "MyGrammars.ecs"
			return new Token((int) _type, _startIndex, InputPosition - _startIndex, NodeStyle.Default, _value);
			#line default
		}
		static readonly HashSet<int> Id_set0 = NewSetOfRanges('0', '9', 'A', 'Z', '_', '_', 'a', 'z');
		void Id()
		{
			int la0;
			Skip();
			// Line 117: ([0-9A-Z_a-z])*
			for (;;) {
				la0 = LA0;
				if (Id_set0.Contains(la0))
					Skip();
				else
					break;
			}
			#line 118 "MyGrammars.ecs"
			_value = (Symbol) (CharSource.Slice(_startIndex, InputPosition - _startIndex).ToString());
			#line default
		}
		void Num()
		{
			int la0, la1;
			#line 122 "MyGrammars.ecs"
			bool dot = false;
			#line default
			// Line 123: ([.])?
			la0 = LA0;
			if (la0 == '.') {
				Skip();
				#line 123 "MyGrammars.ecs"
				dot = true;
				#line default
			}
			MatchRange('0', '9');
			// Line 124: ([0-9])*
			for (;;) {
				la0 = LA0;
				if (la0 >= '0' && la0 <= '9')
					Skip();
				else
					break;
			}
			// Line 125: (&!{dot} [.] [0-9] ([0-9])*)?
			la0 = LA0;
			if (la0 == '.') {
				if (!dot) {
					la1 = LA(1);
					if (la1 >= '0' && la1 <= '9') {
						Skip();
						Skip();
						// Line 125: ([0-9])*
						for (;;) {
							la0 = LA0;
							if (la0 >= '0' && la0 <= '9')
								Skip();
							else
								break;
						}
					}
				}
			}
			#line 126 "MyGrammars.ecs"
			_value = double.Parse(CharSource.Slice(_startIndex, InputPosition - _startIndex).ToString());
			#line default
		}
	}
	public partial class MyParser : BaseParserForList<Token,int>
	{
		public static MyParser New(UString input)
		{
			var lexer = new MyLexer(input);
			var tokens = new List<Token>();
			for (var next = lexer.NextToken(); next.HasValue; next = lexer.NextToken())
				if (next.Value.Kind != TokenKind.Spaces)
					tokens.Add(next.Value);
			return new MyParser(tokens, lexer.SourceFile);
		}
		public MyParser(IList<Token> list, ISourceFile file, int startIndex = 0) : base(list, new Token((int) TT.EOF, 0, 0, null), file, startIndex)
		{
			F = new LNodeFactory(file);
		}
		protected override string ToString(int tokenType)
		{
			return ((TokenType) tokenType).ToString();
		}
		LNodeFactory F;
		LNode BinOp(Symbol type, LNode lhs, LNode rhs)
		{
			return F.Call(type, lhs, rhs, lhs.Range.StartIndex, rhs.Range.EndIndex);
		}
		public LNode Start()
		{
			LNode result = default(LNode);
			result = Expr();
			Match((int) EOF);
			return result;
		}
		public LNode Expr(int prec = 0)
		{
			LNode result = default(LNode);
			result = PrefixExpr();
			// Line 195: greedy( &{prec <= 10} TT.Assign Expr | &{prec < 20} (TT.AndBits|TT.OrBits) Expr | &{prec < 30} (TT.Eq|TT.GE|TT.GT|TT.LE|TT.LT|TT.Neq) Expr | &{prec < 40} (TT.Add|TT.Sub) Expr | &{prec < 50} (TT.Div|TT.Mul|TT.Shl|TT.Shr) Expr )*
			for (;;) {
				switch ((TokenType) LA0) {
				case TT.Assign:
					{
						if (prec <= 10) {
							switch ((TokenType) LA(1)) {
							case TT.Id:
							case TT.LParen:
							case TT.Num:
							case TT.Sub:
								{
									var op = MatchAny();
									var r = Expr(10);
									#line 197 "MyGrammars.ecs"
									result = BinOp((Symbol) op.Value, result, r);
									#line default
								}
								break;
							default:
								goto stop;
							}
						} else
							goto stop;
					}
					break;
				case TT.AndBits:
				case TT.OrBits:
					{
						if (prec < 20) {
							switch ((TokenType) LA(1)) {
							case TT.Id:
							case TT.LParen:
							case TT.Num:
							case TT.Sub:
								{
									var op = MatchAny();
									var r = Expr(20);
									#line 200 "MyGrammars.ecs"
									result = BinOp((Symbol) op.Value, result, r);
									#line default
								}
								break;
							default:
								goto stop;
							}
						} else
							goto stop;
					}
					break;
				case TT.Eq:
				case TT.GE:
				case TT.GT:
				case TT.LE:
				case TT.LT:
				case TT.Neq:
					{
						if (prec < 30) {
							switch ((TokenType) LA(1)) {
							case TT.Id:
							case TT.LParen:
							case TT.Num:
							case TT.Sub:
								{
									var op = MatchAny();
									var r = Expr(30);
									#line 203 "MyGrammars.ecs"
									result = BinOp((Symbol) op.Value, result, r);
									#line default
								}
								break;
							default:
								goto stop;
							}
						} else
							goto stop;
					}
					break;
				case TT.Add:
				case TT.Sub:
					{
						if (prec < 40) {
							switch ((TokenType) LA(1)) {
							case TT.Id:
							case TT.LParen:
							case TT.Num:
							case TT.Sub:
								{
									var op = MatchAny();
									var r = Expr(40);
									#line 206 "MyGrammars.ecs"
									result = BinOp((Symbol) op.Value, result, r);
									#line default
								}
								break;
							default:
								goto stop;
							}
						} else
							goto stop;
					}
					break;
				case TT.Div:
				case TT.Mul:
				case TT.Shl:
				case TT.Shr:
					{
						if (prec < 50) {
							switch ((TokenType) LA(1)) {
							case TT.Id:
							case TT.LParen:
							case TT.Num:
							case TT.Sub:
								{
									var op = MatchAny();
									var r = Expr(50);
									#line 209 "MyGrammars.ecs"
									result = BinOp((Symbol) op.Value, result, r);
									#line default
								}
								break;
							default:
								goto stop;
							}
						} else
							goto stop;
					}
					break;
				default:
					goto stop;
				}
			}
		stop:;
			return result;
		}
		LNode PrefixExpr()
		{
			TokenType la0;
			LNode got_Term = default(LNode);
			// Line 214: (TT.Sub Term | Term)
			la0 = (TokenType) LA0;
			if (la0 == TT.Sub) {
				var minus = MatchAny();
				got_Term = Term();
				#line 214 "MyGrammars.ecs"
				return F.Call(S.Sub, got_Term, minus.StartIndex, got_Term.Range.EndIndex);
				#line default
			} else {
				got_Term = Term();
				#line 215 "MyGrammars.ecs"
				return got_Term;
				#line default
			}
		}
		LNode Term()
		{
			TokenType la0;
			LNode result = default(LNode);
			result = Atom();
			// Line 221: (Atom)*
			for (;;) {
				la0 = (TokenType) LA0;
				if (la0 == TT.Id || la0 == TT.LParen || la0 == TT.Num) {
					var rest = Atom();
					#line 221 "MyGrammars.ecs"
					result = BinOp(S.Mul, result, rest);
					#line default
				} else
					break;
			}
			return result;
		}
		LNode Atom()
		{
			TokenType la0, la1;
			LNode result = default(LNode);
			// Line 225: ( TT.Id | TT.Num | TT.LParen Expr TT.RParen )
			la0 = (TokenType) LA0;
			if (la0 == TT.Id) {
				var t = MatchAny();
				#line 225 "MyGrammars.ecs"
				result = F.Id(t);
				#line default
			} else if (la0 == TT.Num) {
				var t = MatchAny();
				#line 226 "MyGrammars.ecs"
				result = F.Literal(t);
				#line default
			} else if (la0 == TT.LParen) {
				Skip();
				result = Expr();
				Match((int) TT.RParen);
				#line 227 "MyGrammars.ecs"
				result = F.InParens(result);
				#line default
			} else {
				#line 228 "MyGrammars.ecs"
				result = F._Missing;
				#line 228 "MyGrammars.ecs"
				Error(0, "Expected identifer, number, or (parens)");
				#line default
			}
			// Line 231: greedy(TT.Exp Atom)*
			for (;;) {
				la0 = (TokenType) LA0;
				if (la0 == TT.Exp) {
					la1 = (TokenType) LA(1);
					if (la1 == TT.Id || la1 == TT.LParen || la1 == TT.Num) {
						Skip();
						var e = Atom();
						#line 232 "MyGrammars.ecs"
						result = BinOp(S.XorBits, result, e);
						#line default
					} else
						break;
				} else
					break;
			}
			return result;
		}
	}
}
