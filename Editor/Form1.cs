using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WarlordsRevengeEditor.Controls;

namespace WarlordsRevengeEditor
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private Map _map;
        private readonly Layers _layers = new Layers();
        private Palette _palette;

        private Point _panStartPoint;
        private MenuStrip _menuStrip;

        public Form1()
        {
            InitializeComponent();
            CreateMenuControl();
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

            pictureBox1.Image = _map.Render(_palette.GetAllImageLists());
        }

        private void CreateMenuControl()
        {
            // build datastructure
            var menu = new MenuCreator();
            var menuItems = menu.CreateMenuDataStructure(_layers, newToolStripMenuItem_Click, openToolStripMenuItem_Click, saveToolStripMenuItem_Click, exitToolStripMenuItem_Click, layerToolStripMenuItem_Click);

            // create a menustrip control from that data structure
            _menuStrip = menu.CreateMenu(menuItems);

            Controls.Add(_menuStrip);
        }

        private void FillPaletteControl()
        {
            _palette = new Palette();
            _palette.LoadAllTerrain();

            var myTabControl = new TabControlCreator();
            var tabControl = myTabControl.Create(_palette.NumberOfPalettes);

            for (int i = 0; i < _palette.NumberOfPalettes; i++)
            {
                ListView listView =_palette.GetListView(i);
                tabControl.TabPages[i].Controls.Add(listView);
            }

            Controls.Add(tabControl);
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
            // TODO: fix bug where invalid cell is selected
            int layerId = DetermineWhichLayerIsCurrentlySelected();
            int selectedPalette = DetermineWhichPaletteIsCurrentlySelected();
            int selectedImage = DetermineWhichImageIsCurrentlySelected();
            if (selectedImage >= 0)
            {
                _map.SetCell(new Point(e.X, e.Y), layerId, selectedPalette, selectedImage);
            }
            else
            {
                _map.RemoveImageFromCell(new Point(e.X, e.Y), layerId);
            }

            Text = string.Format("Warlords Revenge Editor [{0}]", AppendAsteriskToName(_map.Name));
            pictureBox1.Image = _map.Render(_palette.GetAllImageLists());
        }

        private int DetermineWhichLayerIsCurrentlySelected()
        {
            int layerIdSelected = -1;

            var layers = (ToolStripDropDownItem)_menuStrip.Items[1];
            int count = 1;
            foreach (ToolStripItem layer in layers.DropDownItems)
            {
                var l = (ToolStripMenuItem)layer;
                if (l.Checked)
                {
                    layerIdSelected = count;
                }
                count++;
            }

            return layerIdSelected;
        }

        private int DetermineWhichPaletteIsCurrentlySelected()
        {
            var ctrl = (TabControl)Controls["tabControl"];

            return ctrl.SelectedIndex;
        }

        private int DetermineWhichImageIsCurrentlySelected()
        {
            var ctrl = (TabControl)Controls["tabControl"];
            TabPage selectedPage = ctrl.SelectedTab;
            var listView = (ListView)selectedPage.Controls["listView"];
            if (listView.SelectedItems.Count == 1)
            {
                ListViewItem selected = listView.SelectedItems[0];
                return (int)selected.Tag;
            }

            return -1;
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
            pictureBox1.Image = _map.Render(_palette.GetAllImageLists());
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
            pictureBox1.Image = _map.Render(_palette.GetAllImageLists());
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

        private void layerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var menuClicked = (ToolStripDropDownItem)sender;

            var layers = (ToolStripDropDownItem)_menuStrip.Items[1];

            foreach (ToolStripItem layer in layers.DropDownItems)
            {
                if (menuClicked.Text == layer.Text)
                {
                    var l = (ToolStripMenuItem)layer;
                    l.Checked = true;
                }
                else
                {
                    var l = (ToolStripMenuItem)layer;
                    l.Checked = false;
                }
            }
        }
    }
}