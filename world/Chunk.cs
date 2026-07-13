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
        private List<Color4> chunkColors;
        private List<uint> chunkIndexes;
        private List<Vector3> chunkTransVertexes;
        private List<Vector2> chunkTransTexCoords;
        private List<Color4> chunkTransColors;
        private List<uint> chunkTransIndexes;
        private uint indexCount;
        private uint transIndexCount;
        public Vector2i chunkPos;
        private Vector3i blockOffset;
        private VAO chunkVAO;
        private VBO chunkVBO;
        private VBO chunkTextureVBO;
        private VBO chunkColorVBO;
        private EBO chunkEBO;
        private VAO chunkTransVAO;
        private VBO chunkTransVBO;
        private VBO chunkTransTextureVBO;
        private EBO chunkTransEBO;
        private VBO chunkTransColorVBO;
        public Block[,,] chunkBlocks;
        public Color4[,,] chunkLighting;

        public Chunk(Vector2i chunkPos)
        {
            this.chunkPos = chunkPos;
            blockOffset = new Vector3i(chunkPos.X * World.chunkSize, 0, chunkPos.Y *  World.chunkSize);
            chunkVertexes = new List<Vector3>();
            chunkTexCoords = new List<Vector2>();
            chunkColors = new List<Color4>();
            chunkIndexes = new List<uint>();
            chunkTransVertexes = new List<Vector3>();
            chunkTransTexCoords = new List<Vector2>();
            chunkTransColors = new List<Color4>();
            chunkTransIndexes = new List<uint>();

            chunkBlocks = new Block[World.chunkSize, World.height, World.chunkSize];
            chunkLighting = new Color4[World.chunkSize, World.height, World.chunkSize];
            GenBlocks();
            GenLighting();
        }

        private float[,] GenHeights()
        {
            float[,] heightMap = new float[World.chunkSize, World.chunkSize];

            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int z = 0; z < World.chunkSize; z++)
                {
                    float noiseVal = World.heightNoise.GetNoise((x + blockOffset.X) * 1.3f, (z + blockOffset.Z) * 1.3f);
                    heightMap[x,z] = MathF.Pow(noiseVal * 2, 2) * Math.Sign(noiseVal) / 4f;
                }
            }

            return heightMap;
        }

        private float[,] GenDirt()
        {
            float[,] heightMap = new float[World.chunkSize, World.chunkSize];

            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int z = 0; z < World.chunkSize; z++)
                {
                    heightMap[x,z] = 3 + World.dirtNoise.GetNoise(x + blockOffset.X, z + blockOffset.Z) * 3;
                }
            }

            return heightMap;
        }

        private float[,] GenSandMap()
        {
            float[,] sandMap = new float[World.chunkSize, World.chunkSize];

            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int z = 0; z < World.chunkSize; z++)
                {
                    sandMap[x, z] = World.sandNoise.GetNoise(x + blockOffset.X, z + blockOffset.Z);
                }
            }

            return sandMap;
        }

        private float[,,] GenCaveMap(FastNoiseLite noise)
        {
            float[,,] caveMap = new float[World.chunkSize, World.height, World.chunkSize];

            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int y = 0; y < World.height; y++)
                {
                    for (int z = 0; z < World.chunkSize; z++)
                    {
                        caveMap[x, y, z] = noise.GetNoise(x + blockOffset.X, y, z + blockOffset.Z);
                    }
                }
            }

            return caveMap;
        }

        public void GenBlocks()
        {
            float[,] heightMap = GenHeights();
            float[,] dirtMap = GenDirt();
            float[,] sandMap = GenSandMap();
            float[,,] caveMap = GenCaveMap(World.caveNoise);
            float[,,] caveMap2 = GenCaveMap(World.caveNoise2);
            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int z = 0; z < World.chunkSize; z++)
                {
                    int height = (int)((heightMap[x, z] + 1) * 40);
                    int dirtHeight = height - (int)dirtMap[x, z];
                    for (int y = 0; y < World.height; y++)
                    {
                        BlockType type = BlockType.Air;

                        if (y < dirtHeight)
                        {
                            if (Math.Abs(caveMap[x, y, z]) < 0.05f && Math.Abs(caveMap2[x, y, z]) < 0.05f) type = BlockType.Air;
                            else type = BlockType.Stone;
                        }
                        else if (y < height)
                        {
                            if (dirtHeight < World.seaLevel && y < World.seaLevel + 2)
                            {
                                if (sandMap[x, z] > -0.5f) type = BlockType.Sand;
                                else type = BlockType.Dirt;
                            }
                            else type = BlockType.Dirt;
                        }
                        else if (y == height)
                        {
                            if (y < World.seaLevel + 2)
                            {
                                if (sandMap[x, z] > -0.5f) type = BlockType.Sand;
                                else type = BlockType.Dirt;
                            }
                            else type = BlockType.Grass;
                        }
                        else if (y <= World.seaLevel) type = BlockType.Water;

                        chunkBlocks[x, y, z] = new Block(new Vector3i(x + blockOffset.X, y, z + blockOffset.Z), type);
                    }
                }
            }
        }

        public void GenLighting()
        {
            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int z = 0; z < World.chunkSize; z++)
                {
                    bool lit = true;
                    for (int y = World.height - 1; y >= 0; y--)
                    {
                        BlockType type = chunkBlocks[x, y, z].blockType;
                        if (type != BlockType.Air && !Game.transparentBlocks.Contains(type) && !Game.cutoutBlocks.Contains(type)) lit = false;
                        if (lit) chunkLighting[x, y, z] = new Color4(255, 255, 255, 255);
                        else chunkLighting[x, y, z] = new Color4(128, 128, 128, 255);
                    }
                }
            }
        }

        public void GenFaces()
        {
            chunkVertexes.Clear();
            chunkTexCoords.Clear();
            chunkIndexes.Clear();
            chunkColors.Clear();
            indexCount = 0;
            chunkTransVertexes.Clear();
            chunkTransTexCoords.Clear();
            chunkTransIndexes.Clear();
            chunkTransColors.Clear();
            transIndexCount = 0;

            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int z = 0; z < World.chunkSize; z++)
                {
                    for (int y = 0; y < World.height; y++)
                    {
                        Block block = chunkBlocks[x, y, z];
                        if (block.blockType == BlockType.Air) continue;

                        bool transparent = Game.transparentBlocks.Contains(block.blockType);
                        Vector3i pos = new(x, y, z);

                        if (ModelData.modelByBlockType.ContainsKey(block.blockType))
                        {
                            List<Vector3> vertexes = block.AddTransformedVertexes(ModelData.vertexesByModelType[ModelData.modelByBlockType[block.blockType]]);
                            List<Vector2> texCoords = TextureData.customTextures[block.blockType];
                            Color4 color = World.instance.GetColor(block.pos);
                            for (int i = 0; i < vertexes.Count; i += 4)
                            {
                                chunkVertexes.AddRange(vertexes[i..(i + 4)]);
                                chunkTexCoords.AddRange(texCoords[i..(i + 4)]);
                                chunkColors.Add(color);
                                chunkColors.Add(color);
                                chunkColors.Add(color);
                                chunkColors.Add(color);
                                if (Game.doubleSidedBlocks.Contains(block.blockType)) AddDoubleIndexes(transparent);
                                else AddSingleIndexes(transparent);
                            }
                        }
                        else
                        {
                            foreach (Faces face in Game.allFaces)
                            {
                                if (CanRender(face, pos))
                                {
                                    IntegrateFace(block, face, transparent);
                                }
                            }
                        }
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
                chunkTransColors.AddRange(data.colors);
            }
            else
            {
                chunkVertexes.AddRange(data.vertexes);
                chunkTexCoords.AddRange(data.texCoords);
                chunkColors.AddRange(data.colors);
            }
            if (data.twoSided) AddDoubleIndexes(transparent);
            else AddSingleIndexes(transparent);
        }

        public void AddSingleIndexes(bool transparent)
        {
            List<uint> list = transparent ? chunkTransIndexes : chunkIndexes;
            uint count = transparent ? transIndexCount : indexCount;
            list.Add(0 + count);
            list.Add(1 + count);
            list.Add(2 + count);
            list.Add(2 + count);
            list.Add(3 + count);
            list.Add(0 + count);
            if (transparent) transIndexCount += 4;
            else indexCount += 4;
        }

        public void AddDoubleIndexes(bool transparent)
        {
            List<uint> list = transparent ? chunkTransIndexes : chunkIndexes;
            uint count = transparent ? transIndexCount : indexCount;
            list.Add(0 + count);
            list.Add(1 + count);
            list.Add(2 + count);
            list.Add(2 + count);
            list.Add(3 + count);
            list.Add(0 + count);

            list.Add(1 + count);
            list.Add(0 + count);
            list.Add(3 + count);
            list.Add(3 + count);
            list.Add(2 + count);
            list.Add(1 + count);
            if (transparent) transIndexCount += 4;
            else indexCount += 4;
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

                chunkColorVBO = new VBO(chunkColors);
                chunkColorVBO.Bind();
                chunkVAO.LinkToVAO(2, 4, chunkColorVBO);

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

                chunkTransColorVBO = new VBO(chunkTransColors);
                chunkTransColorVBO.Bind();
                chunkTransVAO.LinkToVAO(2, 4, chunkTransColorVBO);

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
                    ((Game.transparentBlocks.Contains(neighbor.blockType) || Game.cutoutBlocks.Contains(neighbor.blockType)) && block.blockType != neighbor.blockType));
        }

        public void UpdateChunk()
        {
            Delete();
            GenLighting();
            GenFaces();
            BuildChunk();
        }
    }
}