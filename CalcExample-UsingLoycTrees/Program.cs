using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loyc.Syntax;
using Loyc;
using Loyc.Collections;

namespace MyLanguage
{
	using S = CodeSymbols;

	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine(
				"Welcome to the LLLPG-with-Loyc-trees calculator demo.\n" +
				"Please type an expression like pi * 2 or «exit» to quit.\n" +
				"Mathy-looking expressions like 2(x-1)x^2 are also accepted.\n" +
				"This calculator has only six built-in operators: + - * / ^ =\n" +
				"The = operator lets you save a result in a variable: x = 10\n" +
				"\"Exercise for the reader\": try adding additional operators!\n");

			Vars[GSymbol.Get("pi")] = Math.PI;
			Vars[GSymbol.Get("e")] = Math.E;

			string line;
			while ((line = Console.ReadLine()) != "exit") {
				while (line.EndsWith(" ")) // user can add additional lines this way
					line += "\n" + Console.ReadLine();
				try {
					var parser = new MyParser(line, "");
					LNode tree = parser.Start();
					Console.WriteLine("Syntax tree:     " + tree.ToString());
					Console.WriteLine("Computed result: " + Compute(tree));
				} catch (Exception ex) {
					Console.WriteLine(ex.Message);
				}
				Console.WriteLine();
			}
		}

		static Dictionary<Symbol, double> Vars = new Dictionary<Symbol,double>();

		private static double Compute(LNode tree)
		{
			if (tree.IsLiteral) {
				return (double)tree.Value;
			} else if (tree.IsId) {
				double v;
				if (Vars.TryGetValue(tree.Name, out v))
					return v;
				else
					MessageSink.Console.Write(Severity.Error, tree, "'{0}' has no value assigned.", tree.Name);
			} else { // IsCall
				if (tree.Calls(S.Assign, 2)) {
					if (tree.Args[0].IsId)
						Vars[tree.Args[0].Name] = Compute(tree.Args[1]);
					else
						MessageSink.Console.Write(Severity.Error, tree, "Left hand side of '=' must be an identifier.");
				} else if (tree.Calls(S.Mul, 2)) {
					return Compute(tree.Args[0]) * Compute(tree.Args[1]);
				} else if (tree.Calls(S.Div, 2)) {
					return Compute(tree.Args[0]) / Compute(tree.Args[1]);
				} else if (tree.Calls(S.Add, 2)) {
					return Compute(tree.Args[0]) + Compute(tree.Args[1]);
				} else if (tree.Calls(S.Sub, 2)) {
					return Compute(tree.Args[0]) - Compute(tree.Args[1]);
				} else if (tree.Calls(S.XorBits, 2)) {
					return Math.Pow(Compute(tree.Args[0]), Compute(tree.Args[1]));
				}
			}
			return double.NaN;
		}
	}
}

