using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyToolkit.Paging
{
    [DataContract]
    internal class MtFrameDescription
    {
        [DataMember]
        public int PageIndex { get; set; }

        [DataMember]
        public List<MtPageDescription> PageStack { get; set; }
    }
}