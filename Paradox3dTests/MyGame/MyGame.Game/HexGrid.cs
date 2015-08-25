using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Paradox.Engine;
using SiliconStudio.Paradox.Graphics;
using SiliconStudio.Paradox.Input;
using SiliconStudio.Paradox.Rendering;
using SiliconStudio.Paradox.Rendering.Composers;

namespace MyGame
{
    public class HexGrid : AsyncScript
    {
        private SpriteBatch spriteBatch;
        private Texture bgTexture;
        private Texture orangeTexture;

        public CameraComponent Camera { get; set; }

        public Entity Root { get; set; }

        private bool drawHexGrid = true;

        public override async Task Execute()
        {
            var virtualResolution = new Vector3(GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height, 1);
            spriteBatch = new SpriteBatch(GraphicsDevice);// { VirtualResolution = virtualResolution };
            spriteBatch.DefaultDepth = 0;
            bgTexture = await Asset.LoadAsync<Texture>("bgHexagon");
            orangeTexture = await Asset.LoadAsync<Texture>("orangeHexagon");
            galaxyTex = await Asset.LoadAsync<Texture>("galaxy");


            Entity rootSceneEnt = SceneSystem.SceneInstance.FirstOrDefault(e => e.Name == "Entity");
            var childSceneComponent = rootSceneEnt.Get<ChildSceneComponent>();
            Scene scene = childSceneComponent.Scene;
            var compositor = ((SceneGraphicsCompositorLayers)scene.Settings.GraphicsCompositor);
            compositor.Layers[0].Renderers.Add(new SceneDelegateRenderer(Render));

            // model test

            var model = await Asset.LoadAsync<Model>("Sphere");
            Entity entity = new Entity(new Vector3(20, 20, -15), "test", true);
            var modelComponent = new ModelComponent { Model = model };
            entity.Add(ModelComponent.Key, modelComponent);
            var scriptComponent = new ScriptComponent();
            scriptComponent.Scripts.Add(new SpinComponent() { RotateSpeed = 0.1f });
            entity.Add(ScriptComponent.Key, scriptComponent);

            entity.Transform.Scale = new Vector3(10, 10, 10);
            Root.AddChild<Entity>(entity);

            while (Game.IsRunning)
            {
                await Script.NextFrame();

                mousePosition = new Vector2(Input.MousePosition.X * GraphicsDevice.Viewport.Width, Input.MousePosition.Y * GraphicsDevice.Viewport.Height);

                if (Input.IsKeyPressed(Keys.G))
                {
                    drawHexGrid = !drawHexGrid;
                }
            }
        }

        private void Render(RenderContext arg1, RenderFrame arg2)
        {
            view = Camera.ViewMatrix;
            projection = Camera.ProjectionMatrix;
            DrawGalaxyBackground();
            if (drawHexGrid)
            {
                DrawHexGrid();
            }
        }

        private Vector2 GetHexCoords(Vector2 screenSpacePosition)
        {
            Vector2 result = Vector2.Zero;
            GraphicsDevice graphics = GraphicsDevice;
            Vector3 nearPoint = graphics.Viewport.Unproject(new Vector3(screenSpacePosition.X, screenSpacePosition.Y, 0), projection, view, hexWorld);
            Vector3 farPoint = graphics.Viewport.Unproject(new Vector3(screenSpacePosition.X, screenSpacePosition.Y, 1), projection, view, hexWorld);
            Vector3 diff = farPoint - nearPoint;
            Vector3 direction;
            Vector3.Normalize(ref diff, out direction);
            Ray ray = new Ray(nearPoint, direction);

            float distance;
            if (ray.Intersects(ref hexPlane, out distance))
            {
                Vector3 mouseHexPlane = ray.Position + ray.Direction * distance;
                float mq = 2f / 3f * mouseHexPlane.X / HexMap.HexInnerSize;
                float mr = (-1f / 3f * mouseHexPlane.X + 1f / 3f * HexMap.Sqrt3 * mouseHexPlane.Y) / HexMap.HexInnerSize;
                Vector2 rounded = HexMap.RoundHex(mq, mr);
                result.X = rounded.X;
                result.Y = rounded.Y;
            }

            return result;
        }

