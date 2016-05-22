// Generated from JsonParser.ecs by LeMP custom tool. LeMP version: 1.7.5.0
// Note: you can give command-line arguments to the tool via 'Custom Tool Namespace':
// --no-out-header       Suppress this message
// --verbose             Allow verbose messages (shown by VS as 'warnings')
// --timeout=X           Abort processing thread after X seconds (default: 10)
// --macros=FileName.dll Load macros from FileName.dll, path relative to this file 
// Use #importMacros to use macros in a given namespace, e.g. #importMacros(Loyc.LLPG);
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Loyc;
using Loyc.Collections;
using Loyc.Syntax;
using Loyc.Syntax.Lexing;
namespace Json
{
	public class JsonParser : BaseLexer<ICharSource>
	{
		public static object Parse(UString chars, bool allowComments = false)
		{
			return Parse(chars, "", 0, true, allowComments);
		}
		public static object Parse(ICharSource chars, string fileName, int inputPosition = 0, bool checkForEofAfter = true, bool allowComments = false, IMessageSink errSink = null)
		{
			var parser = new JsonParser(chars, fileName, inputPosition, false) { 
				AllowComments = allowComments
			};
			if (errSink != null)
				parser.ErrorSink = errSink;
			var result = parser.Value();
			if (checkForEofAfter && parser.LA0 != -1)
				parser.Error(0, "Expected EOF after JSON value");
			return result;
		}
		public bool AllowComments
		{
			get;
			set;
		}
		public JsonParser(ICharSource chars, string fileName = "", int inputPosition = 0, bool newSourceFile = true) : base(chars, fileName, inputPosition, newSourceFile)
		{
		}
		void SkipWS()
		{
			int la0, la1;
			// Line 37: greedy( [\t ] | Newline | &{AllowComments} ([/] [/] ([^\$\n\r])* (Newline | [\$]) | [/] [*] nongreedy(Newline / [^\$])* [*] [/]) )*
			for (;;) {
				switch (LA0) {
				case '\t':
				case ' ':
					Skip();
					break;
				case '\n':
				case '\r':
					Newline();
					break;
				case '/':
					{
						la1 = LA(1);
						if (la1 == '*' || la1 == '/') {
							Check(AllowComments, "AllowComments");
							// Line 40: ([/] [/] ([^\$\n\r])* (Newline | [\$]) | [/] [*] nongreedy(Newline / [^\$])* [*] [/])
							la1 = LA(1);
							if (la1 == '/') {
								Match('/');
								Skip();
								// Line 40: ([^\$\n\r])*
								for (;;) {
									la0 = LA0;
									if (!(la0 == -1 || la0 == '\n' || la0 == '\r'))
										Skip();
									else
										break;
								}
								// Line 40: (Newline | [\$])
								la0 = LA0;
								if (la0 == '\n' || la0 == '\r')
									Newline();
								else
									Match(-1);
							} else {
								Match('/');
								Match('*');
								// Line 41: nongreedy(Newline / [^\$])*
								for (;;) {
									switch (LA0) {
									case '*':
										{
											la1 = LA(1);
											if (la1 == -1 || la1 == '/')
												goto stop;
											else
												Skip();
										}
										break;
									case -1:
										goto stop;
									case '\n':
									case '\r':
										Newline();
										break;
									default:
										Skip();
										break;
									}
								}
							stop:;
								Match('*');
								Match('/');
							}
						} else
							goto stop2;
					}
					break;
				default:
					goto stop2;
				}
			}
		stop2:;
		}
		double Number()
		{
			int la0;
			double result = default(double);
			// line 47
			int start = InputPosition;
			// Line 48: ([\-])?
			la0 = LA0;
			if (la0 == '-')
				Skip();
			// Line 49: ([0] | [1-9] ([0-9])*)
			la0 = LA0;
			if (la0 == '0')
				Skip();
			else {
				MatchRange('1', '9');
				// Line 49: ([0-9])*
				for (;;) {
					la0 = LA0;
					if (la0 >= '0' && la0 <= '9')
						Skip();
					else
						break;
				}
			}
			// Line 50: ([.] ([0-9])*)?
			la0 = LA0;
			if (la0 == '.') {
				Skip();
				// Line 50: ([0-9])*
				for (;;) {
					la0 = LA0;
					if (la0 >= '0' && la0 <= '9')
						Skip();
					else
						break;
				}
			}
			// Line 51: ([Ee] ([+\-])? [0-9] ([0-9])*)?
			la0 = LA0;
			if (la0 == 'E' || la0 == 'e') {
				Skip();
				// Line 51: ([+\-])?
				la0 = LA0;
				if (la0 == '+' || la0 == '-')
					Skip();
				MatchRange('0', '9');
				// Line 51: ([0-9])*
				for (;;) {
					la0 = LA0;
					if (la0 >= '0' && la0 <= '9')
						Skip();
					else
						break;
				}
			}
			// line 53
			UString str = CharSource.Slice(start, InputPosition - start);
			result = ParseHelpers.TryParseDouble(ref str, 10);
			return result;
		}
		static readonly HashSet<int> String_set0 = NewSetOfRanges(-1, 31, '"', '"', '\\', '\\');
		string String()
		{
			int la0;
			string result = default(string);
			int start = InputPosition;
			bool escaped = false;
			Match('"');
			// Line 60: ([\\] [^\$] | default [^\$-\x1F"\\])*
			for (;;) {
				la0 = LA0;
				if (la0 == '\\') {
					Skip();
					MatchExcept();
					// line 60
					escaped = true;
				} else if (!(la0 >= -1 && la0 <= '\x1F' || la0 == '"'))
					MatchExcept(String_set0);
				else if (la0 == -1 || la0 == '"')
					break;
				else
					MatchExcept(String_set0);
			}
			// Line 61: (["])
			la0 = LA0;
			if (la0 == '"')
				Skip();
			else {
				// line 61
				Error(0, "Expected closing quote");
			}
			// line 63
			UString text = CharSource.Slice(start + 1, InputPosition - start - 2);
			if (escaped) {
				result = ParseHelpers.UnescapeCStyle(text);
			} else {
				result = (string) text;
			}
			return result;
		}
		protected object Value()
		{
			int la0;
			object result = default(object);
			SkipWS();
			// Line 75: ( Dictionary | List | Number | String | WordLiteral )
			la0 = LA0;
			if (la0 == '{')
				result = Dictionary();
			else if (la0 == '[')
				result = List();
			else if (la0 == '-' || la0 >= '0' && la0 <= '9')
				result = Number();
			else if (la0 == '"')
				result = String();
			else if (la0 >= 'A' && la0 <= 'Z' || la0 >= 'a' && la0 <= 'z')
				result = WordLiteral();
			else {
				// line 80
				Error(0, "Expected a value");
				result = null;
				// Line 81: greedy([^\$,\]}])*
				for (;;) {
					la0 = LA0;
					if (!(la0 == -1 || la0 == ',' || la0 == ']' || la0 == '}'))
						Skip();
					else
						break;
				}
			}
			SkipWS();
			return result;
		}
		object WordLiteral()
		{
			int la0, la1, la2, la3, la4;
			object result = default(object);
			int start = InputPosition;
			// Line 86: ( [t] [r] [u] [e] / [f] [a] [l] [s] [e] / [n] [u] [l] [l] / [A-Za-z] ([A-Za-z])* )
			do {
				la0 = LA0;
				if (la0 == 't') {
					la1 = LA(1);
					if (la1 == 'r') {
						la2 = LA(2);
						if (la2 == 'u') {
							la3 = LA(3);
							if (la3 == 'e') {
								switch (LA(4)) {
								case -1:
								case '\t':
								case '\n':
								case '\r':
								case ' ':
								case ',':
								case '/':
								case ']':
								case '}':
									{
										Skip();
										Skip();
										Skip();
										Skip();
										// line 86
										result = G.BoxedTrue;
									}
									break;
								default:
									goto match4;
								}
							} else
								goto match4;
						} else
							goto match4;
					} else
						goto match4;
				} else if (la0 == 'f') {
					la1 = LA(1);
					if (la1 == 'a') {
						la2 = LA(2);
						if (la2 == 'l') {
							la3 = LA(3);
							if (la3 == 's') {
								la4 = LA(4);
								if (la4 == 'e') {
									switch (LA(5)) {
									case -1:
									case '\t':
									case '\n':
									case '\r':
									case ' ':
									case ',':
									case '/':
									case ']':
									case '}':
										{
											Skip();
											Skip();
											Skip();
											Skip();
											Skip();
											// line 87
											return G.BoxedFalse;
										}
									default:
										goto match4;
									}
								} else
									goto match4;
							} else
								goto match4;
						} else
							goto match4;
					} else
						goto match4;
				} else if (la0 == 'n') {
					la1 = LA(1);
					if (la1 == 'u') {
						la2 = LA(2);
						if (la2 == 'l') {
							la3 = LA(3);
							if (la3 == 'l') {
								switch (LA(4)) {
								case -1:
								case '\t':
								case '\n':
								case '\r':
								case ' ':
								case ',':
								case '/':
								case ']':
								case '}':
									{
										Skip();
										Skip();
										Skip();
										Skip();
										// line 88
										return null;
									}
								default:
									goto match4;
								}
							} else
								goto match4;
						} else
							goto match4;
					} else
						goto match4;
				} else
					goto match4;
				break;
			match4:
				{
					MatchRange('A', 'Z', 'a', 'z');
					// Line 89: ([A-Za-z])*
					for (;;) {
						la0 = LA0;
						if (la0 >= 'A' && la0 <= 'Z' || la0 >= 'a' && la0 <= 'z')
							Skip();
						else
							break;
					}
					// line 90
					Error(0, "JSON does not support identifiers");
					return CharSource.Slice(start, InputPosition - start).ToString();
				}
			} while (false);
			return result;
		}
		static readonly HashSet<int> List_set0 = NewSetOfRanges(9, 10, 13, 13, ' ', ' ', '"', '"', '-', '-', '/', '9', 'A', '[', 'a', '{');
		List<object> List()
		{
			int la0;
			List<object> result = default(List<object>);
			// line 96
			result = new List<object>();
			Skip();
			SkipWS();
			// Line 97: (Value ([,] Value)*)?
			la0 = LA0;
			if (List_set0.Contains(la0)) {
				result.Add(Value());
				// Line 97: ([,] Value)*
				for (;;) {
					la0 = LA0;
					if (la0 == ',') {
						Skip();
						result.Add(Value());
					} else
						break;
				}
			}
			Match(']');
			return result;
		}
		Dictionary<string,object> Dictionary()
		{
			int la0;
			Dictionary<string,object> result = default(Dictionary<string,object>);
			// line 100
			result = new Dictionary<string,object>();
			Skip();
			SkipWS();
			// Line 101: (Pair ([,] Pair)*)?
			switch (LA0) {
			case '\t':
			case '\n':
			case '\r':
			case ' ':
			case '"':
			case '/':
				{
					Pair(result);
					// Line 101: ([,] Pair)*
					for (;;) {
						la0 = LA0;
						if (la0 == ',') {
							Skip();
							Pair(result);
						} else
							break;
					}
				}
				break;
			}
			Match('}');
			return result;
		}
		void Pair(Dictionary<string,object> dict)
		{
			int la0;
			string got_String = default(string);
			object got_Value = default(object);
			// Line 104: (SkipWS String SkipWS ([:] Value))
			switch (LA0) {
			case '\t':
			case '\n':
			case '\r':
			case ' ':
			case '"':
			case '/':
				{
					SkipWS();
					got_String = String();
					SkipWS();
					// Line 105: ([:] Value)
					la0 = LA0;
					if (la0 == ':') {
						Skip();
						got_Value = Value();
						// line 106
						dict.Add(got_String, got_Value);
					} else {
						// line 107
						Error(0, "Expected value for '{0}'", got_String);
					}
				}
				break;
			default:
				{
					// line 109
					got_String = "";
					Error(0, "Expected a string key");
					Skip();
				}
				break;
			}
		}
	}
}
