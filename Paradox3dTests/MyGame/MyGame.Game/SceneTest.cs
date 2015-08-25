using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Paradox.Engine;

namespace MyGame
{
    public class SceneTest : SyncScript
    {
        public override void Update()
        {
            if (Entity.Get<ChildSceneComponent>().Scene == null)
            {
                Scene scene = Asset.Load<Scene>("MainMenuScene");
                Entity.Get<ChildSceneComponent>().Scene = scene;

                var testAsset = Asset.Load<TestClass>("TestAsset");
            }
        }
    }
}
