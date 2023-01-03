using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nano
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private GameObject bar;
        public int Health;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            bar.GetComponent<Image>().fillAmount = ((float)Health) / Main.MaxHealth;
        }
    }
}
