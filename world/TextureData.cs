using OpenTK.Mathematics;

namespace minecrap.world
{
    internal static class TextureData
    {
        private const int width = 16;
        private const int height = 16;
        private static readonly Vector2[,][] textures = GetTextures();

        private static Vector2[,][] GetTextures()
        {
            Vector2[,][] result = new Vector2[width, height][];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2[] arr = new Vector2[]
                    {
                        new Vector2((float)x / width, (float)(height - y) / height),
                        new Vector2((float)(x + 1) / width, (float)(height - y) / height),
                        new Vector2((float)(x + 1) / width, (float)(height - y - 1) / height),
                        new Vector2((float)x / width, (float)(height - y - 1) / height),
                    };
                    result[x, y] = arr;
                }
            }
            return result;
        }

        public static readonly Dictionary<BlockType, Dictionary<Faces, Vector2[]>> blockTypeTextures = new()
        {
            [BlockType.Dirt] = new Dictionary<Faces, Vector2[]>()
            {
                [Faces.Front] = textures[0, 0],
                [Faces.Back] = textures[0, 0],
                [Faces.Left] = textures[0, 0],
                [Faces.Right] = textures[0, 0],
                [Faces.Top] = textures[0, 0],
                [Faces.Bottom] = textures[0, 0],
            },
            [BlockType.Grass] = new Dictionary<Faces, Vector2[]>()
            {
                [Faces.Front] = textures[2, 0],
                [Faces.Back] = textures[2, 0],
                [Faces.Left] = textures[2, 0],
                [Faces.Right] = textures[2, 0],
                [Faces.Top] = textures[1, 0],
                [Faces.Bottom] = textures[0, 0],
            },
            [BlockType.Stone] = new Dictionary<Faces, Vector2[]>()
            {
                [Faces.Front] = textures[3, 0],
                [Faces.Back] = textures[3, 0],
                [Faces.Left] = textures[3, 0],
                [Faces.Right] = textures[3, 0],
                [Faces.Top] = textures[3, 0],
                [Faces.Bottom] = textures[3, 0],
            },
            [BlockType.Cobblestone] = new Dictionary<Faces, Vector2[]>()
            {
                [Faces.Front] = textures[4, 0],
                [Faces.Back] = textures[4, 0],
                [Faces.Left] = textures[4, 0],
                [Faces.Right] = textures[4, 0],
                [Faces.Top] = textures[4, 0],
                [Faces.Bottom] = textures[4, 0],
            },
            [BlockType.Water] = new Dictionary<Faces, Vector2[]>()
            {
                [Faces.Front] = textures[5, 0],
                [Faces.Back] = textures[5, 0],
                [Faces.Left] = textures[5, 0],
                [Faces.Right] = textures[5, 0],
                [Faces.Top] = textures[5, 0],
                [Faces.Bottom] = textures[5, 0],
            },
            [BlockType.Glass] = new Dictionary<Faces, Vector2[]>()
            {
                [Faces.Front] = textures[6, 0],
                [Faces.Back] = textures[6, 0],
                [Faces.Left] = textures[6, 0],
                [Faces.Right] = textures[6, 0],
                [Faces.Top] = textures[6, 0],
                [Faces.Bottom] = textures[6, 0],
            },
            [BlockType.Sand] = new Dictionary<Faces, Vector2[]>()
            {
                [Faces.Front] = textures[7, 0],
                [Faces.Back] = textures[7, 0],
                [Faces.Left] = textures[7, 0],
                [Faces.Right] = textures[7, 0],
                [Faces.Top] = textures[7, 0],
                [Faces.Bottom] = textures[7, 0],
            },
            [BlockType.Log] = new Dictionary<Faces, Vector2[]>()
            {
                [Faces.Front] = textures[8, 0],
                [Faces.Back] = textures[8, 0],
                [Faces.Left] = textures[8, 0],
                [Faces.Right] = textures[8, 0],
                [Faces.Top] = textures[9, 0],
                [Faces.Bottom] = textures[9, 0],
            },
            [BlockType.Leaves] = new Dictionary<Faces, Vector2[]>()
            {
                [Faces.Front] = textures[10, 0],
                [Faces.Back] = textures[10, 0],
                [Faces.Left] = textures[10, 0],
                [Faces.Right] = textures[10, 0],
                [Faces.Top] = textures[10, 0],
                [Faces.Bottom] = textures[10, 0],
            },
            [BlockType.Sapling] = new Dictionary<Faces, Vector2[]>()
            {
                [Faces.Inside] =
                [
                    .. textures[0, 15],
                    .. textures[0, 15],
                    .. textures[0, 15],
                    .. textures[0, 15],
                ]
            }
        };
    }
}