//-----------------------------------------------------------------------
// <copyright file="MtPageDescription.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using MyToolkit.Serialization;

namespace MyToolkit.Paging
{
    /// <summary>Describes a page in the page stack. </summary>
	[DataContract]
	public class MtPageDescription
	{
        private Type _type;

        internal MtPageDescription() { } // for serialization

		public MtPageDescription(Type pageType, object parameter)
		{
			_type = pageType;

			SerializationType = pageType.AssemblyQualifiedName;
			Parameter = parameter;
		}

        /// <summary>Gets a value indicating whether the page is instantiated. </summary>
        public bool IsInstantiated
        {
            get { return Page != null; }
        }

        /// <summary>Gets the page type. </summary>
        public Type Type
        {
            get
            {
                if (_type == null)
                    _type = Type.GetType(SerializationType);
                return _type;
            }
        }

        /// <summary>Gets or sets the page parameter. </summary>
		public object Parameter { get; internal set; }

        /// <summary>Gets the page object or null if the page is not instantiated. </summary>
		public MtPage Page { get; internal set; }

        [DataMember]
        internal string SerializationType { get; set; }

        [DataMember(Name = "Parameter")]
        internal object SerializationParameter
        {
            get
            {
                if (Parameter == null)
                    return null;

                return DataContractSerialization.CanSerialize(Parameter.GetType()) ? Parameter : null;
            }
            set { Parameter = value; }
        }

        /// <exception cref="InvalidOperationException">The base type is not an MtPage. Change the base type from Page to MtPage. </exception>
        internal MtPage GetPage(MtFrame frame)
        {
            if (Page == null)
            {
                var page = Activator.CreateInstance(Type);
                if (!(page is MtPage))
                    throw new InvalidOperationException("The base type is not an MtPage. Change the base type from Page to MtPage. ");

                Page = (MtPage)page;
                Page.Frame = frame;
            }

            return Page;
        }
	}
}