using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class Unit : MonoBehaviour
    {
        public Character Character{get;private set;}
        public UnitType Type{get;private set;}
        public int Row{get;private set;}
        public int Col{get;private set;}
        public int Strengh{get;private set;}
        public int Defence{get;private set;}
        public int Life{get;private set;}
        public int Range{get;private set;}
        public int Speed{get;private set;}
        public int Exp{get;private set;}
        public bool CanMove{get;private set;}
        public bool CanAttack{get;private set;}
        private string RelicID;
        
        public Faction Faction{get;private set;}
        private GameObject display=null;
        [SerializeField]
        private float yOffset=2.6f;

        public static IEnumerable<Unit> AllUnitScripts=>GameObject.FindGameObjectsWithTag(Main.UnitTag).Select(g=>g.GetComponent<Unit>());


        // Start is called before the first frame update
        void Start()
        {
            gameObject.tag=Main.UnitTag;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        public void NetUpdate(NetUnit data){
            Character=data.Character;
            Type=data.Type;
            Row=data.Row;
            Col=data.Col;
            Strengh=data.Strengh;
            Defence=data.Defence;
            Life=data.Life;
            Range=data.Range;
            Exp=data.Exp;
            CanMove=data.CanMove;
            CanAttack=data.CanAttack;
            RelicID=data.RelicID;
            Faction=data.Faction;
            Speed=data.Speed;

            var moveTo=Map.Instance.BlockSet[Row,Col].transform.position;
            gameObject.transform.position=moveTo;
            print(display==null);
            if(display==null){
                SetSprite();
            }
        }
        private void SetSprite(){
            var prefab=Resource.Instance.GetCharacterPrefab(Character);
            display = Instantiate(prefab, gameObject.transform.position+Vector3.up*yOffset, Quaternion.identity);
            display.transform.SetParent(gameObject.transform);
        }
        public void OnSelected(){
            UIHandler.Instance.Select(UIHandler.SelectType.Unit,this);
        }
        public void OnSetAsTarget(){
            // UIHandler.Instance.Target(UIHandler.TargetType.Unit)
        }
    }
}
