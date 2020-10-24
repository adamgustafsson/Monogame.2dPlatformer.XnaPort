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
    /// Enemy object class. Inherit; Unit.
    /// </summary>
    class Enemy : Unit
    {
        //Enum for enemy types
        internal enum Type { Crawler = 1, Jumper = 2, Shooter = 3 };

        //Variables
        private Stopwatch m_jumpWatch;
        private Int32 m_jumpTime;

        //Properties
        internal bool IsMovingRight { get; set; }
        internal bool IsMovingLeft { get; set; }
        internal Type EnemyType { get; set; }
        internal Int32 MaxRightLocation { get; set; }
        internal Int32 MaxLeftLocation { get; set; }
        internal float ProjectileCoolDown { get; set; }
        internal float JumpCoolDown { get; set; }

        //Constructor
        public Enemy(MapObject a_enemyObject, Type a_type)
        {
            this.Obj = a_enemyObject;
            this.Obj.Bounds.Width = 32;
            this.Obj.Bounds.Height = 32;
            this.EnemyType = a_type;
            this.IsAlive = true;
            this.Visibility = 1;
            this.MaxLeftLocation = 0;
            this.MaxRightLocation = 0;
            this.m_jumpWatch = new Stopwatch();

            SetTypeSpecificProperties();
        }

        //Setting enemy type specific properties
        private void SetTypeSpecificProperties()
        {
            if (EnemyType == Type.Crawler)
            {
                this.Speed = new Vector2(220f, 5f);
            }
            else if (EnemyType == Type.Jumper)
            {
                this.Obj.Bounds.Width = 38;
                this.Obj.Bounds.Height = 52;
                this.Speed = new Vector2(100f, 10f);
                this.m_jumpTime = 2000;
            }
            else if (EnemyType == Type.Shooter)
            {
                this.UnitState = State.ID.FACING_LEFT;
                this.Obj.Bounds.Width = 48;
                this.Obj.Bounds.Height = 52;
                this.Speed = new Vector2(240f, 25f);
                this.m_jumpTime = 3000;
                this.ProjectileCoolDown = 2;
            }
        }

        internal void Update(float a_elapsedTime)
        {
            //Updating internal jump-timer
            if (!IsJumping)
            {
                m_jumpWatch.Start();
            }
            if (m_jumpWatch.ElapsedMilliseconds > m_jumpTime)
            {
                IsJumping = true;
                m_jumpWatch.Stop();
                m_jumpWatch.Reset();
            }

        }
    }
}
