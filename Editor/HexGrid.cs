using System;
using System.Drawing;
using System.Windows.Forms;

namespace WarlordsRevengeEditor
{
    public class HexGrid
    {
        private const float HALF = 0.5f;
        private const float THREE_QUARTERS = 0.75f;

        private const float HEX_WIDTH = 72.0f;
        private const float THREE_QUARTERS_HEX_WIDTH = HEX_WIDTH * THREE_QUARTERS;
        private const float HALF_HEX_WIDTH = HEX_WIDTH * HALF;

        private const float HEX_HEIGHT = 72.0f;
        private const float HALF_HEX_HEIGHT = HEX_HEIGHT * HALF;

        //                                   z, y, x
        //private int[,,] _cell3D = new int[4, 4, 4] { { { 0,0,0,0 }, { 0,0,3,0 }, { 0,2,0,0 }, { 8,0,0,0 } },
        //                                             { { 0,0,4,0 }, { 0,1,0,0 }, { 7,0,0,0 }, { 0,0,0,0 } },
        //                                             { { 0,5,0,0 }, { 6,0,0,0 }, { 0,0,0,0 }, { 0,0,0,0 } },
        //                                             { { 0,0,0,0 }, { 0,0,0,0 }, { 0,0,0,0 }, { 0,0,0,0 } } };

        private readonly int _size;
        private readonly int[,,] _cell;

        public HexGrid(int size)
        {
            _size = size;
            int arraySize = (size * 2) + 1;
            _cell = new int[arraySize, arraySize, arraySize]; // z,y,x
        }

        public int GetCell(int x, int y, int z)
        {
            int i = _cell[z + _size, y + _size, x + _size];
            Console.WriteLine(@"[x:{0} y:{1} z:{2} is {3}]", x, y, z, i);

            return i;
        }

        public void SetCell(int x, int y, int z, int value)
        {
            _cell[z + _size, y + _size, x + _size] = value;
        }

        public Bitmap Render(ImageList imageList)
        {
            var surface = new Bitmap(400, 400);
            Graphics device = Graphics.FromImage(surface);

            RenderHexagonImages(device, imageList, surface.Width, surface.Height);

            var pen = new Pen(Color.Red, 2);
            device.DrawLine(pen, surface.Width * HALF - 1, surface.Height * HALF- 1, surface.Width * HALF + 1, surface.Height * HALF + 1);

            return surface;
        }


        private void RenderHexagonImages(Graphics device, ImageList imageList, int mapWidth, int mapHeight)
        {
            float centerX = (mapWidth * HALF) - HALF_HEX_WIDTH; // repeated calc
            float centerY = (mapHeight * HALF) - HALF_HEX_HEIGHT; // repeated calc

            for (int cellZ = -_size; cellZ <= _size; cellZ++)
            {
                for (int cellY = -_size; cellY <= _size; cellY++)
                {
                    for (int cellX = -_size; cellX <= _size; cellX++)
                    {
                        int imageId = GetCell(cellX, cellY, cellZ);

                        if (imageId > 0)
                        {
                            float xOffset;
                            float yOffset;
                            if (cellX == 0)
                            {
                                xOffset = 0.0f;

                                yOffset = HALF_HEX_HEIGHT * (Math.Abs(cellY) + Math.Abs(cellZ));
                                yOffset *= cellY >= 0 ? -1 : 1;
                            }
                            else
                            {
                                xOffset = THREE_QUARTERS_HEX_WIDTH * cellX;
                                if (cellY == cellZ)
                                {
                                    yOffset = 0.0f;
                                }
                                else
                                {
                                    yOffset = HALF_HEX_HEIGHT * (Math.Abs(cellY) + Math.Abs(cellZ));

                                    if (cellY == 0)
                                    {
                                        yOffset *= cellZ < 0 ? -1 : 1;
                                    }
                                    else
                                    {
                                        yOffset *= cellY >= 0 ? -1 : 1;
                                    }
                                }
                            }

                            float x = centerX + xOffset;
                            float y = centerY + yOffset;

                            device.DrawImage(imageList.Images[imageId], x, y);
                        }
                    }
                }
            }
        }
    }
}