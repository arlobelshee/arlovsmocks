using System;

namespace ArloVsMocks
{
	internal class Critique
	{
		public Critique(int movieId, int criticId, int stars)
		{
			MovieId = movieId;
			CriticId = criticId;
			Stars = stars;
			IsValid = true;
			ErrorMessage = string.Empty;
		}

		private Critique(string errorMessage)
		{
			MovieId = 0;
			CriticId = 0;
			Stars = 0;
			IsValid = false;
			ErrorMessage = errorMessage;
		}

		public int MovieId { get; }
		public int CriticId { get; }
		public int Stars { get; }
		public bool IsValid { get; }
		public string ErrorMessage { get; }

		public static Critique FromArgs(string[] args)
		{
			int movieId;
			int criticId;
			int stars;
			try
			{
				movieId = int.Parse(args[0]);
				criticId = int.Parse(args[1]);
				stars = int.Parse(args[2]);
				return new Critique(movieId, criticId, stars);
			}
			catch (Exception ex)
			{
				return new Critique(ex.Message);
			}
		}
	}
}