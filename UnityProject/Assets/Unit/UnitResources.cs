using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class UnitResources : MonoBehaviour
    {
        public static UnitResources Instance{get;private set;}
        public GameObject Angelina,Eyjafjalla,Amiya,Skadi,Kaltsit,Surtr,BarRed,BarBlue;
        // Start is called before the first frame update
        void Start()
        {
            Instance=this;
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        public GameObject GetSprite(Character c)=> c switch{
            Character.Angelina=>Angelina,
            Character.Eyjafjalla=>Eyjafjalla,
            Character.Amiya=>Amiya,
            Character.Skadi=>Skadi,
            Character.Kaltsit=>Kaltsit,
            Character.Surtr=>Surtr,
            _=> throw new NotImplementedException(),
        };
        public GameObject GetHealthBar(Faction f)=>f switch{
            Faction.Blue=>BarBlue,
            Faction.Red=>BarRed,
            _=>throw new NotImplementedException(),
        };
    }
}
