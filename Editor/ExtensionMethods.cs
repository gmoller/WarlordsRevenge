using System.Drawing;

namespace WarlordsRevengeEditor
{
    public static class ExtensionMethods
    {
        public static HexAxial PixelToHex(this PointF pixel)
        {
            float q = pixel.X * Constants.TWO_THIRDS / Constants.HALF_HEX_WIDTH;
            //double r = (-pixel.X / 3.0f + (Math.Sqrt(3)/3.0f) * pixel.Y) / Constants.HALF_HEX_HEIGHT;
            double r = (-pixel.X / 3.0f + Constants.HALF * pixel.Y) / Constants.HALF_HEX_HEIGHT;

            var axial = new HexAxial(q, (float)r);

            axial = axial.Round();

            return axial;
        }
    }
}