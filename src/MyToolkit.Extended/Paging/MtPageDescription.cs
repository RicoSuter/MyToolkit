//-----------------------------------------------------------------------
// <copyright file="MtPageDescription.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.Runtime.Serialization;
using MyToolkit.Serialization;

namespace MyToolkit.Paging
{
    /// <summary>Describes a page in the page stack. </summary>
    [DataContract(IsReference = true)]
    public class MtPageDescription
    {
        private Type _type;

        /// <summary>Initializes a new instance of the <see cref="MtPageDescription"/> class.</summary>
        internal MtPageDescription() { } // for serialization

        /// <summary>Initializes a new instance of the <see cref="MtPageDescription"/> class.</summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="parameter">The parameter.</param>
        public MtPageDescription(Type pageType, object parameter)
        {
            _type = pageType;

            SerializationType = pageType.AssemblyQualifiedName;
            Parameter = parameter;
            PageKey = Guid.NewGuid().ToString();
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
        public MtPage Page { get; private set; }

        [DataMember]
        internal string PageKey { get; set; }

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
                var page = Activator.CreateInstance(Type) as MtPage;
                if (page == null)
                    throw new InvalidOperationException("The base type is not an MtPage. Change the base type from Page to MtPage. ");

                page.SetFrame(frame, PageKey);
                Page = page;
            }

            return Page;
        }

        /// <summary>Releases the page so that the GC can collect it.</summary>
        internal void ReleasePage()
        {
            Page = null; 
        }
    }
}

#endif