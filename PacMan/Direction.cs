using System;
using System.Collections.Generic;
using System.Text;

namespace PacMan
{
    public enum Direction
    {
        UP = 1,
        DOWN,
        LEFT,
        RIGHT,
        NO_DIRECTION
    };
    class DirectionControl
    {
        public static Direction GetOpposite(Direction direction)
        {
            switch (direction)
            {
                case Direction.UP:
                    return Direction.DOWN;
                case Direction.DOWN:
                    return Direction.UP;
                case Direction.RIGHT:
                    return Direction.LEFT;
                case Direction.LEFT:
                    return Direction.RIGHT;
            }
            return Direction.UP;
        }

        public static Point DirectionToXY(Direction direction)
        {
            Point p;
            p.x = 0;
            p.y = 0;
            switch (direction)
            {
                case Direction.UP:
                    p.x--;
                    break;
                case Direction.DOWN:
                    p.x++;
                    break;
                case Direction.LEFT:
                    p.y--;
                    break;
                case Direction.RIGHT:
                    p.y++;
                    break;
                case Direction.NO_DIRECTION:
                    break;
            }
            return p;
        }
    }
}
