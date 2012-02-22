using System;

namespace MyToolkit.Utilities
{
	public class WeakEvent<TTarget, TReference, TEventHandler>
	{
		private readonly WeakReference reference;

		public TEventHandler Event { get; set; }
		public TTarget Target { get; set; }

		public WeakEvent(TTarget target, TReference weakReference)
		{
			reference = new WeakReference(weakReference);
			Target = target; 
		}

		public TReference Reference
		{
			get { return (TReference)reference.Target; }
		}

		public bool IsAlive
		{
			get { return reference.IsAlive; }
		}
	}
}