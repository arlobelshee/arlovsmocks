namespace ArloVsMocks.Data.StoredInMemory
{
	public interface Validator<T>
	{
		void Validate(T data);
		void ReportErrors();
	}
}