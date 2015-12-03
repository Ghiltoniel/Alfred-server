namespace Alfred.Model.Core.Sensor
{
    public enum SensorType
    {
        Temperature,
        Humidity,
        Energy
    }
    public enum SensorManufacturer
    {
        NinjaBlocks
    }

    public interface ISensorValue
    {
    }

    public class SensorModel
    {
        public string Id;
        public string Name;
        public SensorType Type;
        public SensorManufacturer Manufacturer;
        public string Value;
    }
}
