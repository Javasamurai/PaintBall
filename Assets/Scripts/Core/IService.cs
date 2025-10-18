namespace Core
{
    public interface IService
    {
        /// <summary>
        /// Initialize the service and prepare it for use.
        /// </summary>
        public abstract void Initialize();
        
        /// <summary>
        /// Cleanup the service and release any resources it holds.
        /// </summary>
        public abstract void Shutdown();
    }
}