        private readonly Vector2 hexTextureSize = new Vector2(41, 41);
        private Matrix view = Matrix.Identity;
        private Matrix projection = Matrix.Identity;
        private readonly Matrix hexWorld = Matrix.Scaling(1f, 1f, 1f) *
            Matrix.RotationZ(MathUtil.DegreesToRadians(180)) *
            Matrix.Translation(Vector3.Zero);

        private readonly Matrix backgroundWorld = Matrix.Scaling(1f, 1f, 1f) * Matrix.RotationZ(MathUtil.DegreesToRadians(180)) * Matrix.Translation(new Vector3(0, 0, 10));

        private Plane hexPlane = new Plane(new Vector3(0, 0, 1), 0);
        private Vector2 mousePosition;
        private Texture galaxyTex;

        private void DrawHexGrid()
        {
            Viewport viewport = GraphicsDevice.Viewport;
            Vector2 topLeft = GetHexCoords(Vector2.Zero);
            Vector2 topRight = GetHexCoords(new Vector2(viewport.Width, 0));
            Vector2 bottonLeft = GetHexCoords(new Vector2(0, viewport.Height));

            short currentMapRadius = 45;

            int fromQ = (int)topLeft.X > -currentMapRadius ? (int)topLeft.X : -currentMapRadius;
            int toQ = (int)topRight.X < currentMapRadius ? (int)topRight.X : currentMapRadius;

            int fromR = (int)topRight.Y > -currentMapRadius ? (int)topRight.Y : -currentMapRadius;
            int toR = (int)bottonLeft.Y < currentMapRadius ? (int)bottonLeft.Y : currentMapRadius;

            //fromQ = -60; toQ = 60;
            //fromR = -60; toR = 60;

            //spriteRenderer.Begin(SpriteSortMode.None);            

            spriteBatch.Begin(Matrix.Identity, hexWorld * view * projection, SpriteSortMode.Deferred,
                GraphicsDevice.BlendStates.AlphaBlend, GraphicsDevice.SamplerStates.AnisotropicClamp, GraphicsDevice.DepthStencilStates.None, GraphicsDevice.RasterizerStates.CullNone);

            Vector2 position;
            for (int q = fromQ; q <= toQ; q++)
            {
                for (int r = fromR; r <= toR; r++)
                {
                    if (HexMap.Distance(0, q, 0, r) < currentMapRadius)
                    {
                        position.X = HexMap.HexInnerSize * 3f / 2f * q - hexTextureSize.X / 2;
                        position.Y = HexMap.HexInnerSizeSqrt * (r + q / 2f) - hexTextureSize.Y / 2;
                        spriteBatch.Draw(bgTexture, new RectangleF(position.X, position.Y, hexTextureSize.X, hexTextureSize.Y), Color.White);

                        if (q == 0 && r == 0)
                        {
                            spriteBatch.Draw(orangeTexture, new RectangleF(position.X, position.Y, hexTextureSize.X, hexTextureSize.Y), Color.Red);
                        }
                    }
                }
            }


            Vector2 mouseHex = GetHexCoords(mousePosition);
            //Debug.WriteLine(mouseHex);
            position.X = HexMap.HexInnerSize * 3f / 2f * mouseHex.X - hexTextureSize.X / 2;
            position.Y = HexMap.HexInnerSizeSqrt * (mouseHex.Y + mouseHex.X / 2f) - hexTextureSize.Y / 2;
            spriteBatch.Draw(orangeTexture, new RectangleF(position.X, position.Y, hexTextureSize.X, hexTextureSize.Y), Color.White);

            spriteBatch.End();
            //spriteRenderer.End(hexProjection);
        }

        private void DrawGalaxyBackground()
        {
            spriteBatch.Begin(Matrix.Identity, backgroundWorld * view * projection);
            spriteBatch.Draw(galaxyTex, new Vector2(-galaxyTex.Width / 2, -galaxyTex.Height / 2), Color.White);
            spriteBatch.End();
        }
    }
}
