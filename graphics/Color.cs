namespace minecrap.graphics
{
    public struct Color
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public Color(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static Color FromLighting(byte light)
        {
            if (light > 15) return new Color(255, 255, 255, 255);
            else
            {
                byte color = (byte)(60 + light * 13);
                return new Color(color, color, color, 255);
            }
        }

        public static Color operator *(Color a, Color b)
        {
            return new Color(
                (byte)(a.r * b.r / 255),
                (byte)(a.g * b.g / 255),
                (byte)(a.b * b.b / 255),
                (byte)(a.a * b.a / 255)
            );
        }
        public static Color operator *(Color a, float b) => new Color((byte)(a.r * b), (byte)(a.g * b), (byte)(a.b * b), a.a);
        public static bool operator ==(Color a, Color b) => a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
        public static bool operator !=(Color a, Color b) => a.r != b.r || a.g != b.g || a.b != b.b || a.a != b.a;
    }
}