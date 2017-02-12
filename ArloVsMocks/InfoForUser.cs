using System;

namespace ArloVsMocks
{
	internal class InfoForUser
	{
		private readonly string[] _messages;

		public InfoForUser(string[] messages)
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