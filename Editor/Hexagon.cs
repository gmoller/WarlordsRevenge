using System;
using System.Drawing;

namespace WarlordsRevengeEditor
{
    public struct Hexagon
    {
        public static PointF[] GetCorners(PointF center)
        {
            var corners = new PointF[7];
            for (int i = 1; i <= 6; i++)
            {
                PointF corner = GetCorner(i);
                var p = new PointF { X = center.X + corner.X, Y = center.Y + corner.Y };
                corners[i - 1] = p;
            }
            corners[6] = corners[0];

            return corners;
        }

        private static PointF GetCorner(int corner)
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

            return p;
        }

        private static PointF GetCorner1()
        {
            float x = -Constants.HALF_HEX_WIDTH;
            float y = 0.0f;

            return new PointF(x, y);
        }

        private static PointF GetCorner2()
        {
            float x = -Constants.QUARTER_HEX_WIDTH;
            float y = -Constants.HALF_HEX_HEIGHT;

            return new PointF(x, y);
        }

        private static PointF GetCorner3()
        {
            float x = Constants.QUARTER_HEX_WIDTH;
            float y = -Constants.HALF_HEX_HEIGHT;

            return new PointF(x, y);
        }

        private static PointF GetCorner4()
        {
            float x = Constants.HALF_HEX_WIDTH;
            float y = 0.0f;

            return new PointF(x, y);
        }

        private static PointF GetCorner5()
        {
            float x = Constants.QUARTER_HEX_WIDTH;
            float y = Constants.HALF_HEX_HEIGHT;

            return new PointF(x, y);
        }

        private static PointF GetCorner6()
        {
            float x = -Constants.QUARTER_HEX_WIDTH;
            float y = Constants.HALF_HEX_HEIGHT;

            return new PointF(x, y);
        }
    }
}