using minecrap.graphics;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

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
        public const int height = 96;
        public const int seaLevel = 35;
        private float timeAfterLastUpdate = 0f;
        private const float tick = 1/20f;
        private ulong ticks = 0;
        private PriorityQueue<Block, ulong> updateSchedule;
        private HashSet<Chunk> chunksToUpdate;
        public static HashSet<BlockType> transparentBlocks = new()
        {
            BlockType.Water,
        };

        public static HashSet<BlockType> cutoutBlocks = new()
        {
            BlockType.Glass,
        };

        private Dictionary<Faces, Vector3i> neighborByFace = new()
        {
            [Faces.Front] = new Vector3i(0, 0, 1),
            [Faces.Back] = new Vector3i(0, 0, -1),
            [Faces.Left] = new Vector3i(-1, 0, 0),
            [Faces.Right] = new Vector3i(1, 0, 0),
            [Faces.Top] = new Vector3i(0, 1, 0),
            [Faces.Bottom] = new Vector3i(0, -1, 0),
        };
        
        public World(int seed, ShaderProgram shaderProgram)
        {
            this.seed = seed;
            this.shaderProgram = shaderProgram;
            texture = new Texture("textures");
            instance = this;
            updateSchedule = new PriorityQueue<Block, ulong>();
            chunksToUpdate = new HashSet<Chunk>();
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
                chunk.RenderNormal(shaderProgram);
            }
            foreach (Chunk chunk in chunks)
            {
                chunk.RenderTransparent(shaderProgram);
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
            ScheduleUpdate(block);
            chunksToUpdate.Add(chunk);

            if (block.pos.X % chunkSize == 0 && chunk.chunkPos.X != 0) chunksToUpdate.Add(chunks[chunkPos.X - 1, chunkPos.Y]);
            if (block.pos.X % chunkSize == chunkSize - 1 && chunk.chunkPos.X != worldSize.X - 1) chunksToUpdate.Add(chunks[chunkPos.X + 1, chunkPos.Y]);
            if (block.pos.Z % chunkSize == 0 && chunk.chunkPos.Y != 0) chunksToUpdate.Add(chunks[chunkPos.X, chunkPos.Y - 1]);
            if (block.pos.Z % chunkSize == chunkSize - 1 && chunk.chunkPos.Y != worldSize.Y - 1) chunksToUpdate.Add(chunks[chunkPos.X, chunkPos.Y + 1]);
        }

        public Dictionary<Faces, Block?> GetNeighbors(Block block)
        {
            Dictionary<Faces, Block?> result = new();
            foreach (Faces face in neighborByFace.Keys)
            {
                result[face] = GetNeighbor(block, face);
            }
            return result;
        }

        public Block? GetNeighbor(Block block, Faces face) => GetBlock(block.pos + neighborByFace[face]);
        
        public List<Block> GetSolidBlocksAroundCollider(Collider collider)
        {
            List<Block> blocks = new();
            for (int x = (int)Math.Floor(collider.pos.X - collider.size.X / 2); x <= (int)Math.Ceiling(collider.pos.X + collider.size.X / 2); x++)
            {
                for (int y = (int)Math.Floor(collider.pos.Y - collider.size.Y / 2); y <= (int)Math.Ceiling(collider.pos.Y + collider.size.Y / 2); y++)
                {
                    for (int z = (int)Math.Floor(collider.pos.Z - collider.size.Z / 2); z <= (int)Math.Ceiling(collider.pos.Z + collider.size.Z / 2); z++)
                    {
                        Block? block = GetBlock(new Vector3i(x, y, z));
                        if (block != null && block.blockType != BlockType.Air && block.blockType != BlockType.Water) blocks.Add(block);
                    }
                }
            }
            return blocks;
        }

        public List<Block> GetWaterAroundCollider(Collider collider)
        {
            List<Block> blocks = new();
            for (int x = (int)Math.Floor(collider.pos.X - collider.size.X / 2); x <= (int)Math.Ceiling(collider.pos.X + collider.size.X / 2); x++)
            {
                for (int y = (int)Math.Floor(collider.pos.Y - collider.size.Y / 2); y <= (int)Math.Ceiling(collider.pos.Y + collider.size.Y / 2); y++)
                {
                    for (int z = (int)Math.Floor(collider.pos.Z - collider.size.Z / 2); z <= (int)Math.Ceiling(collider.pos.Z + collider.size.Z / 2); z++)
                    {
                        Block? block = GetBlock(new Vector3i(x, y, z));
                        if (block != null && block.blockType == BlockType.Water) blocks.Add(block);
                    }
                }
            }
            return blocks;
        }

        public void Update(FrameEventArgs e)
        {
            float deltaTime = (float)e.Time;
            timeAfterLastUpdate += deltaTime;
            if (timeAfterLastUpdate >= tick)
            {
                uint ticksToAdd = (uint)(timeAfterLastUpdate / tick);
                ticks += ticksToAdd;
                timeAfterLastUpdate -= ticksToAdd * tick;

                while (updateSchedule.TryPeek(out Block block, out ulong priority))
                {
                    if (ticks >= priority)
                    {
                        UpdateBlock(block);
                        updateSchedule.Dequeue();
                    }
                    else break;
                }
                if (chunksToUpdate.Count > 0)
                {
                    foreach (Chunk chunk in chunksToUpdate)
                    {
                        chunk.UpdateChunk();
                    }
                    chunksToUpdate.Clear();
                }
            }
        }

        private void UpdateBlock(Block block)
        {
            if (block.lastUpdate == ticks) return;
            block.lastUpdate = ticks;
            if (block.blockType == BlockType.Air)
            {
                Dictionary<Faces, Block?> neighbors = GetNeighbors(block);
                bool updated = false;
                foreach (Faces face in neighbors.Keys)
                {
                    if (face != Faces.Bottom && neighbors[face] != null && neighbors[face].blockType == BlockType.Water)
                    {
                        SetBlock(block.pos, BlockType.Water);
                        updated = true;

                        break;
                    }
                }
                if (updated)
                {
                    foreach (Faces face in neighbors.Keys)
                    {
                        if (neighbors[face] != null) ScheduleUpdate(neighbors[face], 5);
                    }
                }
            }
        }

        private void ScheduleUpdate(Block block, uint futureTicks) => updateSchedule.Enqueue(block, ticks + futureTicks);

        private void ScheduleUpdate(Block block) => ScheduleUpdate(block, 0);
    }
}