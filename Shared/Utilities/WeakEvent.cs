using System;

namespace MyToolkit.Utilities
{
	// see http://stackoverflow.com/questions/1747235/weak-event-handler-model-for-use-with-lambdas

	public static class WeakEvent
	{
		public static TDelegate Set<S, TDelegate, TArgs>(
				Func<EventHandler<TArgs>, TDelegate> converter,
				Action<TDelegate> add, Action<TDelegate> remove,
				S subscriber, Action<S, TArgs> action)
			where TArgs : EventArgs
			where TDelegate : class
			where S : class
		{
			var subs_weak_ref = new WeakReference(subscriber);
			TDelegate handler = null;
			handler = converter(
				(s, e) =>
				{
					var subs_strong_ref = subs_weak_ref.Target as S;
					if (subs_strong_ref != null)
					{
						action(subs_strong_ref, e);
					}
					else
					{
						remove(handler);
						handler = null;
					}
				});
			add(handler);
			return handler; 
		}

		// this overload is simplified for generic EventHandlers
		public static EventHandler<TArgs> Set<S, TArgs>(
			Action<EventHandler<TArgs>> add, Action<EventHandler<TArgs>> remove,
			S subscriber, Action<S, TArgs> action)
			where TArgs : EventArgs
			where S : class
		{
			return Set(h => h, add, remove, subscriber, action);
		}


		//private readonly WeakReference reference;
		//private readonly TTarget target; 

		//public Action<TParent, object, TEventArgs> Action { get; set; }
		//public Action<TTarget, WeakEvent<TParent, TTarget, TEventArgs>> Unregister { get; set; }

		//public WeakEvent(TParent parent, TTarget target)
		//{
		//    this.target = target; 
		//    reference = new WeakReference(parent);
		//}

		//public void RaiseEvent(object sender, TEventArgs args)
		//{
		//    if (reference.IsAlive)
		//        Action((TParent)reference.Target, sender, args);
		//    else
		//        Unregister(target, this);
		//}
	}
}