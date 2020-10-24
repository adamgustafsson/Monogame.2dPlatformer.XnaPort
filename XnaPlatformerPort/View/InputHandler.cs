using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace View
{
    /// <summary>
    /// Class for handeling of all user input
    /// </summary>
    class InputHandler
    {
        #region Variables

        //Keyboard/Mouse states
        private KeyboardState m_kbs;
        private KeyboardState m_prevKbs;
        private MouseState m_mouseState;
        private MouseState m_prevMoseState;

        #endregion

        internal enum Input { MoveLeft = 1 , MoveRight = 2, ClimbUp = 3, ClimbDown = 4, Jump = 5 };

        //Methods for updating and setting Mouse & Keyboard-states
        internal void SetKeyboardState()
        {
            m_prevKbs = m_kbs;
            m_kbs = Keyboard.GetState();
        }
        internal void SetMouseState()
        {
            m_prevMoseState = m_mouseState;
            m_mouseState = Mouse.GetState();
        }

        //Retunrning current mouse-state
        internal MouseState GetMouseState()
        {
            return m_mouseState;
        }

        //Checking input requests
        internal bool IsKeyDown(Input a_input)
        {
            if (a_input == Input.Jump)
                return m_kbs.IsKeyDown(Keys.Space);
            else if (a_input == Input.MoveLeft)
                return m_kbs.IsKeyDown(Keys.Left);
            else if (a_input == Input.MoveRight)
                return m_kbs.IsKeyDown(Keys.Right);
            else if (a_input == Input.ClimbUp)
                return m_kbs.IsKeyDown(Keys.Up);
            else if (a_input == Input.ClimbDown)
                return m_kbs.IsKeyDown(Keys.Down);
            else
                return false;
        }
    }
}
