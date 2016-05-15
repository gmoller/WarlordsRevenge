using System;
using System.Drawing;

namespace WarlordsRevengeEditor
{
    public struct Cell
    {
        public static Cell NullCell { get { return new Cell { ImageId = -1 }; } }

        public int ImageId { get; set; }
        
        public bool IsNullCell { get { return ImageId == -1; } }

        public PointF[] GetCorners(PointF center)
        {
            var corners = new PointF[6];
            for (int i = 1; i <= 6; i++)
            {
                PointF corner = GetCorner(center, i);
                corners[i - 1] = corner;
            }

            return corners;
        }

        private PointF GetCorner(PointF center, int corner)
        {
            PointF p;
            switch (corner)
            {
                case 1:
                    p = GetCorner1();
                    break;
                case 2:
                    p = GetCorner2();
                    break;
                case 3:
                    p = GetCorner3();
                    break;
                case 4:
                    p = GetCorner4();
                    break;
                case 5:
                    p = GetCorner5();
                    break;
                case 6:
                    p = GetCorner6();
                    break;
                default:
                    throw new NotSupportedException(string.Format("Corner {0} is not supported.", corner));
            }

            var p2 = new PointF { X = center.X + p.X, Y = center.Y + p.Y };

            return p2;
        }

        private PointF GetCorner1()
        {
            float x = -Constants.HALF_HEX_WIDTH;
            float y = 0.0f;

            return new PointF(x, y);
        }

        private PointF GetCorner2()
        {
            float x = -Constants.QUARTER_HEX_WIDTH;
            float y = -Constants.HALF_HEX_HEIGHT;

            return new PointF(x, y);
        }

        private PointF GetCorner3()
        {
            float x = Constants.QUARTER_HEX_WIDTH;
            float y = -Constants.HALF_HEX_HEIGHT;

            return new PointF(x, y);
        }

        private PointF GetCorner4()
        {
            float x = Constants.HALF_HEX_WIDTH;
            float y = 0.0f;

            return new PointF(x, y);
        }

        private PointF GetCorner5()
        {
            float x = Constants.QUARTER_HEX_WIDTH;
            float y = Constants.HALF_HEX_HEIGHT;

            return new PointF(x, y);
        }

        private PointF GetCorner6()
        {
            float x = -Constants.QUARTER_HEX_WIDTH;
            float y = Constants.HALF_HEX_HEIGHT;

            return new PointF(x, y);
        }
    }
}