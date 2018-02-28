using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Oshmirto
{
    public static class Parser
    {
        private static readonly XmlSerializer xs;
        static Parser()
        {
            xs = new XmlSerializer(typeof(CameraPlan));
        }

        public static CameraPlan Parse(string filename)
        {
            CameraPlan plan = null;
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                plan = (CameraPlan)xs.Deserialize(fs);
            }
            return plan;
        }

        public static CameraPlan Parse(Stream stream)
        {
            return (CameraPlan)xs.Deserialize(stream);
        }

        public static void Write(this CameraPlan plan, Stream stream)
        {
            xs.Serialize(stream, plan);
        }

        public static void Write(this CameraPlan plan, string filePath)
        {
            Write(filePath, plan);
        }

        public static string WriteToXml(this CameraPlan plan)
        {
            using (StringWriter stream = new StringWriter())
            {
                xs.Serialize(stream, plan);
                stream.Flush();
                return stream.ToString();
            }
        }

        public static void Write(string filename, CameraPlan plan)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                xs.Serialize(fs, plan);
            }
        }
    }
}
