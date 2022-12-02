using System;
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class Block : MonoBehaviour
    {
        public static IEnumerable<Block> AllBlockScripts=>GameObject.FindObjectsOfType<Block>();
        public GameObject Overlay;
        public bool EnableOverlay;
        public int Row { get; private set; }
        public int Col { get; private set; }
        public BlockType BlockType{get;private set;}
        public Vector3 Top{
            get=>gameObject.transform.Find("Top").transform.position;
        }
        public Nano.Unit Unit
        {
            get => Nano.Unit.AllUnitScripts.FirstOrDefault(u => u.Row == Row && u.Col == Col);
        }
        // Start is called before the first frame update
        void Start()
        {
            foreach (Transform child in gameObject.transform)
            {
                var obj = child.gameObject;
                if(obj.name=="Top")
                    continue;
                var mesh = obj.GetComponent<MeshFilter>().sharedMesh;
                obj.AddComponent<MeshCollider>().sharedMesh = mesh;
                var listener = obj.AddComponent<MouseListener>();
                listener.Left = OnLeftClick;
                listener.Right = OnRightClick;
            }

        }

        public void Init(int row, int col,BlockType type)
        {
            Row = row;
            Col = col;
            BlockType=type;
        }

        void OnLeftClick()
        {
            print($"left clicking {Row} {Col}");
            Unit?.OnSelected();
        }

        void OnRightClick()
        {
            print($"right clicking {Row} {Col}");
            UIHandler.Instance.Target(UIHandler.TargetType.Block, this);
        }


        // Update is called once per frame
        void Update()
        {
            if(BlockType!=BlockType.Empty)
                Overlay.SetActive(EnableOverlay);
        }
    }
}