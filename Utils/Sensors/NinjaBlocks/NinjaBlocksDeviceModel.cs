using System.Collections.Generic;

namespace Alfred.Utils.NinjaBlocks
{
    public class DeviceModel
    {
        public int did;
        public int gid;
        public int vid;
        public string css_class;
        public string default_name;
        public string device_type;
        public int is_actuator;
        public int is_sensor;
        public int is_silent;
        public int has_subdevice_count;
        public int has_time_series;
        public DeviceData last_data;
        public DeviceMeta meta;
        public string node;
        public string shortName;
        public Dictionary<string, SubDeviceModel> subDevices;
    }

    public class SubDeviceModel
    {
        public string category;
        public string data;
        public string shortName;
        public string type;
    }

    public class DeviceData
    {
        public string DA;
        public int timestamp;
    }

    public class DeviceMeta
    {

    }
}
