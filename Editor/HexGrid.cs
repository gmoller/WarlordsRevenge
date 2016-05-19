using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace WarlordsRevengeEditor
{
    public class HexGrid
    {
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

            for (int cellZ = 0; cellZ < arraySize; cellZ++)
            {
                for (int cellY = 0; cellY < arraySize; cellY++)
                {
                    for (int cellX = 0; cellX < arraySize; cellX++)
                    {
                        if (cellX - size + cellY - size + cellZ - size == 0)
                        {
                            _cells[cellZ, cellY, cellX] = new Cell();
                        }
                    }
                }
            }
        }

        private delegate void CellHandler(HexCube cube, Cell cell);

        public List<string> GetAllCells()
        {
            var cells = new List<string>();

            CellHandler handler = delegate(HexCube cube, Cell cell)
                {
                    HexAxial axial = cube.ToAxial();
                    cells.Add(string.Format(@"{0};{1}:{2}", axial.Q, axial.R, cell));
                };
            IterateCells(handler);

            return cells;
        }

        public Cell GetCell(HexCube cube)
        {
            Cell cell = _cells[(int)cube.Z + _size, (int)cube.Y + _size, (int)cube.X + _size];

            return cell;
        }

        public void SetCell(HexAxial axial, int value)
        {
            HexCube cube = axial.ToCube();
            //try
            //{
                Cell cell = _cells[(int) cube.Z + _size, (int) cube.Y + _size, (int) cube.X + _size];
                if (cell == null)
                {
                    cell = new Cell();
                    _cells[(int) cube.Z + _size, (int) cube.Y + _size, (int) cube.X + _size] = cell;
                }
                cell.AddTerrainId(value);
            //}
            //catch (IndexOutOfRangeException ex)
            //{
            //    // do nothing
            //}
        }

        public void RemoveImageFromCell(HexAxial axial)
        {
            HexCube cube = axial.ToCube();
            Cell cell = _cells[(int)cube.Z + _size, (int)cube.Y + _size, (int)cube.X + _size];
            if (cell == null)
            {
                cell = new Cell();
                _cells[(int)cube.Z + _size, (int)cube.Y + _size, (int)cube.X + _size] = cell;
            }
            cell.RemoveTerrain();
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
                    int imageId;
                    int i = 0;
                    do
                    {
                        imageId = cell.GetTerrainId(i++); // TODO: remove this implicit link between terrainid and imagelist somehow
                        if (imageId >= 0)
                        {
                            PointF centerOfHex = cube.HexToPixel();
                            var pos = new PointF
                            {
                                X = (centerOfHex.X + _halfMapWidth) - Constants.HALF_HEX_WIDTH,
                                Y = (centerOfHex.Y + _halfMapHeight) - Constants.HALF_HEX_HEIGHT
                            };

                            device.DrawImage(imageList.Images[imageId], pos);

                            //if (i == 1)
                            //{
                            //    device.DrawImage(imageList.Images[imageId], pos);
                            //}
                            //else
                            //{

                            //    float[][] ptsArray =
                            //        {
                            //            new float[] {1, 0, 0, 0, 0},
                            //            new float[] {0, 1, 0, 0, 0},
                            //            new float[] {0, 0, 1, 0, 0},
                            //            new float[] {0, 0, 0, 0.5f, 0},
                            //            new float[] {0, 0, 0, 0, 1}
                            //        };
                            //    var clrMatrix = new ColorMatrix(ptsArray);
                            //    var imgAttributes = new ImageAttributes();
                            //    imgAttributes.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                            //    device.DrawImage(imageList.Images[imageId],
                            //                     new Rectangle((int) pos.X, (int) pos.Y,
                            //                                   imageList.Images[imageId].Height,
                            //                                   imageList.Images[imageId].Width),
                            //                     0.0f, 0.0f, imageList.Images[imageId].Width,
                            //                     imageList.Images[imageId].Height,
                            //                     GraphicsUnit.Pixel, imgAttributes);
                            //}
                        }
                    } while (imageId > 0);
                };
            IterateCells(handler);
        }

        private void RenderHexagonOutlines(Graphics device)
        {
            CellHandler handler = delegate(HexCube cube, Cell cell)
                {
                    PointF centerOfHex = cube.HexToPixel();
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
                        if (cell != null)
                        {
                            handler(cube, cell);
                        }
                    }
                }
            }
        }
    }
}