#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using System.Collections.Generic;
using UnityEngine;

namespace HeathenEngineering.SteamworksIntegration
{
    public class InputAction : ScriptableObject
    {
        public InputActionType Type
        {
            get => type;
#if UNITY_EDITOR
            set => type = value;
#endif
        }
        public string ActionName
        {
            get => actionName;
#if UNITY_EDITOR
            set => actionName = value;
#endif
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [SerializeField]
        private InputActionType type;
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [SerializeField]
        private string actionName;

        public Steamworks.InputAnalogActionHandle_t AnalogHandle => analogHandle;
        public Steamworks.InputDigitalActionHandle_t DigitalHandle => digitalHandle;

        private Steamworks.InputAnalogActionHandle_t analogHandle = new Steamworks.InputAnalogActionHandle_t(0);
        private Steamworks.InputDigitalActionHandle_t digitalHandle = new Steamworks.InputDigitalActionHandle_t(0);
        
        public InputActionData GetActionData(Steamworks.InputHandle_t controller) => API.Input.Client.GetActionData(controller, actionName);
        public InputActionData GetActionData() => API.Input.Client.GetActionData(actionName);
        public Texture2D[] GetInputGlyphs(Steamworks.InputHandle_t controller, InputActionSet set) => GetInputGlyphs(controller, set.Data);
        public Texture2D[] GetInputGlyphs(Steamworks.InputHandle_t controller, InputActionSetLayer set) => GetInputGlyphs(controller, set.Data);
        public Texture2D[] GetInputGlyphs(Steamworks.InputHandle_t controller, Steamworks.InputActionSetHandle_t set)
        {
            if (type == InputActionType.Analog)
            {
                var origns = API.Input.Client.GetAnalogActionOrigins(controller, set, analogHandle);

                var textArray = new Texture2D[origns.Length];
                for (int i = 0; i < origns.Length; i++)
                {
                    textArray[i] = API.Input.Client.GetGlyphActionOrigin(origns[i]);
                }

                return textArray;
            }
            else
            {
                var origns = API.Input.Client.GetDigitalActionOrigins(controller, set, digitalHandle);

                var textArray = new Texture2D[origns.Length];
                for (int i = 0; i < origns.Length; i++)
                {
                    textArray[i] = API.Input.Client.GetGlyphActionOrigin(origns[i]);
                }

                return textArray;
            }
        }

        public string[] GetInputNames(Steamworks.InputHandle_t controller, InputActionSet set) => GetInputNames(controller, set);
        public string[] GetInputNames(Steamworks.InputHandle_t controller, InputActionSetLayer set) => GetInputNames(controller, set);
        public string[] GetInputNames(Steamworks.InputHandle_t controller, Steamworks.InputActionSetHandle_t set)
        {
            if (type == InputActionType.Analog)
            {
                var origns = API.Input.Client.GetAnalogActionOrigins(controller, set, analogHandle);

                var stringArray = new string[origns.Length];
                for (int i = 0; i < origns.Length; i++)
                {
                    stringArray[i] = API.Input.Client.GetStringForActionOrigin(origns[i]);
                }

                return stringArray;
            }
            else
            {
                var origns = API.Input.Client.GetDigitalActionOrigins(controller, set, digitalHandle);

                var stringArray = new string[origns.Length];
                for (int i = 0; i < origns.Length; i++)
                {
                    stringArray[i] = API.Input.Client.GetStringForActionOrigin(origns[i]);
                }

                return stringArray;
            }
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(InputAction))]
    public class InputActionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        { }
    }
#endif
}
#endif