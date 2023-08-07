using System;

namespace NHibernateExample.UnchangedEntityUpdated.Common;

internal abstract class AbstractDisposable : IDisposable
{
	protected bool Disposed { get; private set; }

	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		this.Disposed = true;
	}
}