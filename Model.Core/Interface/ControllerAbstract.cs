namespace Alfred.Model.Core.Interface
{
    public class ControllerAbstract : ControllerInterface
    {
        public bool isOn;
        public bool IsOn
        {
            get { return isOn; }
            set {isOn = value;}
        }
        public virtual void Initialize()
        {
        }

        public virtual void StartDevice()
        {

        }

        public virtual void StopDevice()
        {

        }

        public virtual void StartListening()
        {

        }

        public virtual void StopListening()
        {

        }

        public virtual void RegisterEvents()
        {

        }

        public virtual void UnregisterEvents()
        {

        }

        public virtual void Reload()
        {
            StopListening();
            UnregisterEvents();
            StopDevice();

            Initialize();
            StartDevice();
            RegisterEvents();
            StartListening();
        }
    }
}
