namespace Core
{
    public interface ILifeCycle
    {
        public void Initialize();
        public void Update();
        public void Shutdown();
    }
}