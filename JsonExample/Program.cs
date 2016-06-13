using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loyc;
using Loyc.Collections;
using Loyc.Syntax;
using System.IO;
using Loyc.Syntax.Lexing;
using System.Diagnostics.Contracts;
using System.Diagnostics;

namespace Json
{
	class Program
	{
		static void Main(string[] args)
		{
			RunMenu(new List<Pair<string, Action>>()
			{
				new Pair<string,Action>("Try example JSON strings", ParseAndPrintExample),
				new Pair<string,Action>("Try parsing '*.json' files", ParseJsonFiles),
				new Pair<string,Action>("Try console readline", ParseConsoleReadLine),
			});
		}

		static void ParseAndPrintExample()
		{
			string result, roundTrip;
			object testPrint = new List<object> {
				null, 1.0, 2.0, 
				new Dictionary<string, object> { 
					{ "three", 3 }, 
					{ "four", new List<object> { 4.0, new List<object>() } }
				},
				5
			};
			Console.WriteLine("Printer test:");
			Console.WriteLine(@"   Expect: [null, 1, 2, { ""three"": 3, ""four"": [4, []] }, 5]");
			try {
				result = Json.Print(testPrint, compactMode: true);
				Console.WriteLine("   Output: " + result);
			} catch (LogException ex) {
				ex.Msg.WriteTo(MessageSink.Console);
			}

			Console.WriteLine("Parser test:");
			string testParse = @" [ 1, 2.0, { ""\three"" : 3.0e3 , ""four\n"" : [0.4e+1 , [ ] ]	} , 40.04e-1	, { } ] ";
			Console.WriteLine("    Input: " + testParse);
			result = Json.Print(Json.Parse(testParse));
			Console.WriteLine("   Result: " + result);
			roundTrip = Json.Print(Json.Parse(result));
			Console.WriteLine("Roundtrip: " + roundTrip);
		}
	
		static void ParseJsonFiles()
		{
			int count = 0;
			foreach (string filename in Directory.EnumerateFiles("..", "*.json", SearchOption.AllDirectories)) {
				count++;
				Console.WriteLine(filename);

				// The printer uses '\t' by default which is 8 characters in console. Ask for two spaces.
				var printer = new JsonPrinter(true, "  ", "\n");
				using (var file = File.Open(filename, FileMode.Open, FileAccess.Read)) {
					var obj = Json.Parse(file, filename, false, MessageSink.Console);
					var str = printer.Print(obj).ToString();
					Console.WriteLine(str);
					// Now make sure it round-trips fully
					printer.StringBuilder.Clear();
					Loyc.MiniTest.Assert.AreEqual(str, printer.Print(Json.Parse(str)).ToString());
				}
			}
			Console.WriteLine("--- Parsed & reserialized {0} files ---", count);
		}

		static void ParseConsoleReadLine()
		{
			Console.WriteLine("Please input a JSON string to parse:");
			string line = Console.ReadLine();
			string sparse = Json.Print(Json.Parse(line, false, MessageSink.Console), false);
			// Round trip again to make sure everything works
			Console.WriteLine(Json.Print(Json.Parse(sparse, false, MessageSink.Console), true));
			Console.WriteLine(sparse);
		}

		public static void RunMenu(IList<Pair<string, Action>> menu)
		{
			for (;;)
			{
				Console.WriteLine();
				Console.WriteLine("What do you want to do? (Esc to quit)");
				for (int i = 0; i < menu.Count; i++)
					Console.WriteLine(ParseHelpers.HexDigitChar(i + 1) + ". " + menu[i].Key);

				ConsoleKeyInfo k;
				Console.WriteLine((k = Console.ReadKey(true)).KeyChar);
				if (k.Key == ConsoleKey.Escape || k.Key == ConsoleKey.Enter)
					break;
				else
				{
					int i = ParseHelpers.HexDigitValue(k.KeyChar);
					if (i > 0 && i <= menu.Count)
						menu[i - 1].Value();
				}
			}
		}
	}

	/// <summary>A helper class for invoking JsonPrinter and JsonParser</summary>
	class Json
	{
		/// <summary>Parses a json string into an object.</summary>
		/// <param name="json">JSON data</param>
		/// <returns>a Dictionary{string, object}, List{object}, string or number.</returns>
		public static object Parse(string json, bool allowComments = false, IMessageSink sink = null)
		{
			return JsonParser.Parse((UString)json, "", 0, true, allowComments, sink);
		}
		public static object Parse(Stream stream, string fileName = "", bool allowComments = false, IMessageSink errSink = null)
		{
			return JsonParser.Parse(new StreamCharSource(stream), fileName, 0, true, allowComments, errSink);
		}
		
		public static string Print(object obj, bool compactMode = true)
		{
			return new JsonPrinter(compactMode).Print(obj).ToString();
		}
	}
}
