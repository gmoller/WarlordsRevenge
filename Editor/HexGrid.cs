using System.Collections.Generic;
using System.Drawing;
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
                    if (!cell.IsEmpty())
                    {
                        HexAxial axial = cube.ToAxial();
                        cells.Add(string.Format(@"{0};{1}:{2}", axial.Q, axial.R, cell));
                    }
                };
            IterateCells(handler);

            return cells;
        }

        public Cell GetCell(HexCube cube)
        {
            Cell cell = _cells[(int)cube.Z + _size, (int)cube.Y + _size, (int)cube.X + _size];

            return cell;
        }

        private Cell GetCell(HexAxial axial)
        {
            HexCube cube = axial.ToCube();
            Cell cell = GetCell(cube);
            if (cell == null)
            {
                cell = new Cell();
                _cells[(int)cube.Z + _size, (int)cube.Y + _size, (int)cube.X + _size] = cell;
            }

            return cell;
        }

        public void SetCell(HexAxial axial, int layerId, int paletteId, int terrainId)
        {
            Cell cell = GetCell(axial);
            cell.AddCellData(layerId, paletteId, terrainId);
        }

        public void RemoveImageFromCell(HexAxial axial, int layerId)
        {
            Cell cell = GetCell(axial);
            cell.RemoveTerrain(layerId);
        }

        public Bitmap Render(ImageList[] images)
        {
            var surface = new Bitmap(_mapWidth, _mapHeight);
            Graphics device = Graphics.FromImage(surface);

            RenderHexagonImages(device, images);
            RenderHexagonOutlines(device);

            var pen = new Pen(Color.Red, 2);
            device.DrawLine(pen, _halfMapWidth - 1, _halfMapHeight - 1, _halfMapWidth + 1, _halfMapHeight + 1);

            return surface;
        }

        private void RenderHexagonImages(Graphics device, ImageList[] images)
        {
            CellHandler handler = delegate(HexCube cube, Cell cell)
                {
                    for (int i = 1; i <= Layers.NumberOfLayers; i++)
                    {
                        CellData data = cell.GetCellData(i);
                        if (data.TerrainId >= 0)
                        {
                            PointF centerOfHex = cube.HexToPixel();
                            var pos = new PointF
                            {
                                X = (centerOfHex.X + _halfMapWidth) - Constants.HALF_HEX_WIDTH,
                                Y = (centerOfHex.Y + _halfMapHeight) - Constants.HALF_HEX_HEIGHT
                            };

                            int paletteId = data.PaletteId;
                            device.DrawImage(images[paletteId].Images[data.TerrainId], pos);
                        }
                    }
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