using System;

namespace NHibernateExample.UnchangedEntityUpdated.Models;

public class AbstractPersistableBase
{
	private Guid id = Guid.NewGuid();

	public virtual Guid Id
	{
		get => this.id;
		set => this.id = value;
	}

	public virtual int RowVersion { get; set; }
}