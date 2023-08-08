using NHibernateExample.UnchangedEntityUpdated.Models;

namespace NHibernateExample.UnchangedEntityUpdated.Mappings;

public class TestEntityMap : AbstractPersistableBaseMap<TestEntity>
{
	public TestEntityMap()
	{
		this.References(x => x.Parent)
			.ForeignKey("Parent_Id");

		this.HasMany(x => x.Children)
			.Inverse();

		this.Map(x => x.Name).Not.Nullable();
	}
}