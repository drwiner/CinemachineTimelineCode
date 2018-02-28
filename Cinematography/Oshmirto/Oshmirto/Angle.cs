using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Oshmirto
{
    [XmlType("angleSetting")]
    public enum AngleSetting 
    {
        High,
        Medium,
        Low
    }

    [Serializable]
    public class Angle
    {
        [XmlAttribute("target")]
        public string Target { get; set; }

        [XmlAttribute("angleSetting")]
        public AngleSetting AngleSetting { get; set; }

        public override string ToString()
        {
            return string.Format("({0},{1})", Target, AngleSetting.ToString());
        }
    }
}
