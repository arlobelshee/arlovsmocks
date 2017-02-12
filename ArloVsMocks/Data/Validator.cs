namespace ArloVsMocks.Data
{
	internal interface Validator<T>
	{
		void Validate(T data);
		void ReportErrors();
	}
}