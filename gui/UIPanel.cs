using minecrap.graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace minecrap.gui
{
    internal class UIPanel : UIElement
    {
        public UIPanel(Vector2 relSize, Vector2 offSize, Vector2 relPos, Vector2 offPos, Color color, float aspectRatio = 0f, DomAxis dominantAxis = DomAxis.None, Vector2? pivotPoint = null)
        {
            this.relSize = relSize;
            this.offSize = offSize;
            this.relPos = relPos;
            this.offPos = offPos;
            this.aspectRatio = aspectRatio;
            this.dominantAxis = dominantAxis;
            this.pivotPoint = pivotPoint ?? new Vector2(0.5f, 0.5f);
            this.color = color;
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

            colorVBO = new VBO(new Color[] { color, color, color, color });
            vao.LinkToVAO(2, 4, colorVBO, VertexAttribPointerType.UnsignedByte, true);

            ebo = new EBO(new uint[] { 0, 1, 2, 2, 3, 0 });
            ebo.Bind();
            base.GenElement();
        }

        protected override void OnRender(ShaderProgram shaderProgram)
        {
            shaderProgram.Bind();
            vao.Bind();
            ebo.Bind();
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            base.OnRender(shaderProgram);
        }
    }
}