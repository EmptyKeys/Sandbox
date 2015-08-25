using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame;
using SiliconStudio.Assets;
using SiliconStudio.Assets.Compiler;
using SiliconStudio.Core;
using SiliconStudio.Core.Reflection;
using SiliconStudio.Core.Serialization;

namespace TestLib
{
    [DataContract("RangeAsset")] // Name of the Asset serialized in YAML    
    [AssetCompiler(typeof(RangeAssetCompiler))] // The compiler used to transform this asset to RangeValues
    [AssetDescription(".pdxrange")] // A description used to display in the asset editor
    [ObjectFactory(typeof(RangeAssetFactory))]    
    public class RangeAsset : Asset
    {
        public float From { get; set; }
        public float To { get; set; }
        public float Step { get; set; }

        public RangeAsset()
            : base()
        {
        }

        internal class RangeAssetFactory : IObjectFactory
        {            
            public object New(Type type)
            {
                return new RangeAsset();
            }
        }
    }

    internal class Module
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            

            AssemblyRegistry.Register(typeof(Module).Assembly, AssemblyCommonCategories.Assets);
            AssemblyRegistry.Register(typeof(TestClass).Assembly, AssemblyCommonCategories.Assets);
        }
    }   
}
