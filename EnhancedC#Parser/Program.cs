using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loyc.Syntax;
using Loyc;
using Loyc.Collections;

namespace Ecs.Parser
{
	using S = CodeSymbols;
	using Loyc.Syntax.Lexing;

	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine(
				"Welcome to EC# parser demo.\n\n" +
				"Please type a valid C# or EC# statement such as x = 7; or class Man:Beast {}\n" +
				"or while(true) Console.WriteLine(\"I'm not crazy!\"); (or exit to quit).\n");
			Console.WriteLine(
				"The parser produces Loyc trees, and they are printed out using the default\n" +
				"printer which is the LES printer. The LES printer is highly incomplete at\n" +
				"the moment: it uses a clumsy prefix notation, with no operators or\n" +
				"superexpression notations supported. On the plus side, prefix notation\n" +
				"makes the structure of the syntax tree very apparent in simple cases (yet\n" +
				"very hard to understand in complex cases).\n");
			Console.WriteLine(
				"The parser was ripped out of Ecs.exe for this demo. Ecs.exe also contains\n" +
				"the EC# printer (which LLLPG uses to write output) and EcsLanguageService\n" +
				"(which provides both parser and printer so it couldn't be included here).\n");
			Console.WriteLine(
				"The EC# parser does not support LINQ yet and may have other limitations\n" +
				"that have not been noticed yet.\n");
			Console.WriteLine(
				"Exercise for the reader: write a REPL for C#. Just kidding.\n");

			string line;
			while ((line = Console.ReadLine()) != "exit") {
				while (line.EndsWith(" ")) // user can add additional lines this way
					line += "\n" + Console.ReadLine();
				try {
					// Parse EC#!
					IListSource<LNode> stmts = EcsLanguageService.Value.Parse(line, MessageSink.Console);
					// If you'd like to parse LES instead of EC#, write this instead:
					// IListSource<LNode> stmts = Loyc.Syntax.Les.LesLanguageService.Value.Parse(line, MessageSink.Console);

					var c = Console.ForegroundColor;
					Console.ForegroundColor = ConsoleColor.Cyan;
					foreach (var stmt in stmts)
						Console.WriteLine("Syntax tree (LES format): " + stmt.ToString());
					Console.ForegroundColor = c;
				} catch (Exception ex) {
					Console.WriteLine(ex.GetType().Name + ": " + ex.Message);
				}
				Console.WriteLine();
			}
		}
	}
}

