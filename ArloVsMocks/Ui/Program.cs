using System;
using ArloVsMocks.CritiqueMovies;
using ArloVsMocks.Data;

namespace ArloVsMocks.Ui
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var critique = Critique.FromArgs(args);
			if (!critique.IsValid)
			{
				Console.WriteLine(critique.ErrorMessage);
				return;
			}

			try
			{
				var impact = CritiqueRepository.Process(critique);
				impact.Summarize().Output(Console.WriteLine);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			Console.ReadKey();
		}
	}
}