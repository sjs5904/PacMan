using System;
using System.Collections.Generic;
using System.Media;

namespace PacMan
{
    class Pacman
    {
        // ▀▄ █ ▙ ▀
        private SoundPlayer chompPlayer;
        private SoundPlayer pelletPlayer;
        private SoundPlayer deathPlayer;

        public int _x { get; private set; }
        public int _y { get; private set; }
        public Colors _color { get; private set; }
        public Direction _direction { get; private set; }
        private Direction nextDirection;

        public int _animation { get; private set; } // animation state
        private int _animationtimer;
        private int _animationSpeed; // change animation state at this frame
        private int _moveTimer;
        private int _moveSpeed; // move at this frame
        private int _invincibleTimer;
        private int _invincibleDuration;

        public int _life { get; private set; }
        public bool isInvincible;

        public static Dictionary<int, string[]> _pacmans { get; private set; }
        private string[] _pacman0 = new string[5]
        {
            " ▄▄▄▄ ",
            "██████",
            "██████",
            "██████",
            " ▀▀▀▀ "
        }; // 5*5
        private string[] _pacman1 = new string[5]
        {
            " ▄▄▄▄ ",
            "██████",
            "████▀ ",
            "███▄  ",
            " ▀███▀"
        }; // 5*5
        private string[] _pacman2 = new string[5]
        {
            " ▄▄▄▄ ",
            "██████",
            " ▀████",
            "  ▄███",
            "▀███▀ "
        }; // 5*5
        private string[] _pacman3 = new string[5]
        {
            " ▄▄▄▄ ",
            "██████",
            "██████",
            "█    █",
            "▀█▄▄█▀"
        }; // 5*5
        private string[] _pacman4 = new string[5]
        {
            " ▄▄▄▄ ",
            "██████",
            "██████",
            "██████",
            "▀████▀"
        }; // 5*5

        public Pacman(int x = 67, int y = 57, int life = 1, Colors color = Colors.LightYellow, Direction direction = Direction.NO_DIRECTION)
        {
            chompPlayer = new SoundPlayer();
            chompPlayer.SoundLocation = Environment.CurrentDirectory + @"\pacman_chomp.wav";
            pelletPlayer = new SoundPlayer();
            pelletPlayer.SoundLocation = Environment.CurrentDirectory + @"\power_pellet.wav";
            deathPlayer = new SoundPlayer();
            deathPlayer.SoundLocation = Environment.CurrentDirectory + @"\pacman_death.wav";

            _x = x;
            _y = y;
            _color = color;
            _pacmans = new Dictionary<int, string[]>();
            _pacmans[0] = _pacman0;
            _pacmans[1] = _pacman1;
            _pacmans[2] = _pacman2;
            _pacmans[3] = _pacman3;
            _pacmans[4] = _pacman4;
            _animation = 0;
            _animationtimer = 0;
            _animationSpeed = 5;
            _moveTimer = 0;
            _moveSpeed = 2;
            _invincibleTimer = 0;
            _invincibleDuration = 60;
            _direction = direction;
            nextDirection = Direction.NO_DIRECTION;
            _life = life;
            isInvincible = false;
        }

        public void Update()
        {
            if (isInvincible)
            {
                _invincibleTimer++;
                if (_invincibleTimer == _invincibleDuration)
                {
                    isInvincible = false;
                    _invincibleTimer = 0;
                }
            }
            MoveToDirection();
        }

        private void MoveToDirection()
        {
            Direction prevDirection = _direction;

            ControllAnimation();

            int prevX = _x;
            int prevY = _y;
            _moveTimer++;
            if (_moveTimer != _moveSpeed)
            {
                return;
            }
            _moveTimer = 0;
            Point p = DirectionControl.DirectionToXY(nextDirection);
            if (nextDirection != Direction.NO_DIRECTION && Maze.IsPath(_x + p.x, _y + p.y))
            {
                _direction = nextDirection;
                nextDirection = Direction.NO_DIRECTION;
            }

            if (_x == 47 && _y == 57)
            {
                if (_direction == Direction.DOWN)
                    _direction = prevDirection;
            }

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
            }

            if (Maze.maze[_x,_y] == 1)
            {
                Maze.maze[_x, _y] = 0;
                Program.game.Score += 100;
                chompPlayer.Play();
                for (int i = 0; i < Maze.pacs.Count; i++)
                    if (Maze.pacs[i].x == _x && Maze.pacs[i].y == _y)
                       Maze.pacs.RemoveAt(i);

                if (Maze.pacs.Count == 0 && Maze.pellets.Count == 0)
                {
                    Program.game.isVictory = true;
                    Program.game.GameOver();
                }
            }
            else if (Maze.maze[_x, _y] == 2)
            {
                Maze.maze[_x, _y] = 0;
                Program.game.Score += 200;
                Program.game.TriggerFrightened();
                pelletPlayer.Play();
                for (int i = 0; i < Maze.pellets.Count; i++)
                    if (Maze.pellets[i].x == _x && Maze.pellets[i].y == _y)
                        Maze.pellets.RemoveAt(i);

                if (Maze.pacs.Count == 0 && Maze.pellets.Count == 0)
                {
                    Program.game.isVictory = true;
                    Program.game.GameOver();
                }
            }
        }

        private void ControllAnimation()
        {
            _animationtimer++;
            if (_animation > 1)
                _animation = 1;
            if (_animationtimer == _animationSpeed)
            {
                _animationtimer = 0;
                _animation = (_animation + 1) % 2;
            }
            if (_animation == 1)
            {
                switch (_direction)
                {
                    case Direction.RIGHT:
                        _animation = 1;
                        break;
                    case Direction.LEFT:
                        _animation = 2;
                        break;
                    case Direction.DOWN:
                        _animation = 3;
                        break;
                    case Direction.UP:
                        _animation = 4;
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetDirection(Direction direction)
        {
            Point p = DirectionControl.DirectionToXY(direction);
            nextDirection = direction;
        }

        public void Killed()
        {
            if (!isInvincible)
            {
                TriggerInvincible();
                deathPlayer.Play();
                _life--;
                _x = 67;
                _y = 57;
                _direction = Direction.NO_DIRECTION;
                if (_life <= 0)
                {
                    Program.game.GameOver();
                }
            }
        }

        public void TriggerInvincible()
        {
            isInvincible = true;
        }
    }
}
