using minecrap.graphics;
using OpenTK.Mathematics;

namespace minecrap.world
{
    internal enum BlockType
    {
        Air,
        Dirt,
        Grass,
        Stone,
        Cobblestone,
        Water,
        Glass,
        Sand,
        Sapling,
        Log,
        Leaves,
    }

    internal enum Faces
    {
        Front,
        Back,
        Left,
        Right,
        Top,
        Bottom,
        Inside
    }

    internal struct FaceData
    {
        public Vector3[] vertexes;
        public Vector2[] texCoords; 
        public Color[] colors;
        public bool twoSided;
    };
}