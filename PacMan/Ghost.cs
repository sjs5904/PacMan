using System;
using System.Collections.Generic;
using System.Media;

namespace PacMan
{
    public enum GhostState
    {
        Chase,
        Scatter,
        Frightened,
        Eaten
    }

    public enum GhostType
    {
        Blinky,
        Pinky,
        Inky,
        Clyde
    }

    class Ghost
    {
        // ▀▄ █ ▙ ▀
        private SoundPlayer eatenPlayer;
        private Random rand = new Random();
        public int _x { get; private set; }
        public int _y { get; private set; }
        public Colors _color { get; private set; }
        private GhostType _nickname;

        public int _animation { get; private set; } // animation state
        private int _animationtimer;
        private int _animationSpeed; // change animation state at this frame
        private int _moveTimer;
        private int _moveSpeed; // move at this frame
        private int _stateTimer;
        private int _stateFrequency;
        private int _targetTimer;
        private int _targetFrequency;
        public int _frightenedTimer { get; private set; }
        public int _frightenedDuration { get; private set; }
        private Direction _direction;

        public GhostState state { get; private set; }
        private Point target;
        private Pacman _pack;

        public static Dictionary<int, string[]> _ghosts { get; private set; }
        private static string[] _ghost0 = new string[5]
        {
            " ▄▄▄▄ ",
            "██████",
            "█◦██◦█",
            "██████",
            "▀ ▀▀ ▀"
        }; // 5*5
        private static string[] _ghost1 = new string[5]
        {
            " ▄▄▄▄ ",
            "██████",
            "█◦██◦█",
            "██████",
            " ▀  ▀ "
        }; // 5*5
        private static string[] _ghost2 = new string[5] // Frightened
        {
            " ▄▄▄▄ ",
            "██████",
            "█X██X█",
            " ████ ",
            " ▀  ▀ "
        };
        private static string[] _ghost3 = new string[5] // Eaten
        {
            "      ",
            " ████ ",
            "█X██X█",
            " ████ ",
            "      "
        };

        public Ghost(int x, int y, Colors color, GhostType nickname, Pacman pack, Direction direction = Direction.DOWN)
        {
            eatenPlayer = new SoundPlayer();
            eatenPlayer.SoundLocation = Environment.CurrentDirectory + @"\pacman_eatghost.wav";

            _x = x;
            _y = y;
            _color = color;
            _ghosts = new Dictionary<int, string[]>();
            _ghosts[0] = _ghost0;
            _ghosts[1] = _ghost1;
            _ghosts[2] = _ghost2;
            _ghosts[3] = _ghost3;

            _animation = 0;
            _animationtimer = 0;
            _animationSpeed = 15;

            _moveTimer = 0;
            _moveSpeed = 2;

            _stateTimer = 0;
            _stateFrequency = 120;
            _direction = direction;

            _targetTimer = 0;
            _targetFrequency = 180;

            _frightenedTimer = 0;
            _frightenedDuration = 240;

            state = GhostState.Scatter;

            _nickname = nickname;
            _pack = pack;
            target.x = _x;
            target.y = _y;
        }

        public void Update()
        {
            UpdateTarget();
            MoveToDirection();
        }

        private void UpdateTarget()
        {
            if (state == GhostState.Scatter || state == GhostState.Frightened)
            {
                _targetTimer++;
                if (state == GhostState.Frightened)
                    _targetTimer++;
                if (_targetTimer >= _targetFrequency)
                {
                    ChangeTarget();
                }
            }
            else if ((state == GhostState.Scatter || state == GhostState.Eaten) && target.x == _x && target.y == _y)
            {
                ChangeTarget();
                if (state == GhostState.Eaten)
                {
                    _moveSpeed = _moveSpeed * 2;
                    state = GhostState.Scatter;
                }
            }
            else if (state == GhostState.Eaten)
            {
                target.x = 57;
                target.y = 57;
            }
            else if (state == GhostState.Chase)
            {
                Point tempp;
                tempp.x = _pack._x;
                tempp.y = _pack._y;
                if (_nickname == GhostType.Blinky || tempp.GetTileDist(_x, _y) < 400)
                {
                    target = tempp;
                }
                else if (_nickname == GhostType.Pinky)
                {
                    if (_pack._direction == Direction.UP)
                        tempp.y -= 24;
                    else if (_pack._direction == Direction.DOWN)
                        tempp.y += 24;
                    else if(_pack._direction == Direction.LEFT)
                        tempp.x -= 24;
                    else if(_pack._direction == Direction.RIGHT)
                        tempp.x += 24;
                    target = tempp;
                }
                else if (_nickname == GhostType.Inky)
                {
                    tempp.x = Math.Max(7, tempp.x + rand.Next(-36, 36));
                    tempp.y = Math.Max(7, tempp.y + rand.Next(-36, 36));
                    target = tempp;
                }
                else if (_nickname == GhostType.Clyde)
                {
                    state = GhostState.Scatter;
                    ChangeTarget();
                }
            }
        }

        private void ChangeTarget()
        {   // 62, 57
            _targetTimer = 0;
            if (state == GhostState.Frightened)
            {
                target.x = rand.Next(7, 117);
                target.y = rand.Next(7, 107);
            }
            else if (_nickname == GhostType.Blinky)
            {
                target.x = rand.Next(7, 72);
                target.y = rand.Next(48, 107);
            }
            else if (_nickname == GhostType.Pinky)
            {
                target.x = rand.Next(7, 72);
                target.y = rand.Next(7, 67);
            }
            else if (_nickname == GhostType.Inky)
            {
                target.x = rand.Next(43, 117);
                target.y = rand.Next(48, 107);
            }
            else if (_nickname == GhostType.Clyde)
            {
                target.x = rand.Next(33, 117);
                target.y = rand.Next(7, 77);
            }
        }

