using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Paradox.Engine;
using SiliconStudio.Paradox.Input;

namespace MyGame
{
    public class CameraControlScript : SyncScript
    {
        private Vector2 deltaPos;
        private float deltaZoom;

        public static readonly float MinZ = -100;
        public static readonly float MaxZ = -400;

        private float freeCameraMoveSpeed = 400;
        private Vector3 freeCameraViewPosition = new Vector3(0, -100, -300);
        private Vector3 freeCameraViewRotation = new Vector3(0, -MathUtil.DegreesToRadians(30), 0);

        private Vector3 bounds;
        private Vector3 MoveAmount;

        public override void Update()
        {
            if (Input.IsMouseButtonDown(MouseButton.Left))
            {
                deltaPos = Input.MouseDelta;
            }

            float currentWheel = Input.MouseWheelDelta;
            if (currentWheel != 0)
            {
                deltaZoom = currentWheel * 0.005f;
            }

            if (Input.IsKeyDown(Keys.Right))
            {
                deltaPos.X += 1f;
            }
            else if (Input.IsKeyDown(Keys.Left))
            {
                deltaPos.X -= 1f;
            }

            if (Input.IsKeyDown(Keys.Up))
            {
                deltaPos.Y += 1f;
            }
            else if (Input.IsKeyDown(Keys.Down))
            {
                deltaPos.Y -= 1f;
            }

            MoveAmount.X = deltaPos.X * 60;
            MoveAmount.Y = deltaPos.Y * 50;
            MoveAmount.Z = deltaZoom;
            deltaPos *= 0.95f;
            deltaZoom *= 0.9f;            

            CameraComponent camera = Entity.Get<CameraComponent>();
            camera.UseCustomViewMatrix = true;
            //camera.UseCustomProjectionMatrix = true;
            //camera.ProjectionMatrix = Matrix.PerspectiveFovRH(70f, GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000);
            camera.ViewMatrix = GetFreeCameraView();            
        }

        public Matrix GetFreeCameraView()
        {

            float timescale = (float) Game.UpdateTime.Elapsed.TotalSeconds;
            float rotatescale = 150.0f * timescale;
            float movescale = timescale * freeCameraMoveSpeed;

            freeCameraViewRotation.X -= MathUtil.DegreesToRadians(0 * rotatescale);
            freeCameraViewRotation.Y -= MathUtil.DegreesToRadians(0 * rotatescale);

            if (freeCameraViewRotation.Y > MathUtil.PiOverTwo - 0.01f)
            {
                freeCameraViewRotation.Y = MathUtil.PiOverTwo - 0.01f;
            }
            else if (freeCameraViewRotation.Y < -MathUtil.PiOverTwo + 0.01f)
            {
                freeCameraViewRotation.Y = -MathUtil.PiOverTwo + 0.01f;
            }

            Quaternion rot = Quaternion.RotationYawPitchRoll(freeCameraViewRotation.X, 0, freeCameraViewRotation.Z);
            Vector3 scaledmove;

            scaledmove.X = MoveAmount.X * movescale;
            scaledmove.Y = MoveAmount.Y * movescale;
            scaledmove.Z = 0;

            if (freeCameraViewPosition.Z >= MaxZ && freeCameraViewPosition.Z <= MinZ)
            {
                scaledmove.Y = MoveAmount.Y * movescale + (MoveAmount.Z * movescale * (float)Math.Cos(freeCameraViewRotation.Y));
                scaledmove.Z = MoveAmount.Z * movescale;
            }

            Vector3 newPos = freeCameraViewPosition + Vector3.Transform(scaledmove, rot);
            if (newPos.Z > MinZ || newPos.Z < MaxZ)
            {
                scaledmove.Y = MoveAmount.Y * movescale;
                scaledmove.Z = 0;
                newPos = freeCameraViewPosition + Vector3.Transform(scaledmove, rot);
            }

            /*
            if (newPos.X > bounds.X || newPos.X < -bounds.X)
            {
                scaledmove.X = 0;
                newPos = freeCameraViewPosition + Vector3.Transform(scaledmove, rot);
            }

            if (newPos.Y > bounds.Y || newPos.Y < bounds.Z)
            {
                scaledmove.Y = 0;
                scaledmove.Z = 0;
                newPos = freeCameraViewPosition + Vector3.Transform(scaledmove, rot);
            }
            */

            freeCameraViewPosition = newPos;

            Quaternion rotation = Quaternion.RotationYawPitchRoll(freeCameraViewRotation.X, freeCameraViewRotation.Y, freeCameraViewRotation.Z);
            Vector3 target = freeCameraViewPosition + Vector3.Transform(Vector3.UnitZ, rotation);
            return Matrix.LookAtRH(freeCameraViewPosition, target, Vector3.UnitY);
        }
    }
}
