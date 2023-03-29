#if HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH) && !DISABLESTEAMWORKS 

using UnityEngine;
using HeathenEngineering.SteamworksIntegration;
using System.Collections.Generic;
using HeathenEngineering.SteamworksIntegration.API;

namespace HeathenEngineering.DEMO
{
    /// <summary>
    /// This is for demonstration purposes only
    /// </summary>
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class Scene6Behaviour : MonoBehaviour
    {
        [Header("UI References")]
        public TMPro.TextMeshProUGUI label;

        [Header("Input Action Sets")]
        public InputActionSet menuActionSet;
        public InputActionSet shipActionSet;

        [Header("Input Action Set Layers")]
        public InputActionSetLayer thustLayer;

        [Header("Input Actions")]
        public InputAction analogAction;
        public InputAction leftAction;
        public InputAction rightAction;
        public InputAction forwardAction;
        public InputAction backwardAction;
        public InputAction fireAction;
        public InputAction pauseAction;
        public InputAction menuUpAction;
        public InputAction menuDownAction;
        public InputAction menuLeftAction;
        public InputAction menuRightAction;
        public InputAction selectAction;
        public InputAction cancelAction;

        private InputActionData analogData;
        private InputActionData leftData;
        private InputActionData rightData;
        private InputActionData forwardData;
        private InputActionData backwardData;
        private InputActionData fireData;
        private InputActionData pauseData;
        private InputActionData menuUpData;
        private InputActionData menuDownData;
        private InputActionData menuLeftData;
        private InputActionData menurightData;
        private InputActionData selectData;
        private InputActionData cancelData;

        [Header("Glyph Data")]
        public List<InputActionGlyph> glyphs = new List<InputActionGlyph>();
        public List<UGUIInputActionName> names = new List<UGUIInputActionName>();

        private bool hackRefresh = false;

        private void Start()
        {
            if (!App.Initialized)
                App.evtSteamInitialized.AddListener(HandleInitalization);
            else
                HandleInitalization();
        }

        private void HandleInitalization()
        {
            App.evtSteamInitialized.RemoveListener(HandleInitalization);

            if (SteamInputManager.Controllers.Count > 0)
            {
                shipActionSet.Activate(SteamInputManager.Controllers[0].handle);
                thustLayer.Activate(SteamInputManager.Controllers[0].handle);

                Invoke(nameof(DelayActivate), 1);

                Debug.Log("Steam Input initialized:\n\tControllers Found = " + SteamInputManager.Controllers.Count);
            }
            else
            {
                Debug.LogWarning("Steam Input initialized:\n\tNo controllers found!");
            }
        }

        public void HandleActionEvent(InputActionUpdate data)
        {
            //Demonstrates handling action data as an event
            Debug.Log($"Change Detected: [was : is]" +
                $"\nActive [{data.wasActive} : {data.isActive}]" +
                $"\nState [{data.wasState} : {data.isState}]" +
                $"\nX [{data.wasX} : {data.isX}]" +
                $"\nY [{data.wasY} : {data.isY}]");
        }

        private void Update()
        {
            if (SteamInputManager.Controllers != null && SteamInputManager.Controllers.Count > 0)
            {
                if(!hackRefresh)
                {
                    hackRefresh = true;
                    DelayActivate();
                }
                analogData = analogAction.GetActionData();

                leftData = leftAction.GetActionData();
                rightData = rightAction.GetActionData();
                forwardData = forwardAction.GetActionData();
                backwardData = backwardAction.GetActionData();

                fireData = fireAction.GetActionData();
                pauseData = pauseAction.GetActionData();

                menuUpData = menuUpAction.GetActionData();
                menuDownData = menuDownAction.GetActionData();
                menuLeftData = menuLeftAction.GetActionData();
                menurightData = menuRightAction.GetActionData();
                selectData = selectAction.GetActionData();
                cancelData = cancelAction.GetActionData();

                label.text = "Analog Action: " + analogData.ToString() + "\nLeft Action: " + leftData.ToString() + "\nRight Action: " + rightData.ToString() + "\nForward Action: " + forwardData.ToString() + "\nBackward Action: " + backwardData.ToString() + "\nFire Action: " + fireData.ToString() + "\nPause Action: " + pauseData.ToString() + "\nMenu Up Action: " + menuUpData.ToString() + "\nMenu Down Action: " + menuDownData.ToString() + "\nMenu Right Action: " + menurightData.ToString() + "\nMenu Left Action: " + menuLeftData.ToString() + "\nMenu Select Action: " + selectData.ToString() + "\nCancel Action: " + cancelData.ToString();
            }
            else
                label.text = "No Controllers found";
        }

        private void DelayActivate()
        {
            //Because we have to force the App ID in Unity Editor we need to force a refresh after that
            foreach (var glyph in glyphs)
                glyph.RefreshImage();
            foreach (var iName in names)
                iName.RefreshName();
        }

        public void ActivateMenuControls()
        {
            menuActionSet.Activate();
        }

        public void ActivateShipControls()
        {
            shipActionSet.Activate();
            thustLayer.Activate();
        }

        public void OpenKnowledgeBaseUserData()
        {
            Application.OpenURL("https://kb.heathenengineering.com/assets/steamworks");
        }
    }
}
#endif