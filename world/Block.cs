using OpenTK.Mathematics;

namespace minecrap.world
{
    internal class Block
    {
        public Vector3i pos;
        public BlockType blockType;
        private Dictionary<Faces, List<Vector2>> textures;
        public ulong lastUpdate;

        public Block(Vector3i pos, BlockType blockType = BlockType.Air)
        {
            this.pos = pos;
            lastUpdate = 0;
            SetBlockType(blockType);
        }

        public void SetBlockType(BlockType blockType)
        {
            this.blockType = blockType;
            if (blockType != BlockType.Air && !ModelData.modelByBlockType.ContainsKey(blockType)) textures = TextureData.blockTypeTextures[blockType];
        }

        public List<Vector3> AddTransformedVertexes(List<Vector3> vertexes)
        {
            List<Vector3> result = new();
            foreach (Vector3 vertex in vertexes)
            {
                result.Add(vertex + pos);
            }
            return result;
        }

        private List<Color4> GetColors(Faces face)
        {
            List<Color4> result = new();
            Color4 color = World.instance.GetColor(World.instance.GetNeighborPos(pos, face));
            for (int i = 0; i < 4; i++)
            {
                result.Add(color);
            }
            return result;
        }

        public FaceData GetFace(Faces face)
        {
            FaceData faceData = new()
            {
                vertexes = AddTransformedVertexes(RawFaceData.rawVertexData[face]),
                texCoords = textures[face],
                colors = GetColors(face),
                twoSided = blockType == BlockType.Water,
            };
            return faceData;
        }

        public List<FaceData> GetModel()
        {
            List<FaceData> faces = new();

            return faces;
        }

        public Collider GetCollider()
        {
            return new Collider(pos, Vector3.One);
        }
    }
}