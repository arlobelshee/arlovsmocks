using System;

namespace ArloVsMocks
{
	internal class Critique
	{
		public Critique(int movieId, int criticId, int stars, bool isValid)
		{
			MovieId = movieId;
			CriticId = criticId;
			Stars = stars;
			IsValid = isValid;
		}

		public int MovieId { get; }
		public int CriticId { get; }
		public int Stars { get; }
		public bool IsValid { get; }

		public static Critique FromArgs(string[] args)
		{
			int movieId;
			int criticId;
			int stars;
			Critique critique;
			try
			{
				movieId = Int32.Parse(args[0]);
				criticId = Int32.Parse(args[1]);
				stars = Int32.Parse(args[2]);
				critique = new Critique(movieId, criticId, stars, true);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				critique = new Critique(0, 0, 0, false);
			}
			return critique;
		}
	}
}