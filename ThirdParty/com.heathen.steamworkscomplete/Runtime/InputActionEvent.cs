#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using System.Linq;
using UnityEngine;

namespace HeathenEngineering.SteamworksIntegration
{
    [HelpURL("https://kb.heathen.group/assets/steamworks/for-unity-game-engine/components/input-action-event")]
    public class InputActionEvent : MonoBehaviour
    {
        [SerializeField]
        private InputAction action;

        public ActionUpdateEvent changed;

        private void Start()
        {
            API.Input.Client.EventInputDataChanged.AddListener(HandleEvent);
        }

        private void OnDestroy()
        {
            API.Input.Client.EventInputDataChanged.RemoveListener(HandleEvent);
        }

        private void HandleEvent(InputControllerData controller)
        {
            var actionData = controller.changes.FirstOrDefault(p => p.name == action.ActionName);
            if (action != null
                && actionData.name == action.ActionName)
                changed.Invoke(actionData);
        }
    }
}
#endif