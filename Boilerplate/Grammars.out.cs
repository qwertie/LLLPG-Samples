// Generated from Grammars.ecs by LeMP custom tool. LeMP version: 1.8.0.0
// Note: you can give command-line arguments to the tool via 'Custom Tool Namespace':
// --no-out-header       Suppress this message
// --verbose             Allow verbose messages (shown by VS as 'warnings')
// --timeout=X           Abort processing thread after X seconds (default: 10)
// --macros=FileName.dll Load macros from FileName.dll, path relative to this file 
// Use #importMacros to use macros in a given namespace, e.g. #importMacros(Loyc.LLPG);
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Loyc;
using Loyc.Collections;
using Loyc.Syntax.Lexing;
using Loyc.Syntax;
namespace MyLanguage
{
	using TT = TokenType;
	public enum TokenType
	{
		EOF = 0, Newline = 1, Number = 2
	}
	partial class Lexer
	{
		public override Maybe<Token> NextToken()
		{
			int la0;
			Token t = default(Token);
			// Line 24: ([\t ])*
			for (;;) {
				la0 = LA0;
				if (la0 == '\t' || la0 == ' ')
					Skip();
				else
					break;
			}
			#line 25 "Grammars.ecs"
			_startIndex = InputPosition;
			#line default
			// Line 27: ( Newline / Number / [\$] )
			la0 = LA0;
			if (la0 == '\n' || la0 == '\r') {
				t = Newline();
				#line 27 "Grammars.ecs"
				return t;
				#line default
			} else if (la0 >= '0' && la0 <= '9') {
				t = Number();
				#line 27 "Grammars.ecs"
				return t;
				#line default
			} else {
				Match(-1);
				#line 28 "Grammars.ecs"
				return Maybe<Token>.NoValue;
				#line default
			}
		}
		new Token Newline()
		{
			int la0;
			// Line 33: ([\r] ([\n])? | [\n])
			la0 = LA0;
			if (la0 == '\r') {
				Skip();
				// Line 33: ([\n])?
				la0 = LA0;
				if (la0 == '\n')
					Skip();
			} else
				Match('\n');
			#line 34 "Grammars.ecs"
			AfterNewline();
			return T(TT.Newline, WhitespaceTag.Value);
			#line default
		}
		Token Number()
		{
			int la0, la1;
			Skip();
			// Line 39: ([0-9])*
			for (;;) {
				la0 = LA0;
				if (la0 >= '0' && la0 <= '9')
					Skip();
				else
					break;
			}
			// Line 39: ([.] [0-9] ([0-9])*)?
			la0 = LA0;
			if (la0 == '.') {
				la1 = LA(1);
				if (la1 >= '0' && la1 <= '9') {
					Skip();
					Skip();
					// Line 39: ([0-9])*
					for (;;) {
						la0 = LA0;
						if (la0 >= '0' && la0 <= '9')
							Skip();
						else
							break;
					}
				}
			}
			#line 40 "Grammars.ecs"
			var text = Text();
			return T(TT.Number, ParseHelpers.TryParseDouble(ref text, radix: 10));
			#line default
		}
	}
	partial class Parser
	{
		List<double> Numbers()
		{
			TokenType la0;
			Token n = default(Token);
			List<double> result = default(List<double>);
			result = new List<double>();
			// Line 54: (TT.Number)*
			for (;;) {
				la0 = (TokenType) LA0;
				if (la0 == TT.Number) {
					n = MatchAny();
					#line 54 "Grammars.ecs"
					result.Add((double) n.Value);
					#line default
				} else
					break;
			}
			return result;
		}
	}
}
