using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using C3.XNA;
namespace UrBoardGame
{
    class UrBoard
    {
        public static int maxPionPerPlayer = 7;

        public static byte[,] board = {
            { 3, 1, 2, 1, 0, 0, 3, 6 },
            { 4, 2, 5, 3, 2, 5, 1, 2 },
            { 3, 1, 2, 1, 0, 0, 3, 6 }
        };
        public static Point[] leftRoad =
        {
            new Point(0, 3),
            new Point(0, 2),
            new Point(0, 1),
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1),
            new Point(1, 2),
            new Point(1, 3),
            new Point(1, 4),
            new Point(1, 5),
            new Point(1, 6),
            new Point(1, 7),
            new Point(0, 7),
            new Point(0, 6)
        };
        public static Point[] rightRoad =
        {
            new Point(2, 3),
            new Point(2, 2),
            new Point(2, 1),
            new Point(2, 0),
            new Point(1, 0),
            new Point(1, 1),
            new Point(1, 2),
            new Point(1, 3),
            new Point(1, 4),
            new Point(1, 5),
            new Point(1, 6),
            new Point(1, 7),
            new Point(2, 7),
            new Point(2, 6)
        };
        public static int roadFromTilePos(Point pos, bool left)
        {
            Point[] roadToCheck = left ? leftRoad : rightRoad;
            for (int i = 0; i < roadToCheck.Length; i++)
            {
                if (roadToCheck[i].Equals(pos)) return i;
            }
            return -1;
        }

        public byte[] leftPions { get; private set; }
        public byte[] rightPions { get; private set; }

        Random randomRolls;

