using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace View
{
    /// <summary>
    /// View class for rendering seperate graphical effects
    /// </summary>
    class GraphicView
    {
        //Variables
        private Camera m_camera;
        private Texture2D m_textureLeaf;
        private Texture2D m_platform;
        private Texture2D m_backGround;
        private Vector2[] m_vecLeafPositions;
        private Vector2[] m_vecLeafVelocities;

        //Constructor
        public GraphicView(GraphicsDevice a_graphicsDevice, Camera a_camera)
        {
            this.m_camera = a_camera;
            InitializeLeafGraphic(a_graphicsDevice);
        }

        //Initializing leafs
        private void InitializeLeafGraphic(GraphicsDevice a_graphicsDevice)
        {
            m_vecLeafPositions = new Vector2[20];
            m_vecLeafVelocities = new Vector2[m_vecLeafPositions.Length];
            Random random = new Random();
            for (int index = 0; index < m_vecLeafPositions.Length; index++)
            {
                m_vecLeafPositions[index] = new Vector2(
                    random.Next(a_graphicsDevice.Viewport.Width), random.Next(a_graphicsDevice.Viewport.Height));
                m_vecLeafVelocities[index] = new Vector2(
                    1 + random.Next(4), 1 + random.Next(3));
            }
        }

        //Loading content
        internal void LoadContent(ContentManager a_content)
        {
            m_textureLeaf = a_content.Load<Texture2D>("Graphics/Leaf");
            m_platform = a_content.Load<Texture2D>("Graphics/platform");
            m_backGround = a_content.Load<Texture2D>("Graphics/Background2");

        }

        //Draw methods
        internal void DrawLeaves(float a_elapsedTime, SpriteBatch a_spriteBatch)
        {
            // leaf animations
            Random random = new Random();
            for (int index = 0; index < m_vecLeafPositions.Length; index++)
            {
                m_vecLeafPositions[index] += m_vecLeafVelocities[index];
                if (m_vecLeafPositions[index].X > m_camera.GetScreenRectangle.Width)
                {
                    m_vecLeafPositions[index].X = -m_textureLeaf.Width;
                    m_vecLeafVelocities[index].X = 1 + random.Next(4);
                    m_vecLeafVelocities[index].Y = 1 + random.Next(3);
                }
                if (m_vecLeafPositions[index].Y > m_camera.GetScreenRectangle.Height)
                {
                    m_vecLeafPositions[index].Y = -m_textureLeaf.Height;
                    m_vecLeafVelocities[index].X = 1 + random.Next(4);
                    m_vecLeafVelocities[index].Y = 1 + random.Next(3);
                }
            }

            foreach (Vector2 vecLeaf in m_vecLeafPositions)
            {
                float fRotation = ((vecLeaf.X + vecLeaf.Y) * 0.01f) % MathHelper.TwoPi;
                a_spriteBatch.Draw(m_textureLeaf, vecLeaf, null, Color.White, fRotation, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            }
        }
        internal void DrawBackground(SpriteBatch a_spriteBatch)
        {
            a_spriteBatch.Draw(m_backGround, Vector2.Zero, Color.White);
        }
        internal void DrawPlatforms(SpriteBatch a_spriteBatch, Model.TMXLevel a_level)
        {
            foreach (Model.MovingPlatform platform in a_level.MovingPlatforms.Where(x => x.GetPlatformArea.Intersects(m_camera.GetScreenRectangle)))
            {
                Vector2 position = m_camera.VisualizeCordinates(platform.GetPlatformArea.X, platform.GetPlatformArea.Y);
                a_spriteBatch.Draw(m_platform, position, Color.White);
            }
        }
    }
}
