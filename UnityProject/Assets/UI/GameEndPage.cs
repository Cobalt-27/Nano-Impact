using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Nano
{
    public class GameEndPage : MonoBehaviour
    {
        [SerializeField]
        private GameObject title;
        [SerializeField]
        private GameObject panel;
        public static GameEndPage Instance;
        public static readonly string PanelEnable="enable";
        public static readonly string WinMessage="You Win :)";
        public static readonly string LoseMessage="You Lose :(";
        // Start is called before the first frame update
        void Start()
        {
            Instance=this;
            SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.anyKeyDown){
                SetActive(false);
            }
        }
        private void SetTitle(string s){
            title.GetComponent<TextMeshProUGUI>().text=s;
        }
        private void SetActive(bool active){
            panel.GetComponent<Animator>().SetBool(PanelEnable,active);
        }
        public void Show(bool win){
            print($"Game end page winning={win}");
            SetActive(true);
            SetTitle(win?WinMessage:LoseMessage);
        }
    }
}
