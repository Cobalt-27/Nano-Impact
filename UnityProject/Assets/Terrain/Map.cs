using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Nano
{
    public class Map : MonoBehaviour
    {
        public Block[,] BlockSet;
        [SerializeField]
        private GameObject plainPrefab;
        [SerializeField]
        private GameObject hillsPrefab;
        [SerializeField]
        private GameObject mountainsPrefab;
        [SerializeField]
        private GameObject desertPrefab;
        private GameObject emptyPrefab;


        public int RowCount
        {
            get => BlockSet.GetLength(0);
        }
        public int ColCount
        {
            get => BlockSet.GetLength(1);
        }

        public float UnitLength = 1.3f;
        // Start is called before the first frame update
        void Start()
        {
            // TerrainCreateArgs args=new();
            // args.Types=new BlockType[40,20];
            // args.Height=new float[40,20];
            // for(int i=0;i<40;i++){
            //     for(int j=0;j<20;j++){
            //         args.Types[i,j]=(BlockType)Random.Range(0,4);
            //         args.Height[i,j]=Random.Range(0,0.3f);
            //     }
            // }
            // Generate(args);
        }

        public void NetUpdate(SetMap e)
        {
            Generate(args);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Generate(SetMap args)
        {

            BlockSet = new Block[args.Row,args.Col];
            foreach(var b in args.Blocks){
                BlockSet[b.Row,b.Col]=CreateBlock(Type2Prefab(b.Type), b.Row, b.Col, b.Height);
            }

            Debug.Assert(args.Blocks.GetLength(0) == args.Heights.GetLength(0) && args.Blocks.GetLength(1) == args.Heights.GetLength(1));
            
            
            for (int i = 0; i < args.Blocks.GetLength(0); i++)
            {
                for (int j = 0; j < args.Blocks.GetLength(1); j++)
                {
                    BlockSet[i, j] = CreateBlock(Type2Prefab(args.Blocks[i, j]), i, j, args.Heights[i, j]);
                }
            }
        }

        public Block CreateBlock(GameObject prefab, int row, int col, float height)
        {
            var instance = Instantiate(prefab, GetPosition(row, col, height), Quaternion.identity);
            instance.transform.SetParent(gameObject.transform);
            var block = instance.AddComponent<Block>();
            foreach (Transform child in instance.transform)
            {
                var obj = child.gameObject;
                var mesh = obj.GetComponent<MeshFilter>().sharedMesh;
                obj.AddComponent<MeshCollider>().sharedMesh = mesh;
                var listener = obj.AddComponent<MouseListener>();
                listener.Init(block.onClick);
            }
            block.Init(row, col);
            return block;
        }

        private GameObject Type2Prefab(BlockType type) => type switch
        {
            BlockType.Plain => plainPrefab,
            BlockType.Hills => hillsPrefab,
            BlockType.Mountains => mountainsPrefab,
            BlockType.Desert => desertPrefab,
            BlockType.Empty => new GameObject("EmptyBlock"),
            _ => throw new System.NotImplementedException(),
        };

        private Vector3 GetPosition(int row, int col, float height)
        {
            float sqrt3 = (float)System.Math.Sqrt(3);
            float colShift = row % 2 == 0 ? 0 : UnitLength * 1.5f;
            return new Vector3(sqrt3 * UnitLength * row / 2, height, col * UnitLength * 3 + colShift);
        }


    }
}
