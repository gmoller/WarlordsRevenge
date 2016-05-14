using System;

namespace WarlordsRevengeEditor
{
    public class Grid
    {
        private readonly int[,] _cell2D = new int[2, 2] { { 1, 2 }, { 3, 4 } };

        public int Get(int x, int y)
        {
            int i = _cell2D[y, x];
            Console.WriteLine(@"[x:{0} y:{1} is {2}]", x, y, i);

            return i;
        }
    }
}