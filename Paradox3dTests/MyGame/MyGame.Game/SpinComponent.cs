using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Paradox.Engine;

namespace MyGame
{    
    public class SpinComponent : SyncScript
    {
        public float RotateSpeed { get; set; }

        public override void Update()
        {
            float angle = this.Game.UpdateTime.Elapsed.Milliseconds * RotateSpeed;
            //Debug.WriteLine(this.Game.UpdateTime.Elapsed.Milliseconds);
            Entity.Transform.Rotation *= Quaternion.RotationZ(MathUtil.DegreesToRadians(angle));            
        }
    }
}
