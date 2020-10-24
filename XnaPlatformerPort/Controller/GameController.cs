using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Controller
{
    /// <summary>
    /// GameController
    /// Main controller class for the game
    /// </summary>
    class GameController
    {
        //Variables
        private Model.GameModel m_gameModel;
        private View.GameView m_gameView;
        private Model.Player m_player;

        //Constructor
        public GameController(Model.GameModel a_gameModel, View.GameView a_gameView)
        {
            this.m_gameModel = a_gameModel;
            this.m_gameView = a_gameView;
            this.m_player = a_gameModel.Player;
        }

        internal void UpdateSimulation(float a_elapsedTime)
        {
            //Setting last player location, preventing collission issues
            m_player.SetLastLocation();

            #region Left,Right,Up,Down input controll and status settings

            //Right
            if (m_gameView.DidPressKey(View.InputHandler.Input.MoveRight))
            {
                m_player.Obj.Bounds.X += Convert.ToInt32(a_elapsedTime * m_player.Speed.X);

                if (m_gameModel.Player.PowerUpOneIsActive)
                    m_player.UnitState = Model.State.ID.FLYING_RIGHT;
                else
                    m_player.UnitState = Model.State.ID.MOVING_RIGHT;
            }
            //Left
            else if (m_gameView.DidPressKey(View.InputHandler.Input.MoveLeft))
            {
                m_player.Obj.Bounds.X -= Convert.ToInt32(a_elapsedTime * m_player.Speed.X);

                if (m_gameModel.Player.PowerUpOneIsActive)
                    m_player.UnitState = Model.State.ID.FLYING_LEFT;
                else
                    m_player.UnitState = Model.State.ID.MOVING_LEFT;
            }
            //Up
            else if (m_gameView.DidPressKey(View.InputHandler.Input.ClimbUp) && (m_player.CanClimb || m_player.PowerUpOneIsActive))
            {
                m_player.Obj.Bounds.Y -= Convert.ToInt32(a_elapsedTime * m_player.Speed.X);

                m_player.IsClimbing = true;
                if (!m_gameModel.Player.PowerUpOneIsActive)
                    m_player.UnitState = Model.State.ID.CLIMBING;

            }
            //Down
            else if (m_gameView.DidPressKey(View.InputHandler.Input.ClimbDown) && (m_player.CanClimb || m_player.PowerUpOneIsActive))
            {
                m_player.Obj.Bounds.Y += Convert.ToInt32(a_elapsedTime * m_player.Speed.X);

                m_player.IsClimbing = true;
                if (!m_gameModel.Player.PowerUpOneIsActive)
                    m_player.UnitState = Model.State.ID.CLIMBING;
            }
            //Non-movment states
            else if (!m_player.PowerUpOneIsActive)
            {
                if (m_player.UnitState == Model.State.ID.MOVING_LEFT
                    || m_player.UnitState == Model.State.ID.FLYING_LEFT)
                    m_player.UnitState = Model.State.ID.FACING_LEFT;
                else if (m_player.UnitState == Model.State.ID.MOVING_RIGHT
                    || m_player.UnitState == Model.State.ID.FLYING_RIGHT)
                    m_player.UnitState = Model.State.ID.FACING_RIGHT;
                else if (m_player.UnitState == Model.State.ID.CLIMBING)
                    m_player.UnitState = Model.State.ID.FACING_AWAY;
                else if (m_player.UnitState == Model.State.ID.CLIMBING_DOWN)
                    m_player.UnitState = Model.State.ID.FACING_AWAY;
                else if (m_player.UnitState == Model.State.ID.JUMPING_RIGHT)
                    m_player.UnitState = Model.State.ID.FACING_RIGHT;
                else if (m_player.UnitState == Model.State.ID.JUMPING_LEFT)
                    m_player.UnitState = Model.State.ID.FACING_LEFT;
            } 

            #endregion

            #region Jump/powerup input controll and status settings

            if (m_gameView.DidPressKey(View.InputHandler.Input.Jump) && !m_player.IsJumping && !m_player.IsFalling)
            {
                m_player.IsJumping = true;
            }
            //Jump-states
            if (m_player.IsJumping == true)
            {
                if (m_player.UnitState == Model.State.ID.FACING_LEFT)
                    m_player.UnitState = Model.State.ID.JUMPING_LEFT;
                else if (m_player.UnitState == Model.State.ID.FACING_RIGHT)
                    m_player.UnitState = Model.State.ID.JUMPING_RIGHT;
                else if (m_player.UnitState == Model.State.ID.MOVING_LEFT)
                    m_player.UnitState = Model.State.ID.JUMPING_LEFT;
                else if (m_player.UnitState == Model.State.ID.MOVING_RIGHT)
                    m_player.UnitState = Model.State.ID.JUMPING_RIGHT;
            }
            //Power-up states
            if (m_player.PowerUpOneIsActive)
            {
                if (m_player.UnitState == Model.State.ID.JUMPING_LEFT
                    || m_player.UnitState == Model.State.ID.FACING_LEFT)
                    m_player.UnitState = Model.State.ID.FLYING_LEFT;
                if (m_player.UnitState == Model.State.ID.JUMPING_RIGHT
                    || m_player.UnitState == Model.State.ID.FACING_RIGHT)
                    m_player.UnitState = Model.State.ID.FLYING_RIGHT;
            } 

            #endregion

            //Updating gameengine
            m_gameModel.UpdateSimulation(a_elapsedTime);
        }

        internal void DrawGraphics(float a_elapsedTime)
        {
            //Updating and drawing graphics
            m_gameView.UpdateAndDraw(a_elapsedTime);
        }
    }
}
