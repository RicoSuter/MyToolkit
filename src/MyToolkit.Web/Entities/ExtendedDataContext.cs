using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;

namespace MyToolkit.Entities
{
	public interface IInsertEntity
	{
		void OnInserting();
		void OnInserted();
	}

	public interface IUpdateEntity
	{
		void OnUpdating(IList<string> changedProperties);
		void OnUpdated();
	}

	public interface IDeleteEntity
	{
		void OnDeleting();
		void OnDeleted();
	}

	public class ExtendedObjectContext : ObjectContext
	{
		public static void BeforeSaveChanges(ObjectContext ctx)
		{
			foreach (var e in ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Added))
			{
				var entity = e.Entity as IInsertEntity;
				if (entity != null)
					entity.OnInserting();
			}

			foreach (var e in ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Modified))
			{
				var entity = e.Entity as IUpdateEntity;
				if (entity != null)
					entity.OnUpdating(e.GetModifiedProperties().ToArray());
			}

			foreach (var e in ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Modified))
			{
				var entity = e.Entity as IDeleteEntity;
				if (entity != null)
					entity.OnDeleting();
			}
		}

		public static void AfterSaveChanges(ObjectContext ctx)
		{
			foreach (var e in ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Added))
			{
				var entity = e.Entity as IInsertEntity;
				if (entity != null)
					entity.OnInserted();
			}

			foreach (var e in ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Modified))
			{
				var entity = e.Entity as IUpdateEntity;
				if (entity != null)
					entity.OnUpdated();
			}

			foreach (var e in ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Modified))
			{
				var entity = e.Entity as IDeleteEntity;
				if (entity != null)
					entity.OnDeleted();
			}
		}

		public ExtendedObjectContext(EntityConnection connection) : base(connection) { }
		public ExtendedObjectContext(string connectionString) : base(connectionString) { }
		protected ExtendedObjectContext(string connectionString, string defaultContainerName) : base(connectionString, defaultContainerName) { }
		protected ExtendedObjectContext(EntityConnection connection, string defaultContainerName) : base(connection, defaultContainerName) { }

		public override int SaveChanges(SaveOptions options)
		{
			BeforeSaveChanges(this);
			var result = base.SaveChanges(options);
			AfterSaveChanges(this);
			return result;
		}

		//protected override void Dispose(bool disposing)
		//{
		//	ContextOptions.LazyLoadingEnabled = false; // avoid exceptions on serializing
		//	base.Dispose(disposing);
		//}
	}
}
