using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Web;

namespace MyToolkit.Behaviors
{
	public class ApplyDataContractResolverAttribute : Attribute, IOperationBehavior
	{
		public ApplyDataContractResolverAttribute()
		{

		}

		public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
		{

		}

		public void ApplyClientBehavior(OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy)
		{
			DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior =
				description.Behaviors.Find<DataContractSerializerOperationBehavior>();
			dataContractSerializerOperationBehavior.DataContractResolver =
				new ProxyDataContractResolver();
		}

		public void ApplyDispatchBehavior(OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch)
		{
			DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior =
				description.Behaviors.Find<DataContractSerializerOperationBehavior>();
			dataContractSerializerOperationBehavior.DataContractResolver =
				new ProxyDataContractResolver();
		}

		public void Validate(OperationDescription description)
		{
			// Do validation.
		}
	}
}