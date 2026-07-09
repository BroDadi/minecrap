using OpenTK.Mathematics;

namespace minecrap.world
{
    internal class Block
    {
        public Vector3i pos;
        public BlockType blockType;
        private Dictionary<Faces, List<Vector2>> textures;
        public Collider collider;
        public Chunk chunk;
        public ulong lastUpdate;

        public Block(Vector3i pos, Chunk chunk, BlockType blockType = BlockType.Air)
        {
            this.pos = pos;
            this.chunk = chunk;
            lastUpdate = 0;
            SetBlockType(blockType);
        }

        public void SetBlockType(BlockType blockType)
        {
            this.blockType = blockType;
            if (blockType != BlockType.Air) textures = TextureData.blockTypeTextures[blockType];
        }

        private List<Vector3> AddTransformedVertexes(List<Vector3> vertexes)
        {
            List<Vector3> result = new();
            foreach (Vector3 vertex in vertexes)
            {
                result.Add(vertex + pos);
            }
            return result;
        }

        public FaceData GetFace(Faces face)
        {
            FaceData faceData = new()
            {
                vertexes = AddTransformedVertexes(RawFaceData.rawVertexData[face]),
                texCoords = textures[face],
            };
            return faceData;
        }

        public Collider GetCollider()
        {
            return new Collider(pos, Vector3.One);
        }
    }
}