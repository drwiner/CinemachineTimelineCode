using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class DiscourseClip  {

    [XmlAttribute("name")]
    public string Name;
    [XmlAttribute("type")]
    public string Type;

    public float fabulaStart = -1f;
    public float start = 1f;
    public float duration = 1f;
    public float fov = 35f;
    public float orientation = 0f;
    public float targetOrientation = 35f;
    public float targetDistance = 15f;

    public string startingPos_string;
    public string endingPos_string;
    public string aimTarget;
    public string followTarget;
    public string TimeLineObject;


    public XmlNodeList getChildren(XmlDocument doc, string tag)
    {
        XmlNodeList elemList = doc.GetElementsByTagName(tag);
        return elemList;
    }

    public static Vector3 readVector3(XmlElement vector_string)
    {
        string[] vectorString = new string[3];
        vectorString[0] = vector_string.GetAttribute("x", vector_string.NamespaceURI);
        vectorString[1] = vector_string.GetAttribute("y", vector_string.NamespaceURI);
        vectorString[2] = vector_string.GetAttribute("z", vector_string.NamespaceURI);

        float [] vectorFloat = new float[3];
        for (int i = 0; i < vectorString.Length; i++)
        {
            if (vectorString[i] == null || vectorString[i].Trim() == "" || !float.TryParse(vectorString[i].Trim(), out vectorFloat[i]))
                vectorFloat[i] = 0f;
        }
        Vector3 safeVector3 = new Vector3(vectorFloat[0], vectorFloat[1], vectorFloat[2]);

        return safeVector3;
    }
}
