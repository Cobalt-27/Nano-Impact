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
        public Unit SelectedUnit { get; private set; } = null;

        public void DeSelect()
        {
            SelectedUnit = null;
        }
        public void Select(Unit obj)
        {
            if (obj != SelectedUnit)
            {
                print($"select {obj.name}");
            }
            SelectedUnit = obj;
        }
        public void Target(Block block)
        {
            if (SelectedUnit == null)
            {
                print("select an object first");
                return;
            }
            print($"targetting {block.gameObject.name} from {SelectedUnit.name}");
            Debug.Assert(block != null);
            var from = SelectedUnit as Unit;
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
                    ID = SelectedUnit.gameObject.name,
                    Row = block.Row,
                    Col = block.Col,
                });
            }
        }
        public static double Distance(int row0, int col0, int row1, int col1)
        {
            return Math.Sqrt(Math.Pow(row0 - row1, 2) + Math.Pow(col0 - col1, 2));
        }
        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            if (Main.Instance.BoardState != Main.KeyBoardState.Game)
                return;
            if (SelectedUnit != null)
            {
                var unit = SelectedUnit as Unit;
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
                else if (Input.GetKey(KeyCode.I))
                {
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
