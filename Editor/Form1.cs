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

        private HexGrid _grid;
        private int _x;
        private int _y;

        public Form1()
        {
            InitializeComponent();
            FillPaletteControl();

            _grid = new HexGrid(5, 700, 700);
            SetHexGrid(_grid);

            pictureBox1.Image = _grid.Render(imageList1);

            SetScrollBars();
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
            imageList1.ImageSize = new Size(72, 72);
            imageList1.ColorDepth = ColorDepth.Depth24Bit;
            listView1.LargeImageList = imageList1;

            for (int j = 0; j < imageList1.Images.Count; j++)
            {
                var item = new ListViewItem { ImageIndex = j, Text = fileNames[j] };
                listView1.Items.Add(item);
            }

            ListView_SetSpacing(listView1, 72 + 12, 72 + 4 + 20);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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

        private void SetHexGrid(HexGrid hg)
        {
            Console.WriteLine();

            hg.SetCell(0, 0, 0, 1);
            hg.GetCell(0, 0, 0); // 1

            hg.SetCell(0, 1, -1, 2);
            hg.GetCell(0, 1, -1); // 2

            hg.SetCell(1, 0, -1, 3);
            hg.GetCell(1, 0, -1); // 3

            hg.SetCell(1, -1, 0, 4);
            hg.GetCell(1, -1, 0); // 4

            hg.SetCell(0, -1, 1, 5);
            hg.GetCell(0, -1, 1); // 5

            hg.SetCell(-1, 0, 1, 6);
            hg.GetCell(-1, 0, 1); // 6

            hg.SetCell(-1, 1, 0, 7);
            hg.GetCell(-1, 1, 0); // 7

            hg.SetCell(-1, 2, -1, 8);
            hg.GetCell(-1, 2, -1); // 8

            hg.SetCell(0, 2, -2, 9);
            hg.GetCell(0, 2, -2); // 9

            hg.SetCell(1, 1, -2, 10);
            hg.GetCell(1, 1, -2); // 10

            hg.SetCell(2, 0, -2, 11);
            hg.GetCell(2, 0, -2); // 11

            hg.SetCell(2, -1, -1, 12);
            hg.GetCell(2, -1, -1); // 12

            hg.SetCell(2, -2, 0, 13);
            hg.GetCell(2, -2, 0); // 13

            hg.SetCell(1, -2, 1, 14);
            hg.GetCell(1, -2, 1); // 14

            hg.SetCell(0, -2, 2, 15);
            hg.GetCell(0, -2, 2); // 15

            hg.SetCell(-1, -1, 2, 16);
            hg.GetCell(-1, -1, 2); // 16

            hg.SetCell(-2, 0, 2, 17);
            hg.GetCell(-2, 0, 2); // 17

            hg.SetCell(-2, 1, 1, 18);
            hg.GetCell(-2, 1, 1); // 18

            hg.SetCell(-2, 2, 0, 19);
            hg.GetCell(-2, 2, 0); // 19

            hg.SetCell(0, 3, -3, 20);
            hg.GetCell(0, 3, -3); // 20

            Console.WriteLine();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            _x = hScrollBar1.Value;
            pictureBox1.Refresh();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            _y = vScrollBar1.Value;
            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            pictureBox1.Image = _grid.Render(imageList1);
            e.Graphics.DrawImage(pictureBox1.Image, e.ClipRectangle, _x, _y, e.ClipRectangle.Width, e.ClipRectangle.Height, GraphicsUnit.Pixel);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            SetScrollBars();
            pictureBox1.Refresh();
        }

        private void SetScrollBars()
        {
            hScrollBar1.Width = pictureBox1.Width;
            hScrollBar1.Left = pictureBox1.Left;
            hScrollBar1.Top = pictureBox1.Bottom;
            hScrollBar1.Maximum = pictureBox1.Image.Width - pictureBox1.Width;
            vScrollBar1.Height = pictureBox1.Height;
            vScrollBar1.Left = pictureBox1.Left + pictureBox1.Width;
            vScrollBar1.Top = pictureBox1.Top;
            vScrollBar1.Maximum = pictureBox1.Image.Height - pictureBox1.Height;
        }
    }
}