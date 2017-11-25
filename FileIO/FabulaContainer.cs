using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

[Serializable, XmlRoot("FabulaCollection")]
public class FabulaContainer {

    [XmlArray("Clips"), XmlArrayItem("Clip")]
    public List<FabulaClip> Clips = new List<FabulaClip>();

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(FabulaContainer));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public static FabulaContainer Load(string path)
    {
        var serializer = new XmlSerializer(typeof(FabulaContainer));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as FabulaContainer;
        }
    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static FabulaContainer LoadFromText(string text)
    {
        var serializer = new XmlSerializer(typeof(FabulaContainer));
        return serializer.Deserialize(new StringReader(text)) as FabulaContainer;
    }
}
