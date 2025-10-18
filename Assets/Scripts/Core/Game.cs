namespace Core
{
    public class Game
    {
        public static Game Instance { get; private set; }

        public Game()
        {
            if (Instance != null)
            {
                throw new System.Exception("Game instance already exists.");
            }
            Instance = this;
        }

        public void Initialize()
        {
            
        }

        public void Shutdown()
        {
            Instance = null;
        }
    }
}