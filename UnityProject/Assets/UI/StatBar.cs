using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nano
{
    public class StatBar : MonoBehaviour
    {
        [SerializeField]
        private GameObject textObject;
        [SerializeField]
        private GameObject bar;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        public void SetText(string text){
            textObject.GetComponent<TextMeshProUGUI>().text=text;
        }
        public void SetValue(float value){
            if(value<0)
                value=0;
            if(value>1)
                value=1;
            bar.GetComponent<Image>().fillAmount=value;
        }
    }
}
