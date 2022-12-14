using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Nano
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private GameObject buttonPrefab;
        [SerializeField]
        private GameObject leftList;
        [SerializeField]
        private GameObject rightList;
        [SerializeField]
        private GameObject title;
        [SerializeField]
        private GameObject barTextInstance;
        [SerializeField]
        private GameObject netBarInstance;

        private string netContent;
        private string barContent;
        private string barContentTemp;
        private int clearBarCounter;
        public static UIController Instance;
        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateBar();
            UpdateNetBar();
        }

        private void UpdateBar(){
            var textComponent = barTextInstance.GetComponent<TextMeshProUGUI>();
            if (barContentTemp != null)
            {
                textComponent.text = barContentTemp;
                clearBarCounter++;
                if (clearBarCounter % 5 == 0)
                    barContentTemp = null;
            }
            else
            {
                textComponent.text = barContent;
            }
        }

        private void UpdateNetBar(){
            var textComponent = netBarInstance.GetComponent<TextMeshProUGUI>();
            textComponent.text = netContent;
        }
        public void SetLowerBar(string content)
        {

        }
        public void SetImage(string path)
        {

        }
        public void SetLeftList(IEnumerable<string> content)
        {
            SetList(leftList, content);
        }
        public void SetRightList(IEnumerable<string> content)
        {
            SetList(rightList, content);
        }
        private void SetList(GameObject list, IEnumerable<string> content)
        {
            foreach (Transform child in list.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var s in content)
            {

                var instance = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
                instance.transform.Find("ButtonText").GetComponent<TextMeshProUGUI>().text = s;
                instance.transform.SetParent(list.transform);
            }
        }
        
        public void SetTitle(string s)
        {
            title.GetComponent<TextMeshProUGUI>().text = s;
        }
        public void SetBar(string s)
        {
            barContent = s;
        }
        public void SetBarTemperary(string s)
        {
            barContentTemp = s;
        }
        public void setNetBar(string s){
            netContent=s;
        }
        
    }
}
