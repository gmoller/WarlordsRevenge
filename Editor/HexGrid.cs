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
        private readonly Cell[,,] _cells;

        public PointF Center { get { return new PointF(_halfMapWidth, _halfMapHeight); } }

        public HexGrid(int size, int mapWidth, int mapHeight)
        {
            _size = size;
            _mapWidth = mapWidth;
            _halfMapWidth = mapWidth * Constants.HALF;
            _mapHeight = mapHeight;
            _halfMapHeight = mapHeight * Constants.HALF;
            int arraySize = (size * 2) + 1;
            _cells = new Cell[arraySize, arraySize, arraySize]; // z,y,x
        }

        public Cell GetCell(HexCube cube)
        {
            if (cube.X + cube.Y + cube.Z != 0)
            {
                return Cell.NullCell;
            }

            Cell cell = _cells[(int)cube.Z + _size, (int)cube.Y + _size, (int)cube.X + _size];
            //Console.WriteLine(@"[x:{0} y:{1} z:{2} is {3}]", x, y, z, cell.ImageId);

            return cell;
        }

        public void SetCell(HexCube cube, int value)
        {
            try
            {
                _cells[(int) cube.Z + _size, (int) cube.Y + _size, (int) cube.X + _size].ImageId = value;
            }
            catch (IndexOutOfRangeException ex)
            {
                // do nothing
            }
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
                        var cube = new HexCube(cellX, cellY, cellZ);
                        int imageId = GetCell(cube).ImageId;

                        if (imageId >= 1)
                        {
                            PointF centerOfHex = HexToPixel(cube); //PointF pos = GetPositionToDrawAt(cube);
                            var pos = new PointF
                                {
                                    X = (centerOfHex.X + _halfMapWidth) - Constants.HALF_HEX_WIDTH,
                                    Y = (centerOfHex.Y + _halfMapHeight) - Constants.HALF_HEX_HEIGHT
                                };

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
                        var cube = new HexCube(cellX, cellY, cellZ);
                        Cell cell = GetCell(cube);
                        if (!cell.IsNullCell)
                        {
                            PointF centerOfHex = HexToPixel(cube); //PointF pos = GetPositionToDrawAt(cube);
                            var pos = new PointF
                            {
                                X = (centerOfHex.X + _halfMapWidth),
                                Y = (centerOfHex.Y + _halfMapHeight)
                            };

                            var pen = new Pen(Color.Black, 1);
                            PointF[] corners = cell.GetCorners(pos);
                            device.DrawLines(pen, corners);
                        }
                    }
                }
            }
        }

        private PointF GetPositionToDrawAt(HexCube cube)
        {
            float xOffset;
            float yOffset;
            if (cube.X == 0)
            {
                xOffset = 0.0f;
                yOffset = CalculateYOffset(cube);
            }
            else
            {
                xOffset = Constants.THREE_QUARTERS_HEX_WIDTH * cube.X;
                yOffset = cube.Y == cube.Z ? 0.0f : CalculateYOffset(cube);
            }

            return new PointF(xOffset, yOffset);
        }

        private float CalculateYOffset(HexCube cube)
        {
            float f = Math.Abs(cube.Y - cube.Z) * Constants.HALF;
            float yOffset = Constants.HEX_HEIGHT * f;

            if (cube.Y > cube.Z)
            {
                yOffset *= -1;
            }

            return yOffset;
        }

        private HexAxial ConvertCubeToAxial(HexCube cube)
        {
            float q = cube.X;
            float r = cube.Z;
            var axial = new HexAxial(q, r);

            return axial;
        }

        private HexCube ConvertAxialToCube(HexAxial axial)
        {
            float x = axial.Q;
            float z = axial.R;
            float y = -x - z;
            var hexCube = new HexCube(x, y, z);

            return hexCube;
        }

        /// <summary>
        /// Returns the center pixel of a hexagon
        /// </summary>
        /// <param name="cube"></param>
        private PointF HexToPixel(HexCube cube)
        {
            HexAxial axial = ConvertCubeToAxial(cube);
            float x = Constants.HALF_HEX_WIDTH * 1.5f * axial.Q;
            //double y = Constants.HALF_HEX_HEIGHT * Math.Sqrt(3) * (axial.R + axial.Q / 2);
            float y = Constants.HALF_HEX_HEIGHT * 2.0f * (axial.R + axial.Q / 2.0f);

            return new PointF(x, y);
        }

        /// <summary>
        /// Returns the center pixel of a hexagon
        /// </summary>
        /// <param name="axial"></param>
        private PointF HexToPixel(HexAxial axial)
        {
            float x = Constants.HALF_HEX_WIDTH * 1.5f * axial.Q;
            //double y = Constants.HALF_HEX_HEIGHT * Math.Sqrt(3) * (axial.R + axial.Q / 2);
            float y = Constants.HALF_HEX_HEIGHT * 2.0f * (axial.R + axial.Q / 2.0f);

            return new PointF(x, y);
        }

        public HexCube PixelToHex(PointF pixel)
        {
            float q = pixel.X * Constants.TWO_THIRDS / Constants.HALF_HEX_WIDTH;
            //double r = (-pixel.X / 3.0f + (Math.Sqrt(3)/3.0f) * pixel.Y) / Constants.HALF_HEX_HEIGHT;
            double r = (-pixel.X / 3.0f + Constants.HALF * pixel.Y) / Constants.HALF_HEX_HEIGHT;

            var axial = new HexAxial(q, (float)r);

            axial = HexRound(axial);
            HexCube cube = ConvertAxialToCube(axial);

            return cube;
        }

        private HexAxial HexRound(HexAxial axial)
        {
            HexCube cube = ConvertAxialToCube(axial);
            cube = CubeRound(cube);

            return ConvertCubeToAxial(cube);
        }

        private HexCube CubeRound(HexCube cube)
        {
            double rx = Math.Round(cube.X);
            double ry = Math.Round(cube.Y);
            double rz = Math.Round(cube.Z);

            double xDiff = Math.Abs(rx - cube.X);
            double yDiff = Math.Abs(ry - cube.Y);
            double zDiff = Math.Abs(rz - cube.Z);

            if (xDiff > yDiff && xDiff > zDiff)
            {
                rx = -ry - rz;
            }
            else if (yDiff > zDiff)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }

            return new HexCube((float)rx, (float)ry, (float)rz);
        }
    }
}