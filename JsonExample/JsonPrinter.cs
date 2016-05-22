using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using Loyc.Collections; // for EscapeCStyle
using Loyc.Syntax;
using Loyc; // for EscapeCStyle

namespace Json
{
	/// <summary>A pretty-printer for printing "JSON-like" objects (sequences, 
	/// dictionaries, strings and numbers).</summary>
	/// <remarks>
	/// The main print function is <see cref="Print(object)"/>, which returns
	/// the StringBuilder into which the output was placed.
	/// <para/>
	/// The only requirement to print a dictionary is that it implements 
	/// IEnumerable{KeyValuePair{string,object}}, and to print a list it must
	/// implement IEnumerable{object}. A derived class can implement additional
	/// type conversion logic by overriding <see cref="PrintOther(object)"/>.
	/// <para/>
	/// This is actually quite a sophisticated printer in terms of how it handles 
	/// newlines. 
	/// - Indentation is used to show structure
	/// - It tries to keep lines under a certain width (<see cref="PreferredLineWidth"/>)
	/// - it packs small lists on a single line (<see cref="MaxItemsInSingleLineList"/>)
	/// - for large lists, up to 4 items are printed per line (<see cref="ListGroupSize"/>)
	/// </remarks>
	public class JsonPrinter
	{
		protected PrinterState S;
		
		/// <summary>The JSON printer tries to keep the line width under this size.</summary>
		public int PreferredLineWidth { get; set; }
		/// <summary>Maximum number of items of a dictionary to pack on a single line.</summary>
		public int DictionaryGroupSize { get; set; }
		/// <summary>Maximum number of items of a list to pack on a single line.</summary>
		public int ListGroupSize { get; set; }
		/// <summary>Whether to print a newline after "{" or "[".</summary>
		public bool NewlineAfterOpener { get; set; }
		/// <summary>Maximum length of a list that is printed on a single line.</summary>
		/// <remarks>Regardless of this setting, a list is broken across lines if 
		/// it doesn't fit on the current line. </remarks>
		public int MaxItemsInSingleLineList { get; set; }
		/// <summary>Maximum length of a dictionary that is printed on a single line</summary>
		public int MaxItemsInSingleLineDictionary { get; set; }
		/// <summary>Whether to print a space inside "[" and "]".</summary>
		public bool SpaceInsideSquareBrackets { get; set; }
		/// <summary>Whether to print a space inside "{" and "}".</summary>
		public bool SpaceInsideBraces { get; set; }
		/// <summary>StringBuilder to which output will be appended.</summary>
		public StringBuilder StringBuilder { get { return S.S; } set { S.S = value; } }

		/// <summary>Configures the printer object.</summary>
		/// <param name="compactMode">Chooses which default settings to use. If this is
		/// true, the printer tries to pack more stuff on each line.</param>
		/// <param name="indent">Indent character(s)</param>
		/// <param name="newline">Newline character(s)</param>
		/// <param name="s">A StringBuilder to which to append (if null, a new one is created)</param>
		public JsonPrinter(bool compactMode = true, string indent = "\t", string newline = "\n", StringBuilder s = null)
		{
			S = new PrinterState(s ?? new StringBuilder(), indent, newline);
			PreferredLineWidth = 80;
			NewlineAfterOpener = true;
			SpaceInsideBraces = SpaceInsideSquareBrackets = !compactMode;
			if (compactMode) {
				DictionaryGroupSize = 2;
				ListGroupSize = 4;
				MaxItemsInSingleLineList = 8;
				MaxItemsInSingleLineDictionary = 4;
			} else {
				DictionaryGroupSize = 1;
				ListGroupSize = 1;
				MaxItemsInSingleLineList = 2;
				MaxItemsInSingleLineDictionary = 1;
			}
		}

		/// <summary>Prints an object in JSON format</summary>
		/// <returns>The StringBuilder with which this printer was initialized</returns>
		public virtual StringBuilder Print(object obj)
		{
			if (obj is ValueType) {
				PrintOther(obj);
			} else if (obj is string) {
				PrintString((string)obj);
			} else {
				var dict = obj as IEnumerable<KeyValuePair<string, object>>;
				if (dict != null) {
					PrintDict(dict);
				} else {
					var list = obj as IEnumerable<object>;
					if (list != null)
						PrintList(list);
					else
						PrintOther(obj);
				}
			}
			return S.S;
		}

