using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core;
using SiliconStudio.Core.Serialization;
using SiliconStudio.Core.Serialization.Contents;

namespace MyGame
{
    [DataContract("TestClass")]
    [DataSerializerGlobal(typeof(ReferenceSerializer<TestClass>), Profile = "Asset")]
    [ContentSerializer(typeof(TestContentSerializer))]
    [DataSerializer(typeof(TestSerializer))]
    public class TestClass
    {
        public string Name { get; set; }

        public int Number { get; set; }
    }
}
