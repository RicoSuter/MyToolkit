//-----------------------------------------------------------------------
// <copyright file="MtFrameDescription.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyToolkit.Paging
{
    [DataContract]
    internal class MtFrameDescription
    {
        [DataMember]
        public int CurrentPageIndex { get; set; }

        [DataMember]
        public List<MtPageDescription> PageStack { get; set; }
    }
}

#endif