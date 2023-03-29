#if HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH) && !DISABLESTEAMWORKS 
using System;

namespace HeathenEngineering.DEMO
{
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    [Serializable]
    public struct DemoDataModelType
    {
        public string stringData;
        public int intData;
        public bool boolData;
    }
}
#endif
