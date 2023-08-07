using System;
using NHibernate;
using NHibernateExample.UnchangedEntityUpdated.Common;
using NHibernateExample.UnchangedEntityUpdated.Models;

namespace NHibernateExample.UnchangedEntityUpdated.Repositories;

internal class AbstractPersistableBaseRepository<TModel> : AbstractRepository<TModel>
	where TModel : AbstractPersistableBase
{
	public AbstractPersistableBaseRepository(ISession session)
		: base(session)
	{
	}

	public TModel QueryOneById(Guid id)
	{
		id.AssertArtgumentIsNotEmpty();
		return this.Session.Get<TModel>(id);
	}

	public void SaveOrUpdate(TModel model)
	{
		model.AssertArtgumentIsNotNull();
		this.Session.SaveOrUpdate(model);
	}

	public void SaveOrUpdate(params TModel[] models)
	{
		models.AssertArtgumentIsNotNull();
		foreach (TModel model in models)
		{
			this.SaveOrUpdate(model);
		}
	}
}