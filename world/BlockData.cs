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
    }

    internal enum Faces
    {
        Front,
        Back,
        Left,
        Right,
        Top,
        Bottom
    }

    internal struct FaceData
    {
        public List<Vector3> vertexes;
        public List<Vector2> texCoords; 
        public List<Color4> colors;
        public bool twoSided;
    };

    internal struct RawFaceData
    {
        public static readonly Dictionary<Faces, List<Vector3>> rawVertexData = new()
        {
            [Faces.Front] = new List<Vector3>()
            {
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f)
            },
            [Faces.Back] = new List<Vector3>()
            {
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f)
            },
            [Faces.Left] = new List<Vector3>()
            {
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f)
            },
            [Faces.Right] = new List<Vector3>()
            {
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, 0.5f)
            },
            [Faces.Top] = new List<Vector3>()
            {
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f)
            },
            [Faces.Bottom] = new List<Vector3>()
            {
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f)
            }
        };
    };
}