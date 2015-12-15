namespace Alfred.Model.Core.Interface
{
    public interface ControllerInterface
    {
        bool IsOn { get; set; }
        void Initialize();
        void StartDevice();
        void StopDevice();
        void StartListening();
        void StopListening();
        void RegisterEvents();
        void UnregisterEvents();
    }
}
