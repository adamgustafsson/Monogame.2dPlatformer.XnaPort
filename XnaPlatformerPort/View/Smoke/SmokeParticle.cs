using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace View
{
    /// <summary>
    ///  Object class; Smoke particle
    /// </summary>
    class SmokeParticle
    {
        //Variables
        private const float DELAY_MAX = 15f;
        private const float LIFETIME_MAX = 7f;
        private const float MAX_SIZE = 192f;

        private Random m_random;
        private float m_delay;
        private Vector2 m_position;
        private float m_randomXspeed;
        private float m_timeLived;
        private Vector2 m_speed;
        private Vector2 m_gravity;
        private float m_size;

        //Constructor
        public SmokeParticle(int a_seed)
        {
            this.m_random = new Random(a_seed * 31);
            this.m_timeLived = 8;
            this.m_size = 16f;
            this.m_delay = (float)m_random.NextDouble() * DELAY_MAX;
            this.m_randomXspeed = ((float)m_random.NextDouble()) * 30;
            this.m_gravity = new Vector2((float)m_random.NextDouble(), -50f);
            this.m_speed = new Vector2(m_randomXspeed, 0f);

        }

        //Respawnmethod
        private void Respawn(Vector2 a_position)
        {
            m_timeLived = 0;
            m_size = 16f;
            m_position = a_position;
            m_speed = new Vector2(m_randomXspeed, 0);
            PositionIsSet = false;
        }

        //Updating behaviour
        internal void Update(float a_elapsedTime, Vector2 a_position)
        {

            if (!PositionIsSet)
            {
                m_position = a_position;
                PositionIsSet = true;
            }

            if (m_delay > 0)
            {
                m_delay -= a_elapsedTime;
                return;
            }

            if (m_timeLived > LIFETIME_MAX)
            {
                Respawn(a_position);
            }

            m_timeLived += a_elapsedTime;

            m_speed = m_speed + m_gravity * a_elapsedTime;

            m_position = m_position + m_speed * a_elapsedTime;

            float lifePercent = m_timeLived / LIFETIME_MAX;
            m_size = 16f + lifePercent * MAX_SIZE;

        }

        //Resetting particle life circle
        internal int Reset
        {
            set { m_timeLived = 8; }
        }

        #region Properties

        internal bool IsActive()
        {
            return m_timeLived > 0;
        }

        internal bool PositionIsSet { get; set; }

        internal float GetPositionX
        {
            get { return m_position.X; }
            set { m_position.X = value; }
        }

        internal float GetPositionY
        {
            get { return m_position.Y; }
            set { m_position.Y = value; }
        }

        internal float GetSize
        {
            get { return m_size; }
            set { m_size = value; }
        }

        internal float GetVisibility()
        {
            if (m_timeLived > 7)
                return 0;
            else
                return 0.3f / m_timeLived;
        } 

        #endregion
    }
}
