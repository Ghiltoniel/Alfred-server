using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alfred.Model.Core.Light
{
    public interface ILightInterface
    {
        Task<List<LightModel>> GetDevices();

        void Toggle(string key, bool on);
        
        void ToggleAll(bool on);

        void Light(string key, bool? on = null, byte? bri = null, int? hue = null, int? sat = null);

        void LightAll(bool? on = null, byte? bri = null, int? hue = null, int? sat = null);
    }
}
