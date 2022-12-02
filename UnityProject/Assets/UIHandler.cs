using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class UIHandler : MonoBehaviour
    {

        public static UIHandler Instance;


        public enum SelectType
        {
            Unit,
            Relic,
        }
        public enum TargetType
        {
            Block,
        }
        private Component selected = null;
        private SelectType selectType;

        public void Select(SelectType type, Component obj)
        {
            if (obj != selected)
            {
                print($"select {obj.name} {type}");
            }
            selectType = type;
            selected = obj;
        }
        public void Target(TargetType type, Component c)
        {
            print($"targetting {c.gameObject.name} {type} from {selected.name} {selectType}");
            switch(type){
                case TargetType.Block:
                    var block=c as Block;
                    Debug.Assert(block is not null);
                    if(selectType==SelectType.Unit){
                        Main.Instance.NetSend(new NetMove(){
                            ID=selected.gameObject.name,
                            Row=block.Row,
                            Col=block.Col,
                        });
                    }
                    break;
                default:
                    break;
            }
        }
        public void HandleUnit(){
            throw new NotImplementedException();
        }
        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
