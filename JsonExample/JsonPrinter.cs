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
		
		/// <summary>The JSON printer tries to keep the line width under this size. 
		/// It can't always succeed - long strings or key-value pairs with long key
		/// strings may exceed this size.</summary>
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
			S.Append(ParseHelpers.EscapeCStyle(str, EscapeC.Minimal | EscapeC.DoubleQuotes | EscapeC.ABFV | EscapeC.Control));
			S.Append("\"");
		}

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
				if (pair.Value is string && pair.Key.Length > 2) {
					// Key and value may be too long for one line; try two
					var cp = S.Newline(+1);
					PrintString(pair.Value as string);
					S.Dedent();
					S.RevokeOrCommitNewlines(cp, PreferredLineWidth);
				} else
					Print(pair.Value);
			});
		}

		#region Helper functions

		protected void PrintListOrDictionary<T>(IEnumerable<T> obj, string opener, string closer, bool spaceInside, int maxItemsInSingleLineList, Action<T> printItem)
		{
			var cp = S.GetCheckpoint();
			PrintOpener(opener, spaceInside);
			int count;
			PrintSequence(obj.GetEnumerator(), ListGroupSize, out count, printItem);
			PrintCloser(closer, spaceInside);
			// If the list is short enough, remove associated newlines so it fits on one line
			S.RevokeOrCommitNewlines(cp, count <= maxItemsInSingleLineList ? PreferredLineWidth : 0);
		}

		protected void PrintOpener(string bracket, bool space)
		{
			S.Append(bracket);
			if (space) S.Append(" ");
			if (NewlineAfterOpener)
				S.Newline(+1);
		}

		protected void PrintCloser(string bracket, bool space)
		{
			if (space) S.Append(" ");
			S.Newline(-1);
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
				int preferredLineWidth = PreferredLineWidth;

				// First item in group
				var checkpoint = S.GetCheckpoint();
				printItem(e.Current);
				count++;
				if (S.LineNo != checkpoint.LineNo)
					preferredLineWidth = 0;

				// Rest of items in the current group
				for (int i = 1; i < maxPerLine; i++) {
					if (!e.MoveNext())
						return;

					S.Append(separator);
					checkpoint = S.Newline();
					printItem(e.Current);
					count++;
					if (S.RevokeOrCommitNewlines(checkpoint, preferredLineWidth) >= 0)
						preferredLineWidth = 0;
				}

				if (!e.MoveNext())
					return;

				S.Append(separator);
				S.Newline();
			}
		}

		#endregion
	}
}