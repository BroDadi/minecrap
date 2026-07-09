using System.IO.Compression;
using minecrap.graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace minecrap.world
{
    internal class Chunk
    {
        private List<Vector3> chunkVertexes;
        private List<Vector2> chunkTexCoords;
        private List<uint> chunkIndexes;
        private List<Vector3> chunkTransVertexes;
        private List<Vector2> chunkTransTexCoords;
        private List<uint> chunkTransIndexes;
        private uint indexCount;
        private uint transIndexCount;
        public Vector2i chunkPos;
        private Vector3i blockOffset;
        private VAO chunkVAO;
        private VBO chunkVBO;
        private VBO chunkTextureVBO;
        private EBO chunkEBO;
        private VAO chunkTransVAO;
        private VBO chunkTransVBO;
        private VBO chunkTransTextureVBO;
        private EBO chunkTransEBO;
        private int seed;
        private World world;
        private FastNoiseLite noise;
        public Block[,,] chunkBlocks;

        public Chunk(Vector2i chunkPos, int seed)
        {
            this.chunkPos = chunkPos;
            this.seed = seed;
            world = World.instance;
            blockOffset = new Vector3i(chunkPos.X * World.chunkSize, 0, chunkPos.Y *  World.chunkSize);
            chunkVertexes = new List<Vector3>();
            chunkTexCoords = new List<Vector2>();
            chunkIndexes = new List<uint>();
            chunkTransVertexes = new List<Vector3>();
            chunkTransTexCoords = new List<Vector2>();
            chunkTransIndexes = new List<uint>();

            chunkBlocks = new Block[World.chunkSize, World.height, World.chunkSize];
            noise = new();
            noise.SetSeed(seed);
            GenBlocks();
        }

        private float[,] GenHeights()
        {
            float[,] heightMap = new float[World.chunkSize, World.chunkSize];
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFrequency(0.005f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(8);
            noise.SetFractalLacunarity(2f);

            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int z = 0; z < World.chunkSize; z++)
                {
                    float noiseVal = noise.GetNoise((x + blockOffset.X) * 1.3f, (z + blockOffset.Z) * 1.3f);
                    heightMap[x,z] = MathF.Pow(noiseVal * 2, 2) * Math.Sign(noiseVal) / 2f;
                }
            }

            return heightMap;
        }

        private float[,] GenDirt()
        {
            float[,] heightMap = new float[World.chunkSize, World.chunkSize];
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFrequency(0.01f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(8);
            noise.SetFractalLacunarity(1.96f);

            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int z = 0; z < World.chunkSize; z++)
                {
                    heightMap[x,z] = 3 + noise.GetNoise(x + blockOffset.X, z + blockOffset.Z) * 3;
                }
            }

            return heightMap;
        }

        public void GenBlocks()
        {
            float[,] heightMap = GenHeights();
            float[,] dirtMap = GenDirt();
            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int z = 0; z < World.chunkSize; z++)
                {
                    int height = (int)((heightMap[x, z] + 1) * 20f) + 20;
                    int dirtHeight = height - (int)dirtMap[x, z];
                    for (int y = 0; y < World.height; y++)
                    {
                        BlockType type = BlockType.Air;

                        if (y < dirtHeight) type = BlockType.Stone;
                        else if (y < height) type = BlockType.Dirt;
                        else if (y == height)
                        {
                            if (y < World.seaLevel) type = BlockType.Dirt;
                            else type = BlockType.Grass;
                        }
                        else if (y <= World.seaLevel) type = BlockType.Water;

                        chunkBlocks[x, y, z] = new Block(new Vector3i(x + blockOffset.X, y, z + blockOffset.Z), this, type);
                    }
                }
            }
        }

        public void GenFaces()
        {
            chunkVertexes.Clear();
            chunkTexCoords.Clear();
            chunkIndexes.Clear();
            indexCount = 0;
            chunkTransVertexes.Clear();
            chunkTransTexCoords.Clear();
            chunkTransIndexes.Clear();
            transIndexCount = 0;

            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int z = 0; z < World.chunkSize; z++)
                {
                    for (int y = 0; y < World.height; y++)
                    {
                        Block block = chunkBlocks[x, y, z];
                        if (block.blockType == BlockType.Air) continue;

                        bool transparent = World.transparentBlocks.Contains(block.blockType);
                        Vector3i pos = new(x, y, z);
                        
                        int faces = 0;
                        foreach (Faces face in Enum.GetValues<Faces>())
                        {
                            if (CanRender(face, pos))
                            {
                                IntegrateFace(block, face, transparent);
                                faces++;
                            }
                        }

                        AddIndexes(faces, transparent);
                    }
                }
            }
        }

        public void IntegrateFace(Block block, Faces face, bool transparent)
        {
            FaceData data = block.GetFace(face);
            if (transparent)
            {
                chunkTransVertexes.AddRange(data.vertexes);
                chunkTransTexCoords.AddRange(data.texCoords);
            }
            else
            {
                chunkVertexes.AddRange(data.vertexes);
                chunkTexCoords.AddRange(data.texCoords);
            }
        }

        public void AddIndexes(int faces, bool transparent)
        {
            List<uint> list = transparent ? chunkTransIndexes : chunkIndexes;
            uint count = transparent ? transIndexCount : indexCount;
            for (int i = 0; i < faces; i++)
            {
                list.Add(0 + count);
                list.Add(1 + count);
                list.Add(2 + count);
                list.Add(2 + count);
                list.Add(3 + count);
                list.Add(0 + count);
                if (transparent) transIndexCount += 4;
                else indexCount += 4;
                count += 4;
            }
        }

        public void BuildChunk()
        {
            if (chunkVertexes.Count > 0)
            {
                chunkVAO = new VAO();
                chunkVAO.Bind();

                chunkVBO = new VBO(chunkVertexes);
                chunkVBO.Bind();
                chunkVAO.LinkToVAO(0, 3, chunkVBO);

                chunkTextureVBO = new VBO(chunkTexCoords);
                chunkTextureVBO.Bind();
                chunkVAO.LinkToVAO(1, 2, chunkTextureVBO);

                chunkEBO = new EBO(chunkIndexes);
            }

            if (chunkTransVertexes.Count > 0)
            {
                chunkTransVAO = new VAO();
                chunkTransVAO.Bind();
            
                chunkTransVBO = new VBO(chunkTransVertexes);
                chunkTransVBO.Bind();
                chunkTransVAO.LinkToVAO(0, 3, chunkTransVBO);

                chunkTransTextureVBO = new VBO(chunkTransTexCoords);
                chunkTransTextureVBO.Bind();
                chunkTransVAO.LinkToVAO(1, 2, chunkTransTextureVBO);

                chunkTransEBO = new EBO(chunkTransIndexes);
            }
        }

        public void RenderNormal(ShaderProgram shaderProgram)
        {
            shaderProgram.Bind();
            chunkVAO?.Bind();
            chunkEBO?.Bind();
            GL.DrawElements(PrimitiveType.Triangles, chunkIndexes.Count, DrawElementsType.UnsignedInt, 0);
        }

        public void RenderTransparent(ShaderProgram shaderProgram)
        {
            shaderProgram.Bind();
            chunkTransVAO?.Bind();
            chunkTransEBO?.Bind();
            GL.DrawElements(PrimitiveType.Triangles, chunkTransIndexes.Count, DrawElementsType.UnsignedInt, 0);
        }

        public void Delete()
        {
            chunkVAO?.Delete();
            chunkVBO?.Delete();
            chunkTextureVBO?.Delete();
            chunkEBO?.Delete();
            chunkTransVAO?.Delete();
            chunkTransVBO?.Delete();
            chunkTransTextureVBO?.Delete();
            chunkTransEBO?.Delete();
        }

        private bool CanRender(Faces face, Vector3i pos)
        {
            Block? block = World.instance.GetBlock(pos + blockOffset);
            Block? neighbor = World.instance.GetNeighbor(block, face);
            return block != null && block.blockType != BlockType.Air &&
                    (neighbor == null || neighbor.blockType == BlockType.Air ||
                    ((World.transparentBlocks.Contains(neighbor.blockType) || World.cutoutBlocks.Contains(neighbor.blockType)) && block.blockType != neighbor.blockType));
        }

        public void UpdateChunk()
        {
            Delete();
            GenFaces();
            BuildChunk();
        }
    }
}