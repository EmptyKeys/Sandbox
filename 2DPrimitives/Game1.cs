using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace _2DPrimitives
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        Matrix worldMatrix;
        Matrix viewMatrix;
        Matrix projectionMatrix;

        BasicEffect basicEffect;
        BasicEffect basicEffectTexture;

        VertexBuffer rectangleVertexBuffer;
        VertexPositionTexture[] rectanglePoints;
        short[] rectangleIndices;

        VertexBuffer ellipseVertexBuffer;
        VertexPositionColor[] ellipsePoints;
        short[] ellipseIndices;

        VertexBuffer eStrokeVertexBuffer;
        VertexPositionColor[] eStrokePoints;
        short[] eStrokeIndices;

        VertexBuffer pieVertexBuffer;
        VertexPositionColor[] piePoints;
        short[] pieIndices;

        VertexBuffer roundedRectVertexBuffer;
        VertexPositionColor[] roundedRectPoints;
        short[] roundedRectIndices;

        VertexPositionColor[] pieStrokePoints;
        short[] pieStrokeIndices;

        VertexPositionColor[] rectRoundStrokePoints;
        short[] rectRoundStrokeIndices;

        enum PrimType
        {
            Pie,
            PieStroke,
            Rectangle,
            RoundedRect,
            RoundedRectStroke,
            Ellipse,
            EllipseStroke,
        };
        PrimType typeToDraw = PrimType.RoundedRectStroke;

        RasterizerState rasterizerState;

        KeyboardState currentKeyboardState;
        KeyboardState lastKeyboardState;

        GraphicsDeviceManager graphics;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            InitializeTransform();
            InitializeEffect();            

            InitializeRectangle();
            InitializeRoundedRectangle(300, 150, 50, 50, 12);
            InitializeRectRoundStroke(300, 150, 50, 50, 12, 30);
            InitializeEllipseFill(100, 150, 0, 50);
            InitializeEllipseStroke(100, 150, 20, 50);
            InitializePie(100, 100, (float)(Math.PI / 2), 12);
            InitializePieStroke(100, 100, (float)(Math.PI / 2), 12, 20);

            rasterizerState = new RasterizerState();
            rasterizerState.FillMode = FillMode.WireFrame;
            rasterizerState.CullMode = CullMode.None;
            rasterizerState.ScissorTestEnable = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Initializes the transforms used by the game.
        /// </summary>
        private void InitializeTransform()
        {

            viewMatrix = Matrix.CreateLookAt(
                new Vector3(0.0f, 0.0f, 1.0f),
                Vector3.Zero,
                Vector3.Up
                );

            projectionMatrix = Matrix.CreateOrthographicOffCenter(
                0,
                (float)GraphicsDevice.Viewport.Width,
                (float)GraphicsDevice.Viewport.Height,
                0,
                1.0f, 1000.0f);
        }

        /// <summary>
        /// Initializes the effect (loading, parameter setting, and technique selection)
        /// used by the game.
        /// </summary>
        private void InitializeEffect()
        {            
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.VertexColorEnabled = true;

            worldMatrix = Matrix.CreateTranslation(GraphicsDevice.Viewport.Width / 2f - 150,
                GraphicsDevice.Viewport.Height / 2f - 50, 0);
            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;


            basicEffectTexture = new BasicEffect(GraphicsDevice);
            basicEffect.VertexColorEnabled = false;
            basicEffectTexture.TextureEnabled = true;
            basicEffectTexture.Texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            basicEffectTexture.Texture.SetData<Color>(new Color[1] { Color.Red });
            worldMatrix = Matrix.CreateTranslation(GraphicsDevice.Viewport.Width / 2f - 150,
                GraphicsDevice.Viewport.Height / 2f - 50, 0);
            basicEffectTexture.World = worldMatrix;
            basicEffectTexture.View = viewMatrix;
            basicEffectTexture.Projection = projectionMatrix;
        }       

        private void InitializeRectangle()
        {
            rectanglePoints = new VertexPositionTexture[4]
            {
                new VertexPositionTexture(new Vector3(0,0,0), new Vector2()),
                new VertexPositionTexture(new Vector3(100,0,0), new Vector2(1 , 0)),
                new VertexPositionTexture(new Vector3(0,100,0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(100,100,0), new Vector2(1,1)),                
            };

            rectangleVertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionTexture.VertexDeclaration, 4, BufferUsage.None);
            rectangleVertexBuffer.SetData<VertexPositionTexture>(rectanglePoints);

            rectangleIndices = new short[4] { 0, 1, 2, 3 };
        }

        private void InitializeRoundedRectangle(float width, float height, float radiusX, float radiusY, int sides)
        {
            int totalPoints = 12 + 4 * (2 * sides + 1);
            roundedRectPoints = new VertexPositionColor[totalPoints];

            float innerLeft = 0 + radiusX;
            float innerTop = 0 + radiusY;
            float innerRight = width - radiusX;
            float innerBottom = height - radiusY;
            int totalIndex = 0;
            // inner rect
            roundedRectPoints[totalIndex++] = new VertexPositionColor(new Vector3(innerLeft, innerBottom, 0), Color.White);
            roundedRectPoints[totalIndex++] = new VertexPositionColor(new Vector3(innerRight, innerBottom, 0), Color.White);
            roundedRectPoints[totalIndex++] = new VertexPositionColor(new Vector3(innerLeft, innerTop, 0), Color.White);
            roundedRectPoints[totalIndex++] = new VertexPositionColor(new Vector3(innerRight, innerTop, 0), Color.White);

            // top rect
            roundedRectPoints[totalIndex++] = new VertexPositionColor(new Vector3(innerLeft, 0, 0), Color.White);
            roundedRectPoints[totalIndex++] = new VertexPositionColor(new Vector3(innerRight, 0, 0), Color.White);

            Vector3[] corner = CreateRoundedCorner(innerRight, innerTop, radiusX, radiusY, sides, Corner.RightTop);
            for (int i = 0; i < corner.Length; i++)
            {
                roundedRectPoints[totalIndex++] = new VertexPositionColor(corner[i], Color.White);
            }

            // right rect
            roundedRectPoints[totalIndex++] = new VertexPositionColor(new Vector3(innerRight, innerBottom, 0), Color.White);
            roundedRectPoints[totalIndex++] = new VertexPositionColor(new Vector3(width, innerBottom, 0), Color.White);

            corner = CreateRoundedCorner(innerRight, innerBottom, radiusX, radiusY, sides, Corner.RightBottom);
            for (int i = 0; i < corner.Length; i++)
            {
                roundedRectPoints[totalIndex++] = new VertexPositionColor(corner[i], Color.White);
            }

            // bottom rect
            roundedRectPoints[totalIndex++] = new VertexPositionColor(new Vector3(innerLeft, innerBottom, 0), Color.White);
            roundedRectPoints[totalIndex++] = new VertexPositionColor(new Vector3(innerLeft, height, 0), Color.White);

            corner = CreateRoundedCorner(innerLeft, innerBottom, radiusX, radiusY, sides, Corner.LeftBottom);
            for (int i = 0; i < corner.Length; i++)
            {
                roundedRectPoints[totalIndex++] = new VertexPositionColor(corner[i], Color.White);
            }

            // left rect
            roundedRectPoints[totalIndex++] = new VertexPositionColor(new Vector3(innerLeft, innerTop, 0), Color.White);
            roundedRectPoints[totalIndex++] = new VertexPositionColor(new Vector3(0, innerTop, 0), Color.White);

            corner = CreateRoundedCorner(innerLeft, innerTop, radiusX, radiusY, sides, Corner.LeftTop);
            for (int i = 0; i < corner.Length; i++)
            {
                roundedRectPoints[totalIndex++] = new VertexPositionColor(corner[i], Color.White);
            }

            roundedRectVertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, totalPoints, BufferUsage.None);
            roundedRectVertexBuffer.SetData<VertexPositionColor>(roundedRectPoints);

            roundedRectIndices = new short[totalPoints];
            for (int i = 0; i < totalPoints; i++)
            {
                roundedRectIndices[i] = (short) i;
            }
        }

        private enum Corner
        {
            RightTop,
            RightBottom,
            LeftBottom,
            LeftTop
        }

        private static Vector3[] CreateRoundedCorner(float centerX, float centerY, float radiusX, float radiusY, int sides, Corner corner)
        {
            float start = 0;
            switch (corner)
            {
                case Corner.RightTop:
                    start = - (float)(Math.PI / 2);
                    break;
                case Corner.RightBottom:
                    break;
                case Corner.LeftBottom:
                    start = (float)(Math.PI / 2);
                    break;
                case Corner.LeftTop:
                    start = (float)(Math.PI);
                    break;
                default:
                    break;
            }

            int nrPoints = 2 * sides + 1;
            Vector3[] points = new Vector3[nrPoints];
            float deltaAngle = (float)(Math.PI / 2) / sides;
            int index = 0;

            for (float i = start; index < nrPoints; i += deltaAngle)
            {
                points[index++] = new Vector3(centerX + radiusX * (float)Math.Cos(i), centerY + radiusY * (float)Math.Sin(i), 0);
                if (index == nrPoints)
                {
                    break;
                }

                points[index++] = new Vector3(centerX, centerY, 0);
            }

            return points;
        }

        public void InitializeRectRoundStroke(float width, float height, float radiusX, float radiusY, int sides, float thickness)
        {
            int totalPoints = 4 + (2 * sides) + 3 + (2 * sides) + 3 + (2 * sides) + 3 + (2 * sides) + 2;
            rectRoundStrokePoints = new VertexPositionColor[totalPoints];

            float outerLeft = 0 + radiusX;
            float outerRight = width - radiusX;
            float outerTop = 0 + radiusY;
            float outerBottom = height - radiusY;
            
            float innerTop = 0 + thickness;            
            float innerBottom = height - thickness;
            float innerLeft = 0 + thickness;
            float innerRight = width - thickness;

            int totalIndex = 0;

            rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(new Vector3(outerLeft, innerTop, 0), Color.White);
            rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(new Vector3(outerLeft, 0, 0), Color.White);
            rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(new Vector3(outerRight, innerTop , 0), Color.White);
            rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(new Vector3(outerRight, 0, 0), Color.White);

            Vector3[] corner = CreateRoundedCornerStroke(outerRight, outerTop, radiusX, radiusY, sides, thickness, Corner.RightTop);
            for (int i = 0; i < corner.Length; i++)
            {
                rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(corner[i], Color.White);
            }

            rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(new Vector3(width, outerTop, 0), Color.White);
            rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(new Vector3(innerRight, outerBottom, 0), Color.White);
            rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(new Vector3(width, outerBottom, 0), Color.White);

            corner = CreateRoundedCornerStroke(outerRight, outerBottom, radiusX, radiusY, sides, thickness, Corner.RightBottom);
            for (int i = 0; i < corner.Length; i++)
            {
                rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(corner[i], Color.White);
            }

            rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(new Vector3(outerRight, height, 0), Color.White);
            rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(new Vector3(outerLeft, innerBottom, 0), Color.White);
            rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(new Vector3(outerLeft, height, 0), Color.White);

            corner = CreateRoundedCornerStroke(outerLeft, outerBottom, radiusX, radiusY, sides, thickness, Corner.LeftBottom);
            for (int i = 0; i < corner.Length; i++)
            {
                rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(corner[i], Color.White);
            }

            rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(new Vector3(0, outerBottom, 0), Color.White);
            rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(new Vector3(innerLeft, outerTop, 0), Color.White);
            rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(new Vector3(0, outerTop, 0), Color.White);

            corner = CreateRoundedCornerStroke(outerLeft, outerTop, radiusX, radiusY, sides, thickness, Corner.LeftTop);
            for (int i = 0; i < corner.Length; i++)
            {
                rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(corner[i], Color.White);
            }

            rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(new Vector3(outerLeft, 0, 0), Color.White);
            rectRoundStrokePoints[totalIndex++] = new VertexPositionColor(new Vector3(outerLeft, innerTop, 0), Color.White);

            rectRoundStrokeIndices = new short[totalPoints];
            for (int i = 0; i < totalPoints; i++)
            {
                rectRoundStrokeIndices[i] = (short)i;
            }
        }

        private Vector3[] CreateRoundedCornerStroke(float centerX, float centerY, float radiusX, float radiusY, int sides, float thickness, Corner corner)
        {
            float start = 0;
            switch (corner)
            {
                case Corner.RightTop:
                    start = -(float)(Math.PI / 2);
                    break;
                case Corner.RightBottom:
                    break;
                case Corner.LeftBottom:
                    start = (float)(Math.PI / 2);
                    break;
                case Corner.LeftTop:
                    start = (float)(Math.PI);
                    break;
                default:
                    break;
            }

            int nrPoints = 2 * sides;
            float innerWidth = radiusX - thickness;
            float innerHeight = radiusY - thickness;
            Vector3[] points = new Vector3[nrPoints];
            float deltaAngle = (float)(Math.PI / 2) / sides;
            int index = 0;
            for (float i = start; index < nrPoints; i += deltaAngle)
            {
                points[index++] = new Vector3(centerX + radiusX * (float)Math.Cos(i), centerY + radiusY * (float)Math.Sin(i), 0);
                if (index == nrPoints)
                {
                    break;
                }

                points[index++] = new Vector3(centerX + innerWidth * (float)Math.Cos(i), centerY + innerHeight * (float)Math.Sin(i), 0);
            }
            
            return points;
        }

        public const Single EpsilonFGreater = (float)1.0e-8;

        private void InitializeEllipseFill(float width, float height, float rotation, int sides)
        {
            ellipsePoints = new VertexPositionColor[sides];
            Vector3[] points = new Vector3[sides];
            float step = (float)(2 * Math.PI) / sides;
            int index = 0;            

            for (float i = -(float)(Math.PI / 2); index < sides; i += step)
            {
                Vector3 position = new Vector3((float)(width * Math.Cos(i)), (float)(height * Math.Sin(i)), 0);
                points[index++] = position;
                if (index == 1 || index == sides)
                {
                    continue;
                }

                position.X = -position.X;
                points[index++] = position;
            }
            
            // this transform will should not be part of generation
            if (Math.Abs(rotation - 0) > EpsilonFGreater)
            {
                Matrix transform = Matrix.CreateRotationZ(rotation);
                for (int i = 0; i < sides; i++)
                {
                    Vector3 rotated = Vector3.Transform(points[i], transform);
                    ellipsePoints[i] = new VertexPositionColor(rotated, Color.White);
                }
            }
            else
            {
                for (int i = 0; i < sides; i++)
                {
                    ellipsePoints[i] = new VertexPositionColor(points[i], Color.White);
                }
            }


            ellipseVertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, sides, BufferUsage.None);
            ellipseVertexBuffer.SetData<VertexPositionColor>(ellipsePoints);

            ellipseIndices = new short[sides];
            for (int i = 0; i < sides; i++)
            {
                ellipseIndices[i] = (short)i;
            }            
        }

        private void InitializeEllipseStroke(float width, float height, float thickness, int sides)
        {
            int strokePoints = (sides * 2) + 2;

            float innerWidth = width - (2 * thickness);
            float innerHeight = height - (2 * thickness);
            eStrokePoints = new VertexPositionColor[strokePoints];            
            float step = (float)(2 * Math.PI) / sides;
            int index = 0;

            for (float i = -(float)(Math.PI / 2); index < strokePoints; i += step)
            {
                Vector3 position = new Vector3((float)(width * Math.Cos(i)), (float)(height * Math.Sin(i)), 0);
                eStrokePoints[index] = new VertexPositionColor(position, Color.White);
                if (index == 0)
                {
                    index ++;
                    continue;
                }

                if (index + 1 == strokePoints)
                {
                    eStrokePoints[index] = new VertexPositionColor(eStrokePoints[2].Position, Color.White);
                    break;
                }

                position = new Vector3((float)(innerWidth * Math.Cos(i)), (float)(innerHeight * Math.Sin(i)), 0);
                eStrokePoints[index + 1] = new VertexPositionColor(position, Color.White);

                index += 2;
            }

            eStrokeVertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, strokePoints, BufferUsage.None);
            eStrokeVertexBuffer.SetData<VertexPositionColor>(eStrokePoints);

            eStrokeIndices = new short[strokePoints];
            for (int i = 0; i < strokePoints; i++)
            {
                eStrokeIndices[i] = (short)i;
            }
        }

        private void InitializePie(float width, float height, float angle, int sides)
        {
            int nrPoints = 2 * sides + 1;
            piePoints = new VertexPositionColor[nrPoints];
            float deltaAngle = angle / sides;
            int index = 0;
            for (int i = 0; index < nrPoints; i++)
            {
                piePoints[index++] = new VertexPositionColor(new Vector3(width * (float)Math.Cos(i * deltaAngle), height * (float)Math.Sin(i * deltaAngle), 0), Color.White);
                if (index == nrPoints)
                {
                    break;
                }

                piePoints[index++] = new VertexPositionColor(Vector3.Zero, Color.Red);
            }

            pieVertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, nrPoints, BufferUsage.None);
            pieVertexBuffer.SetData<VertexPositionColor>(piePoints);

            pieIndices = new short[nrPoints];
            for (int i = 0; i < nrPoints; i++)
            {
                pieIndices[i] = (short)i;
            }
        }

        private void InitializePieStroke(float width, float height, float angle, int sides, float thickness)
        {
            int nrPoints = 2 * sides;
            float innerWidth = width - (2 * thickness);
            float innerHeight = height - (2 * thickness);
            pieStrokePoints = new VertexPositionColor[nrPoints];
            float deltaAngle = angle / sides;
            int index = 0;
            for (float i = -(float)(Math.PI / 2); index < nrPoints; i += deltaAngle)
            {
                pieStrokePoints[index++] = new VertexPositionColor(new Vector3(width * (float)Math.Cos(i), height * (float)Math.Sin(i), 0), Color.White);
                if (index == nrPoints)
                {
                    break;
                }

                pieStrokePoints[index++] = new VertexPositionColor(new Vector3(innerWidth * (float)Math.Cos(i), innerHeight * (float)Math.Sin(i), 0), Color.Red);
            }

            pieStrokeIndices = new short[nrPoints];
            for (int i = 0; i < nrPoints; i++)
            {
                pieStrokeIndices[i] = (short)i;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
            
            CheckKeyboardInput();

            base.Update(gameTime);
        }       

        /// <summary>
        /// Determines which primitive to draw based on input from the keyboard
        /// or game pad.
        /// </summary>
        private void CheckKeyboardInput()
        {
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.A) &&
                 lastKeyboardState.IsKeyUp(Keys.A))
            {
                typeToDraw++;

                if (typeToDraw > PrimType.EllipseStroke)
                    typeToDraw = 0;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            //GraphicsDevice.ScissorRectangle = new Rectangle(10, 10, 500, 500);
            GraphicsDevice.Clear(Color.SteelBlue);

            // The effect is a compiled effect created and compiled elsewhere
            // in the application.

            BasicEffect effect = basicEffect;
            if (typeToDraw == PrimType.Rectangle)
            {
                effect = basicEffectTexture;
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                switch (typeToDraw)
                {
                    case PrimType.Pie:
                        GraphicsDevice.RasterizerState = rasterizerState;
                        DrawPie();
                        break;
                    case PrimType.PieStroke:
                        GraphicsDevice.RasterizerState = rasterizerState;
                        DrawPieStroke();
                        break;
                    case PrimType.Rectangle:
                        GraphicsDevice.RasterizerState = rasterizerState;
                        DrawRectangle();
                        break;
                    case PrimType.Ellipse:
                        GraphicsDevice.RasterizerState = rasterizerState;
                        DrawEllipseFill();
                        break;
                    case PrimType.EllipseStroke:
                        GraphicsDevice.RasterizerState = rasterizerState;
                        DrawEllipseStroke();
                        break;
                    case PrimType.RoundedRect:
                        GraphicsDevice.RasterizerState = rasterizerState;
                        DrawRoundedRect();
                        break;
                    case PrimType.RoundedRectStroke:
                        GraphicsDevice.RasterizerState = rasterizerState;
                        DrawRoundedRectStroke();
                        break;
                }

            }

            base.Draw(gameTime);
        }

        private void DrawRectangle()
        {
            GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(
                PrimitiveType.TriangleStrip,
                rectanglePoints,
                0,  // vertex buffer offset to add to each element of the index buffer
                4,  // number of vertices to draw
                rectangleIndices,
                0,  // first index element to read
                2   // number of primitives to draw
            );
        }

        private void DrawEllipseFill()
        {
            GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleStrip,
                ellipsePoints,
                0,   // vertex buffer offset to add to each element of the index buffer
                ellipsePoints.Length,  // number of vertices to draw
                ellipseIndices,
                0,   // first index element to read
                ellipseIndices.Length - 2  // number of primitives to draw
            );
        }

        private void DrawEllipseStroke()
        {
            GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleStrip,
                eStrokePoints,
                0,   // vertex buffer offset to add to each element of the index buffer
                eStrokePoints.Length,  // number of vertices to draw
                eStrokeIndices,
                0,   // first index element to read
                eStrokeIndices.Length - 2 // number of primitives to draw
            );
        }

        private void DrawPie()
        {
            GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleStrip,
                piePoints,
                0,   // vertex buffer offset to add to each element of the index buffer
                piePoints.Length,  // number of vertices to draw
                pieIndices,
                0,   // first index element to read
                pieIndices.Length - 2 // number of primitives to draw
            );
        }

        private void DrawRoundedRect()
        {
            GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleStrip,
                roundedRectPoints,
                0,  // vertex buffer offset to add to each element of the index buffer
                roundedRectPoints.Length,  // number of vertices to draw
                roundedRectIndices,
                0,  // first index element to read
                roundedRectPoints.Length - 2   // number of primitives to draw
            );
        }

        private void DrawPieStroke()
        {
            GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleStrip,
                pieStrokePoints,
                0,   // vertex buffer offset to add to each element of the index buffer
                pieStrokePoints.Length,  // number of vertices to draw
                pieStrokeIndices,
                0,   // first index element to read
                pieStrokePoints.Length - 2 // number of primitives to draw
            );
        }

        private void DrawRoundedRectStroke()
        {
            GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleStrip,
                rectRoundStrokePoints,
                0,   // vertex buffer offset to add to each element of the index buffer
                rectRoundStrokePoints.Length,  // number of vertices to draw
                rectRoundStrokeIndices,
                0,   // first index element to read
                rectRoundStrokePoints.Length - 2 // number of primitives to draw
            );
        }
    }
}
