﻿using System.Drawing;
using System.Windows.Forms;

namespace WarlordsRevengeEditor
{
    public static class InputDialog
    {
        public static DialogResult ShowInputDialog(string caption, ref string input)
        {
            var size = new Size(300, 70);
            var inputBox = new Form { FormBorderStyle = FormBorderStyle.FixedDialog, ClientSize = size, Text = caption };

            var textBox = new TextBox { Size = new Size(size.Width - 10, 23), Location = new Point(5, 5), Text = input };
            inputBox.Controls.Add(textBox);

            var okButton = new Button
                {
                    DialogResult = DialogResult.OK,
                    Name = "okButton",
                    Size = new Size(75, 23),
                    Text = @"&OK",
                    Location = new Point(size.Width - 80 - 80, 39)
                };
            inputBox.Controls.Add(okButton);

            var cancelButton = new Button
                {
                    DialogResult = DialogResult.Cancel,
                    Name = "cancelButton",
                    Size = new Size(75, 23),
                    Text = @"&Cancel",
                    Location = new Point(size.Width - 80, 39)
                };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }
    }
}