using System;
using System.Collections.Generic;
using System.Text;

namespace PacMan
{
    public struct Point
    {
        public int x;
        public int y;

        public int GetTileDist(int nx, int ny)
        {
            return ((nx - x) * (nx - x) + (ny - y) * (ny - y));
        }
    }
    class GameManager
    {
        public char[,] renderingMaze = new char[125, 115];
        public Maze gameMaze;

        private Ghost Blinky;
        private Ghost Pinky;
        private Ghost Inky;
        private Ghost Clyde;

        public Pacman Pack;

        public int Score { get; set; }

        public bool isOver { get; private set; }
        public bool isVictory { get; set; }

        public GameManager()
        {
            gameMaze = new Maze();
            Pack = new Pacman(67, 57, 3, Colors.LightYellow, Direction.NO_DIRECTION);
            Blinky = new Ghost(57, 47, Colors.DarkRed, GhostType.Blinky, Pack, Direction.RIGHT);
            Pinky = new Ghost(57, 52, Colors.LightPurple, GhostType.Pinky, Pack, Direction.RIGHT);
            Inky = new Ghost(57, 62, Colors.LightCyan, GhostType.Inky, Pack, Direction.RIGHT);
            Clyde = new Ghost(57, 67, Colors.DarkYellow, GhostType.Clyde, Pack, Direction.RIGHT);

            Score = 0;
            isOver = false;
        }
        

        public void Update()
        {
            Blinky.Update();
            Pinky.Update();
            Inky.Update();
            Clyde.Update();
            Pack.Update();
        }

        public void Rendering()
        {
            Render.DrawMap();
            Render.DrawCoins();
            Render.DrawPellets();
            Render.DrawGhost(Blinky);
            Render.DrawGhost(Pinky);
            Render.DrawGhost(Inky);
            Render.DrawGhost(Clyde);
            Render.DrawPacman(Pack);
            Render.DrawUI();
            Render.DrawTime();
            Render.DrawScore();
            Render.DrawLife(Pack);
            Render.Plot();
        }

        public void PlayerSetDirection(Direction direction)
        {
            Pack.SetDirection(direction);
        }

        public void TriggerFrightened()
        {
            Blinky.TriggerFrightened();
            Pinky.TriggerFrightened();
            Inky.TriggerFrightened();
            Clyde.TriggerFrightened();
        }

        public void GameOver()
        {
            isOver = true;
        }

        public void PrintVictory()
        {
            Console.Write(Render.victory);
        }

        public void PrintDefeat()
        {
            Console.Write(Render.tombstone);
        }
    }
}
