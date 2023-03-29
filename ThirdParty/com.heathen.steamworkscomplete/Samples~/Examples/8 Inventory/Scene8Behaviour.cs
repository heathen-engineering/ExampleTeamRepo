#if HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH) && !DISABLESTEAMWORKS 

using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

namespace HeathenEngineering.DEMO
{
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class Scene8Behaviour : MonoBehaviour
    {
        public ItemDefinitionObject itemDefinition;

        /// <summary>
        /// This can only be done by a developer on this app.
        /// This is only used for testing purposes
        /// </summary>
        public void Generate()
        {
            if (HeathenEngineering.SteamworksIntegration.API.App.Client.Id.m_AppId == 480)
            {
                Debug.LogWarning("You cannot test Generate with app 480, you must register your own App ID and define your own Items to test with.");
                return;
            }

            itemDefinition.GenerateItem((responce) =>
            {
                if (responce.result == EResult.k_EResultOK)
                {
                    Debug.Log("Generate completed");
                }
                else
                {
                    Debug.LogError("Unexpected result from Valve: " + responce.result);
                }
            });
        }

        /// <summary>
        /// This can only be done on items that are marked as promotional
        /// To understand what that means please read the documentaiton
        /// https://kb.heathenengineering.com/assets/steamworks/learning/core-concepts/inventory
        /// </summary>
        public void AddPromo()
        {
            if (HeathenEngineering.SteamworksIntegration.API.App.Client.Id.m_AppId == 480)
            {
                Debug.LogWarning("You cannot test Add Promo with app 480, you must register your own App ID and define your own Items to test with.");
                return;
            }

            itemDefinition.AddPromoItem((responce) =>
            {
                if (responce.result == EResult.k_EResultOK)
                {
                    Debug.Log("Add promo completed");
                }
                else
                {
                    Debug.LogError("Unexpected result from Valve: " + responce.result);
                }
            });
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

        /// <summary>
        /// This will remove the item from the player's inventory
        /// Its meant for use with consumables e.g. potions
        /// Thit cannot be undone so you should have a high barrier to perform this action e.g. confirmation dialogs, etc.
        /// </summary>
        public void Consume()
        {
            if (HeathenEngineering.SteamworksIntegration.API.App.Client.Id.m_AppId == 480)
            {
                Debug.LogWarning("You cannot test Consume with app 480, you must register your own App ID and define your own Items to test with.");
                return;
            }

            if (itemDefinition.Consume((responce) =>
                     {
                         if (responce.result == EResult.k_EResultOK)
                         {
                             Debug.Log("Item Consume completed");
                         }
                         else
                         {
                             Debug.LogError("Unexpected result from Valve: " + responce.result);
                         }
                     }))
            {
                Debug.LogWarning("The user does not own the required items to perform this consume");
            }
        }

        /// <summary>
        /// This only works for Play Time Generator type items whoes drop rules are satisfied e.g. amount of time played, etc.
        /// </summary>
        public void DropItem()
        {
            if (HeathenEngineering.SteamworksIntegration.API.App.Client.Id.m_AppId == 480)
            {
                Debug.LogWarning("You cannot test Drop Item with app 480, you must register your own App ID and define your own Items to test with.");
                return;
            }

            itemDefinition.TriggerDrop((responce) =>
            {
                if (responce.result == EResult.k_EResultOK)
                {
                    Debug.Log("Drop completed");
                }
                else
                {
                    Debug.LogError("Unexpected result from Valve: " + responce.result);
                }
            });
        }

        public void InventoryChangedHandler(InventoryChangeRecord changes)
        {
            Debug.Log("The user's inventory had " + changes.changes.Length + " items change updating the total item instance count by " + changes.TotalQuantityChange + " instances");
        }
    }
}
#endif