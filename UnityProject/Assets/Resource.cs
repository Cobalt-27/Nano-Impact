using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class Resource : MonoBehaviour
    {
        #region Building Prefabs
        public GameObject Statue;
        public GameObject Church;
        #endregion
        #region UnitPrefabs
        public GameObject Skadi;
        public GameObject Amiya;
        #endregion
        public static Resource Instance;
        // Start is called before the first frame update
        void Start()
        {
            Instance=this;
        }

        public GameObject GetCharacterPrefab(Character c)=>c switch{
            Character.Skadi=>Skadi,
            Character.Amiya=>Amiya,
            _=>throw new NotImplementedException(),
        };
    }
}
