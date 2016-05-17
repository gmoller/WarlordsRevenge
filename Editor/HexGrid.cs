using System;
using System.Collections.Generic;
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

        private delegate void CellHandler(HexCube cube, Cell cell);

        public List<string> GetAllCells()
        {
            var cells = new List<string>();

            CellHandler handler = (cube, cell) => cells.Add(string.Format(@"{0};{1}:{2}", cube.X, cube.Y, cell.ToString()));
            IterateCells(handler);

            return cells;
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

        public void SetCell(HexAxial axial, int value)
        {
            HexCube cube = axial.ToCube();
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
            CellHandler handler = delegate(HexCube cube, Cell cell)
                {
                    int imageId = cell.ImageId;
                    if (imageId >= 1)
                    {
                        PointF centerOfHex = cube.HexToPixel(); //PointF pos = GetPositionToDrawAt(cube);
                        var pos = new PointF
                            {
                                X = (centerOfHex.X + _halfMapWidth) - Constants.HALF_HEX_WIDTH,
                                Y = (centerOfHex.Y + _halfMapHeight) - Constants.HALF_HEX_HEIGHT
                            };
                        device.DrawImage(imageList.Images[imageId], pos);
                    }
                };
            IterateCells(handler);
        }

        private void RenderHexagonOutlines(Graphics device)
        {
            CellHandler handler = delegate(HexCube cube, Cell cell)
                {
                    PointF centerOfHex = cube.HexToPixel(); //PointF pos = GetPositionToDrawAt(cube);
                    var pos = new PointF
                    {
                        X = (centerOfHex.X + _halfMapWidth),
                        Y = (centerOfHex.Y + _halfMapHeight)
                    };

                    var pen = new Pen(Color.Black, 1);
                    PointF[] corners = Hexagon.GetCorners(pos);
                    device.DrawLines(pen, corners);
                };
            IterateCells(handler);
        }

        private void IterateCells(CellHandler handler)
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
                            handler(cube, cell);
                        }
                    }
                }
            }
        }

        //private PointF GetPositionToDrawAt(HexCube cube)
        //{
        //    float xOffset;
        //    float yOffset;
        //    if (cube.X == 0)
        //    {
        //        xOffset = 0.0f;
        //        yOffset = CalculateYOffset(cube);
        //    }
        //    else
        //    {
        //        xOffset = Constants.THREE_QUARTERS_HEX_WIDTH * cube.X;
        //        yOffset = cube.Y == cube.Z ? 0.0f : CalculateYOffset(cube);
        //    }

        //    return new PointF(xOffset, yOffset);
        //}

        //private float CalculateYOffset(HexCube cube)
        //{
        //    float f = Math.Abs(cube.Y - cube.Z) * Constants.HALF;
        //    float yOffset = Constants.HEX_HEIGHT * f;

        //    if (cube.Y > cube.Z)
        //    {
        //        yOffset *= -1;
        //    }

        //    return yOffset;
        //}
    }
}