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

        private readonly HexGrid _grid;

        private Point _panStartPoint;

        public Form1()
        {
            InitializeComponent();
            FillPaletteControl();

            int size = 4;
            float mapWidth = (size*2 + 2) * Constants.HEX_WIDTH * Constants.THREE_QUARTERS;
            float mapHeight = (size*2 + 2) * Constants.HEX_HEIGHT;
            _grid = new HexGrid(size, (int)mapWidth, (int)mapHeight);
            SetHexGrid(_grid);

            pictureBox1.Image = _grid.Render(imageList1);
        }

        private void FillPaletteControl()
        {
            string path = GetPalette();
            List<string> fileNames = LoadImagesIntoImageList(path);
            AddImagesToListview(fileNames);
        }

        private string GetPalette()
        {
            string path = Properties.Settings.Default.PalettePaths;

            return path;
        }

        private List<string> LoadImagesIntoImageList(string path)
        {
            var fileNames = new List<string>();
            var dir = new DirectoryInfo(path);
            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {
                    imageList1.Images.Add(Image.FromFile(file.FullName));
                    fileNames.Add(Path.GetFileNameWithoutExtension(file.Name));
                }
                catch (Exception)
                {
                    Console.WriteLine(@"{0} is not an image file.", file.FullName);
                }
            }

            return fileNames;
        }

        private void AddImagesToListview(List<string> fileNames)
        {
            listView1.View = View.LargeIcon;
            imageList1.ImageSize = new Size((int)Constants.HEX_WIDTH, (int)Constants.HEX_HEIGHT);
            imageList1.ColorDepth = ColorDepth.Depth24Bit;
            listView1.LargeImageList = imageList1;

            for (int j = 0; j < imageList1.Images.Count; j++)
            {
                var item = new ListViewItem { ImageIndex = j, Text = fileNames[j] };
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SetHexGrid(HexGrid hg)
        {
            Console.WriteLine();

            hg.SetCell(new HexCube(0, 0, 0), 1);
            hg.GetCell(new HexCube(0, 0, 0)); // 1

            hg.SetCell(new HexCube(0, 1, -1), 2);
            hg.GetCell(new HexCube(0, 1, -1)); // 2

            hg.SetCell(new HexCube(1, 0, -1), 3);
            hg.GetCell(new HexCube(1, 0, -1)); // 3

            hg.SetCell(new HexCube(1, -1, 0), 4);
            hg.GetCell(new HexCube(1, -1, 0)); // 4

            hg.SetCell(new HexCube(0, -1, 1), 5);
            hg.GetCell(new HexCube(0, -1, 1)); // 5

            hg.SetCell(new HexCube(-1, 0, 1), 6);
            hg.GetCell(new HexCube(-1, 0, 1)); // 6

            hg.SetCell(new HexCube(-1, 1, 0), 7);
            hg.GetCell(new HexCube(-1, 1, 0)); // 7

            hg.SetCell(new HexCube(-1, 2, -1), 8);
            hg.GetCell(new HexCube(-1, 2, -1)); // 8

            hg.SetCell(new HexCube(0, 2, -2), 9);
            hg.GetCell(new HexCube(0, 2, -2)); // 9

            hg.SetCell(new HexCube(1, 1, -2), 10);
            hg.GetCell(new HexCube(1, 1, -2)); // 10

            hg.SetCell(new HexCube(2, 0, -2), 11);
            hg.GetCell(new HexCube(2, 0, -2)); // 11

            hg.SetCell(new HexCube(2, -1, -1), 12);
            hg.GetCell(new HexCube(2, -1, -1)); // 12

            hg.SetCell(new HexCube(2, -2, 0), 13);
            hg.GetCell(new HexCube(2, -2, 0)); // 13

            hg.SetCell(new HexCube(1, -2, 1), 14);
            hg.GetCell(new HexCube(1, -2, 1)); // 14

            hg.SetCell(new HexCube(0, -2, 2), 15);
            hg.GetCell(new HexCube(0, -2, 2)); // 15

            hg.SetCell(new HexCube(-1, -1, 2), 16);
            hg.GetCell(new HexCube(-1, -1, 2)); // 16

            hg.SetCell(new HexCube(-2, 0, 2), 17);
            hg.GetCell(new HexCube(-2, 0, 2)); // 17

            hg.SetCell(new HexCube(-2, 1, 1), 18);
            hg.GetCell(new HexCube(-2, 1, 1)); // 18

            hg.SetCell(new HexCube(-2, 2, 0), 19);
            hg.GetCell(new HexCube(-2, 2, 0)); // 19

            //hg.SetCell(new HexCube(0, 3, -3), 20);
            //hg.GetCell(new HexCube(0, 3, -3)); // 20

            //hg.SetCell(new HexCube(0, 5, -5), 6);

            Console.WriteLine();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _panStartPoint = new Point(e.X, e.Y);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            statusStrip1.Items[0].Text = string.Format("Mouse Position: [X: {0} Y:{1}]", e.X, e.Y);
            PointF offset = DetermineOffset(e.X, e.Y);
            statusStrip1.Items[1].Text = string.Format("Offset From Center: [X: {0} Y:{1}]", offset.X, offset.Y);
            HexCube cube = DetermineHexAtMousePointer(e.X, e.Y);
            statusStrip1.Items[2].Text = string.Format("Selected Hex: [X: {0} Y:{1} Z:{2}]", cube.X, cube.Y, cube.Z);

            if (e.Button == MouseButtons.Left)
            {
                int deltaX = _panStartPoint.X - e.X;
                int deltaY = _panStartPoint.Y - e.Y;

                panel1.AutoScrollPosition = new Point(deltaX - panel1.AutoScrollPosition.X, deltaY - panel1.AutoScrollPosition.Y);
            }
        }

        private PointF DetermineOffset(int mousePosX, int mousePosY)
        {
            var point = new PointF { X = mousePosX - _grid.Center.X, Y = mousePosY - _grid.Center.Y };

            return point;
        }

        private HexCube DetermineHexAtMousePointer(int mousePosX, int mousePosY)
        {
            PointF offset = DetermineOffset(mousePosX, mousePosY);
            HexCube cube = _grid.PixelToHex(offset);

            return cube;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                int imageId = listView1.SelectedItems[0].ImageIndex;
                HexCube cube = DetermineHexAtMousePointer(e.X, e.Y);

                _grid.SetCell(cube, imageId);
                pictureBox1.Image = _grid.Render(imageList1);
            }
        }
    }
}