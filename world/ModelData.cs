using OpenTK.Mathematics;

namespace minecrap.world
{
    public enum ModelType
    {
        Block,
        Decor,
    }

    internal static class ModelData
    {
        public static readonly Dictionary<ModelType, Dictionary<Faces, Vector3[]>> vertexesByModelType = new()
        {
            [ModelType.Block] = new Dictionary<Faces, Vector3[]>()
            {
                [Faces.Front] = new Vector3[]
                {
                    new Vector3(-0.5f, 0.5f, 0.5f),
                    new Vector3(0.5f, 0.5f, 0.5f),
                    new Vector3(0.5f, -0.5f, 0.5f),
                    new Vector3(-0.5f, -0.5f, 0.5f)
                },
                [Faces.Back] = new Vector3[]
                {
                    new Vector3(0.5f, 0.5f, -0.5f),
                    new Vector3(-0.5f, 0.5f, -0.5f),
                    new Vector3(-0.5f, -0.5f, -0.5f),
                    new Vector3(0.5f, -0.5f, -0.5f)
                },
                [Faces.Left] = new Vector3[]
                {
                    new Vector3(-0.5f, 0.5f, -0.5f),
                    new Vector3(-0.5f, 0.5f, 0.5f),
                    new Vector3(-0.5f, -0.5f, 0.5f),
                    new Vector3(-0.5f, -0.5f, -0.5f)
                },
                [Faces.Right] = new Vector3[]
                {
                    new Vector3(0.5f, 0.5f, 0.5f),
                    new Vector3(0.5f, 0.5f, -0.5f),
                    new Vector3(0.5f, -0.5f, -0.5f),
                    new Vector3(0.5f, -0.5f, 0.5f)
                },
                [Faces.Top] = new Vector3[]
                {
                    new Vector3(-0.5f, 0.5f, -0.5f),
                    new Vector3(0.5f, 0.5f, -0.5f),
                    new Vector3(0.5f, 0.5f, 0.5f),
                    new Vector3(-0.5f, 0.5f, 0.5f)
                },
                [Faces.Bottom] = new Vector3[]
                {
                    new Vector3(-0.5f, -0.5f, 0.5f),
                    new Vector3(0.5f, -0.5f, 0.5f),
                    new Vector3(0.5f, -0.5f, -0.5f),
                    new Vector3(-0.5f, -0.5f, -0.5f)
                }
            },
            [ModelType.Decor] = new Dictionary<Faces, Vector3[]>()
            {
                [Faces.Inside] = new Vector3[]
                {
                    new Vector3(-0.5f, 0.5f, -0.5f),
                    new Vector3(0.5f, 0.5f, 0.5f),
                    new Vector3(0.5f, -0.5f, 0.5f),
                    new Vector3(-0.5f, -0.5f, -0.5f),

                    new Vector3(0.5f, 0.5f, 0.5f),
                    new Vector3(-0.5f, 0.5f, -0.5f),
                    new Vector3(-0.5f, -0.5f, -0.5f),
                    new Vector3(0.5f, -0.5f, 0.5f),

                    new Vector3(-0.5f, 0.5f, 0.5f),
                    new Vector3(0.5f, 0.5f, -0.5f),
                    new Vector3(0.5f, -0.5f, -0.5f),
                    new Vector3(-0.5f, -0.5f, 0.5f),

                    new Vector3(0.5f, 0.5f, -0.5f),
                    new Vector3(-0.5f, 0.5f, 0.5f),
                    new Vector3(-0.5f, -0.5f, 0.5f),
                    new Vector3(0.5f, -0.5f, -0.5f),
                }
            },
        };

        public static ModelType ModelByBlockType(BlockType blockType)
        {
            switch (blockType)
            {
                case BlockType.Sapling:
                    return ModelType.Decor;
                default:
                    return ModelType.Block;
            }
        }
    }
}