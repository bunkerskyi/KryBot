using System.Drawing;

namespace KryBot.Functional
{
	public class Log
	{
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

		public Log(string content, Color color, bool success, bool echo)
		{
			Content = $"{content}\n";
			Color = color;
			Success = success;
			Echo = echo;
		}

		public string Content { get; set; }
		public Color Color { get; }
		public bool Success { get; }
		public bool Echo { get; }
	}
}