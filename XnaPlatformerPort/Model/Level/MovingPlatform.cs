using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    ///  Object class for moving platforms
    /// </summary>
    class MovingPlatform
    {
        //Variables
        private MapObject m_platform;
        private Polyline m_path;
        private Int32 m_currentLine;
        private Int32 m_direction;
        private Point m_change;
        private Vector2 m_position;

        //Constructor
        public MovingPlatform(MapObject a_pathObj, MapObject a_platformObj)
        {
            this.m_path = a_pathObj.Polyline;
            this.m_platform = a_platformObj;
            this.m_direction = 1;
            this.m_currentLine = 0;
            this.m_change = Point.Zero;
            this.m_position = new Vector2(m_platform.Bounds.X, m_platform.Bounds.Y);
            this.m_platform.Bounds.Width = 96;
            this.m_platform.Bounds.Height = 32;
        }

        //Updating platform movements/path
        internal void Update(float a_elapsedTime)
        {
            if (Vector2.Distance(m_path.Lines[m_currentLine].Start, m_position) > m_path.Lines[m_currentLine].Length)
            {
                if (m_currentLine + 1 < m_path.Lines.Length)
                {
                    m_currentLine += 1;
                    m_position = m_path.Lines[m_currentLine].Start;
                }
                else
                {
                    m_direction = -1;
                    m_position = m_path.Lines[m_currentLine].End;
                }
            }
            else if(Vector2.Distance(m_position, m_path.Lines[m_currentLine].End) > m_path.Lines[m_currentLine].Length)
            {
                if (m_currentLine - 1 >= 0)
                {
                    m_currentLine -= 1;
                    m_position = m_path.Lines[m_currentLine].End;
                }
                else
                {
                    m_direction = 1;
                    m_position = m_path.Lines[m_currentLine].Start;
                }
            }

            float targetPct = (m_path.Lines[m_currentLine].Length - Vector2.Distance(m_position, m_path.Lines[m_currentLine].End)
                                + (a_elapsedTime * 100) * m_direction) / m_path.Lines[m_currentLine].Length;

            m_position = Vector2.Lerp(m_path.Lines[m_currentLine].Start, m_path.Lines[m_currentLine].End, targetPct);
            m_change.X = (int)m_position.X - this.m_platform.Bounds.X;
            m_change.Y = (int)m_position.Y - this.m_platform.Bounds.Y;


            this.m_platform.Bounds.X = (int)m_position.X;
            this.m_platform.Bounds.Y = (int)m_position.Y;

        }

        //Getters
        internal Rectangle GetPlatformArea
        {
            get { return m_platform.Bounds; }
        }
        internal Point Change
        {
            get { return m_change; }
        }
    }
}
