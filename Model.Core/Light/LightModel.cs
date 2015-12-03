namespace Alfred.Model.Core.Light
{
    public class LightModel
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public bool On { get; set; }
        public byte? Bri { get; set; }
        public int? Hue { get; set; }
        public int? Sat { get; set; }
        public DeviceType Type { get; set; }
        public bool DimEnabled { get; set; }
        public bool ColorEnabled { get; set; }

        // For website scenario binding
        public LightModel(){}

        public LightModel(
            string key, 
            string name, 
            bool on, 
            byte? bri, 
            DeviceType type, 
            bool dimEnabled = false, 
            bool colorEnabled = false,
            int? sat = null,
            int? hue = null
            )
        {
            Key = key;
            Name = name;
            On = on;
            Bri = bri;
            Type = type;
            DimEnabled = type == DeviceType.Hue || dimEnabled;
            ColorEnabled = type == DeviceType.Hue || colorEnabled;
            Sat = sat;
            Hue = hue;
        }

        public void UpdateState(bool? on, byte? bri, int? hue, int? sat)
        {
            On = on.HasValue ? on.Value : (bri.HasValue && bri > 0) || On;
            Bri = bri;
            Hue = hue;
            Sat = sat;
        }
    }

    public class Light360Model : LightModel
    {
        public float Top;
        public float Left;

        public Light360Model(string key, string name, bool on, byte? bright, DeviceType type, bool dimEnabled = false, bool colorEnabled = false, float top = 0, float left = 0) 
            : base(key, name, on, bright, type, dimEnabled, colorEnabled)
        {
            Top = top;
            Left = left;
        }
    }

    public enum DeviceType
    {
        Telldus,
        Hue
    }
}