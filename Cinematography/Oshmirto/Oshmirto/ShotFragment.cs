using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Oshmirto
{
    [Serializable]
    public class ShotFragment
    {

        public ShotFragment()
        {
        }

        [XmlAttribute("anchor")]
        public string Anchor { get; set; }

        [XmlIgnore]
        public float? Height { get; set; }

        /// <summary>
        /// exists for nullable parsing only.  use Height instead
        /// </summary>
        [XmlAttribute("height")]
        public string HeightAsString
        {
            get { return (Height.HasValue) ? Height.ToString() : null; }
            set { Height = !string.IsNullOrEmpty(value) ? float.Parse(value) : default(float?); }
        }

        [XmlIgnore]
        public float? Pan { get; set; }

        /// <summary>
        /// exists for nullable parsing only.  use Pan instead
        /// </summary>
        [XmlAttribute("pan")]
        public string PanAsString
        {
            get { return (Pan.HasValue) ? Pan.ToString() : null; }
            set { Pan = !string.IsNullOrEmpty(value) ? float.Parse(value) : default(float?); }
        }

        [XmlElement("angle")]
        public Angle Angle { get; set; }

        [XmlElement("direction")]
        public Direction Direction { get; set; }

        [XmlArray("framings")]
        [XmlArrayItem("framing")]
        public List<Framing> Framings { get; set; } 

        [XmlAttribute("duration")]
        public uint Duration { get; set; }

        [XmlArray("movements")]
        [XmlArrayItem("movement")]
        public List<CameraMovement> CameraMovements { get; set; }

        [XmlAttribute("lens")]
        [DefaultValue("35mm")]
        public string Lens { get; set; }

        [XmlAttribute("f-stop")]
        [DefaultValue("22")]
        public string FStop { get; set; }

        [XmlAttribute("focus")]
        public string FocusPosition{ get; set; }

        [XmlAttribute("shake")]
        [DefaultValue(0f)]
        public float Shake { get; set; }
    }
}
