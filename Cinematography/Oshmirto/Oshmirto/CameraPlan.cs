using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace Oshmirto
{
    [Serializable, XmlRoot(Namespace="http://liquidnarrative.csc.ncsu.edu/oshmirto/v0.1", ElementName="cameraPlan")]
    public class CameraPlan
    {
        [XmlAttribute]
        public string Version { get; set; }

        [XmlArray("blocks")]
        [XmlArrayItem("block")]
        public List<Block> Blocks { get; set; }
    }
}
