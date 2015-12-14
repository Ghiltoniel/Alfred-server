using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alfred.Model.Core.Sensor
{
    public interface ISensorInterface
    {
        Task<List<SensorModel>> GetDevices();
        IDictionary<DateTime, double> GetDeviceHistory(string deviceGuid);
    }
}
