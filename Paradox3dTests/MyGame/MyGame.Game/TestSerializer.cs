using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SiliconStudio.Core.Serialization;
using SiliconStudio.Core.Serialization.Contents;

namespace MyGame
{
    
    public class TestSerializer : DataSerializer<TestClass>
    {
        private static XmlSerializer serializer;        

        public TestSerializer()
            : base()
        {
            serializer = new XmlSerializer(typeof(TestClass));
        }

        public override void Serialize(ref TestClass obj, ArchiveMode mode, SerializationStream stream)
        {
            if (mode == ArchiveMode.Serialize)
            {
                stream.Write<string>("<?xml version=\"1.0\"?><TestClass><Name>blablsbalsbalsba</Name><Number>33</Number></TestClass>");
            }
            else
            {
                var xml = stream.ReadString();                
                using (TextReader reader = new StringReader(xml))
                {
                    obj = serializer.Deserialize(reader) as TestClass;
                }                
            }
        }        
    }

    public class TestContentSerializer : DataContentSerializer<TestClass>
    {
        public override object Construct(ContentSerializerContext context)
        {
            return new TestClass();
        }
    }


}
