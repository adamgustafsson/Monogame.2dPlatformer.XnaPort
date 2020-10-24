using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// Interface class for Unit objects (Player, Enemy)
    /// </summary>
    class Unit
    {
        //Variables
        private Vector2 m_speed;

        //Unit-Properties
        internal MapObject Obj { get; set; }
        internal Point LastLocation { get; set; }
        internal Point StartLocation { get; set; }

        internal int Life { get; set; }
        internal float Visibility { get; set; }
        internal bool IsAlive { get; set; }
        internal bool IsJumping { get; set; }
        internal bool IsFalling { get; set; }
        internal bool IsOnGround { get; set; }
        internal bool DidHitRoof { get; set; }
        internal bool IsOnTemporaryPlatform { get; set; }
        internal Model.State.ID UnitState { get; set; }

        internal Vector2 Speed
        {
            get { return m_speed; }
            set { m_speed = value; }
        }
        internal float SpeedY
        {
            get { return m_speed.Y; }
            set { m_speed.Y = value; }
        }
        internal float SpeedX
        {
            get { return m_speed.X; }
            set { m_speed.X = value; }
        }
        internal Rectangle Rectangle
        {
            get { return Obj.Bounds; }
        }

    }
}
