using minecrap.graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace minecrap.gui
{
    internal class UIImage : UIElement
    {
        private Texture texture;

        public UIImage(Vector2 relSize, Vector2 offSize, Vector2 relPos, Vector2 offPos, Texture texture, float aspectRatio = 0f, DomAxis dominantAxis = DomAxis.None)
        {
            this.relSize = relSize;
            this.offSize = offSize;
            this.relPos = relPos;
            this.offPos = offPos;
            this.texture = texture;
            this.aspectRatio = aspectRatio;
            this.dominantAxis = dominantAxis;
        }

        public override void GenElement()
        {
            vao = new VAO();
            vao.Bind();

            Vector2 pos = CalculatePos();
            Vector2 size = CalculateSize();
            vbo = new VBO(new List<Vector3>()
            {
                new Vector3(pos.X - size.X / 2, pos.Y + size.Y / 2, 0),
                new Vector3(pos.X + size.X / 2, pos.Y + size.Y / 2, 0),
                new Vector3(pos.X + size.X / 2, pos.Y - size.Y / 2, 0),
                new Vector3(pos.X - size.X / 2, pos.Y - size.Y / 2, 0),
            });
            vbo.Bind();
            vao.LinkToVAO(0, 3, vbo);

            textureVBO = new VBO(new List<Vector2>()
            {
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(0, 0),
            });
            textureVBO.Bind();
            vao.LinkToVAO(1, 2, textureVBO);

            ebo = new EBO(new List<uint>() { 0, 1, 2, 2, 3, 0 });
            ebo.Bind();
        }

        public override void Render(ShaderProgram shaderProgram)
        {
            shaderProgram.Bind();
            texture.Bind();
            vao.Bind();
            ebo.Bind();
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }

        public void Delete()
        {
            vao.Delete();
            vbo.Delete();
            textureVBO.Delete();
            ebo.Delete();
        }
    }
}