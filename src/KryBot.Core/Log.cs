using System.Drawing;

namespace KryBot.Core
{
	public class Log
	{
		public Log(){}

		public Log(string content)
		{
			Content = $"{content}\n";
			Color = Color.White;
		}

		public Log(string content, Color color)
		{
			Content = $"{content}\n";
			Color = color;
		}

		public Log(string content, Color color, bool success)
		{
			Content = $"{content}\n";
			Color = color;
			Success = success;
		}

		public string Content { get; set; }
		public Color Color { get; }
		public bool Success { get; }

		public override string ToString()
		{
			return Content;
		}
	}
}