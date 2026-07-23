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
        public const int seaLevel = 32;
        private float timeAfterLastUpdate = 0f;
        private const float tick = 1/20f;
        private const int randomTickBlocks = 3;
        private ulong ticks = 0;
        private PriorityQueue<Block, ulong> updateSchedule;
        private HashSet<Chunk> chunksToUpdate;
        public static FastNoiseLite heightNoise;
        public static FastNoiseLite dirtNoise;
        public static FastNoiseLite sandNoise;
        public static FastNoiseLite caveNoise;
        public static FastNoiseLite caveNoise2;

        private static Dictionary<Faces, Vector3i> neighborByFace = new()
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
            this.shaderProgram = shaderProgram;
            texture = new Texture("textures");
            instance = this;
            updateSchedule = new PriorityQueue<Block, ulong>();
            chunksToUpdate = new HashSet<Chunk>();

            heightNoise = new(seed);
            heightNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            heightNoise.SetFrequency(0.005f);
            heightNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            heightNoise.SetFractalOctaves(8);
            heightNoise.SetFractalLacunarity(2f);
            heightNoise.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            heightNoise.SetDomainWarpAmp(0.5f);

            dirtNoise = new(seed);
            dirtNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            dirtNoise.SetFrequency(0.01f);
            dirtNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            dirtNoise.SetFractalOctaves(8);
            dirtNoise.SetFractalLacunarity(1.96f);

            sandNoise = new(seed);
            sandNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            sandNoise.SetFrequency(0.01f);
            sandNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            sandNoise.SetFractalOctaves(4);
            sandNoise.SetFractalLacunarity(2f);

            caveNoise = new(seed);
            caveNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            caveNoise.SetFrequency(0.01f);
            caveNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            caveNoise.SetFractalOctaves(3);
            caveNoise.SetFractalLacunarity(2f);

            caveNoise2 = new(seed + 1);
            caveNoise2.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            caveNoise2.SetFrequency(0.01f);
            caveNoise2.SetFractalType(FastNoiseLite.FractalType.FBm);
            caveNoise2.SetFractalOctaves(3);
            caveNoise2.SetFractalLacunarity(2f);
        }

        public void GenerateWorld(Vector2i worldSize)
        {
            chunks = new Chunk[worldSize.X, worldSize.Y];
            this.worldSize = worldSize;
            for (int x = 0; x < worldSize.X; x++)
            {
                for (int z = 0; z < worldSize.Y; z++)
                {
                    chunks[x, z] = new Chunk(new Vector2i(x, z));
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

        public byte GetLighting(Vector3i pos)
        {
            if (pos.X < 0 || pos.X >= worldSize.X * chunkSize || pos.Z < 0 || pos.Z >= worldSize.Y * chunkSize || pos.Y < 0 || pos.Y >= height) return 15;
            return chunks[pos.X / chunkSize, pos.Z / chunkSize].chunkLighting[pos.X % chunkSize, pos.Y, pos.Z % chunkSize];
        }

        public void SetBlock(Vector3i pos, BlockType blockType)
        {
            if (pos.X < 0 || pos.X >= worldSize.X * chunkSize || pos.Z < 0 || pos.Z >= worldSize.Y * chunkSize || pos.Y < 0 || pos.Y >= height) return;

            Vector2i chunkPos = new(pos.X / chunkSize, pos.Z / chunkSize);
            Chunk chunk = chunks[chunkPos.X, chunkPos.Y];
            Block block = chunk.chunkBlocks[pos.X % chunkSize, pos.Y, pos.Z % chunkSize];

            block.blockType = blockType;
            ScheduleUpdate(block);
            chunksToUpdate.Add(chunk);

            if (pos.X % chunkSize == 0 && chunk.chunkPos.X != 0) chunksToUpdate.Add(chunks[chunkPos.X - 1, chunkPos.Y]);
            if (pos.X % chunkSize == chunkSize - 1 && chunk.chunkPos.X != worldSize.X - 1) chunksToUpdate.Add(chunks[chunkPos.X + 1, chunkPos.Y]);
            if (pos.Z % chunkSize == 0 && chunk.chunkPos.Y != 0) chunksToUpdate.Add(chunks[chunkPos.X, chunkPos.Y - 1]);
            if (pos.Z % chunkSize == chunkSize - 1 && chunk.chunkPos.Y != worldSize.Y - 1) chunksToUpdate.Add(chunks[chunkPos.X, chunkPos.Y + 1]);
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

        public static Vector3i GetNeighborPos(Vector3i block, Faces face)
        {
            if (face == Faces.Inside) return block;
            else return block + neighborByFace[face];
        }
        public Block? GetNeighbor(Block block, Faces face) => GetBlock(GetNeighborPos(block.pos, face));
        
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
                        if (block != null && !Game.noColliderBlocks.Contains(block.blockType)) blocks.Add(block);
                    }
                }
            }
            return blocks;
        }

        public List<Block> GetBlocksInZone(Vector3i center, Vector3i bounds)
        {
            List<Block> blocks = new();
            for (int x = center.X - (bounds.X - 1) / 2; x <= center.X + bounds.X / 2; x++)
            {
                for (int y = center.Y - (bounds.Y - 1) / 2; y <= center.Y + bounds.Y / 2; y++)
                {
                    for (int z = center.Z - (bounds.Z - 1) / 2; z <= center.Z + bounds.Z / 2; z++)
                    {
                        Block? block = GetBlock(new Vector3i(x, y, z));
                        if (block != null) blocks.Add(block);
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
                        block.Update();
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

                foreach (Chunk chunk in chunks)
                {
                    chunk.DoRandomTicks(randomTickBlocks);
                }
            }
        }

        public void ScheduleUpdate(Block block, uint futureTicks) => updateSchedule.Enqueue(block, ticks + futureTicks);

        public void ScheduleUpdate(Block block) => ScheduleUpdate(block, 0);

        public ulong GetTicks() => ticks;

        public Block? GetHighestBlock(Vector2i pos)
        {
            for (Vector3i blockPos = new(pos.X, height - 1, pos.Y); blockPos.Y >= 0; blockPos.Y--)
            {
                Block? block = GetBlock(blockPos);
                if (block == null) return null;
                if (block.blockType != BlockType.Air && block.blockType != BlockType.Water) return block;
            }
            return null;
        }
    }
}