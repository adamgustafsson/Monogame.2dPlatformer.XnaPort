using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// Objectclass for enemy projectiles
    /// </summary>
    class Projectile
    {
        //Variables
        private Rectangle m_projectile;

        //Constructor
        public Projectile(int a_x, int a_y, bool a_directionLeft)
        {
            this.m_projectile = new Rectangle(a_x, a_y ,10, 10);
            this.Speed = 200;
            this.DirectionLeft = a_directionLeft;
            this.Active = true;
        }

        //Properties
        internal Boolean DirectionLeft { get; set; }
        internal bool Active { get; set; }
        internal float Speed { get; set; }
        internal Rectangle Rec
        {
            get { return m_projectile; }
        }
        internal Int32 X
        {
            get { return m_projectile.X; }
            set { m_projectile.X = value; }
        }
        internal Int32 Y
        {
            get { return m_projectile.Y; }
            set { m_projectile.Y = value; }
        }

    }
}
