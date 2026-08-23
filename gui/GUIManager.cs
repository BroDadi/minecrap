using OpenTK.Mathematics;
using minecrap.graphics;

namespace minecrap.gui
{
    internal class GUIManager
    {
        private Dictionary<string, GUI> toRender;
        public static GUIManager instance;

        public GUIManager()
        {
            toRender = new();
            instance = this;
        }

        public void AddToList(GUI gui, string id) => toRender[id] = gui;
        public GUI GetGUI(string id) => toRender[id];

        public void RenderAll(ShaderProgram shaderProgram)
        {
            foreach (GUI gui in toRender.Values)
            {
                gui.Render(shaderProgram);
            }
        }

        public void RebuildAll()
        {
            foreach (GUI gui in toRender.Values)
            {
                gui.RebuildGUI();
            }
        }

        public void Click(Vector2 cursor)
        {
            foreach (GUI gui in toRender.Values)
            {
                if (gui.Click(cursor)) break;
            }
        }
    }
}