using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;
using XMLNamespace;

[Serializable, XmlRoot("DiscourseCollection")]
public class DiscourseContainer {

    [XmlArray("Clips"), XmlArrayItem("Clip")]
    public List<DiscourseClip> Clips = new List<DiscourseClip>();

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(DiscourseContainer));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public static DiscourseContainer Load(string path)
    {
        var serializer = new XmlSerializer(typeof(DiscourseContainer));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as DiscourseContainer;
        }
    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static DiscourseContainer LoadFromText(string text)
    {
        var serializer = new XmlSerializer(typeof(DiscourseContainer));
        return serializer.Deserialize(new StringReader(text)) as DiscourseContainer;
    }
}
