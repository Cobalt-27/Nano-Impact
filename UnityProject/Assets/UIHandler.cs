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

        public void DeSelect()
        {
            selectType = SelectType.Empty;
            selected = null;
        }
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
            {
                print("select an object first");
                return;
            }
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
        public static double Distance(int row0,int col0,int row1,int col1){
            return Math.Sqrt(Math.Pow(row0-row1,2)+Math.Pow(col0-col1,2));
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
                    UIController.Instance.SetBarTemperary("Valid Target(s)");
                    Unit.All
                        .Where(u => u.Faction != unit.Faction && Distance(u.Row, u.Col, unit.Row, unit.Col) <= unit.Range)
                        .ToList().ForEach(u => u.Block.EnableOverlay = true);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    UIController.Instance.SetBarTemperary("Reachable");
                    Block.All
                        .Where(b => b.Unit == null && Distance(b.Row, b.Col, unit.Row, unit.Col) <= unit.Speed)
                        .ToList().ForEach(b => b.EnableOverlay = true);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    UIController.Instance.SetBarTemperary("Move");
                    Unit.All
                        .Where(u => u.CanMove)
                        .ToList().ForEach(u => u.Block.EnableOverlay = true);
                }
                else if (Input.GetKey(KeyCode.F))
                {
                    UIController.Instance.SetBarTemperary("Attack");
                    Unit.All
                        .Where(u => u.CanAttack)
                        .ToList().ForEach(u => u.Block.EnableOverlay = true);
                }
                else if(Input.GetKey(KeyCode.I)){
                    unit.PlayAttackAnim();
                }
                else if (unit != null)
                {
                    unit.Block.EnableOverlay = true;
                }

            }
        }
    }
}
