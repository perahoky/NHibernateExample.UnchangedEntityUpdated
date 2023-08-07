using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using log4net;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NHibernateExample.UnchangedEntityUpdated.Common;
using Configuration = NHibernate.Cfg.Configuration;

namespace NHibernateExample.UnchangedEntityUpdated;

internal abstract class AbstractExample<T> : AbstractDisposable
{
	private static ILog log = LogManager.GetLogger(typeof(T));

	private LazyDisposable<ISessionFactory> lazySessionFactory;

	private Lazy<Configuration> lazyConfiguration;

	public AbstractExample()
	{
		this.lazyConfiguration = new(this.CreateConfiguration);
		this.lazySessionFactory = new(this.BuildSessionFactory);
	}

	[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "used for logging")]
	protected ILog Log => AbstractExample<T>.log;

	protected ISessionFactory SessionFactory => this.lazySessionFactory.Value;

	protected Configuration Configuration => this.lazyConfiguration.Value;

	protected abstract Configuration CreateConfiguration();

	protected override void Dispose(bool disposing)
	{
		this.lazySessionFactory.Dispose();
		base.Dispose(disposing);
	}

	protected void ExportSchema(Configuration configuration)
	{
		this.Log.Info("Exporting schema ... ");
		new SchemaExport(this.Configuration).Create(false, true);
		this.Log.Info("Done, schema exported.");
	}

	protected void DoInSessionAndTransaction(Action<ISession> func)
	{
		ITransaction? transaction = null;
		ISession? session = null;

		try
		{
			session = this.SessionFactory.OpenSession();
			transaction = session.BeginTransaction(IsolationLevel.ReadCommitted);

			func(session);

			transaction.Commit();
			session.Flush();
		}
		catch (Exception ex)
		{
			transaction?.Rollback();

			this.Log.Error(ex.Message);
			throw;
		}
		finally
		{
			transaction?.Dispose();
			session?.Dispose();
		}
	}

	private ISessionFactory BuildSessionFactory()
	{
		this.Log.Info("Build session factory.");
		return this.Configuration.BuildSessionFactory();
	}
}