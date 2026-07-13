using System.Globalization;
using minecrap.graphics;
using minecrap.world;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace minecrap.gui
{
    internal class UIBlock : UIElement
    {
        private Dictionary<Faces, List<Vector2>> textures;
        private Texture texture;
        private BlockType blockType;

        public UIBlock(BlockType blockType, Vector2 relSize, Vector2 offSize, Vector2 relPos, Vector2 offPos, float aspectRatio = 0f, DomAxis dominantAxis = DomAxis.None, Vector2? pivotPoint = null, Color4? color = null)
        {
            texture = new Texture("textures");
            SetBlockType(blockType);
            this.relSize = relSize;
            this.offSize = offSize;
            this.relPos = relPos;
            this.offPos = offPos;
            this.aspectRatio = aspectRatio;
            this.dominantAxis = dominantAxis;
            this.pivotPoint = pivotPoint ?? new Vector2(0.5f, 0.5f);
            this.color = color ?? Color4.White;
            SetBlockType(blockType);
        }

        public void SetBlockType(BlockType blockType)
        {
            this.blockType = blockType;
            if (!Game.renderAsSprite.Contains(blockType)) textures = TextureData.blockTypeTextures[blockType];
            GenElement();
        }

        public override void GenElement()
        {
            vao = new VAO();
            vao.Bind();

            Vector2 pos = CalculatePos();
            Vector2 size = CalculateSize();

            if (!Game.renderAsSprite.Contains(blockType))
            {
                vbo = new VBO(new List<Vector3>()
                {
                    new Vector3(pos.X - size.X / 2, pos.Y + size.Y / 4, 0f),
                    new Vector3(pos.X, pos.Y, 0f),
                    new Vector3(pos.X, pos.Y - size.Y / 2, 0f),
                    new Vector3(pos.X - size.X / 2, pos.Y - size.Y / 4, 0f),
                    new Vector3(pos.X, pos.Y, 0f),
                    new Vector3(pos.X + size.X / 2, pos.Y + size.Y / 4, 0f),
                    new Vector3(pos.X + size.X / 2, pos.Y - size.Y / 4, 0f),
                    new Vector3(pos.X, pos.Y - size.Y / 2, 0f),
                    new Vector3(pos.X, pos.Y + size.Y / 2, 0f),
                    new Vector3(pos.X + size.X / 2, pos.Y + size.Y / 4, 0f),
                    new Vector3(pos.X, pos.Y, 0f),
                    new Vector3(pos.X - size.X / 2, pos.Y + size.Y / 4, 0f),
                });
                vbo.Bind();
                vao.LinkToVAO(0, 3, vbo);

                List<Vector2> texCoords =
                [
                    .. textures[Faces.Front],
                    .. textures[Faces.Right],
                    .. textures[Faces.Top],
                ];
                textureVBO = new VBO(texCoords);
                textureVBO.Bind();
                vao.LinkToVAO(1, 2, textureVBO);

                colorVBO = new VBO(new List<Color4>()
                {
                    new Color4(0.75f * color.R, 0.75f * color.G, 0.75f * color.B, 1f * color.A),
                    new Color4(0.75f * color.R, 0.75f * color.G, 0.75f * color.B, 1f * color.A),
                    new Color4(0.75f * color.R, 0.75f * color.G, 0.75f * color.B, 1f * color.A),
                    new Color4(0.75f * color.R, 0.75f * color.G, 0.75f * color.B, 1f * color.A),
                    new Color4(0.66f * color.R, 0.66f * color.G, 0.66f * color.B, 1f * color.A),
                    new Color4(0.66f * color.R, 0.66f * color.G, 0.66f * color.B, 1f * color.A),
                    new Color4(0.66f * color.R, 0.66f * color.G, 0.66f * color.B, 1f * color.A),
                    new Color4(0.66f * color.R, 0.66f * color.G, 0.66f * color.B, 1f * color.A),
                    color,
                    color,
                    color,
                    color,
                });
                colorVBO.Bind();
                vao.LinkToVAO(2, 4, colorVBO);

                ebo = new EBO(new List<uint>() { 0, 1, 2, 2, 3, 0, 4, 5, 6, 6, 7, 4, 8, 9, 10, 10, 11, 8 });
                ebo.Bind();
            }
            else
            {
                vbo = new VBO(new List<Vector3>()
                {
                    new Vector3(pos.X - size.X / 2, pos.Y + size.Y / 2, 0f),
                    new Vector3(pos.X + size.X / 2, pos.Y + size.Y / 2, 0f),
                    new Vector3(pos.X + size.X / 2, pos.Y - size.Y / 2, 0f),
                    new Vector3(pos.X - size.X / 2, pos.Y - size.Y / 2, 0f),
                });
                vbo.Bind();
                vao.LinkToVAO(0, 3, vbo);

                textureVBO = new VBO(TextureData.customTextures[blockType][0..4]);
                textureVBO.Bind();
                vao.LinkToVAO(1, 2, textureVBO);

                colorVBO = new VBO(new List<Color4>() { color, color, color, color, });
                colorVBO.Bind();
                vao.LinkToVAO(2, 4, colorVBO);

                ebo = new EBO(new List<uint>() { 0, 1, 2, 2, 3, 0 });
                ebo.Bind();
            }
        }

        public override void Render(ShaderProgram shaderProgram)
        {
            shaderProgram.Bind();
            texture.Bind();
            vao.Bind();
            ebo.Bind();
            GL.DrawElements(PrimitiveType.Triangles, 18, DrawElementsType.UnsignedInt, 0);
        }
    }
}