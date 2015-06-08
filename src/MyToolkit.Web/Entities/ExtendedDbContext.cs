//using System;
//using System.Collections.Generic;
//using System.Data.Objects;
//using System.Linq;
//using System.Text;
//using System.Data.Entity;

//namespace MyToolkit.Entities
//{
//	public class ExtendedDbContext : DbContext
//	{
//		private static readonly Dictionary<object, ExtendedDbContext> objects =
//			new Dictionary<object, ExtendedDbContext>();

//		private static readonly Dictionary<ObjectContext, ExtendedDbContext> contexts = 
//			new Dictionary<ObjectContext, ExtendedDbContext>();
		
//		public static ExtendedDbContext FromObjectContext(ObjectContext context)
//		{
//			lock (contexts)
//			{
//				if (contexts.ContainsKey(context))
//					return contexts[context];
//				return null; 
//			}
//		}

//		public static ExtendedDbContext FromObject(object obj)
//		{
//			lock (contexts)
//			{
//				if (objects.ContainsKey(obj))
//					return objects[obj];

//				var field = obj.GetType().GetField("_entityWrapper");
//				var wrapper = field.GetValue(obj);
//				var property = wrapper.GetType().GetProperty("Context");
//				var context = (ObjectContext)property.GetValue(wrapper, null);
//				var ctx = FromObjectContext(context);

//				objects[obj] = ctx;
//				return ctx; 
//			}
//		}

//		public ExtendedDbContext()
//		{
//			Configuration.AutoDetectChangesEnabled = true;
//			Configuration.LazyLoadingEnabled = true;
//			Configuration.ProxyCreationEnabled = true;
//			Configuration.ValidateOnSaveEnabled = true;

//			lock (contexts)
//				contexts[((IObjectContextAdapter)this).ObjectContext] = this; 
//		}

//		protected override void Dispose(bool disposing)
//		{
//			lock (contexts)
//			{
//				contexts.Remove(((IObjectContextAdapter)this).ObjectContext);
//				foreach (var p in objects.Where(p => p.Value == this).ToArray())
//					objects.Remove(p.Key);
//			}

//			Configuration.LazyLoadingEnabled = false; // used to avoid context closed exceptions in serializer!
//			base.Dispose(disposing);
//		}
//	}
//}
