using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loyc;

namespace MyLanguage
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Boilerplate: this example demonstrates the 'boilerplate' - a parser");
			Console.WriteLine("stripped down to the minimum amount of code that any two-stage parser");
			Console.WriteLine("needs (i.e. a parser with a separate lexer). By default, this code");
			Console.WriteLine("simply parses a series of numbers into a List<double>, but you are");
			Console.WriteLine("encouraged to replace this parser with your own.");
			Console.WriteLine();
			Console.WriteLine("Awaiting input:");
			string line;
			while ((line = Console.ReadLine()).ToLower() != "exit") {
				try {
					foreach (var result in Parser.Parse(line))
						Console.WriteLine(result);
				} catch (LogException ex) {
					ex.Msg.WriteTo(MessageSink.Console);
				}
				Console.WriteLine();
			}
		}
	}
}
