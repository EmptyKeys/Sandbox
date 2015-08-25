using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame;
using SiliconStudio.Assets.Compiler;
using SiliconStudio.BuildEngine;
using SiliconStudio.Core.IO;
using SiliconStudio.Core.Serialization.Assets;

namespace TestLib
{
    internal class RangeAssetCompiler : AssetCompilerBase<RangeAsset>
    {
        protected override void Compile(AssetCompilerContext context, string urlInStorage, UFile assetAbsolutePath, RangeAsset asset, AssetCompilerResult result)
        {
            result.BuildSteps = new ListBuildStep() { new RangeAssetCommand(urlInStorage, asset) };
        }
        /// <summary>
        /// Command used by the build engine to convert the asset
        /// </summary>
        private class RangeAssetCommand : AssetCommand<RangeAsset>
        {
            public RangeAssetCommand(string url, RangeAsset asset)
                : base(url, asset)
            {
            }

            protected override Task<ResultStatus> DoCommandOverride(ICommandContext commandContext)
            {
                var assetManager = new AssetManager();
                // Generate our data for in-game time
                var inGameAsset = new RangeValues();
                
                for (float index = 0; index <= 40; index += 1)
                    inGameAsset.Values.Add(index);
                // Save in-game asset
                assetManager.Save(Url, inGameAsset);
                return Task.FromResult(ResultStatus.Successful);
            }
        }
    }
}
