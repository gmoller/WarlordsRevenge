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

        public Form1()
        {
            InitializeComponent();
            FillPaletteControl();

            var hg = new HexGrid();

            hg.Get(0, 0); // x;y
            hg.Get(1, 0);
            hg.Get(0, 1);
            hg.Get(1, 1);

            Console.WriteLine();

            //hg.Get(0, 0, 0); // x;y;z
            //hg.Get(1, 0, 0);
            //hg.Get(2, 0, 0);

            //hg.Get(0, 1, 0);
            //hg.Get(1, 1, 0);
            //hg.Get(2, 1, 0);

            //hg.Get(0, 2, 0);
            //hg.Get(1, 2, 0);
            //hg.Get(2, 2, 0);

            //hg.Get(0, 0, 1);
            //hg.Get(1, 0, 1);
            //hg.Get(2, 0, 1);

            //hg.Get(0, 1, 1);
            //hg.Get(1, 1, 1);
            //hg.Get(2, 1, 1);

            //hg.Get(0, 2, 1);
            //hg.Get(1, 2, 1);
            //hg.Get(2, 2, 1);

            //hg.Get(0, 0, 2);
            //hg.Get(1, 0, 2);
            //hg.Get(2, 0, 2);

            //hg.Get(0, 1, 2);
            //hg.Get(1, 1, 2);
            //hg.Get(2, 1, 2);

            //hg.Get(0, 2, 2);
            //hg.Get(1, 2, 2);
            //hg.Get(2, 2, 2);

            hg.Set(0, 0, 0, 1);
            hg.Get(0, 0, 0); // 1

            hg.Set(0, 1, -1, 2);
            hg.Get(0, 1, -1); // 2

            hg.Set(1, 0, -1, 3);
            hg.Get(1, 0, -1); // 3
            
            hg.Set(1, -1, 0, 4);
            hg.Get(1, -1, 0); // 4

            hg.Set(0, -1, 1, 5);
            hg.Get(0, -1, 1); // 5

            hg.Set(-1, 0, 1, 6);
            hg.Get(-1, 0, 1); // 6

            hg.Set(-1, 1, 0, 7);
            hg.Get(-1, 1, 0); // 7

            hg.Set(-1, 2, -1, 8);
            hg.Get(-1, 2, -1); // 8

            hg.Set(0, 2, -2, 9);
            hg.Get(0, 2, -2); // 9

            hg.Set(1, 1, -2, 10);
            hg.Get(1, 1, -2); // 10

            hg.Set(2, 0, -2, 11);
            hg.Get(2, 0, -2); // 11

            hg.Set(2, -1, -1, 12);
            hg.Get(2, -1, -1); // 12

            hg.Set(2, -2, 0, 13);
            hg.Get(2, -2, 0); // 13

            hg.Set(1, -2, 1, 14);
            hg.Get(1, -2, 1); // 14

            hg.Set(0, -2, 2, 15);
            hg.Get(0, -2, 2); // 15

            hg.Set(-1, -1, 2, 16);
            hg.Get(-1, -1, 2); // 16

            hg.Set(-2, 0, 2, 17);
            hg.Get(-2, 0, 2); // 17

            hg.Set(-2, 1, 1, 18);
            hg.Get(-2, 1, 1); // 18

            hg.Set(-2, 2, 0, 19);
            hg.Get(-2, 2, 0); // 19

            hg.Set(0, 3, -3, 20);
            hg.Get(0, 3, -3); // 20

            Console.WriteLine();
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
    }
}