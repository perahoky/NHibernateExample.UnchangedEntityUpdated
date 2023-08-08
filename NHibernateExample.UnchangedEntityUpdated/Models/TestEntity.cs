using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NHibernateExample.UnchangedEntityUpdated.Common;

namespace NHibernateExample.UnchangedEntityUpdated.Models;

public class TestEntity : AbstractPersistableBase
{
	public TestEntity(string name, Guid id)
	{
		id.AssertArtgumentIsNotEmpty();
		name.AssertArtgumentIsNotNull();

		this.Id = id;
		this.Name = name;
	}

	protected TestEntity()
	{
	}

	[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "nhibernate")]
	public virtual ISet<TestEntity> Children { get; protected set; } = new HashSet<TestEntity>();

	public virtual TestEntity? Parent { get; set; }

	public virtual string? Name { get; set; }

	public override string ToString()
	{
		var text = $"{nameof(TestEntity)}: '{this.Name}'";

		text += $" Id:{this.Id} RowVersion:{this.RowVersion}";

		if (this.Children.Count > 0)
		{
			text += $" Children: {this.Children.Count}";
		}

		if (this.Parent is not null)
		{
			text += $" Parent.Name:'{this.Parent.Name}'";
		}

		return text;
	}
}