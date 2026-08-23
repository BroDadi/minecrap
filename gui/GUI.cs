using OpenTK.Mathematics;
using minecrap.graphics;

namespace minecrap.gui
{
    internal class GUI
    {
        private List<UIElement> elements;
        public bool enabled = true;

        public GUI(string id)
        {
            GUIManager.instance.AddToList(this, id);
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

        public bool Click(Vector2 cursor)
        {
            if (!enabled) return false;

            foreach (UIElement element in elements)
            {
                UIElement? clicked = ClickTraversal(element, cursor);

                if (clicked != null && clicked.OnClick != null)
                {
                    clicked.OnClick();
                    return true;
                }
            }

            return false;
        }

        private UIElement? ClickTraversal(UIElement element, Vector2 cursor)
        {
            if (!element.enabled) return null;
            else if (element.IsCursorOnElement(cursor)) return element;
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