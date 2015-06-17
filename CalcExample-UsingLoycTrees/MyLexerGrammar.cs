// Generated from MyLexerGrammar.ecs by LeMP custom tool. LLLPG version: 1.3.1.0
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
	partial class MyLexer : BaseLexer
	{
		TokenType _type;
		object _value;
		int _startIndex;
		public Token NextToken()
		{
			int la0;
			_startIndex = InputPosition;
			_value = null;
			// Line 27: ( Num | Newline | Id | [*] | [/] | [+] | [\-] | ([:])? [=] | [(] | [)] | [\t ] ([\t ])* | [;] )
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
					#line 27 "MyLexerGrammar.ecs"
					_type = TT.Num;
					#line default
					Num();
				}
				break;
			case '\n':
			case '\r':
				{
					#line 28 "MyLexerGrammar.ecs"
					_type = TT.Newline;
					#line default
					Newline();
				}
				break;
			case '*':
				{
					#line 30 "MyLexerGrammar.ecs"
					_type = TT.Mul;
					#line 30 "MyLexerGrammar.ecs"
					_value = S.Mul;
					#line default
					Skip();
				}
				break;
			case '/':
				{
					#line 31 "MyLexerGrammar.ecs"
					_type = TT.Div;
					#line 31 "MyLexerGrammar.ecs"
					_value = S.Div;
					#line default
					Skip();
				}
				break;
			case '+':
				{
					#line 32 "MyLexerGrammar.ecs"
					_type = TT.Add;
					#line 32 "MyLexerGrammar.ecs"
					_value = S.Add;
					#line default
					Skip();
				}
				break;
			case '-':
				{
					#line 33 "MyLexerGrammar.ecs"
					_type = TT.Sub;
					#line 33 "MyLexerGrammar.ecs"
					_value = S.Sub;
					#line default
					Skip();
				}
				break;
			case ':':
			case '=':
				{
					#line 34 "MyLexerGrammar.ecs"
					_type = TT.Set;
					#line 34 "MyLexerGrammar.ecs"
					_value = S.Assign;
					#line default
					// Line 34: ([:])?
					la0 = LA0;
					if (la0 == ':')
						Skip();
					Match('=');
				}
				break;
			case '(':
				{
					#line 35 "MyLexerGrammar.ecs"
					_type = TT.LParen;
					#line default
					Skip();
				}
				break;
			case ')':
				{
					#line 36 "MyLexerGrammar.ecs"
					_type = TT.RParen;
					#line default
					Skip();
				}
				break;
			case '\t':
			case ' ':
				{
					#line 37 "MyLexerGrammar.ecs"
					_type = TT.Spaces;
					#line default
					Skip();
					// Line 37: ([\t ])*
					for (;;) {
						la0 = LA0;
						if (la0 == '\t' || la0 == ' ')
							Skip();
						else
							break;
					}
				}
				break;
			case ';':
				{
					#line 38 "MyLexerGrammar.ecs"
					_type = TT.Semicolon;
					#line default
					Skip();
				}
				break;
			default:
				if (la0 >= 'A' && la0 <= 'Z' || la0 == '_' || la0 >= 'a' && la0 <= 'z') {
					#line 29 "MyLexerGrammar.ecs"
					_type = TT.Id;
					#line default
					Id();
				} else {
					#line 39 "MyLexerGrammar.ecs"
					_type = TT.EOF;
					#line default
					// Line 40: ([^\$])?
					la0 = LA0;
					if (la0 != -1) {
						Skip();
						#line 40 "MyLexerGrammar.ecs"
						_type = TT.Unknown;
						#line default
					}
				}
				break;
			}
			#line 42 "MyLexerGrammar.ecs"
			return new Token((int) _type, _startIndex, InputPosition - _startIndex, NodeStyle.Default, _value);
			#line default
		}
		static readonly HashSet<int> Id_set0 = NewSetOfRanges('0', '9', 'A', 'Z', '_', '_', 'a', 'z');
		void Id()
		{
			int la0;
			Skip();
			// Line 49: ([0-9A-Z_a-z])*
			for (;;) {
				la0 = LA0;
				if (Id_set0.Contains(la0))
					Skip();
				else
					break;
			}
			#line 50 "MyLexerGrammar.ecs"
			_value = GSymbol.Get(CharSource.Slice(_startIndex, InputPosition - _startIndex).ToString());
			#line default
		}
		void Num()
		{
			int la0, la1;
			#line 54 "MyLexerGrammar.ecs"
			bool dot = false;
			#line default
			// Line 55: ([.])?
			la0 = LA0;
			if (la0 == '.') {
				Skip();
				#line 55 "MyLexerGrammar.ecs"
				dot = true;
				#line default
			}
			MatchRange('0', '9');
			// Line 56: ([0-9])*
			for (;;) {
				la0 = LA0;
				if (la0 >= '0' && la0 <= '9')
					Skip();
				else
					break;
			}
			// Line 57: (&!{dot} [.] [0-9] ([0-9])*)?
			la0 = LA0;
			if (la0 == '.') {
				if (!dot) {
					la1 = LA(1);
					if (la1 >= '0' && la1 <= '9') {
						Skip();
						Skip();
						// Line 57: ([0-9])*
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
			#line 58 "MyLexerGrammar.ecs"
			_value = double.Parse(CharSource.Slice(_startIndex, InputPosition - _startIndex).ToString());
			#line default
		}
	}
}
