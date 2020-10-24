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
    /// View class for rendering the game's User Interface
    /// </summary>
    class UIView
    {
        //Variables
        private Model.GameModel m_gameModel;
        private Camera m_camera;
        private AnimationSystem m_animationSystem;
        private SpriteFont m_font;

        //Constructor
        public UIView(Model.GameModel m_gameModel, Camera m_camera, AnimationSystem a_animationSystem)
        {
            this.m_gameModel = m_gameModel;
            this.m_camera = m_camera;
            this.m_animationSystem = a_animationSystem;
        }

        //Loading fonts
        internal void LoadContent(ContentManager a_content)
        {
            m_font = a_content.Load<SpriteFont>("Fonts/ui-font");
        }

        //Drawing Life and Coin status
        internal void Draw(float a_elapsedTime, SpriteBatch a_spriteBatch)
        {
            Vector2 pos = m_camera.VisualizeCordinates(10 + m_camera.GetScreenRectangle.X, 5);
            m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, pos, Model.State.ID.STATIC_ONE, AnimationSystem.Texture.EXTRA_LIFE);
            m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, pos + new Vector2(2,40), Model.State.ID.STATIC_TWO, AnimationSystem.Texture.COIN);
            a_spriteBatch.DrawString(m_font, m_gameModel.Player.Life.ToString(), m_camera.VisualizeCordinates(55 + m_camera.GetScreenRectangle.X, 11), Color.YellowGreen);
            a_spriteBatch.DrawString(m_font, m_gameModel.Player.Coins.ToString(), m_camera.VisualizeCordinates(55 + m_camera.GetScreenRectangle.X, 48), Color.LightGoldenrodYellow);
        }
    }
}
