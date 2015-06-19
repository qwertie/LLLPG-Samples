using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalcExample
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("\n" +
				"Welcome to the LLLPG simple calculator demo.\n" +
				"Please type an expression like (5 + 10) * 2 or «exit» to quit.\n" +
				"Mathy-looking expressions like 2(x-1)x^2 are also accepted.\n" +
				"This calculator has only six built-in operators: + - * / ^ :=\n" +
				"The := operator lets you save a result in a variable: pi := 3.14159265\n" +
				"\"Exercise for the reader\": try adding additional operators!\n");

			var calc = new Calculator();
			calc.Vars["pi"] = Math.PI;
			calc.Vars["e"] = Math.E;

			string line;
			while ((line = Console.ReadLine()).ToLower() != "exit") {
				try {
					Console.WriteLine("== " + calc.Calculate(line));
				} catch (Exception ex) {
					Console.WriteLine(ex.Message);
				}
				Console.WriteLine();
			}
		}
	}
}
