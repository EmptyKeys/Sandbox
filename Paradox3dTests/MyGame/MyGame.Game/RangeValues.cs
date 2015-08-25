using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core;
using SiliconStudio.Core.Serialization;
using SiliconStudio.Core.Serialization.Contents;
using SiliconStudio.Paradox.Engine.Design;

namespace MyGame
{
    [DataContract("RangeValues")] // Specify that this classes is serializable
    [ContentSerializer(typeof(DataContentSerializer<RangeValues>))]  // Specify that this class is serializable through the asset manager
    [DataSerializerGlobal(typeof(CloneSerializer<RangeValues>), Profile = "Clone")]
    [DataSerializerGlobal(typeof(ReferenceSerializer<RangeValues>), Profile = "Asset")]
    public partial class RangeValues
    {
        public RangeValues()
        {
            Values = new List<float>();
        }
        public List<float> Values { get; set; }
    }
}
