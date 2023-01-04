using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Nano
{
    public class GamePausePage : MonoBehaviour
    {
        enum PausePageMode{
            Save,End
        }
        [SerializeField]
        private GameObject title;
        [SerializeField]
        private GameObject subTitle;
        [SerializeField]
        private GameObject panel;
        [SerializeField]
        private GameObject input;
        private PausePageMode mode=PausePageMode.End;
        public static GamePausePage Instance{get;private set;}
        public static readonly string PanelEnable = "enable";
        public static readonly string WinMessage = "You Win :)";
        public static readonly string LoseMessage = "You Lose :(";
        public static readonly string EndSubtitle="Press any key to continue";
        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.anyKeyDown && mode==PausePageMode.End)
            {
                SetActive(false);
            }
            else if(Input.GetKeyDown(KeyCode.Return) &&mode==PausePageMode.Save){
                SetActive(false);
                Main.Instance.NetSend<NetSave>(new NetSave{
                    Name=input.GetComponent<TextMeshProUGUI>().text
                });
            }
        }
        private void SetTitle(string s)
        {
            title.GetComponent<TextMeshProUGUI>().text = s;
        }
        private void SetSubTitle(string s){
            subTitle.GetComponent<TextMeshProUGUI>().text = s;
        }
        private void SetActive(bool active)
        {
            panel.GetComponent<Animator>().SetBool(PanelEnable, active);
        }
        public void ShowGameEnd(Faction win)
        {
            mode=PausePageMode.End;
            print($"Game end page winning={win}");
            SetActive(true);
            SetSubTitle(EndSubtitle);
            if (Main.Instance.GameMode == GameMode.Singleplay)
            {
                SetTitle($"{win.ToString()} Wins!");
            }
            else
            {
                SetTitle(win == Main.Instance.MyFaction ? WinMessage : LoseMessage);
            }
        }
        
    }
}
