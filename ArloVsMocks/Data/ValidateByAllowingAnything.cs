namespace ArloVsMocks.Data
{
	internal class ValidateByAllowingAnything<T> : Validator<T>
	{
		public void Validate(T data)
		{
		}

		public void ReportErrors()
		{
		}
	}
}