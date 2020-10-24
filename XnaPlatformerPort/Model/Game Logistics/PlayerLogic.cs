using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// Model class for handeling of all player logic
    /// </summary>
    class PlayerLogic
    {
        //Variables
        private Player m_player;
        private TMXLevel m_level;

        //Constructor
        public PlayerLogic(TMXLevel a_TMXLevel, Player a_player)
        {
            this.m_player = a_player;
            this.m_level = a_TMXLevel;
        }

        //Updating player logic
        internal void Update(float a_elapsedTime)
        {
            if (!m_player.IsAlive)
                m_level.ResetTriggersAndItems();

            //Updating internal player mechanics
            m_player.Update(a_elapsedTime);

            //Collision modification MovingPlatforms
            //Needs to be updated before Horizontinal & Vertical
            if(!m_level.TriggerOneIsActive)
                m_level.CheckMovingPlatformCollision(m_player.Obj);

            //Collision handeling Horizontinal
            m_level.CheckHorizontalCollision(m_player);

            //Fall & jump mechanics
            //Needs to be updated before Vertical collision
            #region Jumping/falling
            if (!m_player.IsOnGround && !m_player.IsJumping && !m_player.IsClimbing && !m_player.PowerUpOneIsActive)
            {
                m_player.IsFalling = true;
                m_player.Obj.Bounds.Y += Convert.ToInt32(a_elapsedTime * m_player.Speed.X);
            }
            else if (m_player.IsJumping)
            {
                if (m_player.IsClimbing || m_player.PowerUpOneIsActive)
                    m_player.IsJumping = false;
                else
                {
                    m_player.Obj.Bounds.Y -= (int)m_player.Speed.Y;
                    m_player.SpeedY -= a_elapsedTime / 0.10f;
                }
            }
            else
                m_player.IsFalling = false; 
            #endregion

            //Ladder climbing mechanics
            #region Climbing
            if (m_level.UnitIsOnLadder(m_player))
                m_player.CanClimb = true;
            else
            {
                m_player.CanClimb = false;
                m_player.IsClimbing = false;
            }

            if (m_player.IsClimbing)
                m_player.IsOnGround = false;
            #endregion

            //Trampoline mechanics
            if (m_level.UnitTriggeredTrampoline(m_player))
            {
                m_player.SpeedY = 10;
                m_player.IsJumping = true;
            }

            //Collision handeling; Vertical and Obstacles
            m_level.CheckVerticalCollision(m_player);
            m_level.CheckObstaclesCollision(m_player);
            //Checks for interaction with collectable items
            m_level.CheckTriggersAndItems(m_player);

        }
    }
}
