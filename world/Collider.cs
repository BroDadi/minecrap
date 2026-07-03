using OpenTK.Mathematics;

namespace minecrap.world
{
    internal class Collider
    {
        public Vector3 pos;
        public Vector3 size;
        public Vector3 min;
        public Vector3 max;

        public Collider(Vector3 pos, Vector3 size)
        {
            this.pos = pos;
            this.size = size;
            CalculateMinMax();
        }

        public bool Intersects(Collider other)
        {
            return (max.X >= other.min.X) && (min.X <= other.max.X) &&
                    (max.Y >= other.min.Y) && (min.Y <= other.max.Y) &&
                    (max.Z >= other.min.Z) && (min.Z <= other.max.Z);
        }

        public bool IsPointInside(Vector3 point)
        {
            return point.X <= max.X && point.X >= min.X &&
                    point.Y <= max.Y && point.Y >= min.Y &&
                    point.Z <= max.Z && point.Z >= min.Z;
        }

        public void SetPosition(Vector3 pos)
        {
            this.pos = pos;
            CalculateMinMax();
        }

        public void SetSize(Vector3 size)
        {
            this.size = size;
            CalculateMinMax();
        }

        private void CalculateMinMax()
        {
            min = new Vector3(pos.X - size.X / 2, pos.Y - size.Y / 2, pos.Z - size.Z / 2);
            max = new Vector3(pos.X + size.X / 2, pos.Y + size.Y / 2, pos.Z + size.Z / 2);
        }
    }
}