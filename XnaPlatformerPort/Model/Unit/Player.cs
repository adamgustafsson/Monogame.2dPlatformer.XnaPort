using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// Player object class. Inherit; Unit.
    /// </summary>
    class Player : Unit
    {
        //Variables
        private bool m_gotHit;
        private Stopwatch m_hitWatch;
        private Stopwatch m_puOneWatch; //PowerUp one watch

        //Properties
        internal bool CanClimb { get; set; }
        internal bool IsClimbing { get; set; }
        internal bool PowerUpOneIsActive { get; set; }
        internal int Coins { get; set; }

        //Constructor
        public Player(MapObject a_playerObject)
        {
            this.Obj = a_playerObject;
            this.Obj.Bounds.Width = 20;
            this.Obj.Bounds.Height = 64;
            this.StartLocation = new Point(100, 500);
            this.Obj.Bounds.Location = this.StartLocation;
            this.Speed = new Vector2(250f, 5f);
            this.Visibility = 1f;
            this.IsAlive = true;
            this.CanClimb = false;
            this.IsClimbing = false;
            this.UnitState = State.ID.FACING_RIGHT;
            this.Life = 3;
            this.Coins = 0;
            this.m_hitWatch = new Stopwatch();
            this.m_puOneWatch = new Stopwatch();
        }

        //Internal updates
        internal void Update(float a_elapsedTime)
        {
            if (m_hitWatch.ElapsedMilliseconds >= 1700 && m_gotHit)
            {
                m_gotHit = false;
                Visibility = 1f;
                m_hitWatch.Stop();
                m_hitWatch.Reset();
            }

            //Lifehandeling
            if (Life <= 0)
                IsAlive = false;
            if (!IsAlive)
            {
                Obj.Bounds.Location = StartLocation;
                IsAlive = true;
                IsOnGround = false;
                Life = 3;
            }

            if (Coins >= 100)
            {
                Life++;
                Coins = 0;
            }

            //Powerup one timer
            if (PowerUpOneIsActive)
            {
                m_puOneWatch.Start();
            }
            if (m_puOneWatch.ElapsedMilliseconds >= 8000)
            {
                PowerUpOneIsActive = false;
                IsFalling = true;
                IsOnGround = false;
                m_puOneWatch.Stop();
                m_puOneWatch.Reset();
            }
        }

        internal void SetLastLocation()
        {
            LastLocation = Obj.Bounds.Location;
        }

        internal void LostLife()
        {
            if (!m_gotHit)
            {
                m_hitWatch.Start();
                m_gotHit = true;
                Visibility = 0.5f;
                Life--;
            }
        }

    }
}
