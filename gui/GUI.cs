using OpenTK.Mathematics;
using minecrap.graphics;

namespace minecrap.gui
{
    internal class GUI
    {
        private List<UIElement> elements;
        private bool enabled = true;

        public GUI()
        {
            elements = new List<UIElement>();
        }

        public void Enable() => enabled = true;
        public void Disable() => enabled = false;

        public void AddToGUI(UIElement element)
        {
            elements.Add(element);
            element.GenElement();
        }

        public void RebuildGUI()
        {
            foreach (UIElement element in elements)
            {
                element.GenElement();
            }
        }

        public void Render(ShaderProgram shaderProgram)
        {
            if (!enabled) return;

            foreach (UIElement element in elements)
            {
                element.Render(shaderProgram);
            }
        }

        public void Click(Vector2 cursor)
        {
            if (!enabled) return;

            foreach (UIElement element in elements)
            {
                UIElement? clicked = ClickTraversal(element, cursor);

                if (clicked != null)
                {
                    clicked.OnClick();
                    break;
                }
            }
        }

        private UIElement? ClickTraversal(UIElement element, Vector2 cursor)
        {
            if (element.IsCursorOnElement(cursor)) return element;
            else if (element.children != null)
            {
                foreach (UIElement child in element.children)
                {
                    UIElement? clicked = ClickTraversal(child, cursor);
                    if (clicked != null) return clicked;
                }
                return null;
            }
            else return null;
        }
    }
}