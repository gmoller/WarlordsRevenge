using System;
using System.Drawing;
using System.Windows.Forms;

namespace WarlordsRevengeEditor
{
    public class HexGrid
    {
        //                                   z, y, x
        //private int[,,] _cell3D = new int[4, 4, 4] { { { 0,0,0,0 }, { 0,0,3,0 }, { 0,2,0,0 }, { 8,0,0,0 } },
        //                                             { { 0,0,4,0 }, { 0,1,0,0 }, { 7,0,0,0 }, { 0,0,0,0 } },
        //                                             { { 0,5,0,0 }, { 6,0,0,0 }, { 0,0,0,0 }, { 0,0,0,0 } },
        //                                             { { 0,0,0,0 }, { 0,0,0,0 }, { 0,0,0,0 }, { 0,0,0,0 } } };

        private readonly int _size;
        private readonly int _mapWidth;
        private readonly float _halfMapWidth;
        private readonly int _mapHeight;
        private readonly float _halfMapHeight;
        private readonly Cell[,,] _cell;

        public HexGrid(int size, int mapWidth, int mapHeight)
        {
            _size = size;
            _mapWidth = mapWidth;
            _halfMapWidth = mapWidth * Constants.HALF;
            _mapHeight = mapHeight;
            _halfMapHeight = mapHeight * Constants.HALF;
            int arraySize = (size * 2) + 1;
            _cell = new Cell[arraySize, arraySize, arraySize]; // z,y,x
        }

        public Cell GetCell(int x, int y, int z)
        {
            if (x + y + z != 0)
            {
                return Cell.NullCell;
            }

            Cell cell = _cell[z + _size, y + _size, x + _size];
            //Console.WriteLine(@"[x:{0} y:{1} z:{2} is {3}]", x, y, z, cell.ImageId);

            return cell;
        }

        public void SetCell(int x, int y, int z, int value)
        {
            _cell[z + _size, y + _size, x + _size].ImageId = value;
        }

        public Bitmap Render(ImageList imageList)
        {
            var surface = new Bitmap(_mapWidth, _mapHeight);
            Graphics device = Graphics.FromImage(surface);

            RenderHexagonImages(device, imageList);

            RenderHexagonOutlines(device);

            var pen = new Pen(Color.Red, 2);
            device.DrawLine(pen, _halfMapWidth - 1, _halfMapHeight - 1, _halfMapWidth + 1, _halfMapHeight + 1);

            return surface;
        }

        private void RenderHexagonImages(Graphics device, ImageList imageList)
        {
            for (int cellZ = -_size; cellZ <= _size; cellZ++)
            {
                for (int cellY = -_size; cellY <= _size; cellY++)
                {
                    for (int cellX = -_size; cellX <= _size; cellX++)
                    {
                        int imageId = GetCell(cellX, cellY, cellZ).ImageId;

                        if (imageId >= 0)
                        {
                            PointF pos = GetPositionToDrawAt(cellX, cellY, cellZ);
                            pos.X = pos.X + _halfMapWidth - Constants.HALF_HEX_WIDTH;
                            pos.Y = pos.Y + _halfMapHeight - Constants.HALF_HEX_HEIGHT;

                            device.DrawImage(imageList.Images[imageId], pos);
                        }
                    }
                }
            }
        }

        private void RenderHexagonOutlines(Graphics device)
        {
            for (int cellZ = -_size; cellZ <= _size; cellZ++)
            {
                for (int cellY = -_size; cellY <= _size; cellY++)
                {
                    for (int cellX = -_size; cellX <= _size; cellX++)
                    {
                        Cell cell = GetCell(cellX, cellY, cellZ);
                        if (!cell.IsNullCell)
                        {
                            PointF pos = GetPositionToDrawAt(cellX, cellY, cellZ);
                            pos.X = pos.X + _halfMapWidth;
                            pos.Y = pos.Y + _halfMapHeight;

                            var pen = new Pen(Color.Black, 1);
                            device.DrawLines(pen, cell.GetCorners(pos));
                        }
                    }
                }
            }
        }

        private PointF GetPositionToDrawAt(int cellX, int cellY, int cellZ)
        {
            float xOffset;
            float yOffset;
            if (cellX == 0)
            {
                xOffset = 0.0f;
                yOffset = CalculateYOffset(cellY, cellZ);
            }
            else
            {
                xOffset = Constants.THREE_QUARTERS_HEX_WIDTH * cellX;
                yOffset = cellY == cellZ ? 0.0f : CalculateYOffset(cellY, cellZ);
            }

            return new PointF(xOffset, yOffset);
        }

        private float CalculateYOffset(int cellY, int cellZ)
        {
            float f = Math.Abs(cellY - cellZ) * Constants.HALF;
            float yOffset = Constants.HEX_HEIGHT * f;

            if (cellY > cellZ)
            {
                yOffset *= -1;
            }

            return yOffset;
        }
    }
}