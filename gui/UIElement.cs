using minecrap.graphics;
using OpenTK.Mathematics;

namespace minecrap.gui
{
    internal abstract class UIElement
    {
        public Vector2 relSize, offSize, relPos, offPos, pivotPoint;
        public Color color;
        protected VAO vao;
        protected VBO vbo, textureVBO, colorVBO;
        protected EBO ebo;
        public List<UIElement> children;
        public bool enabled = true;
        public bool clickable = false;
        public UIElement parent;
        public DomAxis dominantAxis = DomAxis.None;
        public float aspectRatio = 0f;

        public virtual void GenElement()
        {
            if (children != null)
            {
                foreach (UIElement child in children)
                {
                    child.GenElement();
                }
            }
        }

        public void Render(ShaderProgram shaderProgram)
        {
            if (!enabled) return;
            OnRender(shaderProgram);
        }
        
        protected virtual void OnRender(ShaderProgram shaderProgram)
        {
            if (children != null)
            {
                foreach (UIElement child in children)
                {
                    child.Render(shaderProgram);
                }
            }
        }

        public virtual void AddChild(UIElement child)
        {
            if (children == null) children = new List<UIElement>();
            children.Add(child);
            child.parent = this;
        }

        protected Vector2 CalculatePos()
        {
            Vector2 size = CalculateSize();
            Vector2 parentSize = parent == null ? Game.instance.screenSize : parent.CalculateSize();
            Vector2 parentPos = parent == null ? Vector2.Zero : parent.CalculatePos();
            return parentPos + relPos * parentSize - size * (pivotPoint - new Vector2(0.5f, 0.5f)) + offPos;
        }

        protected Vector2 CalculateSize()
        {
            Vector2 relativeTo = parent == null ? Game.instance.screenSize : parent.CalculateSize();
            float width, height;

            switch (dominantAxis)
            {
                case DomAxis.Width:
                    width = relSize.X * relativeTo.X + offSize.X;
                    height = width / aspectRatio;
                    break;
                case DomAxis.Height:
                    height = relSize.Y * relativeTo.Y + offSize.Y;
                    width = height * aspectRatio;
                    break;
                default:
                    width = relSize.X * relativeTo.X + offSize.X;
                    height = relSize.Y * relativeTo.Y + offSize.Y;
                    break;
            }
            return new Vector2(width, height);
        }

        public bool IsCursorOnElement(Vector2 cursor)
        {
            if (clickable && enabled)
            {
                Vector2 size = CalculateSize();
                Vector2 pos = CalculatePos();
                Vector2 min = pos - size / 2;
                Vector2 max = pos + size / 2;

                return cursor.X >= min.X && cursor.X <= max.X && cursor.Y >= min.Y && cursor.Y <= max.Y;
            }
            return false;
        }

        public virtual void Delete()
        {
            vbo.Delete();
            textureVBO.Delete();
            colorVBO.Delete();
            vao.Delete();
            ebo.Delete();
            parent?.children.Remove(this);
        }

        public virtual void Disable() => enabled = false;
        public virtual void Enable() => enabled = true;
        public virtual void OnClick() {}
    }
}