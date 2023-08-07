using NHibernate;

namespace NHibernateExample.UnchangedEntityUpdated.Repositories;

internal class AbstractRepository<TModel>
{
	public AbstractRepository(ISession session)
	{
		this.Session = session;
	}

	public ISession Session { get; private set; }
}