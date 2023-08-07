using System;
using System.IO;
using log4net;

namespace NHibernateExample.UnchangedEntityUpdated;

internal static class Program
{
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
		var log = LogManager.GetLogger(typeof(Program));
		log.Info("Logging initialized");
		log.Info("Logging file (nhibernate-example.log) contains the nhibernate log.");
	}

	private static void WriteWelcome()
	{
		var log = LogManager.GetLogger(typeof(Program));

		log.Info(new string('-', Console.BufferWidth - 10));
		log.Info("NHibernate Example");
		log.Info(string.Empty);
		log.Info("RowVersion is updated even when entity is not changed.");
		log.Info("Entites whose collections are dirty will be updated, even if there is nothing else to be updated, resulting in an update of the rowversion only.");
		log.Info(new string('-', Console.BufferWidth - 10));
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
			Console.WriteLine(new string('-', 20));
			Console.WriteLine(ex);
		}
		finally
		{
			Console.WriteLine(new string('-', 20));
			Console.ResetColor();
			Console.WriteLine();
			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}
	}
}