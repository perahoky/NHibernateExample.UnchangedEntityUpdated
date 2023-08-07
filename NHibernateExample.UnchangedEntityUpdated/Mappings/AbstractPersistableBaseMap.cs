using FluentNHibernate.Mapping;
using NHibernateExample.UnchangedEntityUpdated.Models;

namespace NHibernateExample.UnchangedEntityUpdated.Mappings;

public class AbstractPersistableBaseMap<TModel> :
	ClassMap<TModel>
	where TModel : AbstractPersistableBase
{
	public AbstractPersistableBaseMap()
	{
		this.DynamicUpdate();

		this.Version(x => x.RowVersion)
			.Not.Nullable()
			.UnsavedValue("0");

		this.Id(x => x.Id)
			.Not.Nullable()
			.GeneratedBy.Assigned();
	}
}