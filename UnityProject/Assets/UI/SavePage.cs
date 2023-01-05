using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Nano
{
    public class SavePage : MonoBehaviour
    {
        [SerializeField]
        private GameObject input;
        private static readonly string enableVarName="enable";
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Z)){
                PageActive(true);
            }
        }
        private void PageActive(bool active){
            Main.Instance.BoardState=active?Main.KeyBoardState.Save:Main.KeyBoardState.Game;
            gameObject.GetComponent<Animator>().SetBool(enableVarName,active);
        }
        public void OnConfirm(){
            var saveName=input.GetComponent<TextMeshProUGUI>().text;
            print($"save {saveName}");
            Main.Instance.NetSend<NetSave>(new NetSave{
                Name=saveName
            });
            PageActive(false);
        }
        public void OnCancel(){
            PageActive(false);
        }
    }
}
