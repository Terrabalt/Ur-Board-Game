using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using C3.XNA;

namespace UrBoardGame
{
    class UrBoardWindowInterface
    {
        UrBoard board;
        public Vector2 position;
        public int size;
        bool leftPlayer;

        List<int> playableMoves;
        bool rolling; bool[] rollResult; int totalRoll;

        public UrBoardWindowInterface(UrBoard _board, Vector2 _position, bool _leftPlayer = false, int _size = 64)
        {
            board = _board;
            position = _position;
            leftPlayer = _leftPlayer;
            size = _size;
            rolling = false;
            playableMoves = null;
        }
        Point hover(Point mousePosition)
        {
            if (mousePosition.X < position.X 
                || mousePosition.Y < position.Y 
                || mousePosition.X > position.X + (size * UrBoard.board.GetLength(0)) - 1 
                || mousePosition.Y > position.Y + (size * UrBoard.board.GetLength(1)) - 1) return new Point(-1);
            Point o;
            o.X = (int)Math.Floor((mousePosition.X - position.X) / size);
            o.Y = (int)Math.Floor((mousePosition.Y - position.Y) / size);
            return o;
        }

        public void play(Point mousePosition)
        {
            if (!rolling && playableMoves == null) { rolling = true; }
            else if (rolling)
            {
                rolling = false;
                rollResult = board.roll();
                totalRoll = 0;
                for (int i = 0; i < rollResult.Length; i++)
                {
                    totalRoll += (rollResult[i] ? 1 : 0);
                }
                playableMoves = board.getPossibleMoves(totalRoll, leftPlayer);
            }
            else
            {
                bool changePlayer = false, nextTurn = false;
                if (playableMoves.Count == 0)
                {
                    changePlayer = true;
                    nextTurn = true;
                }
                else
                {
                    if (hover(mousePosition).X != -1)
                    {
                        int currPos = board.getPionByBoardPos(hover(mousePosition), leftPlayer);
                        if (currPos > -1)
                        {
                            changePlayer = board.move(currPos, totalRoll, leftPlayer);
                            nextTurn = true;
                        }
                        else
                        {
                            int nextPos = UrBoard.roadFromTilePos(hover(mousePosition), leftPlayer) - totalRoll;
                            if (nextPos > -1 && nextPos <= (leftPlayer ? UrBoard.leftRoad : UrBoard.rightRoad).Length)
                            {
                                currPos = board.getPionByBoardPos((leftPlayer ? UrBoard.leftRoad : UrBoard.rightRoad)[nextPos], leftPlayer);
                                if (playableMoves.Contains(currPos))
                                {
                                    changePlayer = board.move(currPos, totalRoll, leftPlayer);
                                    nextTurn = true;
                                }
                            }
                            else if (nextPos == -1)
                            {

                                if ((leftPlayer ? board.leftPions : board.rightPions)[playableMoves[playableMoves.Count - 1]] == 0)
                                {
                                    changePlayer = board.move(playableMoves[playableMoves.Count - 1], totalRoll, leftPlayer);
                                    nextTurn = true;
                                }
                            }
                        }
                    }
                }
                if (nextTurn)
                {
                    playableMoves = null;
                    rolling = true;
                    if (changePlayer) leftPlayer = !leftPlayer;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Point mousePosition)
        {
            int thickness = size / 8;
            for (int x = 0; x < UrBoard.board.GetLength(0); x++)
            {
                for (int y = 0; y < UrBoard.board.GetLength(1); y++)
                {
                    if (UrBoard.board[x, y] > 0)
                    {
                        spriteBatch.FillRectangle((new Vector2(x, y) * size) + position, Vector2.One * size, Color.Brown);
                        spriteBatch.FillRectangle(new Vector2((x * size) + thickness, (y * size) + thickness) + position, Vector2.One * (size - (thickness * 2)), Color.White);
                        switch (UrBoard.board[x, y])
                        {
                            case 1:
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 2, 2);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 2, 6);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 6, 2);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 6, 6);
                                break;
                            case 2:
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 2, 2);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 2, 6);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 6, 2);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 6, 6);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 4, 4);
                                break;
                            case 3:
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 4, 4, 2, 6);
                                break;
                            case 4:
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 1, 1);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 3, 1);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 5, 1);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 7, 1);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 3, 3);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 5, 3);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 3, 5);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 5, 5);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 1, 7);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 3, 7);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 5, 7);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 7, 7);
                                break;
                            case 5:
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 1, 1);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 1, 3);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 3, 1);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 3, 3);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 2, 2);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 5, 1);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 5, 3);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 7, 1);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 7, 3);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 6, 2);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 1, 5);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 1, 7);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 3, 5);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 3, 7);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 2, 6);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 5, 5);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 5, 7);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 7, 5);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 7, 7);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 6, 6);
                                break;
                            case 6:
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 3, 3);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 3, 5);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 5, 3);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 5, 5);
                                drawDecoration(spriteBatch, x, y, size, size, thickness, 4, 4);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            for (int i = 0; i < 7; i++)
            {
                if (board.leftPions[i] == 0)
                    spriteBatch.FillRectangle(position + (new Vector2(-1, 6 - i) * size) + Vector2.One * thickness * 2, Vector2.One * thickness * 4, Color.Red);
                if (board.leftPions[i] > 0 && board.leftPions[i] <= UrBoard.leftRoad.Length)
                    spriteBatch.FillRectangle(UrBoard.leftRoad[board.leftPions[i] - 1].ToVector2() * size + Vector2.One * thickness * 2 + position, Vector2.One * thickness * 4, Color.Red);
                if (board.rightPions[i] == 0)
                    spriteBatch.FillRectangle(position + (new Vector2( UrBoard.board.GetLength(0), 6 - i) * size) + Vector2.One * thickness * 2, Vector2.One * thickness * 4, Color.Blue);
                if (board.rightPions[i] > 0 && board.rightPions[i] <= UrBoard.rightRoad.Length)
                    spriteBatch.FillRectangle(UrBoard.rightRoad[board.rightPions[i] - 1].ToVector2() * size + Vector2.One * thickness * 2 + position, Vector2.One * thickness * 4, Color.Blue);
                if (playableMoves != null && playableMoves.Contains(i))
                {
                    int a = (leftPlayer ? board.leftPions : board.rightPions)[i] - 1;
                    int b = (leftPlayer ? board.leftPions : board.rightPions)[i] + totalRoll - 1;
                    if (a > -1)
                        drawCrosshair(spriteBatch, leftPlayer ? UrBoard.leftRoad[a] : UrBoard.rightRoad[a], thickness, leftPlayer ? Color.Red : Color.Blue);
                    if (b < (leftPlayer ? UrBoard.leftRoad : UrBoard.rightRoad).Length)
                        drawCrosshair(spriteBatch, leftPlayer ? UrBoard.leftRoad[b] : UrBoard.rightRoad[b], thickness, Color.Black);
                }
            }

            if (rolling || playableMoves != null)
            drawRolling(spriteBatch, thickness);
        }

        void drawDecoration(SpriteBatch spriteBatch, int x, int y, int width, int height, int thickness, float xOffset, float yOffset, int size = 1, int sides = 3)
        {
            spriteBatch.DrawCircle(new Vector2(x * width + thickness * xOffset, y * height + thickness * yOffset) + position, thickness * size / 2, sides, Color.DarkBlue);
        }

        void drawCrosshair(SpriteBatch spriteBatch, Point pos, int thickness, Color color)
        {
            spriteBatch.FillRectangle((pos.ToVector2() * size) + position, new Vector2(thickness * 2, thickness), color);
            spriteBatch.FillRectangle((pos.ToVector2() * size) + position + new Vector2(0, thickness), new Vector2(thickness, thickness), color);

            spriteBatch.FillRectangle((pos.ToVector2() * size) + position + new Vector2(thickness * 6, 0), new Vector2(thickness * 2, thickness), color);
            spriteBatch.FillRectangle((pos.ToVector2() * size) + position + new Vector2(thickness * 7, thickness), new Vector2(thickness, thickness), color);

            spriteBatch.FillRectangle((pos.ToVector2() * size) + position + new Vector2(0, thickness * 7), new Vector2(thickness * 2, thickness), color);
            spriteBatch.FillRectangle((pos.ToVector2() * size) + position + new Vector2(0, thickness * 6), new Vector2(thickness, thickness), color);

            spriteBatch.FillRectangle((pos.ToVector2() * size) + position + new Vector2(thickness * 6, thickness * 7), new Vector2(thickness, thickness), color);
            spriteBatch.FillRectangle((pos.ToVector2() * size) + position + new Vector2(thickness * 7, thickness * 6), new Vector2(thickness, thickness * 2), color);
        }

        void drawRolling(SpriteBatch spriteBatch, int thickness)
        {
            Vector2 offset = new Vector2(size * (-2f + (leftPlayer ? 0 : UrBoard.board.GetLength(0) + 3)), 0);
            spriteBatch.FillRectangle(offset + position, size * new Vector2(1, UrBoard.board.GetLength(1)), Color.Black);
            //spriteBatch.FillRectangle((Vector2.UnitX * 2 * -size) + position + (Vector2.One * thickness), size * new Vector2(1, UrBoard.board.GetLength(1)) - (Vector2.One * thickness * 2), Color.Black);

            if (playableMoves != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    offset = new Vector2(size * (-1.5f + (leftPlayer ? 0 : UrBoard.board.GetLength(0) + 3)), 0);
                    spriteBatch.DrawCircle(offset + position + ((i + 0.5f) * (size * (float)UrBoard.board.GetLength(1) / 4) * Vector2.UnitY), thickness * 3, 3, Color.Brown);
                    if (rollResult[i])
                    spriteBatch.DrawCircle(offset + position + ((i + 0.5f) * (size * (float)UrBoard.board.GetLength(1) / 4) * Vector2.UnitY), thickness, 3, Color.White);
                }
            }
        }
    }
}
