using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class MouseListener : MonoBehaviour
    {
        public Action Left;
        public Action Right;
        public Action Middle;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
        void OnMouseOver() 
        {
            if(Input.GetMouseButtonDown(0))
                Left?.Invoke();
            if(Input.GetMouseButtonDown(1))
                Right?.Invoke();
            if(Input.GetMouseButtonDown(2))
                Middle?.Invoke();
        }
    }
}