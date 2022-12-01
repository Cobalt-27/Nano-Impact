using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class Building : MonoBehaviour
    {
        public BuildingType Type{get;private set;}
        public Faction Faction{get;private set;}
        [SerializeField]
        private GameObject statuePrefab;
        [SerializeField]
        private GameObject churchPrefab;
        [SerializeField]
        private float statueY=0.3f;
        [SerializeField]
        private float churchY=0.2f;
        // Start is called before the first frame update
        void Start()
        {
            
        }
        public void NetUpdate(NetBuilding data){
            var pos=Map.Instance.BlockSet[data.Row,data.Col].transform.position;
            gameObject.transform.position=pos;
            Type=data.Type;
            Faction=data.Faction;
            GameObject prefab=null;
            float y=0f;
            switch(Type){
                case BuildingType.Statue:
                    prefab=statuePrefab;
                    y=statueY;
                    break;
                case BuildingType.Church:
                    prefab=statuePrefab;
                    y=churchY;
                    // return;
                    break;
                default:
                    throw new NotImplementedException();
            }
            var instance = Instantiate(prefab, new Vector3(pos.x,pos.y+y,pos.z), Quaternion.identity);
            instance.transform.RotateAround(instance.transform.position, Vector3.up, 60f*UnityEngine.Random.Range(0,6));
            instance.transform.SetParent(gameObject.transform);
        }
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
