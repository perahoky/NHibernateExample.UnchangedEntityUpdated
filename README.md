# NHibernateExample.UnchangedEntityUpdated
An NHibernate example which shows a entity is updated despite its not changed. 

I'm using **FluentNHibernate** for this example.
The following is pseudocode.

Assume we have an entity:

```
class Entity
{
  Public Guid Id { get; set; }
  public int RowVersion { get; set; }
  public Entity Parent { get; set; }
  public ISet<Entity> Children { get; set; }
}
```

With the following map

```
class EntityMap : Classmap<Entity>
{
  public EntityMap()
  {
    this.Id(x => x.Id).Not.Nullable().GeneratedBy.Assigned;
    this.Version(x => x.RowVersion).Not.Nullable().UnsavedValue("0");
    this.HasMany(x => x.Children).Inverse(); // our children are responsible for saving themselves ... 
    this.References(x => x.Parent).Foreignkey("Parent_Id");
  }
}
```

We create our first data (with a fresh session which is committed and flushed at the end etc)

```
Guid parentId = Guid.NewGuid();
Guid childId= Guid.NewGuid();

// session 1
{
  using ISession session = OpenSession();
  // session.BeginTransaction(ReadCommitted);
  var parent = new Entity() { Id = parentId };

  session.SaveOrUpdate(parent);
  // transaction.Commit()
  session.Flush();
}
```

In some other session we create our child data and assign parent:

```
// session 2
{
  using ISession session = OpenSession();

  var child= new Entity();
  var parent = session.Get<Entity>(parentId);

  parent.Children.Add(child); // just as example
  child.Parent = parent;

  session.SaveOrUpdate(parent, child);
  session.Flush();
}
```

**Current behavior:**  
Nhibernate generates SQL which is updating the entities RowVersion and nothing else.
```
  update "Entity" set "RowVersion"=2 where "Id" = parentId;
  insert into "Entity" (Id, RowVersion) values (childId, 1);
```

**Expected behavior:**  
"parent" (identified by "parentId") remains unchanged (RowVersion 1) since it was not changed.
```
  insert into Entity (Id, RowVersion) values (childId, 1);
```

Test:
```
// session 3
{
  using ISession session = OpenSession();

  var child = session.Get<Entity>(childId);
  var parent = session.Get<Entity>(parentId);

  Assert.AreEqual(1, parent.RowVersion);
  Assert.AreEqual(1, child.RowVersion);
}
```

