using System.Drawing;
using System.Windows.Forms;

namespace WarlordsRevengeEditor.Controls
{
    public class TabControlCreator
    {
        public TabControl Create(int numberOfPages)
        {
            var tabControl = new TabControl
                {
                    Dock = DockStyle.Right,
                    Name = "tabControl",
                    SelectedIndex = 0,
                    Size = new Size(265, 0)
                };

            for (int i = 0; i < numberOfPages; i++)
            {
                var tabPage = new TabPage
                    {
                        Name = string.Format("Palette {0}", i + 1),
                        Padding = new Padding(3),
                        TabIndex = i,
                        UseVisualStyleBackColor = true
                    };
                tabPage.Text = tabPage.Name;
                tabControl.Controls.Add(tabPage);
            }

            return tabControl;
        }
    }
}