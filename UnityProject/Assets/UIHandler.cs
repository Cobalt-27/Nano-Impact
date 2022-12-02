using System.Linq;
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
            Empty,
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
            if (selected == null)
                return;
            print($"targetting {c.gameObject.name} {type} from {selected.name} {selectType}");
            switch (type)
            {
                case TargetType.Block:
                    var block = c as Block;
                    Debug.Assert(block != null);
                    if (selectType == SelectType.Unit)
                    {
                        var from = selected as Unit;
                        Debug.Assert(from != null);
                        if (block.Unit != null)
                        {
                            Main.Instance.NetSend(new NetInteract()
                            {
                                From = from.name,
                                To = block.Unit.name,
                            });
                        }
                        else
                        {
                            Main.Instance.NetSend(new NetMove()
                            {
                                ID = selected.gameObject.name,
                                Row = block.Row,
                                Col = block.Col,
                            });
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public void HandleUnit()
        {
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
            if (selectType == SelectType.Unit && selected as Unit != null)
            {
                var unit = selected as Unit;
                GameObject.FindObjectsOfType<Block>().ToList().ForEach(
                    b => b.EnableOverlay = false
                );
                if (Input.GetKey(KeyCode.A))
                {
                    Block.AllBlockScripts
                        .Where(b => b.Unit != null && Main.Distance(b.Unit.Row, b.Unit.Col, unit.Row, unit.Col) <= unit.Range)
                        .ToList().ForEach(b => b.EnableOverlay = true);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    Block.AllBlockScripts
                        .Where(b => b.Unit == null && Main.Distance(b.Row, b.Col, unit.Row, unit.Col) <= unit.Speed)
                        .ToList().ForEach(b => b.EnableOverlay = true);
                }
                else
                {
                    if (unit != null)
                        GameObject.FindObjectsOfType<Block>()
                            .First(b => b.Col == unit.Col && b.Row == unit.Row).EnableOverlay = true;
                }
            }
        }
    }
}
