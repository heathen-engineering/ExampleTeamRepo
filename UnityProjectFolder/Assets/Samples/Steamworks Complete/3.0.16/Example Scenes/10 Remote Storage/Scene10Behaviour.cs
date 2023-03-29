#if HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH) && !DISABLESTEAMWORKS 
using System.Collections.Generic;
using UnityEngine;

namespace HeathenEngineering.DEMO
{
    /// <summary>
    /// This is for demonstration purposes only
    /// </summary>
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class Scene10Behaviour : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_InputField saveFileName;
        [SerializeField]
        private TMPro.TMP_InputField stringField;
        [SerializeField]
        private TMPro.TMP_InputField intField;
        [SerializeField]
        private UnityEngine.UI.Toggle boolField;
        [SerializeField]
        private Transform fileListRoot;
        [SerializeField]
        private GameObject displayRecordTemplate;
        [SerializeField]
        private DemoDataModel model;

        private List<GameObject> listedRecords = new List<GameObject>();

        public void OpenKnowledgeBaseUserData()
        {
            Application.OpenURL("https://kb.heathenengineering.com/assets/steamworks");
        }

        public void Save()
        {
            model.data.stringData = stringField.text;
            if (int.TryParse(intField.text, out int result))
                model.data.intData = result;
            model.data.boolData = boolField.isOn;

            //Make sure we will have a valid name, this will overwrite if the name matches
            if (!string.IsNullOrEmpty(saveFileName.text.ToLower().Replace(model.extension, "")))
            {
                model.Save(saveFileName.text);
                Refresh();
            }
        }

        public void Loaded()
        {
            stringField.text = model.data.stringData;
            intField.text = model.data.intData.ToString();
            boolField.isOn = model.data.boolData;
        }

        public void Refresh()
        {
            //Remove the old records
            while(listedRecords.Count > 0)
            {
                var target = listedRecords[0];
                listedRecords.Remove(target);
                Destroy(target);
            }

            //Refresh our view of these files
            model.Refresh();

            //Create a record for each file present
            foreach(var file in model.availableFiles)
            {
                var go = Instantiate(displayRecordTemplate, fileListRoot);
                var comp = go.GetComponent<DemoFileDisplayRecord>();
                comp.Initialize(file);
                listedRecords.Add(go);
            }
        }
    }
}
#endif