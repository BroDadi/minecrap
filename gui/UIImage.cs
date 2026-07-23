using minecrap.graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace minecrap.gui
{
    internal class UIImage : UIElement
    {
        private Texture texture;

        public UIImage(Vector2 relSize, Vector2 offSize, Vector2 relPos, Vector2 offPos, Texture texture, float aspectRatio = 0f, DomAxis dominantAxis = DomAxis.None, Vector2? pivotPoint = null, Color? color = null)
        {
            this.relSize = relSize;
            this.offSize = offSize;
            this.relPos = relPos;
            this.offPos = offPos;
            this.texture = texture;
            this.aspectRatio = aspectRatio;
            this.dominantAxis = dominantAxis;
            this.pivotPoint = pivotPoint ?? new Vector2(0.5f, 0.5f);
            this.color = color ?? new Color(255, 255, 255, 255);
        }

        public override void GenElement()
        {
            vao = new VAO();
            vao.Bind();

            Vector2 pos = CalculatePos() / Game.instance.screenSize;
            Vector2 size = CalculateSize() / Game.instance.screenSize;
            vbo = new VBO(new Vector3[]
            {
                new Vector3(pos.X - size.X / 2, pos.Y + size.Y / 2, 0),
                new Vector3(pos.X + size.X / 2, pos.Y + size.Y / 2, 0),
                new Vector3(pos.X + size.X / 2, pos.Y - size.Y / 2, 0),
                new Vector3(pos.X - size.X / 2, pos.Y - size.Y / 2, 0),
            });
            vbo.Bind();
            vao.LinkToVAO(0, 3, vbo);

            textureVBO = new VBO(new Vector2[]
            {
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(0, 0),
            });
            textureVBO.Bind();
            vao.LinkToVAO(1, 2, textureVBO);

            colorVBO = new VBO(new Color[] { color, color, color, color });
            vao.LinkToVAO(2, 4, colorVBO, VertexAttribPointerType.UnsignedByte, true);

            ebo = new EBO(new uint[] { 0, 1, 2, 2, 3, 0 });
            ebo.Bind();
            base.GenElement();
        }

        public override void Render(ShaderProgram shaderProgram)
        {
            base.Render(shaderProgram);
            shaderProgram.Bind();
            texture.Bind();
            vao.Bind();
            ebo.Bind();
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            base.Render(shaderProgram);
        }
    }
}