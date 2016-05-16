namespace WarlordsRevengeEditor
{
    public struct HexAxial
    {
        private readonly float _q;
        private readonly float _r;

        public HexAxial(float q, float r)
        {
            _q = q;
            _r = r;
        }

        public float Q { get { return _q; } }
        public float R { get { return _r; } }
    }

    public struct HexCube
    {
        private readonly float _x;
        private readonly float _y;
        private readonly float _z;

        public HexCube(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public float X { get { return _x; } }
        public float Y { get { return _y; } }
        public float Z { get { return _z; } }
    }
}