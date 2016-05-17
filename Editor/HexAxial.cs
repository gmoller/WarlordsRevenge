using System.Drawing;

namespace WarlordsRevengeEditor
{
    public struct HexAxial
    {
        private readonly float _q;
        private readonly float _r;

        public float Q { get { return _q; } }
        public float R { get { return _r; } }

        public HexAxial(float q, float r)
        {
            _q = q;
            _r = r;
        }

        public HexCube ToCube()
        {
            float x = Q;
            float z = R;
            float y = -x - z;
            var hexCube = new HexCube(x, y, z);

            return hexCube;
        }

        /// <summary>
        /// Returns the center pixel of a hexagon
        /// </summary>
        public PointF HexToPixel()
        {
            float x = Constants.HALF_HEX_WIDTH * 1.5f * Q;
            //double y = Constants.HALF_HEX_HEIGHT * Math.Sqrt(3) * (axial.R + axial.Q / 2);
            float y = Constants.HALF_HEX_HEIGHT * 2.0f * (R + Q / 2.0f);

            return new PointF(x, y);
        }

        public HexAxial Round()
        {
            HexCube cube = ToCube();
            cube = cube.Round();

            return cube.ToAxial();
        }
    }
}