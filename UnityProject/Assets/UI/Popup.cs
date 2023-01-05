using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Nano
{
    public class Popup : MonoBehaviour
    {
        [SerializeField]
        private int lifeSpan;
        [SerializeField]
        private GameObject textObject;
        // Start is called before the first frame update
        void Start()
        {
            // Destroy(gameObject,lifeSpan);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        public void SetText(string s){
            textObject.GetComponent<TextMeshProUGUI>().text=s;
        }
    }
}
