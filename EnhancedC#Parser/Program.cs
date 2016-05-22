using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loyc.Syntax;
using Loyc;
using Loyc.Collections;

namespace Loyc.Ecs
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
				"printer which is the LES printer.\n");
			Console.WriteLine(
				"The parser was ripped out of Loyc.Ecs.dll for this demo. Loyc.Ecs.dll, unlike \n" +
				"this demo, also contains the EC# printer (which LLLPG uses to write output).\n");
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

	partial class EcsValidators
	{
		/// <summary>Given a complex name such as <c>global::Foo&lt;int>.Bar&lt;T></c>,
		/// this method identifies the base name component, which in this example 
		/// is Bar. This is used, for example, to identify the expected name for
		/// a constructor based on the class name, e.g. <c>Foo&lt;T></c> => Foo.</summary>
		/// <remarks>It is not verified that name is a complex identifier. There
		/// is no error detection but in some cases an empty name may be returned, 
		/// e.g. for input like <c>Foo."Hello"</c>.</remarks>
		public static Symbol KeyNameComponentOf(LNode name)
		{
			if (name == null)
				return null;
			// global::Foo<int>.Bar<T> is structured (((global::Foo)<int>).Bar)<T>
			// So if #of, get first arg (which cannot itself be #of), then if #dot, 
			// get second arg.
			if (name.CallsMin(S.Of, 1))
				name = name.Args[0];
			if (name.CallsMin(S.Dot, 1))
				name = name.Args.Last;
			if (name.IsCall)
				return KeyNameComponentOf(name.Target);
			return name.Name;
		}
	}
}

