using System;

namespace NHibernateExample.UnchangedEntityUpdated.Common;
internal sealed class LazyDisposable<T> : Lazy<T>, IDisposable
	where T : IDisposable
{
	public LazyDisposable(Func<T> func)
		: base(func)
	{
	}

	public void Dispose()
	{
		if (this.IsValueCreated)
		{
			this.Value.Dispose();
		}
	}
}