#if HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH) && !DISABLESTEAMWORKS 
using UnityEngine;

namespace HeathenEngineering.DEMO
{
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    [CreateAssetMenu(menuName = "Steamworks/DEMO/DataModel")]
    public class DemoDataModel : SteamworksIntegration.DataModel<DemoDataModelType>
    { }
}
#endif
