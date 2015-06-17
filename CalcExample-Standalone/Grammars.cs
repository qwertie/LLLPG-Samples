// Generated from Grammars.ecs by LeMP custom tool. LLLPG version: 1.3.1.0
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
	public struct Token
	{
		public TokenType Type;
		public object Value;
		public int StartIndex;
	}
	class CalculatorLexer : BaseLexer<List<char>>
	{
		public CalculatorLexer(List<char> charSource) : base(charSource)
		{
		}
		protected override void Error(int lookahead, string message)
		{
			Console.WriteLine("At index {0}: {1}", InputPosition + lookahead, message);
		}
		TokenType _type;
		object _value;
		int _startIndex;
		static string Substring(List<char> list, int start, int count)
		{
			var sb = new StringBuilder(count);
			for (int i = start; i < start + count; i++)
				sb.Append(list[i]);
			return sb.ToString();
		}
		static readonly HashSet<int> Id_set0 = NewSetOfRanges('0', '9', 'A', 'Z', '_', '_', 'a', 'z');
		void Id()
		{
			int la0;
			Skip();
			// Line 93: ([0-9A-Z_a-z])*
			for (;;) {
				la0 = LA0;
				if (Id_set0.Contains(la0))
					Skip();
				else
					break;
			}
			#line 94 "Grammars.ecs"
			_value = Substring(CharSource, _startIndex, InputPosition - _startIndex);
			#line default
		}
		void Num()
		{
			int la0, la1;
			#line 97 "Grammars.ecs"
			bool dot = false;
			#line default
			// Line 98: ([.])?
			la0 = LA0;
			if (la0 == '.') {
				Skip();
				#line 98 "Grammars.ecs"
				dot = true;
				#line default
			}
			MatchRange('0', '9');
			// Line 99: ([0-9])*
			for (;;) {
				la0 = LA0;
				if (la0 >= '0' && la0 <= '9')
					Skip();
				else
					break;
			}
			// Line 100: (&!{dot} [.] [0-9] ([0-9])*)?
			la0 = LA0;
			if (la0 == '.') {
				if (!dot) {
					la1 = LA(1);
					if (la1 >= '0' && la1 <= '9') {
						Skip();
						Skip();
						// Line 100: ([0-9])*
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
			#line 101 "Grammars.ecs"
			_value = double.Parse(Substring(CharSource, _startIndex, InputPosition - _startIndex));
			#line default
		}
		public Token NextToken()
		{
			int la0, la1;
			_startIndex = InputPosition;
			_value = null;
			// Line 115: ( Num | Id | [\^] | [*] | [/] | [+] | [\-] | [:] [=] | [.] [n] [a] [n] | [.] [i] [n] [f] | [(] | [)] | [\t ] )
			do {
				la0 = LA0;
				switch (la0) {
				case '.':
					{
						la1 = LA(1);
						if (la1 >= '0' && la1 <= '9')
							goto matchNum;
						else if (la1 == 'n') {
							#line 123 "Grammars.ecs"
							_type = TT.Num;
							#line default
							Skip();
							Skip();
							Match('a');
							Match('n');
							#line 123 "Grammars.ecs"
							_value = double.NaN;
							#line default
						} else if (la1 == 'i') {
							#line 124 "Grammars.ecs"
							_type = TT.Num;
							#line default
							Skip();
							Skip();
							Match('n');
							Match('f');
							#line 124 "Grammars.ecs"
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
						#line 117 "Grammars.ecs"
						_type = TT.Exp;
						#line default
						Skip();
					}
					break;
				case '*':
					{
						#line 118 "Grammars.ecs"
						_type = TT.Mul;
						#line default
						Skip();
					}
					break;
				case '/':
					{
						#line 119 "Grammars.ecs"
						_type = TT.Div;
						#line default
						Skip();
					}
					break;
				case '+':
					{
						#line 120 "Grammars.ecs"
						_type = TT.Add;
						#line default
						Skip();
					}
					break;
				case '-':
					{
						#line 121 "Grammars.ecs"
						_type = TT.Sub;
						#line default
						Skip();
					}
					break;
				case ':':
					{
						#line 122 "Grammars.ecs"
						_type = TT.Set;
						#line default
						Skip();
						Match('=');
					}
					break;
				case '(':
					{
						#line 125 "Grammars.ecs"
						_type = TT.LParen;
						#line default
						Skip();
					}
					break;
				case ')':
					{
						#line 126 "Grammars.ecs"
						_type = TT.RParen;
						#line default
						Skip();
					}
					break;
				case '\t':
				case ' ':
					{
						#line 127 "Grammars.ecs"
						_type = TT.Space;
						#line default
						Skip();
					}
					break;
				default:
					if (la0 >= 'A' && la0 <= 'Z' || la0 == '_' || la0 >= 'a' && la0 <= 'z') {
						#line 116 "Grammars.ecs"
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
					#line 115 "Grammars.ecs"
					_type = TT.Num;
					#line default
					Num();
				}
				break;
			error:
				{
					#line 129 "Grammars.ecs"
					_type = TT.EOF;
					#line default
					// Line 129: ([^\$])?
					la0 = LA0;
					if (la0 != -1) {
						Skip();
						#line 129 "Grammars.ecs"
						_type = TT.Unknown;
						#line default
					}
				}
			} while (false);
			#line 130 "Grammars.ecs"
			return new Token { 
				Type = _type, Value = _value, StartIndex = _startIndex
			};
			#line default
		}
	}
	public partial class Calculator : BaseParser<Token>
	{
		public Dictionary<string,double> Vars = new Dictionary<string,double>();
		List<Token> _tokens = new List<Token>();
		string _input;
		public double Calculate(string input)
		{
			_input = input;
			var lexer = new CalculatorLexer(input.ToList());
			_tokens.Clear();
			Token t;
			while (((t = lexer.NextToken()).Type != EOF)) {
				if ((t.Type != TT.Space))
					_tokens.Add(t);
			}
			InputPosition = 0;
			return Expr();
		}
		protected override int EofInt()
		{
			return (int) EOF;
		}
		protected override int LA0Int
		{
			get {
				return (int) LT0.Type;
			}
		}
		protected override Token LT(int i)
		{
			i += InputPosition;
			if (i < _tokens.Count) {
				return _tokens[i];
			} else {
				return new Token { 
					Type = EOF
				};
			}
		}
		protected override void Error(int lookahead, string message)
		{
			int tIndex = InputPosition + lookahead;
			int cIndex = _input.Length;
			if (tIndex < _tokens.Count)
				cIndex = _tokens[tIndex].StartIndex;
			throw new Exception(string.Format("Error at index {0}: {1}", cIndex, message));
		}
		protected override string ToString(int tokenType)
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
		const TokenType EOF = TT.EOF;
		TokenType LA0
		{
			get {
				return LT0.Type;
			}
		}
		TokenType LA(int offset)
		{
			return LT(offset).Type;
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
			#line 251 "Grammars.ecs"
			double result;
			#line default
			// Line 252: ( TT.Id | TT.Num | TT.LParen Expr TT.RParen )
			la0 = (TokenType) LA0;
			if (la0 == TT.Id) {
				var t = MatchAny();
				#line 252 "Grammars.ecs"
				result = Vars[(string) t.Value];
				#line default
			} else if (la0 == TT.Num) {
				var t = MatchAny();
				#line 253 "Grammars.ecs"
				result = (double) t.Value;
				#line default
			} else if (la0 == TT.LParen) {
				Skip();
				result = Expr();
				Match((int) TT.RParen);
			} else {
				#line 255 "Grammars.ecs"
				result = double.NaN;
				#line 255 "Grammars.ecs"
				Error(0, "Expected identifer, number, or (parens)");
				#line default
			}
			// Line 258: greedy(TT.Exp Atom)*
			for (;;) {
				la0 = (TokenType) LA0;
				if (la0 == TT.Exp) {
					la1 = (TokenType) LA(1);
					if (la1 == TT.Id || la1 == TT.LParen || la1 == TT.Num) {
						Skip();
						var exp = Atom();
						#line 258 "Grammars.ecs"
						result = Math.Pow(result, exp);
						#line default
					} else
						break;
				} else
					break;
			}
			#line 259 "Grammars.ecs"
			return result;
			#line default
		}
		double Term()
		{
			TokenType la0;
			var result = Atom();
			// Line 264: (Atom)*
			for (;;) {
				la0 = (TokenType) LA0;
				if (la0 == TT.Id || la0 == TT.LParen || la0 == TT.Num) {
					var rest = Atom();
					#line 264 "Grammars.ecs"
					result *= rest;
					#line default
				} else
					break;
			}
			#line 265 "Grammars.ecs"
			return result;
			#line default
		}
		double PrefixExpr()
		{
			TokenType la0;
			// Line 268: (TT.Sub Term | Term)
			la0 = (TokenType) LA0;
			if (la0 == TT.Sub) {
				Skip();
				var r = Term();
				#line 268 "Grammars.ecs"
				return -r;
				#line default
			} else {
				var r = Term();
				#line 269 "Grammars.ecs"
				return r;
				#line default
			}
		}
		double MulExpr()
		{
			TokenType la0;
			var result = PrefixExpr();
			// Line 273: ((TT.Div|TT.Mul) PrefixExpr)*
			for (;;) {
				la0 = (TokenType) LA0;
				if (la0 == TT.Div || la0 == TT.Mul) {
					var op = MatchAny();
					var rhs = PrefixExpr();
					#line 273 "Grammars.ecs"
					result = Do(result, op, rhs);
					#line default
				} else
					break;
			}
			#line 274 "Grammars.ecs"
			return result;
			#line default
		}
		double AddExpr()
		{
			TokenType la0;
			var result = MulExpr();
			// Line 278: ((TT.Add|TT.Sub) MulExpr)*
			for (;;) {
				la0 = (TokenType) LA0;
				if (la0 == TT.Add || la0 == TT.Sub) {
					var op = MatchAny();
					var rhs = MulExpr();
					#line 278 "Grammars.ecs"
					result = Do(result, op, rhs);
					#line default
				} else
					break;
			}
			#line 279 "Grammars.ecs"
			return result;
			#line default
		}
		double Expr()
		{
			TokenType la0, la1;
			#line 282 "Grammars.ecs"
			double result;
			#line default
			// Line 283: (TT.Id TT.Set Expr | AddExpr)
			la0 = (TokenType) LA0;
			if (la0 == TT.Id) {
				la1 = (TokenType) LA(1);
				if (la1 == TT.Set) {
					var t = MatchAny();
					Skip();
					result = Expr();
					#line 283 "Grammars.ecs"
					Vars[t.Value.ToString()] = result;
					#line default
				} else
					result = AddExpr();
			} else
				result = AddExpr();
			#line 285 "Grammars.ecs"
			return result;
			#line default
		}
	}
}
