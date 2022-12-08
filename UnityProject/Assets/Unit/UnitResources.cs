using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class UnitResources : MonoBehaviour
    {
        public GameObject Angelina,Eyjafjalla;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        public GameObject GetSprite(Character c)=> c switch{
            Character.Angelina=>Angelina,
            Character.Eyjafjalla=>Eyjafjalla,
            _=> throw new NotImplementedException(),
        };
    }
}
