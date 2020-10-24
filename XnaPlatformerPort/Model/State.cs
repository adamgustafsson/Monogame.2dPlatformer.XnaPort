using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// Class containing all Unit/Object states
    /// </summary>

    class State
    {
        internal enum ID
        {
            FACING_CAMERA = 1,
            FACING_LEFT = 2,
            FACING_RIGHT = 3,
            FACING_AWAY = 4,
            MOVING_LEFT = 5,
            MOVING_RIGHT = 6,
            CLIMBING = 7,
            CLIMBING_DOWN = 8,
            JUMPING_RIGHT = 9,
            JUMPING_LEFT = 10,
            FLYING_RIGHT = 11,
            FLYING_LEFT = 12,
            IS_DEAD_RIGHT = 13,
            IS_DEAD_LEFT = 14,
            VERTICAL_ANIMATION = 15,
            STATIC_ONE = 16,
            STATIC_TWO = 17
        };
    }
}
