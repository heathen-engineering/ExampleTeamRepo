#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET

using System.Linq;
using Unity.Mathematics;

namespace HeathenEngineering.SteamworksIntegration
{
    [System.Serializable]
    public struct InputControllerData
    {
        public Steamworks.InputHandle_t handle;
        public InputActionData[] inputs;
        public InputActionUpdate[] changes;

        public InputActionData GetActionData(InputAction action) => GetActionData(action.ActionName);
        public InputActionData GetActionData(string name) => inputs.FirstOrDefault(p => p.name == name);
        public bool GetActive(string name) => inputs.FirstOrDefault(p => p.name == name).active;
        public bool GetState(string name) => inputs.FirstOrDefault(p => p.name == name).state;
        public float GetFloat(string name) => inputs.FirstOrDefault(p => p.name == name).x;
        public float2 GetFloat2(string name)
        {
            var data = inputs.FirstOrDefault(p => p.name == name);
            return new float2(data.x, data.y);
        }
        public Steamworks.EInputSourceMode GetMode(string name) => inputs.FirstOrDefault(p => p.name == name).mode;
    }
}
#endif