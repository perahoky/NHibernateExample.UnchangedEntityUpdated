using System;
using System.IO;
using log4net;

namespace NHibernateExample.UnchangedEntityUpdated;

internal static class Program
{
	private static Lazy<ILog> log = new(() => LogManager.GetLogger(typeof(Program)));

	public static ILog Log => Program.log.Value;

	public static void Main(string[] args)
	{
		Program.TryCatchConsole(() =>
		{
			Program.Initialize();
			Program.WriteWelcome();

			using var example = new Example();
			example.Run();
		});
	}

	private static void Initialize()
	{
		Console.WriteLine("Initializing  ....");

		Console.SetError(Console.Out);

		log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));

		Program.Log.Info("Logging initialized");
		Program.Log.Info("Logging file (nhibernate-example.log) contains the nhibernate log.");
	}

	private static void WriteWelcome()
	{
		Program.Log.Info(new string('-', Console.BufferWidth - 10));
		Program.Log.Info("NHibernate Example");
		Program.Log.Info(string.Empty);
		Program.Log.Info("RowVersion is updated even when entity is not changed.");
		Program.Log.Info("Entities whose collections are dirty will be updated, even if there is nothing else to be updated, resulting in an update of the rowversion only.");
		Program.Log.Info("It is expected that nhibernate does not update entities if there is nothing else to be updated except the rowversion.");
		Program.Log.Info(new string('-', Console.BufferWidth - 10));
	}

	private static void TryCatchConsole(Action action)
	{
		try
		{
			action();
		}
		catch (Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(new string('-', Console.BufferWidth - 10));
			Console.WriteLine(ex);
		}
		finally
		{
			Console.WriteLine(new string('-', Console.BufferWidth - 10));
			Console.ResetColor();
			Console.WriteLine();
			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}
	}
}