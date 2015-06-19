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
	using Loyc.Math;

	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine(
				"Welcome to the LLLPG-with-Loyc-trees calculator demo.\n" +
				"Please type an expression like pi * 2 or «exit» to quit.\n" +
				"Mathy-looking expressions like 2(x-1)x^2 are also accepted.\n" +
				"This calculator has only six built-in operators: + - * / ^ =\n" +
				"The = operator lets you save a result in a variable: x = 10\n");

			Vars[GSymbol.Get("pi")] = Math.PI;
			Vars[GSymbol.Get("e")] = Math.E;

			string line;
			while ((line = Console.ReadLine()) != "exit") {
				while (line.EndsWith(" ")) // user can add additional lines this way
					line += "\n" + Console.ReadLine();
				try {
					var parser = MyParser.New(line);
					LNode tree = parser.Start();
					Console.WriteLine("Syntax tree:     " + tree.ToString());
					Console.WriteLine("Computed result: " + Compute(tree));
				} catch (Exception ex) {
					Console.WriteLine(ex.Message);
				}
				Console.WriteLine();
			}
		}

		static Dictionary<Symbol, double> Vars = new Dictionary<Symbol, double>();
		static Dictionary<Symbol, Func<double, double, double>> BinOps = new Dictionary<Symbol, Func<double, double, double>>()
		{
			{ S.XorBits, (x, y) => Math.Pow(x, y) },
			{ S.Mul,     (x, y) => x * y },
			{ S.Div,     (x, y) => x / y },
			{ S.Shr,     (x, y) => MathEx.ShiftRight(x, (int)y) },
			{ S.Shl,     (x, y) => MathEx.ShiftLeft(x, (int)y) },
			{ S.Add,     (x, y) => x + y },
			{ S.Sub,     (x, y) => x - y },
			{ S.Eq,      (x, y) => x == y ? 1.0 : 0.0 },
			{ S.Neq,     (x, y) => x != y ? 1.0 : 0.0 },
			{ S.GT,      (x, y) => x > y  ? 1.0 : 0.0 },
			{ S.LT,      (x, y) => x < y  ? 1.0 : 0.0 },
			{ S.GE,      (x, y) => x >= y ? 1.0 : 0.0 },
			{ S.LE,      (x, y) => x <= y ? 1.0 : 0.0 },
			{ S.AndBits, (x, y) => (int)x & (int)y },
			{ S.OrBits,  (x, y) => (int)x | (int)y },
		};

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
					if (tree.Args[0].IsId) {
						return Vars[tree.Args[0].Name] = Compute(tree.Args[1]);
					} else
						MessageSink.Console.Write(Severity.Error, tree, "Left hand side of '=' must be an identifier.");
				} else {
					var fn = BinOps.TryGetValue(tree.Name, null);
					if (fn != null && tree.ArgCount == 2)
						return fn(Compute(tree.Args[0]), Compute(tree.Args[1]));
					else
						MessageSink.Console.Write(Severity.Error, tree, "Operator {0} is not implemented.", tree.Name);
				}
			}
			return double.NaN;
		}
	}
}

