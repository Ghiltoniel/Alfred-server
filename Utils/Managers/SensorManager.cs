using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Alfred.Model.Core;
using Alfred.Model.Core.Sensor;
using Alfred.Utils.Devices.NinjaBlocks;
using Newtonsoft.Json;

namespace Alfred.Utils.Managers
{
    public class SensorManager
    {
        public ObservableCollection<SensorModel> Devices = new ObservableCollection<SensorModel>();
        private List<ISensorInterface> _deviceInterfaces = new List<ISensorInterface>();
        private readonly Timer _refreshTimer = new Timer(15000);

        public SensorManager() 
        {
            SetInterfaces();
            _refreshTimer.Elapsed += RefreshTimer_Elapsed;
            _refreshTimer.Enabled = true;
        }

        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SetDevices();
        }

        private void SetInterfaces()
        {
            _deviceInterfaces.Add(new NinjaBlocksInterface());
        }

        public async Task SetDevices()
        {
            var newDevices = new List<SensorModel>();
            foreach (var deviceInterface in _deviceInterfaces)
            {
                var techDevices = await deviceInterface.GetDevices();
                newDevices.AddRange(techDevices);
            }

            var deletedIds = new HashSet<string>();

            foreach (var device in Devices)
            {
                var newDevice = newDevices.SingleOrDefault(d => d.Id == device.Id);
                if (newDevice != null)
                {
                    device.Name = newDevice.Name;
                    device.Type = newDevice.Type;
                }
                else
                    deletedIds.Add(device.Id);
            }

            foreach(var id in deletedIds)
                Devices.Remove(Devices.Single(d=>d.Id == id));

            var addedIds = newDevices.Select(d => d.Id).Except(Devices.Select(d => d.Id));

            foreach(var newId in addedIds)
                Devices.Add(newDevices.Single(d => d.Id == newId));
        }

        public ISensorInterface GetDeviceInterface(SensorManufacturer manufacturer)
        {
            switch (manufacturer)
            {
                case SensorManufacturer.NinjaBlocks:
                    return _deviceInterfaces.SingleOrDefault(d => d is NinjaBlocksInterface);
                default:
                    return null;
            }
        }

        public SensorModel GetDevice(string id)
        {
            return Devices.SingleOrDefault(d => d.Id == id);
        }
    }
}
