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
        public const int height = 128;
        public const int seaLevel = 64;
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
        public static FastNoiseLite coalNoise;
        public static FastNoiseLite ironNoise;
        public static FastNoiseLite goldNoise;
        public static FastNoiseLite diamondNoise;

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
            this.seed = seed;
            ticks = 0;
            updateSchedule?.Clear();
            chunksToUpdate?.Clear();
            timeAfterLastUpdate = 0;

            texture = Game.blocks;
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

            coalNoise = new(seed);
            coalNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            coalNoise.SetFrequency(0.05f);
            coalNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            coalNoise.SetFractalOctaves(2);
            coalNoise.SetFractalLacunarity(2f);

            ironNoise = new(seed);
            ironNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            ironNoise.SetFrequency(0.06f);
            ironNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            ironNoise.SetFractalOctaves(2);
            ironNoise.SetFractalLacunarity(2f);

            goldNoise = new(seed);
            goldNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            goldNoise.SetFrequency(0.066f);
            goldNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            goldNoise.SetFractalOctaves(2);
            goldNoise.SetFractalLacunarity(2f);

            diamondNoise = new(seed);
            diamondNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            diamondNoise.SetFrequency(0.075f);
            diamondNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            diamondNoise.SetFractalOctaves(2);
            diamondNoise.SetFractalLacunarity(2f);
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

        public void SaveWorld(string path)
        {
            string[] data =
            {
                $"seed: {seed}",
                $"ticks: {ticks}",
                $"timeAfterLastUpdate: {timeAfterLastUpdate}",
                $"worldSizeX: {worldSize.X}",
                $"worldSizeY: {worldSize.Y}",
            };
            File.WriteAllLines(Path.Combine(path, "data.txt"), data);

            foreach (Chunk chunk in chunks)
            {
                string chunkPath = Path.Combine(path, $"{chunk.chunkPos.X} {chunk.chunkPos.Y}.mcrap");
                chunk.SaveBlocks(chunkPath);
            }
        }

        public void LoadWorld(string path)
        {
            if (File.Exists(Path.Join(path, "data.txt")))
            {
                string[] dataFile = File.ReadAllLines(Path.Join(path, "data.txt"));
                Dictionary<string, string> data = new();

                foreach (string str in dataFile)
                {
                    string[] a = str.Split(": ");
                    if (a.Length >= 2) data[a[0]] = a[1];
                }

                seed = Convert.ToInt32(data["seed"]);
                ticks = Convert.ToUInt64(data["ticks"]);
                timeAfterLastUpdate = Convert.ToSingle(data["timeAfterLastUpdate"]);
                worldSize = new Vector2i(Convert.ToInt32(data["worldSizeX"]), Convert.ToInt32(data["worldSizeY"]));

                for (int x = 0; x < worldSize.X; x++)
                {
                    for (int z = 0; z < worldSize.Y; z++)
                    {
                        chunks[x, z] = new Chunk(new Vector2i(x, z), Path.Join(path, $"{x} {z}.mcrap"));
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
        }

        public List<Chunk> GetChunksAroundPlayer(int radius)
        {
            Vector2i centerChunk = new((int)Player.instance.pos.X / 16, (int)Player.instance.pos.Z / 16);
            List<Chunk> result = new();

            for (int x = -radius; x <= radius; x++)
            {
                int chunkX = centerChunk.X + x;
                for (int z = -radius; z <= radius; z++)
                {
                    int chunkZ = centerChunk.Y + z;
                    if (chunkX >= 0 && chunkX < worldSize.X && chunkZ >= 0 && chunkZ < worldSize.Y && Math.Abs(x * x) + Math.Abs(z * z) <= radius * radius)
                    {
                        result.Add(chunks[chunkX, chunkZ]);
                    }
                }
            }
            return result;
        }

        public void RenderChunks(List<Chunk> chunksToRender)
        {
            texture.Bind();
            foreach (Chunk chunk in chunksToRender)
            {
                chunk.RenderNormal(shaderProgram);
            }
            foreach (Chunk chunk in chunksToRender)
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

        public Dictionary<Faces, Block> GetNeighbors(Block block)
        {
            Dictionary<Faces, Block> result = new();
            foreach (Faces face in neighborByFace.Keys)
            {
                Block? neighbor = GetNeighbor(block, face);
                if (neighbor != null) result[face] = (Block)neighbor;
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
                        if (block != null && !Game.nonSolidBlocks.Contains(block.blockType)) blocks.Add(block);
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
            if (Game.instance.paused) return;
            
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

                foreach (Chunk chunk in GetChunksAroundPlayer(8))
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