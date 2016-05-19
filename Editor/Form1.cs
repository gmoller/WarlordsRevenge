using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WarlordsRevengeEditor
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private Map _map;
        private List<Terrain> _terrainList;

        private Point _panStartPoint;

        public Form1()
        {
            InitializeComponent();
            FillPaletteControl();

            string previousMap = Properties.Settings.Default.PreviousMap;
            if (string.IsNullOrEmpty(previousMap))
            {
                NewMap("First");
            }
            else
            {
                string path = Path.Combine(Environment.CurrentDirectory, Properties.Settings.Default.MapsPath, previousMap + ".map");
                LoadMap(path);
            }

            pictureBox1.Image = _map.Render(imageList1);
        }

        private void FillPaletteControl()
        {
            string[] paths = GetPalettePaths();
           _terrainList = LoadImagesIntoImageList(paths[0]);
           AddImagesToListview(_terrainList);
        }

        private string[] GetPalettePaths()
        {
            string paths = Properties.Settings.Default.PalettePaths;
            string[] p = paths.Split('|');

            return p;
        }

        private List<Terrain> LoadImagesIntoImageList(string path)
        {
            string path2 = Path.Combine(Environment.CurrentDirectory, path, string.Format("{0}.txt", path));
            string[] lines = FileReader.ReadFile(path2);

            var terrainList = new List<Terrain>();
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

            foreach (Terrain terrain in terrainList)
            {
                string path3 = Path.Combine(Environment.CurrentDirectory, path, terrain.Filename);
                imageList1.Images.Add(Image.FromFile(path3));
            }

            return terrainList;
        }

        private void AddImagesToListview(IEnumerable<Terrain> terrainList)
        {
            listView1.View = View.LargeIcon;
            imageList1.ImageSize = new Size((int)Constants.HEX_WIDTH, (int)Constants.HEX_HEIGHT);
            imageList1.ColorDepth = ColorDepth.Depth24Bit;
            listView1.LargeImageList = imageList1;

            foreach (Terrain terrain in terrainList)
            {
                var item = new ListViewItem { ImageIndex = terrain.Id - 1, Text = terrain.Name, Tag = terrain.Id - 1 };
                listView1.Items.Add(item);
            }

            ListView_SetSpacing(listView1, (int)Constants.HEX_WIDTH + 12, (int)Constants.HEX_HEIGHT + 4 + 20);
        }

        public void ListView_SetSpacing(ListView listview, short cx, short cy)
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

        public int MakeLong(short lowPart, short highPart)
        {
            return (int)(((ushort)lowPart) | (uint)(highPart << 16));
        }

        //private void SetHexGrid(HexGrid hg)
        //{
        //    Console.WriteLine();

        //    hg.SetCell(new HexCube(0, 0, 0), 1);
        //    hg.GetCell(new HexCube(0, 0, 0)); // 1

        //    hg.SetCell(new HexCube(0, 1, -1), 2);
        //    hg.GetCell(new HexCube(0, 1, -1)); // 2

        //    hg.SetCell(new HexCube(1, 0, -1), 3);
        //    hg.GetCell(new HexCube(1, 0, -1)); // 3

        //    hg.SetCell(new HexCube(1, -1, 0), 4);
        //    hg.GetCell(new HexCube(1, -1, 0)); // 4

        //    hg.SetCell(new HexCube(0, -1, 1), 5);
        //    hg.GetCell(new HexCube(0, -1, 1)); // 5

        //    hg.SetCell(new HexCube(-1, 0, 1), 6);
        //    hg.GetCell(new HexCube(-1, 0, 1)); // 6

        //    hg.SetCell(new HexCube(-1, 1, 0), 7);
        //    hg.GetCell(new HexCube(-1, 1, 0)); // 7

        //    hg.SetCell(new HexCube(-1, 2, -1), 8);
        //    hg.GetCell(new HexCube(-1, 2, -1)); // 8

        //    hg.SetCell(new HexCube(0, 2, -2), 9);
        //    hg.GetCell(new HexCube(0, 2, -2)); // 9

        //    hg.SetCell(new HexCube(1, 1, -2), 10);
        //    hg.GetCell(new HexCube(1, 1, -2)); // 10

        //    hg.SetCell(new HexCube(2, 0, -2), 11);
        //    hg.GetCell(new HexCube(2, 0, -2)); // 11

        //    hg.SetCell(new HexCube(2, -1, -1), 12);
        //    hg.GetCell(new HexCube(2, -1, -1)); // 12

        //    hg.SetCell(new HexCube(2, -2, 0), 13);
        //    hg.GetCell(new HexCube(2, -2, 0)); // 13

        //    hg.SetCell(new HexCube(1, -2, 1), 14);
        //    hg.GetCell(new HexCube(1, -2, 1)); // 14

        //    hg.SetCell(new HexCube(0, -2, 2), 15);
        //    hg.GetCell(new HexCube(0, -2, 2)); // 15

        //    hg.SetCell(new HexCube(-1, -1, 2), 16);
        //    hg.GetCell(new HexCube(-1, -1, 2)); // 16

        //    hg.SetCell(new HexCube(-2, 0, 2), 17);
        //    hg.GetCell(new HexCube(-2, 0, 2)); // 17

        //    hg.SetCell(new HexCube(-2, 1, 1), 18);
        //    hg.GetCell(new HexCube(-2, 1, 1)); // 18

        //    hg.SetCell(new HexCube(-2, 2, 0), 19);
        //    hg.GetCell(new HexCube(-2, 2, 0)); // 19

        //    //hg.SetCell(new HexCube(0, 3, -3), 20);
        //    //hg.GetCell(new HexCube(0, 3, -3)); // 20

        //    //hg.SetCell(new HexCube(0, 5, -5), 6);

        //    Console.WriteLine();
        //}

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _panStartPoint = new Point(e.X, e.Y);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            statusStrip1.Items[0].Text = string.Format("Mouse Position: [X: {0} Y:{1}]", e.X, e.Y);
            PointF offset = DetermineOffset(e.X, e.Y);
            statusStrip1.Items[1].Text = string.Format("Offset From Center: [X: {0} Y:{1}]", offset.X, offset.Y);
            HexAxial axial = _map.DetermineHexAtMousePointer(new Point(e.X, e.Y));
            statusStrip1.Items[2].Text = string.Format("Selected Hex: [X: {0} Y:{1}]", axial.Q, axial.R);

            if (e.Button == MouseButtons.Left)
            {
                int deltaX = _panStartPoint.X - e.X;
                int deltaY = _panStartPoint.Y - e.Y;

                panel1.AutoScrollPosition = new Point(deltaX - panel1.AutoScrollPosition.X, deltaY - panel1.AutoScrollPosition.Y);
            }
        }

        private PointF DetermineOffset(int mousePosX, int mousePosY)
        {
            var point = new PointF { X = mousePosX - _map.Center.X, Y = mousePosY - _map.Center.Y };

            return point;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                var selected = listView1.SelectedItems[0];
                _map.SetCell(new Point(e.X, e.Y), (int)selected.Tag);
            }
            else
            {
                _map.RemoveImageFromCell(new Point(e.X, e.Y));
            }

            Text = string.Format("Warlords Revenge Editor [{0}]", AppendAsteriskToName(_map.Name));
            pictureBox1.Image = _map.Render(imageList1);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckIfOk("create a new map") == DialogResult.Yes)
            {
                string name = string.Empty;
                DialogResult dialogResult = InputDialog.ShowInputDialog(@"Enter Map Name", ref name);

                if (dialogResult == DialogResult.OK)
                {
                    NewMap(name);
                }
            }
        }

        private void NewMap(string name)
        {
            string path = Path.Combine(Environment.CurrentDirectory, Properties.Settings.Default.MapsPath);
            _map = Map.NewMap(name, path, 10);
            Text = string.Format("Warlords Revenge Editor [{0}]", AppendAsteriskToName(_map.Name));
            pictureBox1.Image = _map.Render(imageList1);
        }

        private string AppendAsteriskToName(string name)
        {
            return _map.IsDirty ? string.Format("{0}*", name) : name;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckIfOk("open a map") == DialogResult.Yes)
            {
                // open file dialog
                string path = Path.Combine(Environment.CurrentDirectory, Properties.Settings.Default.MapsPath);
                openFileDialog1.Title = @"Open map";
                openFileDialog1.InitialDirectory = path;
                openFileDialog1.FileName = string.Empty;
                openFileDialog1.Filter = @"map files (*.map)|*.map";
                DialogResult result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    LoadMap(openFileDialog1.FileName);
                }
            }
        }

        private void LoadMap(string path)
        {
            _map = Map.LoadMap(path);
            Text = string.Format("Warlords Revenge Editor [{0}]", AppendAsteriskToName(_map.Name));
            Properties.Settings.Default.PreviousMap = _map.Name;
            Properties.Settings.Default.Save();
            pictureBox1.Image = _map.Render(imageList1);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _map.Save();
            Text = string.Format("Warlords Revenge Editor [{0}]", AppendAsteriskToName(_map.Name));
            Properties.Settings.Default.PreviousMap = _map.Name;
            Properties.Settings.Default.Save();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckIfOk("exit") == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private DialogResult CheckIfOk(string s)
        {
            if (_map.IsDirty)
            {
                DialogResult result = MessageBox.Show(string.Format("Map has not been saved. Are you sure you want to {0}?", s),
                                                      @"Warning", MessageBoxButtons.YesNo);

                return result;
            }

            return DialogResult.Yes;
        }
    }
}