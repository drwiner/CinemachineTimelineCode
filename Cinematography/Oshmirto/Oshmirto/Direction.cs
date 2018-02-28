using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Oshmirto
{
    public enum Heading
    {
        Toward,
        Away,
        Right,
        Left
    }

    [Serializable]
    public class Direction
    {
        [XmlAttribute("target")]
        public String Target { get; set; }

        [XmlAttribute("heading")]
        public Heading Heading { get; set; }

        public override string ToString()
        {
            return string.Format("({0},{1})", Target, Heading.ToString());
        }
    }
}
