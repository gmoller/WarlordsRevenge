using System;
using System.Drawing;

namespace WarlordsRevengeEditor
{
    public struct Cell
    {
        public static Cell NullCell { get { return new Cell { ImageId = -1 }; } }

        public int ImageId { get; set; }
        
        public bool IsNullCell { get { return ImageId == -1; } }

        public override string ToString()
        {
            return string.Format("{0}", ImageId);
        }
    }
}