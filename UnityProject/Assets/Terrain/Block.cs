using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class Block : MonoBehaviour
    {
        public int Row { get; private set; }
        public int Col { get; private set; }
        // Start is called before the first frame update
        void Start()
        {

        }

        public void Init(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public void onClick()
        {
            print($"click on block ({Row},{Col})");
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}