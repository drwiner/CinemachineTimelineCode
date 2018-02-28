using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Oshmirto
{
    [Serializable]
    public class Block
    {
        [XmlArray("shotFragments")]
        [XmlArrayItem("shotFragment")]
        public List<ShotFragment> ShotFragments { get; set; }

        [XmlIgnore]
        public float? StoryTime { get; set; }

        /// <summary>
        /// this property exists to make parsing the nullable float work.  
        /// use StoryTime when querying the target story time for the block entry
        /// </summary>
        [XmlAttribute("storyTime")]
        public string storyTimeAsText
        {
            get { return (StoryTime.HasValue) ? StoryTime.ToString() : null; }
            set { StoryTime = !string.IsNullOrEmpty(value) ? float.Parse(value) : default(float?); }
        }
    }
}
