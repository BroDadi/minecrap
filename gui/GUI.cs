using minecrap.graphics;

namespace minecrap.gui
{
    internal class GUI
    {
        private List<UIElement> elements;

        public GUI()
        {
            elements = new List<UIElement>();
        }

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
            foreach (UIElement element in elements)
            {
                element.Render(shaderProgram);
            }
        }
    }
}