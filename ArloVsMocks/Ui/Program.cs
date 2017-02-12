using System;
using ArloVsMocks.CritiqueMovies;

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
				var summary = CritiqueProcessor.ProcessNewCritiqueAndGenerateSummary(critique);
				summary.Output(Console.WriteLine);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			Console.ReadKey();
		}
	}
}