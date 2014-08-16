namespace MyToolkit.Utilities
{
	public class Thread
	{
		public static void Sleep(int ms)
		{
			new System.Threading.ManualResetEvent(false).WaitOne(ms);
		}
	}
}
