using OpenTK.Mathematics;

namespace minecrap.world
{
    internal struct Collider
    {
        public Vector3 pos;
        public Vector3 size;
        public Vector3 min;
        public Vector3 max;
        public static Collider Null = new(Vector3.NegativeInfinity, Vector3.NegativeInfinity);
        public readonly bool isNull;

        public Collider(Vector3 pos, Vector3 size)
        {
            if (pos == Vector3.NegativeInfinity && size == Vector3.NegativeInfinity) isNull = true;
            else
            {
                isNull = false;
                this.pos = pos;
                this.size = size;
                CalculateMinMax();
            }
        }

        public bool Intersects(Collider other)
        {
            return !isNull &&
                    (max.X >= other.min.X) && (min.X <= other.max.X) &&
                    (max.Y >= other.min.Y) && (min.Y <= other.max.Y) &&
                    (max.Z >= other.min.Z) && (min.Z <= other.max.Z);
        }

        public Vector2 RayIntersections(Vector3 origin, Vector3 direction)
        {
            Vector3 tMin = (min - origin) / direction;
            Vector3 tMax = (max - origin) / direction;
            Vector3 t1 = Vector3.ComponentMin(tMin, tMax);
            Vector3 t2 = Vector3.ComponentMax(tMin, tMax);
            float tNear = Math.Max(Math.Max(t1.X, t1.Y), t1.Z);
            float tFar = Math.Min(Math.Min(t2.X, t2.Y), t2.Z);
            return new Vector2(tNear, tFar);
        }

        public bool IsPointInside(Vector3 point)
        {
            return !isNull &&
                    point.X <= max.X && point.X >= min.X &&
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
            min = pos - size / 2;
            max = pos + size / 2;
        }

        public static bool operator ==(Collider a, Collider b) => (a.isNull && b.isNull) || (a.pos == b.pos && a.size == b.size);
        public static bool operator !=(Collider a, Collider b) => (a.isNull != b.isNull) || (!a.isNull && !b.isNull && (a.pos != b.pos || a.size != b.size));
    }
}