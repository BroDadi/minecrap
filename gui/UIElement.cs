using minecrap.graphics;
using OpenTK.Mathematics;

namespace minecrap.gui
{
    internal abstract class UIElement
    {
        public Vector2 relSize, offSize, relPos, offPos;
        protected VAO vao;
        protected VBO vbo, textureVBO;
        protected EBO ebo;
        public DomAxis dominantAxis = DomAxis.None;
        public float aspectRatio = 0f;

        public abstract void GenElement();
        public abstract void Render(ShaderProgram shaderProgram);
        protected Vector2 CalculatePos() => relPos + new Vector2(offPos.X / Game.instance.width, offPos.Y / Game.instance.height);
        protected Vector2 CalculateSize()
        {
            float width, height;
            switch (dominantAxis)
            {
                case DomAxis.Width:
                    width = relSize.X * Game.instance.width + offSize.X;
                    height = width / aspectRatio;
                    break;
                case DomAxis.Height:
                    height = relSize.Y * Game.instance.height + offSize.Y;
                    width = height * aspectRatio;
                    break;
                default:
                    width = relSize.X * Game.instance.width + offSize.X;
                    height = relSize.Y * Game.instance.height + offSize.Y;
                    break;
            }
            return new Vector2(width / Game.instance.width, height / Game.instance.height);
        }
    }
}