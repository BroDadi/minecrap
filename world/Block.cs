using minecrap.graphics;
using OpenTK.Mathematics;

namespace minecrap.world
{
    internal class Block
    {
        public Vector3i pos;
        public BlockType blockType;
        public ulong lastUpdate;

        public Block(Vector3i pos, BlockType blockType = BlockType.Air)
        {
            this.pos = pos;
            this.blockType = blockType;
            lastUpdate = 0;
        }

        public Vector3[] AddTransformedVertexes(Vector3[] vertexes)
        {
            Vector3[] result = new Vector3[vertexes.Length];
            for (int i = 0; i < vertexes.Length; i++)
            {
                result[i] = vertexes[i] + pos;
            }
            return result;
        }

        private Color[] GetColors(Faces face, int length)
        {
            byte light = World.instance.GetLighting(World.GetNeighborPos(pos, face));
            Color color = Color.FromLighting(light) * Game.shadeSides[face];

            Color[] result = new Color[length];
            Array.Fill(result, color);
            return result;
        }

        public FaceData GetFace(Faces face)
        {
            Vector3[] vertexArr = AddTransformedVertexes(ModelData.vertexesByModelType[ModelData.ModelByBlockType(blockType)][face]);
            FaceData faceData = new()
            {
                vertexes = vertexArr,
                texCoords = TextureData.blockTypeTextures[blockType][face],
                colors = GetColors(face, vertexArr.Length),
                twoSided = Game.doubleSidedBlocks.Contains(blockType),
            };
            return faceData;
        }

        public Collider GetCollider() => new Collider(pos, Vector3.One);

        public void Update()
        {
            ulong ticks = World.instance.GetTicks();
            if (lastUpdate == ticks) return;
            lastUpdate = ticks;

            switch (blockType)
            {
                case BlockType.Air:
                    Dictionary<Faces, Block?> neighbors = World.instance.GetNeighbors(this);
                    bool updated = false;
                    foreach (Faces face in neighbors.Keys)
                    {
                        Block? neighbor = neighbors[face];
                        if (face != Faces.Bottom && neighbor != null && neighbor.blockType == BlockType.Water)
                        {
                            World.instance.SetBlock(pos, BlockType.Water);
                            updated = true;

                            break;
                        }
                    }
                    if (updated)
                    {
                        foreach (Faces face in neighbors.Keys)
                        {
                            Block? neighbor = neighbors[face];
                            if (neighbor != null && neighbor.blockType == BlockType.Air) World.instance.ScheduleUpdate(neighbor, 5);
                        }
                    }
                    break;
            }
        }

        public void RandomUpdate()
        {
            switch(blockType)
            {
                case BlockType.Dirt:
                    if (World.instance.GetLighting(World.GetNeighborPos(pos, Faces.Top)) != 15) return;

                    List<Block> possibleGrass = World.instance.GetBlocksInZone(pos + new Vector3i(0, 1, 0), new Vector3i(3, 5, 3));
                    foreach (Block block in possibleGrass)
                    {
                        if (block.blockType == BlockType.Grass) World.instance.SetBlock(pos, BlockType.Grass);
                    }
                    break;
                case BlockType.Grass:
                    if (World.instance.GetLighting(World.GetNeighborPos(pos, Faces.Top)) != 15) World.instance.SetBlock(pos, BlockType.Dirt);
                    break;
                case BlockType.Sapling:
                    Block? standingOn = World.instance.GetNeighbor(this, Faces.Bottom);
                    if (standingOn == null || standingOn.blockType != BlockType.Dirt && standingOn.blockType != BlockType.Grass) World.instance.SetBlock(pos, BlockType.Air);
                    break;
                case BlockType.Air:
                    Update();
                    break;
            }
        }
    }
}