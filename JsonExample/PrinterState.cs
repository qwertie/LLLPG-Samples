using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Json
{
	/// <summary>A helper type for printer objects. It manages indentation and 
	/// keeps track of the current line/column number.</summary>
	public struct PrinterState
	{
		public StringBuilder S;
		public int IndentLevel;
		public int LineNo;
		public string IndentString;
		public string NewlineString;
		private int _newlineIndex;
		public int IndexInCurrentLine { get { return S.Length - _newlineIndex; } }

		public PrinterState(StringBuilder s, string indent = "\t", string newline = "\n")
		{
			S = s ?? new StringBuilder();
			IndentLevel = 0;
			IndentString = indent;
			NewlineString = newline;
			_newlineIndex = 0;
			LineNo = 1;
		}

		public StringBuilder Append(string s)
		{
			return S.Append(s);
		}
		public void Indent()
		{
			IndentLevel++;
		}
		public void Dedent()
		{
			IndentLevel--;
		}

		/// <summary>Current length of the output string</summary>
		public int Length { get { return S.Length; } }

		/// <summary>Writes a newline and the appropriate amount of indentation afterward.</summary>
		/// <param name="indent">Whether to call Indent() beore writing the newline</param>
		/// <returns>An object that can be used to delete the newline later, if it 
		/// is decided later that the output fits on a single line. Note that 
		/// "revoking" a newline does _not_ restore the original indent level.</returns>
		public Revokable Newline(int changeIndentLevel = 0)
		{
			IndentLevel += changeIndentLevel;
			Contract.Assert(IndentLevel >= 0);

			var r = new Revokable(_newlineIndex, S.Length, NewlineString.Length + IndentString.Length * IndentLevel);
			S.Append(NewlineString);
			_newlineIndex = S.Length;
			LineNo++;
			for (int i = 0; i < IndentLevel; i++)
				S.Append(IndentString);
			return r;
		}

		/// <summary>Revokes (deletes) the last newline created, and its indent.</summary>
		/// <param name="r">Object returned from Newline()</param>
		/// <remarks>Only the most recent newline can be revoked, and of course, 
		/// it can only be revoked once. Multiple newlines can be revoked if 
		/// they are revoked in the reverse order in which they were created.</remarks>
		public void Revoke(Revokable r)
		{
			S.Remove(r._index, r._length);
			_newlineIndex = r._oldNewlineIndex;
			LineNo--;
		}

		public struct Revokable
		{
			internal int _index;
			internal int _length;
			internal int _oldNewlineIndex;
			public Revokable(int oldNewlineIndex, int index, int length)
			{
				_oldNewlineIndex = oldNewlineIndex;
				_index = index;
				_length = length;
			}
			public int Length { get { return _length; } }
		}
	}
}
