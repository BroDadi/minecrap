using minecrap.graphics;
using OpenTK.Mathematics;

namespace minecrap.world
{
    internal class World
    {
        public Texture texture;
        public Chunk[,] chunks;
        private int seed;
        private ShaderProgram shaderProgram;
        public static World instance;
        public Vector2i worldSize;
        public const int chunkSize = 16;
        public const int height = 64;
        
        public World(int seed, ShaderProgram shaderProgram)
        {
            this.seed = seed;
            this.shaderProgram = shaderProgram;
            texture = new Texture("textures");
            instance = this;
        }

        public void GenerateWorld(Vector2i worldSize)
        {
            chunks = new Chunk[worldSize.X, worldSize.Y];
            this.worldSize = worldSize;
            for (int x = 0; x < worldSize.X; x++)
            {
                for (int z = 0; z < worldSize.Y; z++)
                {
                    chunks[x, z] = new Chunk(new Vector2i(x, z), seed);
                }
            }

            for (int x = 0; x < worldSize.X; x++)
            {
                for (int z = 0; z < worldSize.Y; z++)
                {
                    chunks[x, z].GenFaces();
                    chunks[x, z].BuildChunk();
                }
            }
        }

        public void RenderWorld()
        {
            texture.Bind();
            foreach (Chunk chunk in chunks)
            {
                chunk.Render(shaderProgram);
            }
        }

        public Block? GetBlock(Vector3i pos)
        {
            if (pos.X < 0 || pos.X >= worldSize.X * chunkSize || pos.Z < 0 || pos.Z >= worldSize.Y * chunkSize || pos.Y < 0 || pos.Y >= height) return null;
            return chunks[pos.X / chunkSize, pos.Z / chunkSize].chunkBlocks[pos.X % chunkSize, pos.Y, pos.Z % chunkSize];
        }

        public void SetBlock(Vector3i pos, BlockType blockType)
        {
            if (pos.X < 0 || pos.X >= worldSize.X * chunkSize || pos.Z < 0 || pos.Z >= worldSize.Y * chunkSize || pos.Y < 0 || pos.Y >= height) return;

            Vector2i chunkPos = new(pos.X / chunkSize, pos.Z / chunkSize);
            Chunk chunk = chunks[chunkPos.X, chunkPos.Y];
            Block block = chunk.chunkBlocks[pos.X % chunkSize, pos.Y, pos.Z % chunkSize];

            block.SetBlockType(blockType);
            chunk.UpdateChunk();

            if (block.pos.X % chunkSize == 0 && chunk.chunkPos.X != 0) chunks[chunkPos.X - 1, chunkPos.Y].UpdateChunk();
            if (block.pos.X % chunkSize == chunkSize - 1 && chunk.chunkPos.X != worldSize.X - 1) chunks[chunkPos.X + 1, chunkPos.Y].UpdateChunk();
            if (block.pos.Z % chunkSize == 0 && chunk.chunkPos.Y != 0) chunks[chunkPos.X, chunkPos.Y - 1].UpdateChunk();
            if (block.pos.Z % chunkSize == chunkSize - 1 && chunk.chunkPos.Y != worldSize.Y - 1) chunks[chunkPos.X, chunkPos.Y + 1].UpdateChunk();
        }

        public List<Block> GetBlocksAroundCollider(Collider collider)
        {
            List<Block> blocks = new();
            for (int x = (int)Math.Floor(collider.pos.X - collider.size.X / 2); x <= (int)Math.Ceiling(collider.pos.X + collider.size.X / 2); x++)
            {
                for (int y = (int)Math.Floor(collider.pos.Y - collider.size.Y / 2); y <= (int)Math.Ceiling(collider.pos.Y + collider.size.Y / 2); y++)
                {
                    for (int z = (int)Math.Floor(collider.pos.Z - collider.size.Z / 2); z <= (int)Math.Ceiling(collider.pos.Z + collider.size.Z / 2); z++)
                    {
                        Block block = GetBlock(new Vector3i(x, y, z));
                        if (block != null && block.blockType != BlockType.Air) blocks.Add(block);
                    }
                }
            }
            return blocks;
        }
    }
}