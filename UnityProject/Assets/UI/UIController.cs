using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nano
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private GameObject barPrefab;
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
        [SerializeField]
        private GameObject panelImage;
        [SerializeField]
        private GameObject popupPrefab;

        private string netContent;
        private string barContent;
        private string barContentTemp;
        private int clearBarCounter;
        public static UIController Instance;
        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            panelImage.SetActive(false);
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
        public void SetLeftList(List<string> names,List<float> values)
        {
            SetList(leftList, names,values);
        }
        public void SetRightList(List<string> names,List<float> values)
        {
            SetList(rightList, names,values);
        }
        private void SetList(GameObject list, List<string> names,List<float> values)
        {
            Debug.Assert(names.Count()==values.Count());
            foreach (Transform child in list.transform)
            {
                Destroy(child.gameObject);
            }
            for(int i=0;i<names.Count();i++)
            {
                var item = Instantiate(barPrefab, Vector3.zero, Quaternion.identity);
                var stat=item.transform.GetComponent<StatBar>();
                stat.SetText(names[i]);
                stat.SetValue(values[i]);
                item.transform.SetParent(list.transform);
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
        public void SetPortrait(Sprite img){
             panelImage.SetActive(true);
            panelImage.GetComponent<Image>().sprite=img;
        }
        public void SetBarTemperary(string s)
        {
            barContentTemp = s;
        }
        public void SetNetBar(string s){
            netContent=s;
        }
        public void PopMessage(int row,int col,string s){
            var pop=Instantiate(popupPrefab,Map.Instance.BlockSet[row,col].Top,Quaternion.identity);
            pop.GetComponent<Popup>().SetText(s);
        }
    }
}
