using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace PacMan
{

    [Flags]
    public enum Colors : short
    {
        HighIntensity = 0x0008,
        Black = 0x0000,
        DarkBlue = 0x0001,
        DarkGreen = 0x0002,
        DarkRed = 0x0004,
        Gray = DarkBlue | DarkGreen | DarkRed,
        DarkYellow = DarkRed | DarkGreen,
        DarkPurple = DarkRed | DarkBlue,
        DarkCyan = DarkGreen | DarkBlue,
        LightBlue = DarkBlue | HighIntensity,
        LightGreen = DarkGreen | HighIntensity,
        LightRed = DarkRed | HighIntensity,
        LightWhite = Gray | HighIntensity,
        LightYellow = DarkYellow | HighIntensity,
        LightPurple = DarkPurple | HighIntensity,
        LightCyan = DarkCyan | HighIntensity
    }
    static class Render
    {
        public static string victory =
        "\n                                       o\n" +
        "                                      $\"\"$o\n" +
        "                                     $\"  $$\n" +
        "                                      $$$$\n" +
        "                                      o \"$o\n" +
        "                                     o\"  \"$\n" +
        "                oo\"$$$\"  oo$\"$ooo   o$    \"$    ooo\"$oo  $$$\"o\n" +
        "   o o o o    oo\"  o\"      \"o    $$o$\"     o o$\"\"  o$      \"$  " +
        "\"oo   o o o o\n" +
        "   \"$o   \"\"$$$\"   $$         $      \"   o   \"\"    o\"         $" +
        "   \"o$$\"    o$$\n" +
        "     \"\"o       o  $          $\"       $$$$$       o          $  ooo" +
        "     o\"\"\n" +
        "        \"o   $$$$o $o       o$        $$$$$\"       $o        \" $$$$" +
        "   o\"\n" +
        "         \"\"o $$$$o  oo o  o$\"         $$$$$\"        \"o o o o\"  " +
        "\"$$$  $\n" +
        "           \"\" \"$\"     \"\"\"\"\"            \"\"$\"            \"" +
        "\"\"      \"\"\" \"\n" +
        "            \"oooooooooooooooooooooooooooooooooooooooooooooooooooooo$\n" +
        "             \"$$$$\"$$$$\" $$$$$$$\"$$$$$$ \" \"$$$$$\"$$$$$$\"  $$$\"" +
        "\"$$$$\n" +
        "              $$$oo$$$$   $$$$$$o$$$$$$o\" $$$$$$$$$$$$$$ o$$$$o$$$\"\n" +
        "              $\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"" +
        "\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"$\n" +
        "              $\"                                                 \"$\n" +
        "              $\"$\"$\"$\"$\"$\"$\"$\"$\"$\"$\"$\"$\"$\"$\"$\"$\"$\"$\"" +
        "$\"$\"$\"$\"$\"$\"$\"$\n" +
        "                                      You win!\n\n";

        public static string tombstone =
          "\n\n\n\n                /\"\"\"\"\"/\"\"\"\"\"\"\".\n" +
          "               /     /         \\             __\n" +
          "              /     /           \\            ||\n" +
          "             /____ /   Rest in   \\           ||\n" +
          "            |     |    Pieces     |          ||\n" + 
          "            |     |               |          ||\n" +
          "            |     |   A. Luser    |          ||\n" +
          "            |     |               |          ||\n" +
          "            |     |     * *   * * |         _||_\n" +
          "            |     |     *\\/* *\\/* |        | TT |\n" +
          "            |     |     *_\\_  /   ...\"\"\"\"\"\"| |" +
          "| |.\"\"....\"\"\"\"\"\"\"\".\"\"\n" +
          "            |     |         \\/..\"\"\"\"\"...\"\"\"" +
          "\\ || /.\"\"\".......\"\"\"\"...\n" +
          "            |     |....\"\"\"\"\"\"\"........\"\"\"\"\"" + 
          "\"^^^^\".......\"\"\"\"\"\"\"\"..\"\n" +
          "            |......\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"......" + 
          "..\"\"\"\"\"....\"\"\"\"\"..\"\"...\"\"\".\n\n" +
          "            You're dead.  Better luck in the next life.\n\n\n";

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "WriteConsoleOutputW")]
        static extern bool WriteConsoleOutput(
          SafeFileHandle hConsoleOutput,
          CharInfo[] lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          ref SmallRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct CharUnion
        {
            [FieldOffset(0)] public char UnicodeChar;
            [FieldOffset(0)] public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo
        {
            [FieldOffset(0)] public CharUnion Char;
            [FieldOffset(2)] public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public Coord dwFontSize;
            public int FontFamily;
            public int FontWeight;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] // Edit sizeconst if the font name is too big
            public string FaceName;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern Int32 SetCurrentConsoleFontEx(
        IntPtr ConsoleOutput,
        bool MaximumWindow,
        ref CONSOLE_FONT_INFO_EX ConsoleCurrentFontEx);

        private enum StdHandle
        {
            OutputHandle = -11
        }

        [DllImport("kernel32")]
        private static extern IntPtr GetStdHandle(StdHandle index);

        public static CharInfo[] buf = new CharInfo[230 * 135];
        public static SmallRect rect = new SmallRect() { Left = 0, Top = 0, Right = 230, Bottom = 135 };

        // 0: 점수, 1: 통로, 7: 빈공간, 9: 벽
        // 3: 빨간유령, 4: 파란유령, 5: 주황유령, 6: 분홍유령
        public static void DrawPacman(Pacman p)
        {
            int x = p._x;
            int y = p._y * 2;

            for (int i = x - 2; i < x + 3; i++)
            {
                for (int j = y - 2; j < y + 4; j++)
                {
                    if (Pacman._pacmans[p._animation][i - x + 2][j - y + 2] == ' ')
                        continue;
                    if (p.isInvincible && p._animation == 0)
                        buf[i * 230 + j].Attributes = (short)Colors.HighIntensity ;
                    else 
                        buf[i * 230 + j].Attributes = (short)p._color;
                    buf[i * 230 + j].Char.UnicodeChar = Pacman._pacmans[p._animation][i - x + 2][j - y + 2];
                }
            }
        }

        public static void DrawGhost(Ghost g)
        {
            int x = g._x;
            int y = g._y * 2;
            
            for (int i = x - 2; i < x + 3; i++)
            {
                for (int j = y - 2; j < y + 4; j++)
                {
                    if (Ghost._ghosts[g._animation][i - x + 2][j - y + 2] == ' ')
                        continue;
                    if (g.state == GhostState.Frightened)
                    {
                        if (g._frightenedTimer > g._frightenedDuration - 60 && g._frightenedTimer % 2 == 0)
                        {
                            buf[i * 230 + j].Attributes = (short)g._color;
                        }
                        else
                        {
                            buf[i * 230 + j].Attributes = (short)Colors.LightBlue;
                        }
                    }
                    else if (g.state == GhostState.Eaten)
                        buf[i * 230 + j].Attributes = (short)Colors.Gray;
                    else
                        buf[i * 230 + j].Attributes = (short)g._color;
                    buf[i * 230 + j].Char.UnicodeChar = Ghost._ghosts[g._animation][i - x + 2][j - y + 2];
                }
            }
        }

        public static void DrawCoins()
        {
            foreach (Point p in Maze.pacs)
            {
                buf[p.x * 230 + p.y * 2].Attributes = (short)Colors.LightYellow;
                buf[p.x * 230 + p.y * 2 + 1].Attributes = (short)Colors.LightYellow;
                buf[p.x * 230 + p.y * 2].Char.UnicodeChar = '█';
                buf[p.x * 230 + p.y * 2 + 1].Char.UnicodeChar = '█';
            }
        }

        public static void DrawPellets()
        {
            foreach (Point p in Maze.pellets)
            {
                int x = p.x;
                int y = p.y * 2;
                for (int i = x - 1; i < x + 2; i++)
                {
                    for (int j = y - 1; j < y + 3; j++)
                    {
                        if (Maze._pellet[i - x + 1][j - y + 1] == ' ')
                            continue;
                        buf[i * 230 + j].Attributes = (short)Colors.LightYellow;
                        buf[i * 230 + j].Char.UnicodeChar = Maze._pellet[i - x + 1][j - y + 1];
                    }
                }
            }
        }

        public static void DrawMap()
        {
            for (int i = 0; i < Maze.maze.GetLength(0); i++)
            {
                for (int j = 0; j < Maze.maze.GetLength(1); j++)
                {
                    if (Maze.maze[i, j] == 0 || Maze.maze[i, j] == 1 || Maze.maze[i, j] == 2 || Maze.maze[i, j] == 3 || Maze.maze[i, j] == 7)
                    {
                        buf[i * 230 + j * 2].Char.UnicodeChar = ' ';
                        buf[i * 230 + j * 2 + 1].Char.UnicodeChar = ' ';
                    }
                    else if (Maze.maze[i, j] == 6 || Maze.maze[i, j] == 8)
                    {
                        buf[i * 230 + j * 2].Attributes = (short)Colors.DarkYellow;
                        buf[i * 230 + j * 2 + 1].Attributes = (short)Colors.DarkYellow;
                        buf[i * 230 + j * 2].Char.UnicodeChar = '█';
                        buf[i * 230 + j * 2 + 1].Char.UnicodeChar = '█';
                    }
                    else if (Maze.maze[i, j] == 9)
                    {
                        buf[i * 230 + j * 2].Attributes = (short)Colors.DarkBlue;
                        buf[i * 230 + j * 2 + 1].Attributes = (short)Colors.DarkBlue;
                        buf[i * 230 + j * 2].Char.UnicodeChar = '█';
                        buf[i * 230 + j * 2 + 1].Char.UnicodeChar = '█';
                    }
                }
            }
        }

        public static void DrawUI()
        {
            for (int i = 0; i < Maze.bottomUI.GetLength(0); i++)
            {
                for (int j = 0; j < Maze.bottomUI.GetLength(1); j++)
                {
                    if (Maze.bottomUI[i, j] == 7)
                    {
                        buf[(i + 125) * 230 + j * 2].Char.UnicodeChar = ' ';
                        buf[(i + 125) * 230 + j * 2 + 1].Char.UnicodeChar = ' ';
                    }
                    else if (Maze.bottomUI[i, j] == 8)
                    {
                        buf[(i + 125) * 230 + j * 2].Attributes = (short)Colors.LightWhite;
                        buf[(i + 125) * 230 + j * 2 + 1].Attributes = (short)Colors.LightWhite;
                        buf[(i + 125) * 230 + j * 2].Char.UnicodeChar = '█';
                        buf[(i + 125) * 230 + j * 2 + 1].Char.UnicodeChar = '█';
                    }
                    else if (Maze.bottomUI[i, j] == 9)
                    {
                        buf[(i + 125) * 230 + j * 2].Attributes = (short)Colors.DarkBlue;
                        buf[(i + 125) * 230 + j * 2 + 1].Attributes = (short)Colors.DarkBlue;
                        buf[(i + 125) * 230 + j * 2].Char.UnicodeChar = '█';
                        buf[(i + 125) * 230 + j * 2 + 1].Char.UnicodeChar = '█';
                    }
                }
            }
        }

        public static void DrawTime()
        {
            int initialPosX = 127;
            int initialPosY = 23;

            long time = (Program.timeStamp - Program.startTime) / 100;
            string stringTime = time.ToString();
            int timeLength = stringTime.Length;
            if (timeLength > 10)
                stringTime = "9999999999";
            for (int i = 0; i < timeLength; i++)
            {
                char digit = stringTime[i];
                for (int j = 0; j < 5; j++)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        buf[(initialPosX + j) * 230 + initialPosY * 2 + k + 8 * i].Attributes = (short)Colors.LightWhite;
                        buf[(initialPosX + j) * 230 + initialPosY * 2 + k + 8 * i].Char.UnicodeChar = Numbers.numberset[digit][j][k];
                    }
                }
            }
        }

        public static void DrawLife(Pacman p)
        {
            int initialPosX = 67;
            int initialPosY = 3;

            for (int n = 0; n < p._life - 1; n++) {
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        buf[(i + initialPosX) * 230 + (j + initialPosY) + n * 8].Attributes = (short)Colors.LightYellow;
                        buf[(i + initialPosX) * 230 + (j + initialPosY) + n * 8].Char.UnicodeChar = Pacman._pacmans[1][i][j];
                    }
                }
            }
        }

        public static void DrawScore()
        {
            int initialPosX = 127;
            int initialPosY = 110;

            int score = Program.game.Score;
            string stringScore = score.ToString();
            int timeLength = stringScore.Length;
            for (int i = 0; i < timeLength; i++)
            {
                char digit = stringScore[timeLength - i - 1];
                for (int j = 0; j < 5; j++)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        buf[(initialPosX + j) * 230 + initialPosY * 2 + k - 8 * i].Attributes = (short)Colors.LightWhite;
                        buf[(initialPosX + j) * 230 + initialPosY * 2 + k - 8 * i].Char.UnicodeChar = Numbers.numberset[digit][j][k];
                    }
                }
            }
        }

        public static void Plot()
        {
            SafeFileHandle h = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            CONSOLE_FONT_INFO_EX ConsoleFontInfo = new CONSOLE_FONT_INFO_EX();
            ConsoleFontInfo.cbSize = (uint)Marshal.SizeOf(ConsoleFontInfo);
            ConsoleFontInfo.FaceName = "Lucida Console";
            ConsoleFontInfo.dwFontSize.X = 4;
            ConsoleFontInfo.dwFontSize.Y = 7;
            SetCurrentConsoleFontEx(GetStdHandle(StdHandle.OutputHandle), false, ref ConsoleFontInfo);

            if (!h.IsInvalid)
            {
                
                bool b = WriteConsoleOutput(h, buf,
                          new Coord() { X = 230, Y = 135 },
                          new Coord() { X = 0, Y = 0 },
                          ref rect);
            }
        }
    }
}