        int[,] pionsBoardPositions (byte[] leftPionsToCheck = null, byte[] rightPionsToCheck = null)
        {
            if (leftPionsToCheck == null) leftPionsToCheck = leftPions;
            if (rightPionsToCheck == null) rightPionsToCheck = rightPions;
            int[,] pionsLoc = new int[3, 7];
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 7; y++)
                    pionsLoc[x, y] = 0;
            Point pionPos;
            for (int i = 0; i < 7; i++)
            {
                if (leftPionsToCheck[i] > 0 && leftPionsToCheck[i] <= 14)
                {
                    pionPos = leftRoad[leftPionsToCheck[i]];
                    pionsLoc[pionPos.X, pionPos.Y] = -i;
                }
                if (rightPionsToCheck[i] > 0 && rightPionsToCheck[i] <= 14)
                {
                    pionPos = rightRoad[rightPionsToCheck[i]];
                    pionsLoc[pionPos.X, pionPos.Y] = i;
                }
            }
            return pionsLoc;
        }

        public bool[] roll()
        {
            bool[] o = new bool[4];

            for (int i = 0; i < o.Length; i++)
                o[i] = randomRolls.Next(2) == 1;

            return o;
        }

        public int getPionByBoardPos(Point pos, bool checkLeft, byte[] leftPionsToCheck = null, byte[] rightPionsToCheck = null)
        {
            if (leftPionsToCheck == null) leftPionsToCheck = leftPions;
            if (rightPionsToCheck == null) rightPionsToCheck = rightPions;
            for (int i = 0; i < leftPionsToCheck.Length; i++)
            {
                if (checkLeft)
                {
                    if (leftPionsToCheck[i] > 0 && leftPionsToCheck[i] <= leftRoad.Length)
                    {
                        if (leftRoad[leftPionsToCheck[i] - 1].X == pos.X
                            && leftRoad[leftPionsToCheck[i] - 1].Y == pos.Y) return i;
                    }
                }
                else
                {
                    if (rightPionsToCheck[i] > 0 && rightPionsToCheck[i] <= rightRoad.Length)
                    {
                        if (rightRoad[rightPionsToCheck[i] - 1].X == pos.X
                            && rightRoad[rightPionsToCheck[i] - 1].Y == pos.Y) return i;
                    }
                }
            }
            return -1;
        }

        public List<int>[] getAllPossibleMoves(bool checkLeft, byte[] leftPionsToCheck = null, byte[] rightPionsToCheck = null, int lowerBound = 0, int upperBound = 4)
        {
            if (leftPionsToCheck == null) leftPionsToCheck = leftPions;
            if (rightPionsToCheck == null) rightPionsToCheck = rightPions;
            List<int>[] o = new List<int>[(upperBound + 1 - lowerBound)];
            int i;
            for (i = lowerBound; i <= upperBound; i++)
            {
                o[i] = getPossibleMoves(i, checkLeft, leftPionsToCheck, rightPionsToCheck);
            }
            return o;
        }

        public List<int> getPossibleMoves(int move, bool checkLeft, byte[] leftPionsToCheck = null, byte[] rightPionsToCheck = null)
        {
            List<int> possibleToMove = new List<int>();
            if (move <= 0) return possibleToMove;
            if (leftPionsToCheck == null) leftPionsToCheck = leftPions;
            if (rightPionsToCheck == null) rightPionsToCheck = rightPions;
            byte[] pionsToCheck = checkLeft ? leftPionsToCheck : rightPionsToCheck;
            Point[] roadToCheck = checkLeft ? leftRoad : rightRoad;

            Point NextPionPos; int NextPion;
            int earliestUnmovedPions = -1;
            for (int i = 0; i < pionsToCheck.Length; i++)
            {
                if (earliestUnmovedPions == -1 && pionsToCheck[i] == 0) earliestUnmovedPions = i;
                else if (pionsToCheck[i] <= (checkLeft ? leftRoad: rightRoad).Length)
                {
                    NextPion = pionsToCheck[i] + move - 1;
                    if (NextPion < roadToCheck.Length) {
                        NextPionPos = roadToCheck[NextPion];
                        if (getPionByBoardPos(NextPionPos, checkLeft, leftPionsToCheck, rightPionsToCheck) == -1)
                        {
                            if (board[NextPionPos.X, NextPionPos.Y] != 3 || getPionByBoardPos(NextPionPos, !checkLeft, leftPionsToCheck, rightPionsToCheck) == -1)
                                possibleToMove.Add(i);
                        }
                    }
                    else if (NextPion == roadToCheck.Length)
                    {
                        possibleToMove.Add(i);
                    }
                }
            }
            if (earliestUnmovedPions > -1 && getPionByBoardPos(roadToCheck[move - 1], checkLeft, leftPionsToCheck, rightPionsToCheck) == -1)
                possibleToMove.Add(earliestUnmovedPions);
            return possibleToMove;
        }

        public bool move(int pionNumber, int move, bool leftPlayer)
        {
            int opposite;
            bool rosette = false;
            if (leftPlayer)
            {
                leftPions[pionNumber] += (byte)move;
                if (leftPions[pionNumber] <= rightRoad.Length)
                {
                    opposite = getPionByBoardPos(leftRoad[leftPions[pionNumber] - 1], false);
                    if (opposite > -1)
                    {
                        rightPions[opposite] = 0;
                    }
                    rosette = board[leftRoad[leftPions[pionNumber] - 1].X, leftRoad[leftPions[pionNumber] - 1].Y] == 3;
                }
            }
            else
            {
                rightPions[pionNumber] += (byte)move;
                if (rightPions[pionNumber] <= rightRoad.Length)
                {
                    opposite = getPionByBoardPos(rightRoad[rightPions[pionNumber] - 1], true);
                    if (opposite > -1)
                    {
                        leftPions[opposite] = 0;
                    }
                    rosette = board[rightRoad[rightPions[pionNumber] - 1].X, rightRoad[rightPions[pionNumber] - 1].Y] == 3;
                }
            }
            return !rosette;
        }

        public UrBoard(string rollSeed = "Hilda Catherine Zephyr")
        {
            leftPions = new byte[7] { 0, 0, 0, 0, 0, 0, 0};
            rightPions = new byte[7] { 0, 0, 0, 0, 0, 0, 0 };

            if (rollSeed.Equals("Hilda Catherine Zephyr"))
                randomRolls = new Random(DateTime.Now.GetHashCode());
            else
                randomRolls = new Random(rollSeed.GetHashCode());
        }

    }
}
