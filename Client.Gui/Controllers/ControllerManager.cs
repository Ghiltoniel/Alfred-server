using System;
using System.Collections.Generic;
using System.Linq;
using Alfred.Client.Gui.Controllers.Gesture;
using Alfred.Client.Gui.Controllers.Ink;
using Alfred.Client.Gui.Controllers.Leap;
using Alfred.Client.Gui.Controllers.MindWave;
using Alfred.Client.Gui.Controllers.Speech;
using Alfred.Model.Core.Interface;

namespace Alfred.Client.Gui.Controllers
{
    public class ControllerManager
    {
        public HashSet<ControllerInterface> Controllers;

        public ControllerManager()
        {
            Controllers = new HashSet<ControllerInterface>();
            Controllers.Add(new GestureController());
            Controllers.Add(new SpeechController());
            Controllers.Add(new InkController());
            Controllers.Add(new LeapController());
            Controllers.Add(new MindWaveController());
            //Controllers.Add(new MyoController());

            foreach (var controller in Controllers)
            {
                controller.IsOn = false;
            }
        }

        public void InitializeController(string type)
        {
            var controller = GetControllerFromName(type);
            if(controller != null)
            {
                controller.Initialize();
                controller.StartDevice();
                controller.RegisterEvents();
                controller.StartListening();
                controller.IsOn = true;
            }
        }

        public void StopController(string type)
        {
            var controller = GetControllerFromName(type);
            if (controller != null)
            {
                controller.StopListening();
                controller.UnregisterEvents();
                controller.StopDevice();
                controller.IsOn = false;
            }
        }

        private ControllerInterface GetControllerFromName(string name)
        {
            ControllerInterface controller = null;
            switch (name)
            {
                case "Gesture":
                    controller = GetController<GestureController>(typeof(GestureController));
                    break;
                case "Speech":
                    controller = GetController<SpeechController>(typeof(SpeechController));
                    break;
                case "Leap":
                    controller = GetController<LeapController>(typeof(LeapController));
                    break;
                case "Ink":
                    controller = GetController<InkController>(typeof(InkController));
                    break;
                case "MindWave":
                    controller = GetController<MindWaveController>(typeof(MindWaveController));
                    break;
                //case "Myo":
                //    controller = GetController<MyoController>(typeof(MyoController));
                //    break;
            }
            return controller;
        }

        public T GetController<T>(Type type)
        {
            return (T)Controllers.SingleOrDefault(c => c.GetType() == type);
        }
    }
}
