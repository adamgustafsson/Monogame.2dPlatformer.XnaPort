using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace View
{
    /// <summary>
    /// View class for updating and drawing game items and triggers
    /// </summary>
    class ItemView
    {
        //Variables
        private Model.Player m_player;
        private Model.TMXLevel m_level;
        private Camera m_camera;
        private AnimationSystem m_animationSystem;
        private Stopwatch m_trampolineWatch;

        //Constructor
        public ItemView(Model.GameModel a_gameModel, Camera a_camera, AnimationSystem a_animationSystem)
        {
            this.m_player = a_gameModel.Player;
            this.m_level = a_gameModel.TMXLevel;
            this.m_camera = a_camera;
            this.m_animationSystem = a_animationSystem;
            this.m_trampolineWatch = new Stopwatch();
        }

        //Drawing items and triggers
        internal void Draw(float a_elapsedTime, SpriteBatch a_spriteBatch)
        {
            #region Collectable Items & Triggers
            //Seperate updating calls for items
            m_animationSystem.UpdateMultibleAnimations(a_elapsedTime, AnimationSystem.Texture.COIN, 0);
            m_animationSystem.UpdateMultibleAnimations(a_elapsedTime, AnimationSystem.Texture.POWER_UP_ONE, 0);
            m_animationSystem.UpdateMultibleAnimations(a_elapsedTime, AnimationSystem.Texture.EXTRA_LIFE, 0);

            foreach (Vector4 item in m_level.GetItemsInRegion(m_camera.GetScreenRectangle))
            {
                //Getting the items/triggers as a Vector4: position x, position y, visibility, type
                Vector2 pos = m_camera.VisualizeCordinates((int)item.X, (int)item.Y);
                float isVisible = item.W;
                float type = item.Z;

                //Coins
                if (isVisible == 1 && type == (int)Model.TMXLevel.ItemType.Coin)
                    m_animationSystem.DrawMultibleAnimations(Color.White, pos, 0, AnimationSystem.Texture.COIN);
                //Power up one
                if (isVisible == 1 && type == (int)Model.TMXLevel.ItemType.PowerUpOne && !m_player.PowerUpOneIsActive)
                {
                    pos = pos - new Vector2(16, 16);
                    m_animationSystem.DrawMultibleAnimations(Color.White, pos, 0, AnimationSystem.Texture.POWER_UP_ONE);
                }
                //Extra life
                if (isVisible == 1 && type == (int)Model.TMXLevel.ItemType.ExtraLife)
                {
                    pos = pos - new Vector2(4, 5);
                    m_animationSystem.DrawMultibleAnimations(Color.White, pos, 0, AnimationSystem.Texture.EXTRA_LIFE);
                }
                //Trigger one
                if (isVisible == 1 && type == (int)Model.TMXLevel.ItemType.TriggerOne)
                {
                    if (m_level.TriggerOneIsActive)
                    {
                        pos = pos - new Vector2(7, 20);
                        m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, pos, Model.State.ID.STATIC_ONE, AnimationSystem.Texture.TRIGGER_ONE);
                    }
                    else
                    {
                        pos = pos - new Vector2(18, 20);
                        m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, pos, Model.State.ID.STATIC_TWO, AnimationSystem.Texture.TRIGGER_ONE);
                    }
                }
                //Triger two
                if (isVisible == 1 && type == (int)Model.TMXLevel.ItemType.TriggerTwo)
                {
                    if (m_level.TriggerTwoIsActive)
                    {
                        pos = pos - new Vector2(7, 20);
                        m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, pos, Model.State.ID.STATIC_ONE, AnimationSystem.Texture.TRIGGER_TWO);
                    }
                    else
                    {
                        pos = pos - new Vector2(18, 20);
                        m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, pos, Model.State.ID.STATIC_TWO, AnimationSystem.Texture.TRIGGER_TWO);
                    }
                }
            } 
            #endregion

            #region MovementAids
            foreach (Vector4 item in m_level.GetMovmentAidsInRegion(m_camera.GetScreenRectangle))
            {
                //Getting the MovementAid object as a Vector4: position x, position y, visibility, type
                Vector2 pos = m_camera.VisualizeCordinates((int)item.X - 10, (int)item.Y - 10);
                float isVisible = item.W;
                float type = item.Z;

                //Check if the player hits a trampoline
                if (m_player.SpeedY == 10)
                    m_trampolineWatch.Start();

                //Differentiate animation when triggered
                if (type == (int)Model.TMXLevel.MovementAidType.Trampoline && m_trampolineWatch.IsRunning && m_trampolineWatch.ElapsedMilliseconds < 1000)
                    m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, pos, Model.State.ID.STATIC_TWO, AnimationSystem.Texture.TRAMPOLINE);

                else if (type == (int)Model.TMXLevel.MovementAidType.Trampoline)
                {
                    m_trampolineWatch.Stop();
                    m_trampolineWatch.Reset();
                    m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, pos, Model.State.ID.STATIC_ONE, AnimationSystem.Texture.TRAMPOLINE);
                }
            } 
            #endregion
        }
    }
}
