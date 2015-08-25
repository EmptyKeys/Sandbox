using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Assets;
using SiliconStudio.Assets.Compiler;
using SiliconStudio.BuildEngine;
using SiliconStudio.Core;
using SiliconStudio.Core.IO;
using SiliconStudio.Core.Reflection;
using SiliconStudio.Core.Serialization;
using SiliconStudio.Core.Serialization.Assets;

namespace TestLib
{
    [DataContract("TestAsset")]
    [ObjectFactory(typeof(TestAssetFactory))]
    [AssetCompiler(typeof(TestAssetCompiler))]
    [AssetDescription(".pdxml")]
    public class TestAsset : AssetImport
    {        
        public TestAsset()
            : base()
        {
        }

        private class TestAssetFactory : IObjectFactory
        {
            public object New(Type type)
            {
                return new TestAsset();
            }
        }
    }

    public class TestAssetCompiler : AssetCompilerBase<TestAsset>
    {

        protected override void Compile(AssetCompilerContext context, string urlInStorage, SiliconStudio.Core.IO.UFile assetAbsolutePath, TestAsset asset, AssetCompilerResult result)
        {
            if (!EnsureSourceExists(result, asset, assetAbsolutePath))
            {
                return;
            }

            var assetSource = GetAbsolutePath(assetAbsolutePath, asset.Source);
            if (!File.Exists(assetSource))
            {
                result.Error("The source '{0}' does not exist on the PC.", assetSource);
                return;
            }

            result.BuildSteps = new ListBuildStep() { new TestAssetCommand(urlInStorage, asset, assetSource) };
        }

        private class TestAssetCommand : AssetCommand<TestAsset>
        {
            private string assetSource;

            public TestAssetCommand(string url, TestAsset asset, string source)
                : base(url, asset)
            {
                assetSource = source;
            }

            protected override Task<ResultStatus> DoCommandOverride(ICommandContext commandContext)
            {
                using (TextReader reader = new StringReader(assetSource))
                {
                    string result = reader.ReadToEnd();

                    var assetManager = new AssetManager();
                    assetManager.Save(Url, result);
                }                

                return Task.FromResult(ResultStatus.Successful);
            }
        }
    }
}
