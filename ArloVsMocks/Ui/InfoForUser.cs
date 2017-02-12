using System;

namespace ArloVsMocks.Ui
{
	public class InfoForUser
	{
		private readonly string[] _messages;

		public InfoForUser(params string[] messages)
		{
			_messages = messages;
		}

		public void Output(Action<string> showToUser)
		{
			foreach (var message in _messages)
			{
				showToUser(message);
			}
		}
	}
}