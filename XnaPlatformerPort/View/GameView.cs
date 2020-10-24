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
    /// Main view class for game graphics
    /// </summary>
    class GameView
    {
        //Variables
        private Model.GameModel m_gameModel;
        private SpriteBatch m_spriteBatch;
        private GraphicsDevice m_graphicsDevice;
        private InputHandler m_inputHandler;
        private Camera m_camera;
        private GraphicView m_graphicView;
        private UnitView m_unitView;
        private UIView m_uIView;
        private ItemView m_itemView;
        private AnimationSystem m_animationSystem;
        private SmokeSystem m_smokeSystem;

        //Constructor
        public GameView(Model.GameModel a_gameModel, InputHandler a_inputHandler, SpriteBatch a_spriteBatch, GraphicsDevice a_graphicsDevice)
        {
            this.m_smokeSystem = new SmokeSystem();
            this.m_animationSystem = new AnimationSystem(a_spriteBatch);
            this.m_gameModel = a_gameModel;
            this.m_spriteBatch = a_spriteBatch;
            this.m_graphicsDevice = a_graphicsDevice;
            this.m_inputHandler = a_inputHandler;
            this.m_camera = new Camera(a_graphicsDevice, a_gameModel);
            this.m_graphicView = new GraphicView(a_graphicsDevice, m_camera);
            this.m_unitView = new UnitView(m_gameModel, m_camera, m_smokeSystem, m_animationSystem, a_graphicsDevice);
            this.m_uIView = new UIView(m_gameModel, m_camera, m_animationSystem);
            this.m_itemView = new ItemView(m_gameModel, m_camera, m_animationSystem);
        }

        //Loading visual content
        internal void LoadContent(ContentManager a_content)
        {
            m_graphicView.LoadContent(a_content);
            m_uIView.LoadContent(a_content);
            m_animationSystem.LoadContent(a_content);
            m_smokeSystem.LoadContent(a_content);
        }

        //Draw method
        internal void UpdateAndDraw(float a_elapsedTime)
        {
            //Updating camera position
            m_camera.UpdateCamera();

            m_spriteBatch.Begin();

            //Drawing background
            m_graphicView.DrawBackground(m_spriteBatch);

            //Drawing map layers
            #region TileLayers
            m_gameModel.CurrentMap.DrawLayer(m_spriteBatch, m_gameModel.TMXLevel.BackgroundIndex, m_camera.GetScreenRectangle, 0f);
            m_gameModel.CurrentMap.DrawLayer(m_spriteBatch, m_gameModel.TMXLevel.ForegroundIndex, m_camera.GetScreenRectangle, 0f);
            //Trigger layer
            if (m_gameModel.TMXLevel.TriggerTwoIsActive)
                m_gameModel.CurrentMap.DrawLayer(m_spriteBatch, m_gameModel.TMXLevel.TriggerIndex, m_camera.GetScreenRectangle, 0f);
            #endregion
            #region ObjLayers (Debug only)
            //m_gameModel.CurrentMap.DrawObjectLayer(m_spriteBatch, m_gameModel.TMXLevel.CollisionIndex, m_camera.GetScreenRectangle, 0f);
            //m_gameModel.CurrentMap.DrawObjectLayer(m_spriteBatch, m_gameModel.TMXLevel.DeathIndex, m_camera.GetScreenRectangle, 0f);
            //m_gameModel.CurrentMap.DrawObjectLayer(m_spriteBatch, m_gameModel.TMXLevel.PlayerIndex, m_camera.GetScreenRectangle, 0f);
            //m_gameModel.CurrentMap.DrawObjectLayer(m_spriteBatch, m_gameModel.TMXLevel.EnemiesIndex, m_camera.GetScreenRectangle, 0f);
            //m_gameModel.CurrentMap.DrawObjectLayer(m_spriteBatch, m_gameModel.TMXLevel.MovementAidIndex, m_camera.GetScreenRectangle, 0f);
            //m_gameModel.CurrentMap.DrawObjectLayer(m_spriteBatch, m_gameModel.TMXLevel.ItemsAndTriggersIndex, m_camera.GetScreenRectangle, 0f);
            #endregion

            //Drawing game componets via the correspondent view-classes
            m_graphicView.DrawLeaves(a_elapsedTime, m_spriteBatch);
            m_itemView.Draw(a_elapsedTime, m_spriteBatch);
            m_graphicView.DrawPlatforms(m_spriteBatch, m_gameModel.TMXLevel);
            m_unitView.Draw(a_elapsedTime, m_spriteBatch);
            m_uIView.Draw(a_elapsedTime, m_spriteBatch);

            m_spriteBatch.End();
        }

        //Inputhandling
        internal bool DidPressKey(InputHandler.Input a_input)
        {
            return m_inputHandler.IsKeyDown(a_input);
        }
    }
}
