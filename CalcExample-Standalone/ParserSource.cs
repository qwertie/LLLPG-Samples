using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Loyc.Syntax
{
	/// <summary>
	/// A standalone base class designed for parsers that use LLLPG (Loyc LL(k) 
	/// Parser Generator).
	/// </summary>
	/// <remarks>
	/// This is the standalone version that does not require references to
	/// Loyc.Essentials.dll, Loyc.Collections.dll and Loyc.Syntax.dll. It was 
	/// adapted from the original version in Loyc.Syntax.dll. It combines the 
	/// BaseParserForList and ParserSource classes from Loyc.Syntax.dll into 
	/// a single class. The difference between the two is that BaseParserForList
	/// is meant as a base class, so its LLLPG API consists of protected methods, 
	/// whereas ParserSource is an "external" LLLPG API so its methods are public. 
	/// This class contains public methods, but it can, of course, also be used as 
	/// a base class.
	/// <para/>
	/// ParserSource assumes that the token type implements IEquatable (e.g. int),
	/// but .NET enums do not. Many parsers, however, will use an "enum TokenType" 
	/// or something like that instead of an integer. When using LLLPG with this 
	/// base class and a non-integer token type, you must tell LLLPG to cast the 
	/// token type to an integer when calling the <c>Match(...)</c> methods, like 
	/// so:
	/// <code>
	///     LLLPG (parser(laType(YourTokenType), matchType(int)));
	///     
	///     rule ...;
	///     rule ...;
	/// </code> 
	/// </remarks>
	public abstract class ParserSource<Token, MatchType, List>
		where Token : ISimpleToken<MatchType>
		where MatchType : IEquatable<MatchType>
		where List : IList<Token>
	{
		public static HashSet<MatchType> NewSet(params MatchType[] items) { return new HashSet<MatchType>(items); }

		/// <summary>Initializes this object to begin parsing the specified tokens.</summary>
		/// <param name="list">A list of tokens that the derived class will parse.</param>
		/// <param name="eofToken">A token value to return when the input position 
		/// reaches the end of the token list. Ideally <c>eofToken.StartIndex</c>
		/// should contain the position of EOF, but the method
		/// <see cref="LaIndexToCharIndex"/> does not trust this
		/// value, and will ensure that the character index returned for EOF is at 
		/// least as large as the character index of the last token in the file. 
		/// This means that it is safe to set <c>ISimpleToken{MatchType}.StartIndex</c> 
		/// to 0 in the EOF token, because when an error message involves EOF, the
		/// base class will find a more accurate EOF position.</param>
		/// <param name="startIndex">The initial index from which to start reading
		/// tokens from the list (normally 0).</param>
		public ParserSource(IList<Token> list, Token eofToken, int startIndex = 0)
		{
			Reset(list, eofToken, startIndex);
		}
		public virtual void Reset(IList<Token> list, Token eofToken, int startIndex = 0)
		{
			EofToken = eofToken;
			EOF = EofToken.Type;
			_tokenList = list;
			InputPosition = startIndex;
		}
		public void Reset()
		{
			Reset(TokenList, EofToken);
		}

		private Action<SourcePos, string, object[]> _errorSink;
		/// <summary>Gets or sets the object to which error messages are sent. The
		/// default object is <see cref="FormatExceptionErrorSink"/>, which throws
		/// <see cref="FormatException"/> if an error occurs.</summary>
		public Action<SourcePos, string, object[]> ErrorSink
		{
			get { return _errorSink ?? FormatExceptionErrorSink; }
			set { _errorSink = value; }
		}

		protected Token EofToken;
		public MatchType EOF { get; protected set; }

		/// <summary>The IList{Token} that was provided to the constructor, if any.</summary>
		/// <remarks>Instead of setting this property, call Reset() to parse a new source file.</remarks>
		public IList<Token> TokenList { get { return _tokenList; } }
		protected IList<Token> _tokenList;
		// cached list size to avoid frequently calling the virtual Count property.
		// (don't worry, it's updated automatically by LT() if the list size changes)
		private int _listCount;

		/// <summary>Throws FormatException when it receives an error. Non-errors
		/// are sent to <see cref="MessageSink.Current"/>.</summary>
		public static readonly Action<SourcePos, string, object[]> FormatExceptionErrorSink = 
			(location, fmt, args) =>
			{
				throw new FormatException(location.ToString() + ": " + string.Format(fmt, args));
			};

		public MatchType LA(int i) { return LT(i).Type; }
		public MatchType LA0 { get { return _lt0.Type; } }

		/// <summary>Converts from MatchType (usually integer) to string (used in 
		/// error messages).</summary>
		public Func<MatchType, string> TokenTypeToString { get; set; }

		public virtual string ToString(MatchType tokenType)
		{
			if (TokenTypeToString == null)
				return tokenType.ToString();
			else
				return TokenTypeToString(tokenType);
		}

		protected Token _lt0;
		/// <summary>Next token to parse (cached; is set to LT(0) whenever InputPosition is changed).</summary>
		public Token LT0 { [DebuggerStepThrough] get { return _lt0; } }

		protected int _inputPosition;
		/// <summary>Current position of the next token to be parsed.</summary>
		public int InputPosition
		{
			[DebuggerStepThrough]
			get { return _inputPosition; }
			set {
				_inputPosition = value;
				_lt0 = LT(0);
			}
		}

		/// <summary>Returns the token type of _lt0 (normally _lt0.TypeInt)</summary>
		protected MatchType LA0Int { get { return _lt0.Type; } }
		/// <summary>Returns the token at lookahead i (e.g. <c>Source[InputPosition + i]</c>
		/// if the tokens come from a list called Source) </summary>
		public Token LT(int i)
		{
			i += InputPosition;
			if ((uint)i < (uint)_listCount || (uint)i < (uint)(_listCount = _tokenList.Count))
			{
				try
				{
					return _tokenList[i];
				}
				catch
				{
					_listCount = _tokenList.Count;
				}
			}
			return EofToken;
		}

		/// <summary>Converts a lookahead token index to a character index (used 
		/// for error reporting).</summary>
		/// <remarks>
		/// The default implementation does this by trying to cast 
		/// <c>LT(lookaheadIndex)</c> to <c>ISimpleToken{MatchType}</c>. Returns -1
		/// on failure.
		/// <para/>
		/// The <c>StartIndex</c> reported by an EOF token is assumed not 
		/// to be trustworthy: this method will ensure that the character index 
		/// returned for EOF is at least as large as <c>SourceFile.Text.Count</c>
		/// if a <see cref="SourceFile"/> was provided, or, otherwise, at least as 
		/// large as the last token in the file, by scanning backward to find the 
		/// last token in the file.
		/// </remarks>
		public virtual int LaIndexToCharIndex(int lookaheadIndex)
		{
			var token = LT(lookaheadIndex) as ISimpleToken<MatchType>;
			if (token == null)
				return -1;
			int charIdx = token.StartIndex;
			if (token.Type.Equals(EOF)) {
				for (int li = lookaheadIndex; li > lookaheadIndex-100;  li--) {
					var token2 = LT(li) as ISimpleToken<MatchType>;
					if (!token2.Type.Equals(EOF)) {
						charIdx = System.Math.Max(charIdx, token2.StartIndex);
						break;
					}
				}
			}
			return charIdx;
		}

		/// <summary>Converts a lookahead token index to a <see cref="SourcePos"/>
		/// object using <see cref="LaIndexToCharIndex"/>.</summary>
		/// <remarks>The derived class should override this method to figure out 
		/// the line number for a character index. This method doesn't know what
		/// the line number is, and returns a SourcePos with Line=1.</remarks>
		public virtual SourcePos LaIndexToSourcePos(int lookaheadIndex)
		{
			int charIdx = LaIndexToCharIndex(lookaheadIndex);
			return new SourcePos("", 1, charIdx);
		}

		/// <summary>Records an error or throws an exception.</summary>
		/// <param name="lookaheadIndex">Location of the error relative to the
		/// current <c>InputPosition</c>. When called by BaseParser, lookaheadIndex 
		/// is always equal to 0.</param>
		/// <remarks>
		/// The default implementation throws a <see cref="FormatException"/>.
		/// When overriding this method, you can convert the lookaheadIndex
		/// to a <see cref="SourcePos"/> using the expression
		/// <c>SourceFile.IndexToLine(LT(lookaheadIndex).StartIndex)</c>. This only
		/// works if an <c>ISourceFile</c> object was provided to the constructor of 
		/// this class, and <c>Token</c> implements <see cref="ISimpleToken"/>.
		/// </remarks>
		public virtual void Error(int lookaheadIndex, string message)
		{
			ErrorSink(LaIndexToSourcePos(lookaheadIndex), message, new object[0]);
		}
		/// <inheritdoc cref="Error(int,string)">
		public virtual void Error(int lookaheadIndex, string format, params object[] args)
		{
			ErrorSink(LaIndexToSourcePos(lookaheadIndex), format, args);
		}

		public void Skip()
		{
			// Called when prediction already verified the input (and LA(0) is not saved, so we return void)
			InputPosition++;
		}

		#region Normal matching

		public Token MatchAny()
		{
			Token lt = _lt0;
			InputPosition++;
			return lt;
		}
		public Token Match(HashSet<MatchType> set, bool inverted = false)
		{
			Token lt = _lt0;
			if (set.Contains(LA0Int) == inverted)
				Error(false, set);
			else
				InputPosition++;
			return lt;
		}
		public Token Match(MatchType a)
		{
			Token lt = _lt0; MatchType la = LA0Int;
			if (!la.Equals(a))
				Error(false, a);
			else
				InputPosition++;
			return lt;
		}
		public Token Match(MatchType a, MatchType b)
		{
			Token lt = _lt0; MatchType la = LA0Int;
			if (!la.Equals(a) && !la.Equals(b))
				Error(false, a, b);
			else
				InputPosition++;
			return lt;
		}
		public Token Match(MatchType a, MatchType b, MatchType c)
		{
			Token lt = _lt0; MatchType la = LA0Int;
			if (!la.Equals(a) && !la.Equals(b) && !la.Equals(c))
				Error(false, a, b, c);
			else
				InputPosition++;
			return lt;
		}
		public Token Match(MatchType a, MatchType b, MatchType c, MatchType d)
		{
			Token lt = _lt0; MatchType la = LA0Int;
			if (!la.Equals(a) && !la.Equals(b) && !la.Equals(c) && !la.Equals(d))
				Error(false, a, b, c, d);
			else
				InputPosition++;
			return lt;
		}
		public Token MatchExcept()
		{
			Token lt = _lt0; MatchType la = LA0Int;
			if (la.Equals(EOF))
				Error(true);
			else
				InputPosition++;
			return lt;
		}
		public Token MatchExcept(MatchType a)
		{
			Token lt = _lt0; MatchType la = LA0Int;
			if (la.Equals(a) || la.Equals(EOF))
				Error(true, a);
			else
				InputPosition++;
			return lt;
		}
		public Token MatchExcept(MatchType a, MatchType b)
		{
			Token lt = _lt0; MatchType la = LA0Int;
			if (la.Equals(a) || la.Equals(b) || la.Equals(EOF))
				Error(true, a, b);
			else
				InputPosition++;
			return lt;
		}
		public Token MatchExcept(MatchType a, MatchType b, MatchType c)
		{
			Token lt = _lt0; MatchType la = LA0Int;
			if (la.Equals(a) || la.Equals(b) || la.Equals(c) || la.Equals(EOF))
				Error(true, a, b, c);
			else
				InputPosition++;
			return lt;
		}
		public Token MatchExcept(MatchType a, MatchType b, MatchType c, MatchType d)
		{
			Token lt = _lt0; MatchType la = LA0Int;
			if (la.Equals(a) || la.Equals(b) || la.Equals(c) || la.Equals(d) || la.Equals(EOF))
				Error(true, a, b, c, d);
			else
				InputPosition++;
			return lt;
		}
		public Token MatchExcept(HashSet<MatchType> set)
		{
			return Match(set, true);
		}

		#endregion

		#region Try-matching

		/// <summary>A helper class used by LLLPG for backtracking.</summary>
		public struct SavePosition : IDisposable
		{
			ParserSource<Token,MatchType,List> _parser;
			int _oldPosition;
			public SavePosition(ParserSource<Token,MatchType,List> parser, int lookaheadAmt)
				{ _parser = parser; _oldPosition = parser.InputPosition; parser.InputPosition += lookaheadAmt; }
			public void Dispose() { _parser.InputPosition = _oldPosition; }
		}
		public bool TryMatch(HashSet<MatchType> set, bool inverted = false)
		{
			if (set.Contains(LA0Int) == inverted)
				return false;
			else
				InputPosition++;
			return true;
		}
		public bool TryMatch(MatchType a)
		{
			if (!(LA0Int.Equals(a)))
				return false;
			else
				InputPosition++;
			return true;
		}
		public bool TryMatch(MatchType a, MatchType b)
		{
			MatchType la = LA0Int;
			if (!la.Equals(a) && !la.Equals(b))
				return false;
			else
				InputPosition++;
			return true;
		}
		public bool TryMatch(MatchType a, MatchType b, MatchType c)
		{
			MatchType la = LA0Int;
			if (!la.Equals(a) && !la.Equals(b) && !la.Equals(c))
				return false;
			else
				InputPosition++;
			return true;
		}
		public bool TryMatch(MatchType a, MatchType b, MatchType c, MatchType d)
		{
			MatchType la = LA0Int;
			if (!la.Equals(a) && !la.Equals(b) && !la.Equals(c) && !la.Equals(d))
				return false;
			else
				InputPosition++;
			return true;
		}
		public bool TryMatchExcept()
		{
			if ((LA0Int.Equals(EOF)))
				return false;
			else
				InputPosition++;
			return true;
		}
		public bool TryMatchExcept(MatchType a)
		{
			MatchType la = LA0Int;
			if (la.Equals(EOF) || la.Equals(a))
				return false;
			else
				InputPosition++;
			return true;
		}
		public bool TryMatchExcept(MatchType a, MatchType b)
		{
			MatchType la = LA0Int;
			if (la.Equals(EOF) || la.Equals(a) || la.Equals(b))
				return false;
			else
				InputPosition++;
			return true;
		}
		public bool TryMatchExcept(MatchType a, MatchType b, MatchType c)
		{
			MatchType la = LA0Int;
			if (la.Equals(EOF) || la.Equals(a) || la.Equals(b) || la.Equals(c))
				return false;
			else
				InputPosition++;
			return true;
		}
		public bool TryMatchExcept(MatchType a, MatchType b, MatchType c, MatchType d)
		{
			MatchType la = LA0Int;
			if (la.Equals(EOF) || la.Equals(a) || la.Equals(b) || la.Equals(c) || la.Equals(d))
				return false;
			else
				InputPosition++;
			return true;
		}
		public bool TryMatchExcept(HashSet<MatchType> set)
		{
			return TryMatch(set, true);
		}

		#endregion

		protected void Error(bool inverted, params MatchType[] expected) { Error(inverted, (IEnumerable<MatchType>)expected); }
		protected virtual void Error(bool inverted, IEnumerable<MatchType> expected)
		{
			Error(0, string.Format("'{0}': expected {1}", ToString(LA0Int), ToString(inverted, expected)));
		}
		protected virtual string ToString(bool inverted, IEnumerable<MatchType> expected)
		{
			int plural = expected.Take(2).Count();
			if (plural == 0)
				return string.Format(inverted ? "anything" : "nothing");
			else if (inverted)
				return string.Format("anything except {0}", ToString(false, expected));
			else if (plural == 1)
				return ToString(expected.First());
			else
				return string.Join("|", expected.Select(e => ToString(e)));
		}
		public virtual void Check(bool expectation, string expectedDescr = "")
		{
			if (!expectation)
				Error(0, "An expected condition was false: {0}", expectedDescr);
		}

		#region Down & Up
		// These are used to traverse into token subtrees, e.g. given w=(x+y)*z, 
		// the outer token list is w=()*z, and the 3 tokens x+y are children of '('
		// So the parser calls something like Down(lparen) to begin parsing inside,
		// then it calls Up() to return to the parent tree.

		private Stack<KeyValuePair<IList<Token>, int>> _parents;

		/// <summary>Switches to parsing the specified token list at position zero
		/// (typically the value of <see cref="Token.Children"/> in a token tree 
		/// produced by <see cref="TokensToTree"/>.) The original token list and
		/// the original <see cref="InputPosition"/> are placed on a stack, so you
		/// can restore the old list by calling <see cref="Up()"/>.</summary>
		/// <returns>True if successful, false if <c>children</c> is null.</returns>
		public bool Down(IList<Token> children)
		{
			if (children != null)
			{
				if (_parents == null)
					_parents = new Stack<KeyValuePair<IList<Token>, int>>();
				_parents.Push(new KeyValuePair<IList<Token>, int>(_tokenList, InputPosition));
				_tokenList = children;
				InputPosition = 0;
				return true;
			}
			return false;
		}
		/// <summary>Returns to the old token list saved by <see cref="Down"/>.</summary>
		public void Up()
		{
			Debug.Assert(_parents.Count > 0);
			var pair = _parents.Pop();
			_tokenList = pair.Key;
			InputPosition = pair.Value;
		}
		/// <summary>Calls <see cref="Up()"/> and returns <c>value</c>.</summary>
		public T Up<T>(T value)
		{
			Up();
			return value;
		}

		#endregion
	}

	public abstract class ParserSource<Token, MatchType> : ParserSource<Token, MatchType, IList<Token>>
		where Token : ISimpleToken<MatchType>
		where MatchType : IEquatable<MatchType>
	{
		public ParserSource(IList<Token> list, Token eofToken, int startIndex = 0)
			: base(list, eofToken, startIndex) { }
		public ParserSource(IEnumerable<Token> list, Token eofToken, int startIndex = 0)
			: this(list.ToList(), eofToken, startIndex) { }
	}

	public class ParserSource<Token> : ParserSource<Token, int>
		where Token : ISimpleToken<int>
	{
		public ParserSource(IList<Token> list, Token eofToken, int startIndex = 0)
			: base(list, eofToken, startIndex) { }
		public ParserSource(IEnumerable<Token> list, Token eofToken, int startIndex = 0)
			: this(list.ToList(), eofToken, startIndex) { }
	}

	/// <summary>Basic information about a token as expected by <see cref="BaseParser{Token}"/>:
	/// a token <see cref="Type"/>, which is the type of a "word" in the program 
	/// (string, identifier, plus sign, etc.), a value (e.g. the name of an 
	/// identifier), and an index where the token starts in the source file.</summary>
	public interface ISimpleToken<TokenType>
	{
		/// <summary>The category of the token (integer, keyword, etc.) used as
		/// the primary value for identifying the token in a parser.</summary>
		TokenType Type { get; }
		/// <summary>Character index where the token starts in the source file.</summary>
		int StartIndex { get; }
		/// <summary>Value of the token. The meaning of this property is defined
		/// by the particular implementation of this interface, but typically this 
		/// property contains a parsed form of the token (e.g. if the token came 
		/// from the text "3.14", its value might be <c>(double)3.14</c>.</summary>
		object Value { get; }
	}
}