        private void SelectDirection()
        {
            Direction prevDirection = _direction;

            _stateTimer++;
            if (_stateFrequency == _stateTimer)
            {
                _stateTimer = 0;
                ChangeState();
            }
            //if (state == GhostState.Scatter || state == GhostState.Chase || state == GhostState.Eaten || state == GhostState.Frightened)
            //{
                int minDist = int.MaxValue;
                if (Maze.IsPath(_x, _y - 1) && prevDirection != Direction.RIGHT && minDist > target.GetTileDist(_x, _y - 1))
                {
                    minDist = target.GetTileDist(_x, _y - 1);
                    _direction = Direction.LEFT;
                }
                if (Maze.IsPath(_x, _y + 1) && prevDirection != Direction.LEFT && minDist > target.GetTileDist(_x, _y + 1))
                {
                    minDist = target.GetTileDist(_x, _y + 1);
                    _direction = Direction.RIGHT;
                }
                if (Maze.IsPath(_x - 1, _y) && prevDirection != Direction.DOWN && minDist > target.GetTileDist(_x - 1, _y))
                {
                    minDist = target.GetTileDist(_x - 1, _y);
                    _direction = Direction.UP;
                }
                if (Maze.IsPath(_x + 1, _y) && prevDirection != Direction.UP && minDist > target.GetTileDist(_x + 1, _y))
                {
                    minDist = target.GetTileDist(_x + 1, _y);
                    _direction = Direction.DOWN;
                }

                if (_x == 47 && _y == 57 && state != GhostState.Eaten)
                {
                    if (_direction == Direction.DOWN)
                        _direction = prevDirection;
                }
                else if (_x == 92 && _y == 52 && state != GhostState.Eaten)
                {
                    if (_direction == Direction.UP)
                        _direction = prevDirection;
                }
                else if (_x == 92 && _y == 62 && state != GhostState.Eaten)
                {
                    if (_direction == Direction.UP)
                        _direction = prevDirection;
                }
            //}
            /*
            else if (state == GhostState.Frightened)
            {
                int rng = rand.Next(1, 5);
                Point p = DirectionControl.DirectionToXY((Direction) rng);
                while(rng == (int) DirectionControl.GetOpposite(_direction) || !Maze.IsPath(_x + p.x, _y + p.y))
                {
                    rng = rand.Next(1, 5);
                    p = DirectionControl.DirectionToXY((Direction)rng);
                }
                _direction = (Direction)rng;
            }*/
        }

        private void MoveToDirection()
        {
            ControlAnimation();

            int prevX = _x;
            int prevY = _y;
            _moveTimer++;
            if (_moveTimer != _moveSpeed)
            {
                return;
            }
            _moveTimer = 0;
            SelectDirection();
            switch (_direction)
            {
                case Direction.UP:
                    _x--;
                    break;
                case Direction.DOWN:
                    _x++;
                    break;
                case Direction.LEFT:
                    _y--;
                    break;
                case Direction.RIGHT:
                    _y++;
                    break;
                default:
                    break;
            }

            if (_x < 0)
                _x = Maze.maze.GetLength(0) - 1;
            else if (_x >= Maze.maze.GetLength(0))
                _x = 0;
            else if (_y < 0)
                _y = Maze.maze.GetLength(1) - 1;
            else if (_y >= Maze.maze.GetLength(1))
                _y = 0;

            if (!Maze.IsPath(_x, _y))
            {
                _x = prevX;
                _y = prevY;
                TurnDirection();
                if (state == GhostState.Scatter)
                    ChangeTarget();
            }

            if ((_pack._x - _x) * (_pack._x - _x) + (_pack._y - _y) * (_pack._y - _y) < 8)
            {
                MeetPacman();
            }
        }

        private void ChangeState()
        {
            if (state == GhostState.Scatter)
            {
                state = GhostState.Chase;
            }
            else if (state == GhostState.Chase)
            {
                state = GhostState.Scatter;
            }
        }

        private void TurnDirection()
        {
            if (_direction == Direction.DOWN)
            {
                _direction = Direction.UP;
            }
            else if (_direction == Direction.UP)
            {
                _direction = Direction.DOWN;
            }
            else if (_direction == Direction.RIGHT)
            {
                _direction = Direction.LEFT;
            }
            else if (_direction == Direction.LEFT)
            {
                _direction = Direction.RIGHT;
            }
        }

        private void MeetPacman()
        {
            if (state == GhostState.Chase || state == GhostState.Scatter)
                _pack.Killed();
            else if (state == GhostState.Frightened) 
            { 
                TriggerEaten();
                Program.game.Score += 500;
            }
        }

        private void ControlAnimation()
        {
            if (state == GhostState.Chase || state == GhostState.Scatter)
            {
                _animationtimer++;
                if (_animationtimer == _animationSpeed)
                {
                    _animationtimer = 0;
                    _animation = (_animation + 1) % 2;
                }
            }
            else if (state == GhostState.Frightened)
            {
                _animation = 2;
                _frightenedTimer++;
                if (_frightenedTimer == _frightenedDuration)
                {
                    _frightenedTimer = 0;
                    state = GhostState.Scatter;
                }
            }
            else if (state == GhostState.Eaten)
            {
                _animation = 3;
            }
        }

        public void SetState(GhostState newState)
        {
            state = newState;
        }

        public void TriggerFrightened()
        {
            state = GhostState.Frightened;
            TurnDirection();
        }

        public void TriggerEaten()
        {
            eatenPlayer.Play();
            _moveSpeed = _moveSpeed / 2;
            state = GhostState.Eaten;
            TurnDirection();
        }
    }
}
