using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Oshmirto
{
    [XmlType("framingType")]
    public enum FramingType
    {
        [XmlEnum("")]
        None,
        ExtremeCloseUp,
        CloseUp,
        Waist,
        Full,
        Long,
        ExtremeLong,
        ExtremeLongLong,//or some such        
        Angle
    }

    [Serializable]
    public class Framing
    {
        [XmlAttribute("framingType")]
        public FramingType FramingType { get; set; }

        [XmlAttribute("framingTarget")]
        public string FramingTarget { get; set; }

        public override string ToString()
        {
            return string.Format("({0},{1})", FramingTarget, FramingType.ToString());
        }
    }
}
