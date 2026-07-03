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
        public Vector2i chunkPos;
        private Vector2i blockOffset;
        private uint indexCount;
        private VAO chunkVAO;
        private VBO chunkVBO;
        private VBO chunkTextureVBO;
        private EBO chunkEBO;
        private int seed;
        private World world;
        public Block[,,] chunkBlocks;

        public Chunk(Vector2i chunkPos, int seed)
        {
            this.chunkPos = chunkPos;
            this.seed = seed;
            world = World.instance;
            blockOffset = new Vector2i(chunkPos.X * World.chunkSize, chunkPos.Y *  World.chunkSize);
            chunkVertexes = new List<Vector3>();
            chunkTexCoords = new List<Vector2>();
            chunkIndexes = new List<uint>();

            chunkBlocks = new Block[World.chunkSize, World.height, World.chunkSize];
            float[,] heightMap = GenChunk();
            GenBlocks(heightMap);
        }

        public float[,] GenChunk()
        {
            float[,] heightMap = new float[World.chunkSize, World.chunkSize];

            SimplexNoise.Noise.Seed = seed;
            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int z = 0; z < World.chunkSize; z++)
                {
                    heightMap[x,z] = SimplexNoise.Noise.CalcPixel2D(x + blockOffset.X, z + blockOffset.Y, 0.01f);
                }
            }

            return heightMap;
        }

        public void GenBlocks(float[,] heightMap)
        {
            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int z = 0; z < World.chunkSize; z++)
                {
                    int colHeight = (int)(heightMap[x, z] / 10);
                    for (int y = 0; y < World.height; y++)
                    {
                        BlockType type = BlockType.Air;

                        if (y < colHeight) type = BlockType.Dirt;
                        else if (y == colHeight) type = BlockType.Grass;

                        chunkBlocks[x, y, z] = new Block(new Vector3(x + blockOffset.X, y, z + blockOffset.Y), this, type);
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

            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int z = 0; z < World.chunkSize; z++)
                {
                    for (int y = 0; y < World.height; y++)
                    {
                        Block block = chunkBlocks[x, y, z];
                        if (block.blockType == BlockType.Air) continue;
                        Vector3i pos = new(x, y, z);
                        
                        int faces = 0;
                        foreach (Faces face in Enum.GetValues<Faces>())
                        {
                            if (CanRender(face, pos))
                            {
                                IntegrateFace(block, face);
                                faces++;
                            }
                        }

                        AddIndexes(faces);
                    }
                }
            }
        }

        public void IntegrateFace(Block block, Faces face)
        {
            FaceData data = block.GetFace(face);
            chunkVertexes.AddRange(data.vertexes);
            chunkTexCoords.AddRange(data.texCoords);
        }

        public void AddIndexes(int faces)
        {
            for (int i = 0; i < faces; i++)
            {
                chunkIndexes.Add(0 + indexCount);
                chunkIndexes.Add(1 + indexCount);
                chunkIndexes.Add(2 + indexCount);
                chunkIndexes.Add(2 + indexCount);
                chunkIndexes.Add(3 + indexCount);
                chunkIndexes.Add(0 + indexCount);
                indexCount += 4;
            }
        }

        public void BuildChunk()
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

        public void Render(ShaderProgram shaderProgram)
        {
            shaderProgram.Bind();
            chunkVAO.Bind();
            chunkEBO.Bind();
            GL.DrawElements(PrimitiveType.Triangles, chunkIndexes.Count, DrawElementsType.UnsignedInt, 0);
        }

        public void Delete()
        {
            chunkVAO.Delete();
            chunkVBO.Delete();
            chunkTextureVBO.Delete();
            chunkEBO.Delete();
        }

        private bool CanRender(Faces face, Vector3i pos)
        {
            switch (face)
            {
                case Faces.Left:
                    if (pos.X != 0) return chunkBlocks[pos.X - 1, pos.Y, pos.Z].blockType == BlockType.Air;
                    else if (chunkPos.X == 0) return true;
                    else return world.chunks[chunkPos.X - 1, chunkPos.Y].chunkBlocks[World.chunkSize - 1, pos.Y, pos.Z].blockType == BlockType.Air;
                case Faces.Right:
                    if (pos.X != World.chunkSize - 1) return chunkBlocks[pos.X + 1, pos.Y, pos.Z].blockType == BlockType.Air;
                    else if (chunkPos.X == world.worldSize.X - 1) return true;
                    else return world.chunks[chunkPos.X + 1, chunkPos.Y].chunkBlocks[0, pos.Y, pos.Z].blockType == BlockType.Air;
                case Faces.Front:
                    if (pos.Z != World.chunkSize - 1) return chunkBlocks[pos.X, pos.Y, pos.Z + 1].blockType == BlockType.Air;
                    else if (chunkPos.Y == world.worldSize.Y - 1) return true;
                    else return world.chunks[chunkPos.X, chunkPos.Y + 1].chunkBlocks[pos.X, pos.Y, 0].blockType == BlockType.Air;
                case Faces.Back:
                    if (pos.Z != 0) return chunkBlocks[pos.X, pos.Y, pos.Z - 1].blockType == BlockType.Air;
                    else if (chunkPos.Y == 0) return true;
                    else return world.chunks[chunkPos.X, chunkPos.Y - 1].chunkBlocks[pos.X, pos.Y, World.chunkSize - 1].blockType == BlockType.Air;
                case Faces.Bottom:
                    if (pos.Y != 0) return chunkBlocks[pos.X, pos.Y - 1, pos.Z].blockType == BlockType.Air;
                    else return true;
                case Faces.Top:
                    if (pos.Y != World.height - 1) return chunkBlocks[pos.X, pos.Y + 1, pos.Z].blockType == BlockType.Air;
                    else return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(face), face, null);
            }
        }

        public void UpdateChunk()
        {
            Delete();
            GenFaces();
            BuildChunk();
        }
    }
}