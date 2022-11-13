using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class MouseListener : MonoBehaviour
    {
        private Action action;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
        void OnMouseDown()
        {
            action();
        }
        public void Init(Action onClick)
        {
            this.action = onClick;
        }
    }
}