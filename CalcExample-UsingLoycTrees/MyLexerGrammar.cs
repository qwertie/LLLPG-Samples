// Generated from MyLexerGrammar.ecs by LLLPG custom tool. LLLPG version: 1.1.0.0
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
					_type = TT.Num;
					Num();
				}
				break;
			case '\n':
			case '\r':
				{
					_type = TT.Newline;
					Newline();
				}
				break;
			case '*':
				{
					_type = TT.Mul;
					_value = S.Mul;
					Skip();
				}
				break;
			case '/':
				{
					_type = TT.Div;
					_value = S.Div;
					Skip();
				}
				break;
			case '+':
				{
					_type = TT.Add;
					_value = S.Add;
					Skip();
				}
				break;
			case '-':
				{
					_type = TT.Sub;
					_value = S.Sub;
					Skip();
				}
				break;
			case ':':
			case '=':
				{
					_type = TT.Set;
					_value = S.Set;
					// Line 34: ([:])?
					la0 = LA0;
					if (la0 == ':')
						Skip();
					Match('=');
				}
				break;
			case '(':
				{
					_type = TT.LParen;
					Skip();
				}
				break;
			case ')':
				{
					_type = TT.RParen;
					Skip();
				}
				break;
			case '\t':
			case ' ':
				{
					_type = TT.Spaces;
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
					_type = TT.Semicolon;
					Skip();
				}
				break;
			default:
				if (la0 >= 'A' && la0 <= 'Z' || la0 == '_' || la0 >= 'a' && la0 <= 'z') {
					_type = TT.Id;
					Id();
				} else {
					_type = TT.EOF;
					// Line 40: ([^\$])?
					la0 = LA0;
					if (la0 != -1) {
						Skip();
						_type = TT.Unknown;
					}
				}
				break;
			}
			return new Token((int) _type, _startIndex, InputPosition - _startIndex, NodeStyle.Default, _value);
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
			_value = GSymbol.Get(CharSource.Slice(_startIndex, InputPosition - _startIndex).ToString());
		}
		void Num()
		{
			int la0, la1;
			bool dot = false;
			// Line 55: ([.])?
			la0 = LA0;
			if (la0 == '.') {
				Skip();
				dot = true;
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
			_value = double.Parse(CharSource.Slice(_startIndex, InputPosition - _startIndex).ToString());
		}
	}
}
