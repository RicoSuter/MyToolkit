using System;
using System.Runtime.Serialization;
using MyToolkit.Utilities;

namespace MyToolkit.Paging
{
	[DataContract]
	public class PageDescription
	{
		public PageDescription() { } // for serialization
		public PageDescription(Type pageType, object parameter)
		{
			type = pageType;

			TypeName = pageType.AssemblyQualifiedName;
			Parameter = parameter;
		}

		[DataMember]
		public string TypeName { get; set; }
		public object Parameter { get; set; }

		[DataMember(Name = "Parameter")]
		public object ParameterData
		{
			get
			{
				if (Parameter == null)
					return null;
				return DataContractSerialization.CanSerialize(Parameter.GetType()) ? Parameter : null;
			}
			set { Parameter = value; }
		}

		public bool IsInstanciated
		{
			get { return Page != null; }
		}

		public Page Page { get; private set; }

		internal Page GetPage(Frame frame)
		{
			if (Page == null)
			{
				Page = (Page)Activator.CreateInstance(Type);
				Page.Frame = frame;
			}
			return Page;
		}

		private Type type;
		public Type Type
		{
			get
			{
				if (type == null)
					type = Type.GetType(TypeName);
				return type;
			}
		}
	}
}