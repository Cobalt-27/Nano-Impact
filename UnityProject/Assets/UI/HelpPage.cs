using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Nano
{
    public class HelpPage : MonoBehaviour
    {
        [SerializeField]
        private GameObject page;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKey(KeyCode.Space)){
                page.SetActive(true);
            }
            else{
                page.SetActive(false);
            }
        }
    }
}
