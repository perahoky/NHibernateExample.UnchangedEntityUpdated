using System;
using System.Data;
using System.IO;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernateExample.UnchangedEntityUpdated.Common;
using NHibernateExample.UnchangedEntityUpdated.Models;
using NHibernateExample.UnchangedEntityUpdated.Repositories;
using Configuration = NHibernate.Cfg.Configuration;
using Hbm2DDLKeyWords = NHibernate.Cfg.Hbm2DDLKeyWords;
using NHibernateEnvironment = NHibernate.Cfg.Environment;

namespace NHibernateExample.UnchangedEntityUpdated;

internal class Example : AbstractExample<Example>
{
	private const string sqliteFile = "nhibernate-example.sqlite";

	private static readonly string sqliteFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sqliteFile);

	public void Run()
	{
		// this.Configuration is in base "AbstractExample" for basic stuff
		// this.CreateConfiguration().BuildSessionFactory() ....
		this.ExportSchema(this.Configuration);

		var parentId = new Guid("00000001-0000-0000-0000-000000000000");
		var childId = new Guid("00000002-0000-0000-0000-000000000000");

		this.Log.Info(string.Empty);
		this.Log.Info(" ### Starting example: ###");

		this.Log.Info(string.Empty);
		this.Log.Info("1) Create test entity 'parent' ... ");
		this.Step1CreateParent(parentId, childId);

		this.Log.Info(string.Empty);
		this.Log.Info("2) Create test entity 'child', load 'parent' and add 'child' to 'parent.Children'-collection ... ");
		this.Step2CreateChildAndAssignParent(parentId, childId);

		this.Log.Info(string.Empty);
		this.Log.Info("3) Load parent & child and check rowversion equals 1 because 'parent' itself was not changed");
		this.Log.Warn("   Parent.RowVersion will be 2 which is considered as error because the entity 'parent' was not intended to be changed. Only 'child' (it's .parent) was changed.");
		this.Log.Warn("   Expected Solution could be: unchanged entitie's Versions are not updated.");
		this.Step3CheckRowVersionNotChangedAndSaveNoChanges(parentId, childId);
	}

	protected void Step1CreateParent(Guid parentId, Guid childId)
	{
		this.DoInSessionAndTransaction(session =>
		{
			var repository = new TestEntityRepository(session);

			// checking objects already exist
			var parent = repository.QueryOneById(parentId);
			var child = repository.QueryOneById(childId);

			Assert.IsNull(parent, "parent was not created and should be null. Current database is not empty.");
			Assert.IsNull(child, "child was not created and should be null. Current database is not empty.");

			parent = new TestEntity("parent", parentId);

			Assert.AreEqual(parentId, parent.Id, "parentid should be equal.");
			Assert.AreEqual(0, parent.RowVersion, "rowversion should be 0 after creation.");

			repository.SaveOrUpdate(parent);

			Assert.AreEqual(1, parent.RowVersion, "rowversion should be 1 after saving.");

			this.Log.Info(parent);
		});
	}

	protected void Step2CreateChildAndAssignParent(Guid parentId, Guid childId)
	{
		this.DoInSessionAndTransaction(session =>
		{
			var repository = new TestEntityRepository(session);
			TestEntity parent = repository.QueryOneById(parentId);

			Assert.AreEqual(1, parent.RowVersion, "rowversion or parent should be 1 after loading.");

			var child = new TestEntity("child1", childId);

			Assert.AreEqual(0, child.RowVersion, "rowversion of child should be 0 after creation.");

			parent.Children.Add(child);
			child.Parent = parent;

			Assert.IsTrue(parent.Children.Contains(child));

			repository.SaveOrUpdate(child, parent);
			Assert.AreEqual(1, parent.RowVersion, "rowversion should be 1 after saving.");
			this.Log.Info(parent);
			this.Log.Info(child);
		});
	}

	protected void Step3CheckRowVersionNotChangedAndSaveNoChanges(Guid parentId, Guid childId)
	{
		this.DoInSessionAndTransaction(session =>
		{
			var repository = new TestEntityRepository(session);
			TestEntity parent = repository.QueryOneById(parentId);
			TestEntity child = repository.QueryOneById(childId);

			this.Log.Info(parent);
			this.Log.Info(child);

			Assert.IsNotNull(child.Parent, "child.Parent should be not null because 'parent' was assigned to 'child.Parent' in previous transaction.");
			Assert.AreEqual(1, parent.RowVersion, "rowversion of 'parent' should be 1 after loading because it was not intended to be changed.");
			Assert.AreEqual(1, child.RowVersion, "rowversion of child should be 1 after loading because it was created and saved in first instance.");

			repository.SaveOrUpdate(child, parent);
			Assert.AreEqual(1, parent.RowVersion, "rowversion should be 1 after saving.");
			Assert.AreEqual(1, child.RowVersion, "rowversion should be 1 after saving.");
		});
	}

	protected void Step4CheckRowVersionNotChangedAndSaveNoChanges(Guid parentId, Guid childId)
	{
		this.DoInSessionAndTransaction(session =>
		{
			var repository = new TestEntityRepository(session);
			TestEntity parent = repository.QueryOneById(parentId);
			TestEntity child = repository.QueryOneById(childId);

			Assert.AreEqual(1, parent.RowVersion, "rowversion should be 1 because no changes were made.");
			Assert.AreEqual(1, child.RowVersion, "rowversion should be 1 because no changes were made.");

			repository.SaveOrUpdate(child, parent);
			Assert.AreEqual(1, parent.RowVersion, "rowversion should be 1 after saving.");
		});
	}

	protected override Configuration CreateConfiguration()
	{
		var config = this.CreateConfiguration(this.CreateSqliteConfiguration());
		this.Log.Info("Configuration initialized.");
		return config;
	}

	private Configuration CreateConfiguration(IPersistenceConfigurer databaseConfiguration)
	{
		var assembly = typeof(TestEntity).Assembly;

		this.Log.Info("Mapping assembly " + assembly.FullName);

		return Fluently
			.Configure()
			.Database(databaseConfiguration)
			.Mappings(m => m.FluentMappings.AddFromAssembly(assembly))
			.BuildConfiguration()
			.SetProperty(NHibernateEnvironment.Hbm2ddlKeyWords, Hbm2DDLKeyWords.AutoQuote.ToString());
	}

	private IPersistenceConfigurer CreateSqliteConfiguration()
	{
		string sqliteFilePath = Example.sqliteFilePath;

		this.Log.Info("Initialize with sqlite ... ");
		this.Log.Info($"Using file {sqliteFilePath}");

		return SQLiteConfiguration
			.Standard
			.UsingFile(sqliteFilePath)
			.IsolationLevel(IsolationLevel.ReadCommitted)
			.AdoNetBatchSize(500);
	}
}