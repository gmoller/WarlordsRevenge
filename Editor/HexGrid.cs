using System;

namespace WarlordsRevengeEditor
{
    public class HexGrid
    {
        //private const int OFFSET = 3; // can hold info up to 3 away from center  // 1=3,2=5,3=7, so 4 should be 9
        private const int OFFSET = 50; // can hold info up to 3 away from center  // 1=3,2=5,3=7, so 4 should be 9

        private int[,] _cell2D = new int[2,2] { {1,2}, {3,4} };

        //                                   z, y, x
        //private int[, ,] _cell3D = new int[4, 4, 4] { { { 0,0,0,0 }, { 0,0,3,0 }, { 0,2,0,0 }, { 8,0,0,0 } },
        //                                              { { 0,0,4,0 }, { 0,1,0,0 }, { 7,0,0,0 }, { 0,0,0,0 } },
        //                                              { { 0,5,0,0 }, { 6,0,0,0 }, { 0,0,0,0 }, { 0,0,0,0 } },
        //                                              { { 0,0,0,0 }, { 0,0,0,0 }, { 0,0,0,0 }, { 0,0,0,0 } } };

        //private readonly int[,,] _cell = new int[7,7,7]; // z,y,x
        private readonly int[, ,] _cell = new int[101, 101, 101]; // z,y,x

        public int Get(int x, int y)
        {
            int i = _cell2D[y, x];
            Console.WriteLine(@"[x:{0} y:{1} is {2}]", x, y, i);

            return i;
        }

        public int Get(int x, int y, int z)
        {
            int i = _cell[z + OFFSET, y + OFFSET, x + OFFSET];
            Console.WriteLine(@"[x:{0} y:{1} z:{2} is {3}]", x, y, z, i);

            return i;
        }

        public void Set(int x, int y, int z, int value)
        {
            _cell[z + OFFSET, y + OFFSET, x + OFFSET] = value;
        }
    }
}