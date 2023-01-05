using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class RelicPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject relicButtons;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            var s=UIHandler.Instance.SelectedUnit;
            if(s==null){
                relicButtons.SetActive(false);
            }
            else if(s.RelicID==null||s.RelicID.Length==0){
                relicButtons.SetActive(true);
            }
            else{
                relicButtons.SetActive(false);
            }
        }
        public void SetRelic(int x){
            if(UIHandler.Instance.SelectedUnit==null)
                return;
            print($"Set relic {x}");
            Main.Instance.NetSend<NetAssignRelic>(new NetAssignRelic{
                Relic=Index2Relic(x).ToString(),
                ID=UIHandler.Instance.SelectedUnit.ID,
            });
        }
        private RelicType Index2Relic(int x)=>x switch{
            0=>RelicType.R0,
            1=>RelicType.R1,
            2=>RelicType.R2,
            _=>throw new NotImplementedException(),
        };
    }
}
