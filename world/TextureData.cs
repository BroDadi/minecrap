using OpenTK.Mathematics;

namespace minecrap.world
{
    internal static class TextureData
    {
        private const int width = 16;
        private const int height = 16;
        private static readonly List<Vector2>[,] textures = GetTextures();

        private static List<Vector2>[,] GetTextures()
        {
            List<Vector2>[,] result = new List<Vector2>[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    List<Vector2> list = new()
                    {
                        new Vector2((float)x / width, (float)(height - y) / height),
                        new Vector2((float)(x + 1) / width, (float)(height - y) / height),
                        new Vector2((float)(x + 1) / width, (float)(height - y - 1) / height),
                        new Vector2((float)x / width, (float)(height - y - 1) / height),
                    };
                    result[x, y] = list;
                }
            }
            return result;
        }

        public static readonly Dictionary<BlockType, Dictionary<Faces, List<Vector2>>> blockTypeTextures = new()
        {
            [BlockType.Dirt] = new Dictionary<Faces, List<Vector2>>()
            {
                [Faces.Front] = textures[0, 0],
                [Faces.Back] = textures[0, 0],
                [Faces.Left] = textures[0, 0],
                [Faces.Right] = textures[0, 0],
                [Faces.Top] = textures[0, 0],
                [Faces.Bottom] = textures[0, 0],
            },
            [BlockType.Grass] = new Dictionary<Faces, List<Vector2>>()
            {
                [Faces.Front] = textures[2, 0],
                [Faces.Back] = textures[2, 0],
                [Faces.Left] = textures[2, 0],
                [Faces.Right] = textures[2, 0],
                [Faces.Top] = textures[1, 0],
                [Faces.Bottom] = textures[0, 0],
            },
            [BlockType.Stone] = new Dictionary<Faces, List<Vector2>>()
            {
                [Faces.Front] = textures[3, 0],
                [Faces.Back] = textures[3, 0],
                [Faces.Left] = textures[3, 0],
                [Faces.Right] = textures[3, 0],
                [Faces.Top] = textures[3, 0],
                [Faces.Bottom] = textures[3, 0],
            },
            [BlockType.Cobblestone] = new Dictionary<Faces, List<Vector2>>()
            {
                [Faces.Front] = textures[4, 0],
                [Faces.Back] = textures[4, 0],
                [Faces.Left] = textures[4, 0],
                [Faces.Right] = textures[4, 0],
                [Faces.Top] = textures[4, 0],
                [Faces.Bottom] = textures[4, 0],
            },
            [BlockType.Water] = new Dictionary<Faces, List<Vector2>>()
            {
                [Faces.Front] = textures[5, 0],
                [Faces.Back] = textures[5, 0],
                [Faces.Left] = textures[5, 0],
                [Faces.Right] = textures[5, 0],
                [Faces.Top] = textures[5, 0],
                [Faces.Bottom] = textures[5, 0],
            },
            [BlockType.Glass] = new Dictionary<Faces, List<Vector2>>()
            {
                [Faces.Front] = textures[6, 0],
                [Faces.Back] = textures[6, 0],
                [Faces.Left] = textures[6, 0],
                [Faces.Right] = textures[6, 0],
                [Faces.Top] = textures[6, 0],
                [Faces.Bottom] = textures[6, 0],
            },
            [BlockType.Sand] = new Dictionary<Faces, List<Vector2>>()
            {
                [Faces.Front] = textures[7, 0],
                [Faces.Back] = textures[7, 0],
                [Faces.Left] = textures[7, 0],
                [Faces.Right] = textures[7, 0],
                [Faces.Top] = textures[7, 0],
                [Faces.Bottom] = textures[7, 0],
            },
        };

        public static readonly Dictionary<BlockType, List<Vector2>> customTextures = new()
        {
            [BlockType.Sapling] =
            [
                .. textures[0, 15],
                .. textures[0, 15]
            ]
        };
    }
}