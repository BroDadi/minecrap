namespace minecrap
{
    class Program
    {
        private static void Main(string[] args)
        {
            using (Game game = new(800, 600))
            {
                game.Run();
            }
        }
    }
}