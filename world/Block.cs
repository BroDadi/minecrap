using OpenTK.Mathematics;

namespace minecrap.world
{
    internal class Block
    {
        public Vector3 pos;
        public BlockType blockType;
        private Dictionary<Faces, List<Vector2>> textures;
        public Collider collider;
        public Chunk chunk;

        public Block(Vector3 pos, Chunk chunk, BlockType blockType = BlockType.Air)
        {
            this.pos = pos;
            this.chunk = chunk;
            collider = new(pos, Vector3.One);
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
    }
}