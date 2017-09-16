using System;
using System.Reflection;
using System.Windows.Forms;
using pWonders;

namespace pWonders.App.DockLive
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			new SingleInstance
			(
				delegate()
				{
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new DockForm());
				},
				Assembly.GetExecutingAssembly()
			);
		}
	}
}
