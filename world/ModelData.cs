using OpenTK.Mathematics;

namespace minecrap.world
{
    public enum CustomModelType
    {
        None,
        Decor,
    }

    internal static class ModelData
    {
        public static readonly Dictionary<BlockType, CustomModelType> modelByBlockType = new()
        {
            [BlockType.Sapling] = CustomModelType.Decor
        };

        public static readonly Dictionary<CustomModelType, List<Vector3>> vertexesByModelType = new()
        {
            [CustomModelType.None] = new List<Vector3>(),
            [CustomModelType.Decor] = new List<Vector3>()
            {
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
            },
        };
    }
}