		protected virtual void PrintOther(object obj)
		{
			if (obj == null)
				S.Append("null");
			else if (obj is bool)
				S.Append((bool)obj ? "true" : "false");
			else if (obj is UString)
				PrintString((UString)obj);
			else
				S.Append(obj.ToString());
		}

		public void PrintString(UString str)
		{
			int oldLen = S.S.Length;
			S.Append("\"");
			// TODO: add EscapeCStyle option to use only \u, not \x which JSON doesn't support
			S.Append(ParseHelpers.EscapeCStyle(str, EscapeC.DoubleQuotes | EscapeC.ABFV | EscapeC.Control));
			S.Append("\"");
		}

		List<PrinterState.Revokable> _newlines = new List<PrinterState.Revokable>();

		public void PrintList(IEnumerable<object> obj)
		{
			PrintListOrDictionary(obj, "[", "]", SpaceInsideSquareBrackets, MaxItemsInSingleLineList, item => Print(item));
		}
		
		public void PrintDict(IEnumerable<KeyValuePair<string, object>> obj)
		{
			PrintListOrDictionary(obj, "{", "}", SpaceInsideBraces, MaxItemsInSingleLineDictionary, pair =>
			{
				PrintString(pair.Key);
				S.Append(": ");
				Print(pair.Value);
			});
		}

		#region Helper functions

		protected void PrintListOrDictionary<T>(IEnumerable<T> obj, string opener, string closer, bool spaceInside, int maxItemsInSingleLineList, Action<T> printItem)
		{
			int oldLength = S.S.Length;
			int oldNewlineCount = _newlines.Count;
			int count;
			PrintOpener(opener, spaceInside);
			PrintSequence(obj.GetEnumerator(), ListGroupSize, out count, printItem);
			PrintCloser(closer, spaceInside);
			ConditionallyRevokeNewlines(count <= maxItemsInSingleLineList, oldLength, oldNewlineCount);
		}

		protected void PrintOpener(string bracket, bool space)
		{
			S.Append(bracket);
			if (space) S.Append(" ");
			if (NewlineAfterOpener)
				_newlines.Add(S.Newline(+1));
		}

		protected void PrintCloser(string bracket, bool space)
		{
			if (space) S.Append(" ");
			_newlines.Add(S.Newline(-1));
			S.Append(bracket);
		}

		// Prints a sequence with multiple items on each line.
		// Returns the length of the sequence in characters.
		protected void PrintSequence<T>(IEnumerator<T> e, int maxPerLine, out int count, Action<T> printItem, string separator = ", ")
		{
			Contract.Assert(maxPerLine > 0);
			count = 0;
			if (!e.MoveNext())
				return;
			for (;;) {
				// First item in group
				printItem(e.Current);
				count++;

				// Rest of items in the current group
				for (int i = 1; i < maxPerLine; i++, count++) {
					if (!e.MoveNext())
						return;

					S.Append(separator);
					int lineWidthBefore = S.IndexInCurrentLine;
					_newlines.Add(S.Newline());
					
					int curLine = S.LineNo;
					int oldIndex = S.S.Length;
					int oldNewlineCount = _newlines.Count;
					printItem(e.Current);
					Contract.Assert(oldNewlineCount == _newlines.Count);
					
					int itemLength = S.S.Length - oldIndex;
					if (S.LineNo == curLine && lineWidthBefore + itemLength < PreferredLineWidth)
						RevokeLastNewline();
				}

				if (!e.MoveNext())
					return;

				S.Append(separator);
				_newlines.Add(S.Newline());
			}
		}

		protected void ConditionallyRevokeNewlines(bool fewEnoughItems, int oldLength, int oldNewlineCount)
		{
			if (fewEnoughItems) {
				int length = S.Length - oldLength;
				for (int i = oldNewlineCount; i < _newlines.Count; i++)
					length -= _newlines[i].Length;

				int indentSize = S.IndentLevel * S.IndentString.Length;
				if (length < PreferredLineWidth - indentSize)
					while (_newlines.Count > oldNewlineCount)
						RevokeLastNewline();
			}
			_newlines.Resize(oldNewlineCount);
		}

		void RevokeLastNewline()
		{
			S.Revoke(_newlines[_newlines.Count - 1]);
			_newlines.RemoveAt(_newlines.Count - 1);
		}

		#endregion
	}
}