using System;
using System.Xml.Serialization;

namespace MyToolkit.Paging
{
	public class PageDescription
	{
		public PageDescription() { } // for serialization
		public PageDescription(Type pageType, object parameter)
		{
			type = pageType;

			TypeName = pageType.AssemblyQualifiedName;
			Paramter = parameter;
		}

		public string TypeName { get; set; }
		public object Paramter { get; set; }

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
		[XmlIgnore]
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