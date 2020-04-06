using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Formatting = Newtonsoft.Json.Formatting;

namespace Aaron.Utils
{
    public static class Serialization
    {
        private static readonly JsonSerializer jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            Converters =
            {
                new StringEnumConverter(new DefaultNamingStrategy(), false)
            },
            TypeNameHandling = TypeNameHandling.Auto
        });

        public static string DataContractSerializeObject<T>(this T objectToSerialize) where T : class
        {
            var ds = new DataContractSerializer(typeof(T));
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment
            };

            using (var ss = new StringWriter())
            {
                using (var w = XmlWriter.Create(ss, settings))
                {
                    ds.WriteObject(w, objectToSerialize);
                }

                return ss.ToString();
            }
        }

        public static T DataContractDeserializeObject<T>(this string str) where T : class
        {
            var ds = new DataContractSerializer(typeof(T));
            XmlReaderSettings settings = new XmlReaderSettings { NameTable = new NameTable() };
            XmlNamespaceManager xmlns = new XmlNamespaceManager(settings.NameTable);
            xmlns.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xmlns.AddNamespace("i", "http://www.w3.org/2001/XMLSchema-instance");
            XmlParserContext context = new XmlParserContext(null, xmlns, "", XmlSpace.Default);

            using (var ss = new StringReader(str))
            {
                using (var r = XmlReader.Create(ss, settings, context))
                {
                    return (T)ds.ReadObject(r);
                }
            }
        }

        public static T Deserialize<T>(Stream stream) where T : class
        {
            using (var sr = new StreamReader(stream))
            using (JsonReader jr = new JsonTextReader(sr))
            {
                return jsonSerializer.Deserialize<T>(jr);
            }
        }

        public static T Deserialize<T>(string text) where T : class
        {
            using (var sr = new StringReader(text))
            using (JsonReader jr = new JsonTextReader(sr))
            {
                return jsonSerializer.Deserialize<T>(jr);
            }
        }

        public static string Serialize<T>(T obj) where T : class
        {
            using (var sw = new StringWriter())
            {
                jsonSerializer.Serialize(sw, obj);

                return sw.ToString();
            }
        }


        public static void Serialize<T>(T value, Stream s) where T : class
        {
            using (StreamWriter writer = new StreamWriter(s))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                jsonSerializer.Serialize(jsonWriter, value);
                jsonWriter.Flush();
            }
        }

    }
}
