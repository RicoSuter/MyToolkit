using System;
using System.Data.Objects;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace MyToolkit.Behaviors
{
	public class ApplyDataContractResolverAttribute : Attribute, IOperationBehavior
	{
		public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
		{

		}

		public void ApplyClientBehavior(OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy)
		{
			var dataContractSerializerOperationBehavior =
				description.Behaviors.Find<DataContractSerializerOperationBehavior>();
			dataContractSerializerOperationBehavior.DataContractResolver =
				new ProxyDataContractResolver();
		}

		public void ApplyDispatchBehavior(OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch)
		{
			var dataContractSerializerOperationBehavior =
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