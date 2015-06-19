// Generated from Grammars.ecs by LeMP custom tool. LLLPG version: 1.3.2.0
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
using Loyc.Syntax.Lexing;
using Loyc.Syntax;
namespace CalcExample
{
	using TT = TokenType;
	public enum TokenType
	{
		EOF = -1, Space, Id, Num, Set, Mul, Div, Add, Sub, Exp, LParen, RParen, Unknown
	}
	public struct Token : ISimpleToken<int>
	{
		public TokenType Type
		{
			get;
			set;
		}
		public object Value
		{
			get;
			set;
		}
		public int StartIndex
		{
			get;
			set;
		}
		int ISimpleToken<int>.Type
		{
			get {
				return (int) Type;
			}
		}
	}
	class CalculatorLexer
	{
		LexerSource _src;
		public CalculatorLexer(string charSource)
		{
			_src = new LexerSource(charSource);
		}
		TokenType _type;
		object _value;
		int _startIndex;
		static readonly HashSet<int> Id_set0 = LexerSource.NewSetOfRanges('0', '9', 'A', 'Z', '_', '_', 'a', 'z');
		void Id()
		{
			int la0;
			_src.Skip();
			// Line 93: ([0-9A-Z_a-z])*
			for (;;) {
				la0 = _src.LA0;
				if (Id_set0.Contains(la0))
					_src.Skip();
				else
					break;
			}
			#line 94 "Grammars.ecs"
			_value = _src.CharSource.Substring(_startIndex, _src.InputPosition - _startIndex);
			#line default
		}
		void Num()
		{
			int la0, la1;
			#line 97 "Grammars.ecs"
			bool dot = false;
			#line default
			// Line 98: ([.])?
			la0 = _src.LA0;
			if (la0 == '.') {
				_src.Skip();
				#line 98 "Grammars.ecs"
				dot = true;
				#line default
			}
			_src.MatchRange('0', '9');
			// Line 99: ([0-9])*
			for (;;) {
				la0 = _src.LA0;
				if (la0 >= '0' && la0 <= '9')
					_src.Skip();
				else
					break;
			}
			// Line 100: (&!{dot} [.] [0-9] ([0-9])*)?
			la0 = _src.LA0;
			if (la0 == '.') {
				if (!dot) {
					la1 = _src.LA(1);
					if (la1 >= '0' && la1 <= '9') {
						_src.Skip();
						_src.Skip();
						// Line 100: ([0-9])*
						for (;;) {
							la0 = _src.LA0;
							if (la0 >= '0' && la0 <= '9')
								_src.Skip();
							else
								break;
						}
					}
				}
			}
			#line 101 "Grammars.ecs"
			_value = double.Parse(_src.CharSource.Substring(_startIndex, _src.InputPosition - _startIndex));
			#line default
		}
		public Token NextToken()
		{
			int la0, la1;
			_startIndex = _src.InputPosition;
			_value = null;
			// Line 108: ( Num | Id | [\^] | [*] | [/] | [+] | [\-] | [:] [=] | [.] [n] [a] [n] | [.] [i] [n] [f] | [(] | [)] | [\t ] )
			do {
				la0 = _src.LA0;
				switch (la0) {
				case '.':
					{
						la1 = _src.LA(1);
						if (la1 >= '0' && la1 <= '9')
							goto matchNum;
						else if (la1 == 'n') {
							#line 116 "Grammars.ecs"
							_type = TT.Num;
							#line default
							_src.Skip();
							_src.Skip();
							_src.Match('a');
							_src.Match('n');
							#line 116 "Grammars.ecs"
							_value = double.NaN;
							#line default
						} else if (la1 == 'i') {
							#line 117 "Grammars.ecs"
							_type = TT.Num;
							#line default
							_src.Skip();
							_src.Skip();
							_src.Match('n');
							_src.Match('f');
							#line 117 "Grammars.ecs"
							_value = double.PositiveInfinity;
							#line default
						} else
							goto error;
					}
					break;
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
					goto matchNum;
				case '^':
					{
						#line 110 "Grammars.ecs"
						_type = TT.Exp;
						#line default
						_src.Skip();
					}
					break;
				case '*':
					{
						#line 111 "Grammars.ecs"
						_type = TT.Mul;
						#line default
						_src.Skip();
					}
					break;
				case '/':
					{
						#line 112 "Grammars.ecs"
						_type = TT.Div;
						#line default
						_src.Skip();
					}
					break;
				case '+':
					{
						#line 113 "Grammars.ecs"
						_type = TT.Add;
						#line default
						_src.Skip();
					}
					break;
				case '-':
					{
						#line 114 "Grammars.ecs"
						_type = TT.Sub;
						#line default
						_src.Skip();
					}
					break;
				case ':':
					{
						#line 115 "Grammars.ecs"
						_type = TT.Set;
						#line default
						_src.Skip();
						_src.Match('=');
					}
					break;
				case '(':
					{
						#line 118 "Grammars.ecs"
						_type = TT.LParen;
						#line default
						_src.Skip();
					}
					break;
				case ')':
					{
						#line 119 "Grammars.ecs"
						_type = TT.RParen;
						#line default
						_src.Skip();
					}
					break;
				case '\t':
				case ' ':
					{
						#line 120 "Grammars.ecs"
						_type = TT.Space;
						#line default
						_src.Skip();
					}
					break;
				default:
					if (la0 >= 'A' && la0 <= 'Z' || la0 == '_' || la0 >= 'a' && la0 <= 'z') {
						#line 109 "Grammars.ecs"
						_type = TT.Id;
						#line default
						Id();
					} else
						goto error;
					break;
				}
				break;
			matchNum:
				{
					#line 108 "Grammars.ecs"
					_type = TT.Num;
					#line default
					Num();
				}
				break;
			error:
				{
					#line 122 "Grammars.ecs"
					_type = TT.EOF;
					#line default
					// Line 123: ([^\$])?
					la0 = _src.LA0;
					if (la0 != -1) {
						#line 123 "Grammars.ecs"
						_type = TT.Unknown;
						#line 123 "Grammars.ecs"
						_src.Error(0, "Unexpected character");
						#line default
						_src.Skip();
					}
				}
			} while (false);
			#line 125 "Grammars.ecs"
			return new Token { 
				Type = _type, Value = _value, StartIndex = _startIndex
			};
			#line default
		}
	}
	public partial class Calculator
	{
		public ParserSource<Token> _src;
		public Dictionary<string,double> Vars = new Dictionary<string,double>();
		List<Token> _tokens = new List<Token>();
		public double Calculate(string input)
		{
			var lexer = new CalculatorLexer(input);
			_tokens.Clear();
			Token t;
			while (((t = lexer.NextToken()).Type != TT.EOF)) {
				if ((t.Type != TT.Space))
					_tokens.Add(t);
			}
			_src = new ParserSource<Token>(_tokens, new Token { 
				Type = TT.EOF
			}) { 
				TokenTypeToString = TokenTypeToString
			};
			return Expr();
		}
		string TokenTypeToString(int tokenType)
		{
			switch ((TT) tokenType) {
			case TT.Id:
				return "identifier";
			case TT.Num:
				return "number";
			case TT.Set:
				return "':='";
			case TT.LParen:
				return "'('";
			case TT.RParen:
				return "')'";
			default:
				return ((TokenType) tokenType).ToString();
			}
		}
		double Do(double left, Token op, double right)
		{
			switch (op.Type) {
			case TT.Add:
				return left + right;
			case TT.Sub:
				return left - right;
			case TT.Mul:
				return left * right;
			case TT.Div:
				return left / right;
			}
			return double.NaN;
		}
		double Atom()
		{
			TokenType la0, la1;
			#line 215 "Grammars.ecs"
			double result;
			#line default
			// Line 216: ( TT.Id | TT.Num | TT.LParen Expr TT.RParen )
			la0 = (TokenType) _src.LA0;
			if (la0 == TT.Id) {
				var t = _src.MatchAny();
				#line 216 "Grammars.ecs"
				result = Vars[(string) t.Value];
				#line default
			} else if (la0 == TT.Num) {
				var t = _src.MatchAny();
				#line 217 "Grammars.ecs"
				result = (double) t.Value;
				#line default
			} else if (la0 == TT.LParen) {
				_src.Skip();
				result = Expr();
				_src.Match((int) TT.RParen);
			} else {
				#line 219 "Grammars.ecs"
				result = double.NaN;
				#line 219 "Grammars.ecs"
				_src.Error(0, "Expected identifer, number, or (parens)");
				#line default
			}
			// Line 222: greedy(TT.Exp Atom)*
			for (;;) {
				la0 = (TokenType) _src.LA0;
				if (la0 == TT.Exp) {
					la1 = (TokenType) _src.LA(1);
					if (la1 == TT.Id || la1 == TT.LParen || la1 == TT.Num) {
						_src.Skip();
						var exp = Atom();
						#line 222 "Grammars.ecs"
						result = Math.Pow(result, exp);
						#line default
					} else
						break;
				} else
					break;
			}
			#line 223 "Grammars.ecs"
			return result;
			#line default
		}
		double Term()
		{
			TokenType la0;
			var result = Atom();
			// Line 228: (Atom)*
			for (;;) {
				la0 = (TokenType) _src.LA0;
				if (la0 == TT.Id || la0 == TT.LParen || la0 == TT.Num) {
					var rest = Atom();
					#line 228 "Grammars.ecs"
					result *= rest;
					#line default
				} else
					break;
			}
			#line 229 "Grammars.ecs"
			return result;
			#line default
		}
		double PrefixExpr()
		{
			TokenType la0;
			// Line 232: (TT.Sub Term | Term)
			la0 = (TokenType) _src.LA0;
			if (la0 == TT.Sub) {
				_src.Skip();
				var r = Term();
				#line 232 "Grammars.ecs"
				return -r;
				#line default
			} else {
				var r = Term();
				#line 233 "Grammars.ecs"
				return r;
				#line default
			}
		}
		double MulExpr()
		{
			TokenType la0;
			var result = PrefixExpr();
			// Line 237: ((TT.Div|TT.Mul) PrefixExpr)*
			for (;;) {
				la0 = (TokenType) _src.LA0;
				if (la0 == TT.Div || la0 == TT.Mul) {
					var op = _src.MatchAny();
					var rhs = PrefixExpr();
					#line 237 "Grammars.ecs"
					result = Do(result, op, rhs);
					#line default
				} else
					break;
			}
			#line 238 "Grammars.ecs"
			return result;
			#line default
		}
		double AddExpr()
		{
			TokenType la0;
			var result = MulExpr();
			// Line 242: ((TT.Add|TT.Sub) MulExpr)*
			for (;;) {
				la0 = (TokenType) _src.LA0;
				if (la0 == TT.Add || la0 == TT.Sub) {
					var op = _src.MatchAny();
					var rhs = MulExpr();
					#line 242 "Grammars.ecs"
					result = Do(result, op, rhs);
					#line default
				} else
					break;
			}
			#line 243 "Grammars.ecs"
			return result;
			#line default
		}
		double Expr()
		{
			TokenType la0, la1;
			#line 246 "Grammars.ecs"
			double result;
			#line default
			// Line 247: (TT.Id TT.Set Expr | AddExpr)
			la0 = (TokenType) _src.LA0;
			if (la0 == TT.Id) {
				la1 = (TokenType) _src.LA(1);
				if (la1 == TT.Set) {
					var t = _src.MatchAny();
					_src.Skip();
					result = Expr();
					#line 247 "Grammars.ecs"
					Vars[t.Value.ToString()] = result;
					#line default
				} else
					result = AddExpr();
			} else
				result = AddExpr();
			#line 249 "Grammars.ecs"
			return result;
			#line default
		}
	}
}
