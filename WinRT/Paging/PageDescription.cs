using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
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

		private MyPage page;
		public MyPage GetPage(MyFrame frame)
		{
			if (page == null)
			{
				page = (MyPage)Activator.CreateInstance(Type);
				page.Frame = frame;
			}
			return page;
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