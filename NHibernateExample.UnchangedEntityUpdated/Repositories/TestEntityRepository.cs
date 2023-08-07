using NHibernate;
using NHibernateExample.UnchangedEntityUpdated.Models;

namespace NHibernateExample.UnchangedEntityUpdated.Repositories;

internal class TestEntityRepository : AbstractPersistableBaseRepository<TestEntity>
{
	public TestEntityRepository(ISession session)
		: base(session)
	{
	}
}