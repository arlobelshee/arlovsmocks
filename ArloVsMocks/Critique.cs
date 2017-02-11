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
	}
}