using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace View
{
    /// <summary>
    /// View class for rendering all Unit objects of the game
    /// </summary>
    
    class UnitView
    {
        //Variables
        private Model.GameModel m_gameModel;
        private SmokeSystem m_smokeSystem;
        private AnimationSystem m_animationSystem;
        private Camera m_camera;

        //Constructor
        public UnitView(Model.GameModel m_gameModel, Camera a_camera, SmokeSystem m_smokeSystem, AnimationSystem a_animationSystem, GraphicsDevice a_graphicsDevice)
        {
            this.m_gameModel = m_gameModel;
            this.m_smokeSystem = m_smokeSystem;
            this.m_animationSystem = a_animationSystem;
            this.m_camera = a_camera;
        }

        //Draw method
        internal void Draw(float a_elapsedTime, SpriteBatch m_spriteBatch)
        {
            #region Enemies
            foreach (Model.Enemy enemy in m_gameModel.Enemies)
            {
                //Visualizing coordinates
                Vector2 enemyPosition = m_camera.VisualizeCordinates(enemy.Obj.Bounds.X, enemy.Obj.Bounds.Y);

                if (enemy.IsAlive)
                {
                    //Crawler
                    if (enemy.EnemyType == Model.Enemy.Type.Crawler)
                    {
                        //Adjusting position
                        enemyPosition = enemyPosition - new Vector2(9, 6);
                        if (m_gameModel.TMXLevel.TriggerOneIsActive && enemy.UnitState == Model.State.ID.MOVING_RIGHT)
                            enemy.UnitState = Model.State.ID.FACING_RIGHT;
                        else if (m_gameModel.TMXLevel.TriggerOneIsActive && enemy.UnitState == Model.State.ID.MOVING_LEFT)
                            enemy.UnitState = Model.State.ID.FACING_LEFT;
                        
                            m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, enemyPosition, enemy.UnitState, AnimationSystem.Texture.CRAWLER);
                    }
                    //Shooter
                    else if (enemy.EnemyType == Model.Enemy.Type.Shooter)
                    {
                        m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, enemyPosition, enemy.UnitState, AnimationSystem.Texture.SHOOTER);
                    }
                    //Jumper
                    else if (enemy.EnemyType == Model.Enemy.Type.Jumper)
                    {
                        enemyPosition = enemyPosition - new Vector2(12, 0);
                        if (m_gameModel.TMXLevel.TriggerOneIsActive && enemy.UnitState == Model.State.ID.MOVING_LEFT)
                            m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, enemyPosition, Model.State.ID.FACING_LEFT, AnimationSystem.Texture.JUMPER);
                        else if (m_gameModel.TMXLevel.TriggerOneIsActive)
                            m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, enemyPosition, Model.State.ID.FACING_RIGHT, AnimationSystem.Texture.JUMPER);
                        else if (enemy.IsJumping && enemy.UnitState == Model.State.ID.MOVING_LEFT)
                            m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, enemyPosition, Model.State.ID.JUMPING_LEFT, AnimationSystem.Texture.JUMPER);
                        else if (enemy.IsJumping)
                            m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, enemyPosition, Model.State.ID.JUMPING_RIGHT, AnimationSystem.Texture.JUMPER);
                        else
                            m_animationSystem.UpdateAndDraw(a_elapsedTime, Color.White, enemyPosition, enemy.UnitState, AnimationSystem.Texture.JUMPER);
                    }
                }
                else
                {
                    //Drawing dead enemy animations
                    AnimationSystem.Texture texture;

                    if (enemy.UnitState == Model.State.ID.MOVING_RIGHT
                        || enemy.UnitState == Model.State.ID.JUMPING_RIGHT
                        || enemy.UnitState == Model.State.ID.FACING_RIGHT)
                        enemy.UnitState = Model.State.ID.IS_DEAD_RIGHT;
                    if (enemy.UnitState == Model.State.ID.MOVING_LEFT
                        || enemy.UnitState == Model.State.ID.JUMPING_LEFT
                        || enemy.UnitState == Model.State.ID.FACING_LEFT)
                        enemy.UnitState = Model.State.ID.IS_DEAD_LEFT;
                    if (enemy.EnemyType == Model.Enemy.Type.Crawler)
                        texture = AnimationSystem.Texture.CRAWLER;
                    else if (enemy.EnemyType == Model.Enemy.Type.Jumper)
                        texture = AnimationSystem.Texture.JUMPER;
                    else 
                        texture = AnimationSystem.Texture.SHOOTER;

                    //Getting a decreasing float number in order to fade the animation
                    float rbg = enemy.Visibility;
                    m_animationSystem.UpdateAndDraw(a_elapsedTime, new Color(rbg,rbg,rbg,rbg), enemyPosition, enemy.UnitState, texture);
                }
            }

            //Drawing enemy projectiles
            int frameY = 0;
            foreach (Model.Projectile projectile in m_gameModel.Projectiles)
            {
                if (projectile.DirectionLeft)
                    frameY = 1;
                else
                    frameY = 0;
                if (m_camera.GetScreenRectangle.Contains(projectile.Rec))
                    m_animationSystem.DrawMultibleAnimations(Color.White, m_camera.VisualizeCordinates(projectile.Rec.X, projectile.Rec.Y), frameY, AnimationSystem.Texture.SHOOT);
                else
                    projectile.Active = false;
            }
            if (!m_gameModel.TMXLevel.TriggerOneIsActive)
                m_animationSystem.UpdateMultibleAnimations(a_elapsedTime, AnimationSystem.Texture.SHOOT, frameY);

            #endregion

            #region Player
            
            //Visualizing player position
            Vector2 playerPosition = m_camera.VisualizeCordinates(m_gameModel.Player.Obj.Bounds.X - 22, m_gameModel.Player.Obj.Bounds.Y);

            //Drawing smoke @ player if powerup one is active
            if (m_gameModel.Player.PowerUpOneIsActive)
            {
                if (m_smokeSystem.SmokeWasReset)
                    m_smokeSystem.UpdateAndDraw(a_elapsedTime, playerPosition + new Vector2(40, 50), m_spriteBatch);
                else
                {
                    m_smokeSystem.LoadNewSmoke();
                    m_smokeSystem.SmokeWasReset = true;
                }
            }
            else
                m_smokeSystem.SmokeWasReset = false;

            //Getting a decreasing float number in order to fade the animation when the player gets hit
            float c = m_gameModel.Player.Visibility;
            m_animationSystem.UpdateAndDraw(a_elapsedTime, new Color(c,c,c,c), playerPosition, m_gameModel.Player.UnitState, AnimationSystem.Texture.AVATAR); 
           
            #endregion
        }
    }
}
