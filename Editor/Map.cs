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
        private string _name;
        private int _size;
        private HexGrid _grid;

        public PointF Center { get { return _grid.Center; } }

        private Map(string name, string path, int size)
        {
            _name = name;
            _path = path;

            _size = size;
            float mapWidth = (_size * 2 + 2) * Constants.HEX_WIDTH * Constants.THREE_QUARTERS;
            float mapHeight = (_size * 2 + 2) * Constants.HEX_HEIGHT;
            _grid = new HexGrid(_size, (int)mapWidth, (int)mapHeight);
        }

        public static Map NewMap(string name, string path, int size)
        {
            var map = new Map(name, path, size);

            return map;
        }

        public void SetCell(Point mousePosition, int imageId)
        {
            HexAxial axial = DetermineHexAtMousePointer(mousePosition);
            _grid.SetCell(axial, imageId);
        }

        public HexAxial DetermineHexAtMousePointer(Point mousePosition)
        {
            var mousePositionRelativeToCenter = new PointF { X = mousePosition.X - Center.X, Y = mousePosition.Y - Center.Y };
            HexAxial axial = mousePositionRelativeToCenter.PixelToHex();

            return axial;
        }

        public Bitmap Render(ImageList imageList)
        {
            return _grid.Render(imageList);
        }

        public void Load(string path)
        {
            _path = Path.GetDirectoryName(path);
            ReadFromFile(path);
        }

        public void Save()
        {
            string path = Path.Combine(_path, _name + ".map");
            WriteToFile(path);
        }

        private void ReadFromFile(string path)
        {
            using (var readtext = new StreamReader(path))
            {
                string line;
                do
                {
                    line = readtext.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line.StartsWith("Name: "))
                        {
                            _name = line.Remove(0, 6);
                        }
                        else if (line.StartsWith("Size: "))
                        {
                            _size = Convert.ToInt32(line.Remove(0, 6));
                            float mapWidth = (_size*2 + 2)*Constants.HEX_WIDTH*Constants.THREE_QUARTERS;
                            float mapHeight = (_size*2 + 2)*Constants.HEX_HEIGHT;
                            _grid = new HexGrid(_size, (int) mapWidth, (int) mapHeight);
                        }
                        else
                        {
                            string[] pieces = line.Split(':');
                            string[] coords = pieces[0].Split(';');
                            float q = Convert.ToSingle(coords[0]);
                            float r = Convert.ToSingle(coords[1]);
                            int imageId = Convert.ToInt32(pieces[1]);
                            var axial = new HexAxial(q, r);
                            _grid.SetCell(axial, imageId);
                            // TODO: fix inverted bug!!!
                        }
                    }
                } while (line != null);
            }
        }

        private void WriteToFile(string path)
        {
            using (var writetext = new StreamWriter(path))
            {
                writetext.WriteLine("Name: {0}", _name);
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