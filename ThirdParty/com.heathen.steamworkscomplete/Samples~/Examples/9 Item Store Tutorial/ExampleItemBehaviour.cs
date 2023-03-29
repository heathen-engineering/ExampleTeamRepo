#if HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH) && !DISABLESTEAMWORKS 

using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using InventoryAPI = HeathenEngineering.SteamworksIntegration.API.Inventory.Client;

namespace HeathenEngineering.DEMO
{
    /// <summary>
    /// This script serves as a crude example of using an Item Definition to update UI elements
    /// such as for use in an in-game item store
    /// </summary>
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class ExampleItemBehaviour : MonoBehaviour
    {
        [SerializeField]
        private ItemDefinitionObject itemDefinition;
        [SerializeField]
        private UnityEngine.UI.Text ownedLable;
        [SerializeField]
        private UnityEngine.UI.Button exchangeButton;

        private List<ExchangeEntry> recipie = null;

        private void Start()
        {
            //Update the quantity and exchange button ... we dont use the param but need it for the listener so just pass in default here
            RefreshItem(default);

            //This occurs for many reasons including any changes or query of any items.
            //We can use this to refresh the quantity
            InventoryAPI.EventSteamInventoryResultReady.AddListener(RefreshItem);
        }

        private void OnDestroy()
        {
            //We should always clear listeners that are no longer valid or needed
            InventoryAPI.EventSteamInventoryResultReady.RemoveListener(RefreshItem);
        }

        public void RefreshItem(InventoryResult responce)
        {
            //On any change to the inventory check if we have an item definition
            if (itemDefinition == null)
                return;

            //Since there is little processing we will simply update the quantity every time no matter what item was changed
            ownedLable.text = itemDefinition.TotalQuantity.ToString();

            //We will check if this item has an exchange and if it does if the user can use it
            //If they can we will enable the exchange button ... if they cant we will disable it
            var recipies = itemDefinition.Recipies;
            if (recipies != null //Does it have an exchange
                && recipies.Length > 0 //are there any recipies in the recipie list
                && itemDefinition.CanExchange(recipies[0], out recipie)) //can the user use this recipie
                exchangeButton.interactable = true;
            else
                exchangeButton.interactable = false;
        }

        /// <summary>
        /// This can only be done on items that have a properly defined price or price catigory and are not marked as hidden.
        /// This will not work in the Unity Editor since this operation opens the Steam overlay to the shoping cart with this item added to it
        /// This will not work on apps that are not yet published as the Steam client item store doesn't exist until publish
        /// </summary>
        public void StartPurchase()
        {
            if (HeathenEngineering.SteamworksIntegration.API.App.Client.Id.m_AppId == 480)
            {
                Debug.LogWarning("You cannot test Start Purchase with app 480, you must register your own App ID and define your own Items to test with.");
                return;
            }
            else if (itemDefinition == null)
            {
                Debug.LogWarning("You must populate the Item Definition field before using Start Purchase.");
                return;
            }
            else if (Application.isEditor)
            {
                Debug.LogWarning("You cannot test Start Purcahse in the editor because it must open the Steam Overlay which will not work in editor.");
                return;
            }

            itemDefinition.StartPurchase((responce, ioError) =>
            {
                if (!ioError)
                {
                    if (responce.m_result == EResult.k_EResultOK)
                    {
                        Debug.Log("Start purchase completed");
                    }
                    else
                    {
                        Debug.LogError("Unexpected result from Valve: " + responce.m_result);
                    }
                }
                else
                {
                    Debug.LogError("Valve indicated an IO Error occured. i.e. failed to start the process at all.");
                }
            });
        }

        /// <summary>
        /// This can only be done on items that have an exchange defined
        /// To understande more about item exchange please read the documentation.
        /// https://kb.heathenengineering.com/assets/steamworks/learning/core-concepts/inventory
        /// </summary>
        public void Exchange()
        {
            if (HeathenEngineering.SteamworksIntegration.API.App.Client.Id.m_AppId == 480)
            {
                Debug.LogWarning("You cannot test Exchange with app 480, you must register your own App ID and define your own Items to test with.");
                return;
            }
            else if (itemDefinition == null)
            {
                Debug.LogWarning("You must populate the Item Definition field before using Exchange.");
                return;
            }

            //This assumes the linked item has a recipie
            if (itemDefinition.CanExchange(itemDefinition.Recipies[0], out List<ExchangeEntry> recipie))
            {
                //If the user owns the required items to satisfy the recipie defined in the first index of the recipies 
                //then go ahead and exchange for it

                itemDefinition.Exchange(recipie, (responce) =>
                {
                    if (responce.result == EResult.k_EResultOK)
                    {
                        Debug.Log("Exchange completed");
                    }
                    else
                    {
                        Debug.LogError("Unexpected result from Valve: " + responce.result);
                    }
                });
            }
            else
            {
                Debug.LogWarning("The user does not own the required items to perform this exchange");
            }
        }
    }
}
#endif