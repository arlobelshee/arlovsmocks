namespace ArloVsMocks.Data
{
	public interface Validator<T>
	{
		void Validate(T data);
		void ReportErrors();
	}
}