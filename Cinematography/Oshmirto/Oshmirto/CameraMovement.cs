using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Oshmirto
{
    public enum CameraMovementType
    {
        Dolly,
        Crane,
        Pan,
        Tilt,
        Focus
    }

    public enum CameraMovementDirective //TODO better name
    {
        With,
        To
    }

    [Serializable]
    public class CameraMovement
    {
        [XmlAttribute("type")]
        public CameraMovementType Type { get; set; }

        [XmlAttribute("directive")]
        public CameraMovementDirective Directive { get; set; }

        [XmlAttribute("subject")]
        public string Subject { get; set; }

    }
}
