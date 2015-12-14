using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Alfred.Model.Core;
using Alfred.Model.Core.Light;
using Alfred.Model.Db;
using Alfred.Utils.Lights;
using Alfred.Utils.Lights.Hue;
using Alfred.Utils.Lights.Telldus;
using Newtonsoft.Json;
using Alfred.Model.Db.Repositories;

namespace Alfred.Utils.Managers
{
    public class LightManager
    {
        public ObservableCollection<LightModel> Devices = new ObservableCollection<LightModel>();
        public List<ILightInterface> DeviceInterfaces = new List<ILightInterface>();
        private readonly Timer _refreshTimer = new Timer(15000);
        private DateTime _lastUpdated;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public LightManager()
        {
            _refreshTimer.Elapsed += RefreshTimer_Elapsed;
            Devices.CollectionChanged += CollectionChanged;
            SetInterfaces();
        }

        public void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        private async void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await SetDevices();
        }

        public void SetInterfaces()
        {
            var configurations = new ConfigurationRepository().GetAll();

            DeviceInterfaces.Clear();
            if(configurations.Single(c => c.Name == "Device_HueEnabled").Value == "1")
            {
                var hueBridgeIp = configurations.Single(c => c.Name == "Device_HueBridgeIp").Value;
                var hueBridgeUser = configurations.Single(c => c.Name == "Device_HueBridgeUser").Value;
                DeviceInterfaces.Add(new HueInterface(hueBridgeIp, hueBridgeUser));
            }

            if (configurations.Single(c => c.Name == "Device_TelldusEnabled").Value == "1")
            {
                var tck = configurations.Single(c => c.Name == "Device_TelldusConsumerKey").Value;
                var tcks = configurations.Single(c => c.Name == "Device_TelldusConsumerSecret").Value;
                var tt = configurations.Single(c => c.Name == "Device_TelldusToken").Value;
                var tts = configurations.Single(c => c.Name == "Device_TelldusTokenSecret").Value;
                DeviceInterfaces.Add(new TelldusInterface(tck, tcks, tt, tts));
            }
        }

        public async Task SetDevices()
        {
            if (DateTime.Now.Subtract(_lastUpdated).TotalSeconds < 60)
                return;

            _lastUpdated = DateTime.Now;
            var newDevices = new List<LightModel>();
            foreach (var deviceInterface in DeviceInterfaces)
            {
                var techDevices = await deviceInterface.GetDevices();
                newDevices.AddRange(techDevices);
            }

            var deletedIds = new HashSet<string>();

            foreach (var device in Devices)
            {
                var newDevice = newDevices.SingleOrDefault(d => d.Key == device.Key);
                if (newDevice != null)
                {
                    device.Name = newDevice.Name;
                    device.Type = newDevice.Type;
                    device.ColorEnabled = newDevice.ColorEnabled;
                    device.DimEnabled = newDevice.DimEnabled;
                    device.UpdateState(newDevice.On, newDevice.Bri, newDevice.Hue, newDevice.Sat);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, device));
                }
                else
                    deletedIds.Add(device.Key);
            }

            foreach(var id in deletedIds)
                Devices.Remove(Devices.Single(d=>d.Key == id));

            var addedIds = newDevices.Select(d => d.Key).Except(Devices.Select(d => d.Key));

            foreach(var newId in addedIds)
                Devices.Add(newDevices.Single(d => d.Key == newId));
        }

        public ILightInterface GetDeviceInterface(DeviceType type)
        {
            switch (type)
            {
                case DeviceType.Telldus:
                    return DeviceInterfaces.SingleOrDefault(d => d is TelldusInterface);
                case DeviceType.Hue:
                    return DeviceInterfaces.SingleOrDefault(d => d is HueInterface);
                default:
                    return null;
            }
        }

        public LightModel GetDevice(string key)
        {
            return Devices.SingleOrDefault(d => d.Key == key);
        }

        public void Light(string key, bool? on = null, byte? bri = null, int? hue = null, int? sat = null)
        {
            var device = Devices.SingleOrDefault(d => d.Key == key);
            Light(device, on, bri, hue, sat);
        }

        public void Light(LightModel device, bool? on = null, byte? bri = null, int? hue = null, int? sat = null)
        {
            if (device != null)
            {
                var deviceInterface = GetDeviceInterface(device.Type);
                if (deviceInterface != null)
                {
                    deviceInterface.Light(device.Key, on, bri, hue, sat);
                    device.UpdateState(on, bri, hue, sat);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, device));
                }
            }
        }

        public void LightAll(bool? on, byte? bri = null, int? hue = null, int? sat = null, DeviceType? type = null)
        {
            var devices = type.HasValue ? DevicesType(type.Value) : Devices;
            if (type.HasValue)
            {
                var deviceInterface = GetDeviceInterface(type.Value);
                if (deviceInterface != null)
                    deviceInterface.LightAll(on, bri, hue, sat);
            }
            else
                DeviceInterfaces.ForEach(d => d.LightAll(on, bri, hue, sat));

            foreach (var device in devices)
            {
                device.UpdateState(on, bri, hue, sat);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, device));
            }
        }

        public IEnumerable<LightModel> DevicesType(DeviceType type)
        {
            return Devices.Where(d => d.Type == type);
        }

        public void BroadcastLights(IEnumerable<LightModel> lights = null)
        {
            Init.WebSocketServer.Broadcast(new AlfredTask
            {
                Arguments = new Dictionary<string, string>
                {
                    { "lights", JsonConvert.SerializeObject(CommonManagers.LightManager.Devices) }
                }
            });
        }
    }
}
