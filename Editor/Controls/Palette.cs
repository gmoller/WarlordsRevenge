using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WarlordsRevengeEditor.Controls
{
    public class Palette
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private readonly List<TerrainList> _allTerrainList = new List<TerrainList>();

        public void LoadAllTerrain()
        {
            IEnumerable<string> paths = GetPalettePaths();

            foreach (string path in paths)
            {
                TerrainList terrainList = LoadImagesIntoImageList(path);
                terrainList.ImageList = AddImagesToImageList(path, terrainList);
                terrainList.ListView = AddImagesToListview(terrainList, terrainList.ImageList);
                _allTerrainList.Add(terrainList);
            }
        }

        public int NumberOfPalettes { get { return _allTerrainList.Count; } }

        public ImageList[] GetAllImageLists()
        {
            var imageLists = new ImageList[NumberOfPalettes];
            for (int i = 0; i < NumberOfPalettes; i++)
            {
                imageLists[i] = _allTerrainList[i].ImageList;
            }

            return imageLists;
        }

        public ImageList GetImageList(int index)
        {
            return _allTerrainList[index].ImageList;
        }

        public ListView GetListView(int index)
        {
            return _allTerrainList[index].ListView;
        }

        private IEnumerable<string> GetPalettePaths()
        {
            string paths = Properties.Settings.Default.PalettePaths;
            string[] p = paths.Split('|');

            return p;
        }

        private TerrainList LoadImagesIntoImageList(string path)
        {
            string path2 = Path.Combine(Environment.CurrentDirectory, path, string.Format("{0}.txt", path));
            string[] lines = FileReader.ReadFile(path2);

            var terrainList = new TerrainList();
            foreach (string line in lines)
            {
                string[] pieces = line.Split(':');
                var terrain = new Terrain
                {
                    Id = Convert.ToInt32(pieces[0]),
                    Filename = pieces[1].Replace("\"", string.Empty)
                };
                terrain.Name = Path.GetFileNameWithoutExtension(terrain.Filename);
                terrainList.Add(terrain);
            }

            return terrainList;
        }

        private ImageList AddImagesToImageList(string path, IEnumerable<Terrain> terrainList)
        {
            var imageList = new ImageList
                {
                    ImageSize = new Size((int) Constants.HEX_WIDTH, (int) Constants.HEX_HEIGHT),
                    ColorDepth = ColorDepth.Depth24Bit
                };
            foreach (Terrain terrain in terrainList)
            {
                string path3 = Path.Combine(Environment.CurrentDirectory, path, terrain.Filename);
                imageList.Images.Add(Image.FromFile(path3));
            }

            return imageList;
        }

        private ListView AddImagesToListview(IEnumerable<Terrain> terrainList, ImageList imageList)
        {
            var listView = new ListView { Name = "listView", View = View.LargeIcon, LargeImageList = imageList, Dock = DockStyle.Fill };

            foreach (Terrain terrain in terrainList)
            {
                var item = new ListViewItem { ImageIndex = terrain.Id - 1, Text = terrain.Name, Tag = terrain.Id - 1 };
                listView.Items.Add(item);
            }

            ListView_SetSpacing(listView, (int)Constants.HEX_WIDTH + 12, (int)Constants.HEX_HEIGHT + 4 + 20);

            return listView;
        }

        private void ListView_SetSpacing(ListView listview, short cx, short cy)
        {
            const int lvmFirst = 0x1000;
            const int lvmSeticonspacing = lvmFirst + 53;
            // http://msdn.microsoft.com/en-us/library/bb761176(VS.85).aspx
            // minimum spacing = 4
            SendMessage(listview.Handle, lvmSeticonspacing, IntPtr.Zero, (IntPtr)MakeLong(cx, cy));

            // http://msdn.microsoft.com/en-us/library/bb775085(VS.85).aspx
            // DOESN'T WORK!
            // can't find ListView_SetIconSpacing in dll comctl32.dll
            //ListView_SetIconSpacing(listView.Handle, 5, 5);
        }

        private int MakeLong(short lowPart, short highPart)
        {
            return (int)(((ushort)lowPart) | (uint)(highPart << 16));
        }
    }
}