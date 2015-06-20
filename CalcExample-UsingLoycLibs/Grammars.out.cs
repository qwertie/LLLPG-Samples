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
using Loyc.Collections;
using Loyc.Syntax.Lexing;
using Loyc.Syntax;
namespace CalcExample
{
	using TT = TokenType;
	public enum TokenType
	{
		EOF = 0, Id, Num, Shr, Shl, Assign, GT, LT, Exp, Mul, Div, Add, Sub, Semicolon, LParen, RParen, Unknown
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
	partial class CalculatorLexer : IEnumerator<Token>
	{
		public LexerSource Src
		{
			get;
			set;
		}
		public CalculatorLexer(string text, string fileName = "")
		{
			Src = (LexerSource) text;
		}
		public CalculatorLexer(ICharSource text, string fileName = "")
		{
			Src = new LexerSource(text);
		}
		Token _tok;
		public Token Current
		{
			get {
				return _tok;
			}
		}
		object System.Collections.IEnumerator.Current
		{
			get {
				return Current;
			}
		}
		void System.Collections.IEnumerator.Reset()
		{
			Src.Reset();
		}
		void IDisposable.Dispose()
		{
		}
		public bool MoveNext()
		{
			int la0, la1;
			// Line 160: ([\t ])*
			for (;;) {
				la0 = Src.LA0;
				if (la0 == '\t' || la0 == ' ')
					Src.Skip();
				else
					break;
			}
			_tok.StartIndex = Src.InputPosition;
			_tok.Value = null;
			// Line 163: ( (Num | Id | [.] [n] [a] [n] | [.] [i] [n] [f]) | ([>] [>] / [<] [<] / [=] / [>] / [<] / [\^] / [*] / [/] / [+] / [\-] / [;] / [(] / [)]) )
			do {
				la0 = Src.LA0;
				switch (la0) {
				case '.':
					{
						la1 = Src.LA(1);
						if (la1 >= '0' && la1 <= '9')
							goto matchNum;
						else if (la1 == 'n') {
							#line 165 "Grammars.ecs"
							_tok.Type = TT.Num;
							#line default
							Src.Skip();
							Src.Skip();
							Src.Match('a');
							Src.Match('n');
							#line 165 "Grammars.ecs"
							_tok.Value = double.NaN;
							#line default
						} else if (la1 == 'i') {
							#line 166 "Grammars.ecs"
							_tok.Type = TT.Num;
							#line default
							Src.Skip();
							Src.Skip();
							Src.Match('n');
							Src.Match('f');
							#line 166 "Grammars.ecs"
							_tok.Value = double.PositiveInfinity;
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
				case '>':
					{
						la1 = Src.LA(1);
						if (la1 == '>') {
							Src.Skip();
							Src.Skip();
							#line 197 "Grammars.ecs"
							_tok.Type = TT.Shr;
							#line default
						} else {
							Src.Skip();
							#line 197 "Grammars.ecs"
							_tok.Type = TT.GT;
							#line default
						}
					}
					break;
				case '<':
					{
						la1 = Src.LA(1);
						if (la1 == '<') {
							Src.Skip();
							Src.Skip();
							#line 197 "Grammars.ecs"
							_tok.Type = TT.Shl;
							#line default
						} else {
							Src.Skip();
							#line 197 "Grammars.ecs"
							_tok.Type = TT.LT;
							#line default
						}
					}
					break;
				case '=':
					{
						Src.Skip();
						#line 197 "Grammars.ecs"
						_tok.Type = TT.Assign;
						#line default
					}
					break;
				case '^':
					{
						Src.Skip();
						#line 197 "Grammars.ecs"
						_tok.Type = TT.Exp;
						#line default
					}
					break;
				case '*':
					{
						Src.Skip();
						#line 197 "Grammars.ecs"
						_tok.Type = TT.Mul;
						#line default
					}
					break;
				case '/':
					{
						Src.Skip();
						#line 197 "Grammars.ecs"
						_tok.Type = TT.Div;
						#line default
					}
					break;
				case '+':
					{
						Src.Skip();
						#line 197 "Grammars.ecs"
						_tok.Type = TT.Add;
						#line default
					}
					break;
				case '-':
					{
						Src.Skip();
						#line 197 "Grammars.ecs"
						_tok.Type = TT.Sub;
						#line default
					}
					break;
				case ';':
					{
						Src.Skip();
						#line 197 "Grammars.ecs"
						_tok.Type = TT.Semicolon;
						#line default
					}
					break;
				case '(':
					{
						Src.Skip();
						#line 197 "Grammars.ecs"
						_tok.Type = TT.LParen;
						#line default
					}
					break;
				case ')':
					{
						Src.Skip();
						#line 197 "Grammars.ecs"
						_tok.Type = TT.RParen;
						#line default
					}
					break;
				default:
					if (la0 >= 'A' && la0 <= 'Z' || la0 == '_' || la0 >= 'a' && la0 <= 'z') {
						#line 164 "Grammars.ecs"
						_tok.Type = TT.Id;
						#line default
						Id();
					} else
						goto error;
					break;
				}
				break;
			matchNum:
				{
					#line 163 "Grammars.ecs"
					_tok.Type = TT.Num;
					#line default
					Num();
				}
				break;
			error:
				{
					#line 169 "Grammars.ecs"
					_tok.Type = TT.EOF;
					#line default
					// Line 169: ([^\$])?
					la0 = Src.LA0;
					if (la0 != -1) {
						Src.Skip();
						#line 169 "Grammars.ecs"
						_tok.Type = TT.Unknown;
						#line default
					}
				}
			} while (false);
			#line 171 "Grammars.ecs"
			return _tok.Type != TT.EOF;
			#line default
		}
		static readonly HashSet<int> Id_set0 = LexerSource.NewSetOfRanges('0', '9', 'A', 'Z', '_', '_', 'a', 'z');
		void Id()
		{
			int la0;
			Src.Skip();
			// Line 176: ([0-9A-Z_a-z])*
			for (;;) {
				la0 = Src.LA0;
				if (Id_set0.Contains(la0))
					Src.Skip();
				else
					break;
			}
			#line 177 "Grammars.ecs"
			_tok.Value = Src.CharSource.Slice(_tok.StartIndex, Src.InputPosition - _tok.StartIndex).ToString();
			#line default
		}
		void Num()
		{
			int la0, la1;
			int dot = 0;
			// Line 180: ([.])?
			la0 = Src.LA0;
			if (la0 == '.')
				dot = Src.MatchAny();
			Src.MatchRange('0', '9');
			// Line 181: ([0-9])*
			for (;;) {
				la0 = Src.LA0;
				if (la0 >= '0' && la0 <= '9')
					Src.Skip();
				else
					break;
			}
			// Line 182: (&{dot == 0} [.] [0-9] ([0-9])*)?
			la0 = Src.LA0;
			if (la0 == '.') {
				if (dot == 0) {
					la1 = Src.LA(1);
					if (la1 >= '0' && la1 <= '9') {
						Src.Skip();
						Src.Skip();
						// Line 182: ([0-9])*
						for (;;) {
							la0 = Src.LA0;
							if (la0 >= '0' && la0 <= '9')
								Src.Skip();
							else
								break;
						}
					}
				}
			}
			#line 183 "Grammars.ecs"
			_tok.Value = double.Parse(Src.CharSource.Slice(_tok.StartIndex, Src.InputPosition - _tok.StartIndex).ToString());
			#line default
		}
	}
	public partial class Calculator
	{
		public Dictionary<string,double> Vars = new Dictionary<string,double>();
		public ParserSource<Token> Src
		{
			get;
			set;
		}
		const TokenType EOF = TT.EOF;
		public double Calculate(string input)
		{
			Token EOF = new Token { 
				Type = TT.EOF
			};
			var lexer = new CalculatorLexer(input);
			Src = new ParserSource<Token>(lexer, EOF, lexer.Src.SourceFile) { 
				TokenTypeToString = tt => ((TT) tt).ToString()
			};
			return ExprSequence();
		}
		static double Do(double left, Token op, double right)
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
			case TT.Semicolon:
				return right;
			}
			return double.NaN;
		}
		double Atom()
		{
			TokenType la0, la1;
			double got_Atom = default(double);
			double result = default(double);
			// Line 259: ( TT.Id | TT.Num | TT.LParen ExprSequence TT.RParen )
			la0 = (TokenType) Src.LA0;
			if (la0 == TT.Id) {
				var t = Src.MatchAny();
				#line 259 "Grammars.ecs"
				result = Vars[(string) t.Value];
				#line default
			} else if (la0 == TT.Num) {
				var t = Src.MatchAny();
				#line 260 "Grammars.ecs"
				result = (double) t.Value;
				#line default
			} else if (la0 == TT.LParen) {
				Src.Skip();
				result = ExprSequence();
				Src.Match((int) TT.RParen);
			} else {
				#line 262 "Grammars.ecs"
				result = double.NaN;
				#line 262 "Grammars.ecs"
				Src.Error(0, "Expected identifer, number, or (parens)");
				#line default
			}
			// Line 265: greedy(TT.Exp Atom)*
			for (;;) {
				la0 = (TokenType) Src.LA0;
				if (la0 == TT.Exp) {
					la1 = (TokenType) Src.LA(1);
					if (la1 == TT.Id || la1 == TT.LParen || la1 == TT.Num) {
						Src.Skip();
						got_Atom = Atom();
						#line 265 "Grammars.ecs"
						result = Math.Pow(result, got_Atom);
						#line default
					} else
						break;
				} else
					break;
			}
			return result;
		}
		double Term()
		{
			TokenType la0;
			var result = Atom();
			// Line 270: (Atom)*
			for (;;) {
				la0 = (TokenType) Src.LA0;
				if (la0 == TT.Id || la0 == TT.LParen || la0 == TT.Num) {
					var rest = Atom();
					#line 270 "Grammars.ecs"
					result *= rest;
					#line default
				} else
					break;
			}
			#line 271 "Grammars.ecs"
			return result;
			#line default
		}
		double PrefixExpr()
		{
			TokenType la0;
			// Line 274: (TT.Sub Term | Term)
			la0 = (TokenType) Src.LA0;
			if (la0 == TT.Sub) {
				Src.Skip();
				var r = Term();
				#line 274 "Grammars.ecs"
				return -r;
				#line default
			} else {
				var r = Term();
				#line 275 "Grammars.ecs"
				return r;
				#line default
			}
		}
		double MulExpr()
		{
			TokenType la0;
			var result = PrefixExpr();
			// Line 279: ((TT.Div|TT.Mul) PrefixExpr)*
			for (;;) {
				la0 = (TokenType) Src.LA0;
				if (la0 == TT.Div || la0 == TT.Mul) {
					var op = Src.MatchAny();
					var rhs = PrefixExpr();
					#line 279 "Grammars.ecs"
					result = Do(result, op, rhs);
					#line default
				} else
					break;
			}
			#line 280 "Grammars.ecs"
			return result;
			#line default
		}
		double AddExpr()
		{
			TokenType la0;
			var result = MulExpr();
			// Line 284: ((TT.Add|TT.Sub) MulExpr)*
			for (;;) {
				la0 = (TokenType) Src.LA0;
				if (la0 == TT.Add || la0 == TT.Sub) {
					var op = Src.MatchAny();
					var rhs = MulExpr();
					#line 284 "Grammars.ecs"
					result = Do(result, op, rhs);
					#line default
				} else
					break;
			}
			#line 285 "Grammars.ecs"
			return result;
			#line default
		}
		double AssignExpr()
		{
			TokenType la0, la1;
			double result = default(double);
			// Line 289: (TT.Id TT.Assign AssignExpr | AddExpr)
			la0 = (TokenType) Src.LA0;
			if (la0 == TT.Id) {
				la1 = (TokenType) Src.LA(1);
				if (la1 == TT.Assign) {
					var t = Src.MatchAny();
					Src.Skip();
					result = AssignExpr();
					#line 289 "Grammars.ecs"
					Vars[t.Value.ToString()] = result;
					#line default
				} else
					result = AddExpr();
			} else
				result = AddExpr();
			return result;
		}
		double ExprSequence()
		{
			TokenType la0;
			double result = default(double);
			result = AssignExpr();
			// Line 293: (TT.Semicolon AssignExpr)*
			for (;;) {
				la0 = (TokenType) Src.LA0;
				if (la0 == TT.Semicolon) {
					Src.Skip();
					result = AssignExpr();
				} else
					break;
			}
			Src.Match((int) EOF);
			return result;
		}
	}
}
