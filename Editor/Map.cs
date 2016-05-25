using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WarlordsRevengeEditor
{
    public class Map
    {
        private string _path;
        private int _size;
        private HexGrid _grid;

        public PointF Center { get { return _grid.Center; } }

        public string Name { get; private set; }

        public bool IsDirty { get; private set; }

        private Map()
        {
            IsDirty = false;
        }

        private Map(string name, string path, int size)
        {
            Name = name;
            _path = path;

            _size = size;
            float mapWidth = (_size * 2 + 2) * Constants.HEX_WIDTH * Constants.THREE_QUARTERS;
            float mapHeight = (_size * 2 + 2) * Constants.HEX_HEIGHT;
            _grid = new HexGrid(_size, (int)mapWidth, (int)mapHeight);
            IsDirty = true;
        }

        public static Map NewMap(string name, string path, int size)
        {
            var map = new Map(name, path, size);

            return map;
        }

        public static Map LoadMap(string path)
        {
            var map = new Map();
            map.Load(path);

            return map;
        }

        public void SetCell(Point mousePosition, int layerId, int paletteId, int terrainId)
        {
            HexAxial axial = DetermineHexAtMousePointer(mousePosition);
            _grid.SetCell(axial, layerId, paletteId, terrainId);
            IsDirty = true;
        }

        public void RemoveImageFromCell(Point mousePosition, int layerId)
        {
            HexAxial axial = DetermineHexAtMousePointer(mousePosition);
            _grid.RemoveImageFromCell(axial, layerId);
            IsDirty = true;
        }

        public HexAxial DetermineHexAtMousePointer(Point mousePosition)
        {
            var mousePositionRelativeToCenter = new PointF { X = mousePosition.X - Center.X, Y = mousePosition.Y - Center.Y };
            HexAxial axial = mousePositionRelativeToCenter.PixelToHex();

            return axial;
        }

        public Bitmap Render(ImageList[] images)
        {
            return _grid.Render(images);
        }

        private void Load(string path)
        {
            _path = Path.GetDirectoryName(path);
            Name = Path.GetFileNameWithoutExtension(path);
            ReadFromFile(path);
        }

        public void Save()
        {
            string path = Path.Combine(_path, Name + ".map");
            WriteToFile(path);
            IsDirty = false;
        }

        private void ReadFromFile(string path)
        {
            string[] lines = FileReader.ReadFile(path);

            foreach (string line in lines)
            {
                if (line.StartsWith("Size: "))
                {
                    _size = Convert.ToInt32(line.Remove(0, 6));
                    float mapWidth = (_size * 2 + 2) * Constants.HEX_WIDTH * Constants.THREE_QUARTERS;
                    float mapHeight = (_size * 2 + 2) * Constants.HEX_HEIGHT;
                    _grid = new HexGrid(_size, (int)mapWidth, (int)mapHeight);
                }
                else
                {
                    string[] pieces = line.Split(':');

                    //string[] coords = pieces[0].Split(';');
                    //float q = Convert.ToSingle(coords[0]);
                    //float r = Convert.ToSingle(coords[1]);
                    //
                    //var axial = new HexAxial(q, r);

                    var axial = GetHexagon(pieces[0]);

                    //string[] subPieces = pieces[1].Split('|');
                    //int count = 1;
                    //foreach (string subPiece in subPieces)
                    //{
                    //    int imageId = Convert.ToInt32(subPiece);
                    //    if (count <= Layers.NumberOfLayers)
                    //    {
                    //        _grid.SetCell(axial, count, imageId);
                    //    }
                    //    count++;
                    //}

                    CellData[] cellData = GetCellData(pieces[1]);
                    for (int i = 0; i < cellData.Length; i++)
                    {
                        _grid.SetCell(axial, i + 1, cellData[i].PaletteId, cellData[i].TerrainId);
                    }
                }
            }
        }

        private HexAxial GetHexagon(string coordsString)
        {
            string[] coords = coordsString.Split(';');
            float q = Convert.ToSingle(coords[0]);
            float r = Convert.ToSingle(coords[1]);

            var axial = new HexAxial(q, r);

            return axial;
        }

        private CellData[] GetCellData(string cellDataString)
        {
            var cellData = new List<CellData>();

            string[] pieces = cellDataString.Split('|');
            int count = 1;
            foreach (string piece in pieces)
            {
                string[] subPieces = piece.Split(';');
                int paletteId = Convert.ToInt32(subPieces[0]);
                int terrainId = Convert.ToInt32(subPieces[1]);
                if (count <= Layers.NumberOfLayers)
                {
                    cellData.Add(new CellData { PaletteId = paletteId, TerrainId = terrainId });
                }
                count++;
            }

            return cellData.ToArray();
        }

        private void WriteToFile(string path)
        {
            using (var writetext = new StreamWriter(path))
            {
                writetext.WriteLine("Size: {0}", _size);
                List<string> allCells = _grid.GetAllCells();
                foreach (string cell in allCells)
                {
                    writetext.WriteLine(cell);
                }
            }
        }
    }
}