using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Nano
{
    public class Map : MonoBehaviour
    {
        public static Map Instance;
        public Block[,] BlockSet=new Block[0,0];
        [SerializeField]
        private GameObject plainPrefab;
        [SerializeField]
        private GameObject hillsPrefab;
        [SerializeField]
        private GameObject mountainsPrefab;
        [SerializeField]
        private GameObject desertPrefab;
        [SerializeField]
        private GameObject emptyPrefab;

        public float f=10;

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
            gameObject.tag=Main.MapTag;
            Instance=this;
        }

        public void TestNetUpdate()
        {
            ServerSetMap args = new();
            args.Blocks = new NetBlock[40 * 20];
            args.Row = 40;
            args.Col = 20;
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    args.Blocks[i * 20 + j] = new NetBlock
                    {
                        Row = i,
                        Col = j,
                        Type = (BlockType)Random.Range(0, 5),
                    };
                }
            }
            NetUpdate(args);
        }

        // Update is called once per frame
        void Update()
        {
            
        }


        

        private Vector3 GetPosition(int row, int col, float height)
        {
            float sqrt3 = (float)System.Math.Sqrt(3);
            float colShift = row % 2 == 0 ? 0 : UnitLength * 1.5f;
            return new Vector3(sqrt3 * UnitLength * row / 2, height, col * UnitLength * 3 + colShift);
        }

        public void NetUpdate(ServerSetMap args)
        {
            Debug.Assert(args.Blocks.Length==args.Row*args.Col);
            Clear();
            BlockSet = new Block[args.Row, args.Col];
            foreach (var b in args.Blocks)
            {
                BlockSet[b.Row, b.Col] = CreateBlock(Type2Prefab(b.Type), b.Row, b.Col, b.Height);
            }
        }

        private void Clear()
        {
            foreach (Transform child in gameObject.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private Block CreateBlock(GameObject prefab, int row, int col, float height)
        {
            var instance = Instantiate(prefab, GetPosition(row, col, height), Quaternion.identity);
            instance.transform.RotateAround(instance.transform.position, Vector3.up, 60f*Random.Range(0,6));
            instance.transform.SetParent(gameObject.transform);
            var block = instance.AddComponent<Block>();
            block.Init(row, col);
            return block;
        }

        private GameObject Type2Prefab(BlockType type) => type switch
        {
            BlockType.Plain => plainPrefab,
            BlockType.Hills => hillsPrefab,
            BlockType.Mountains => mountainsPrefab,
            BlockType.Desert => desertPrefab,
            BlockType.Empty => emptyPrefab,
            _ => throw new System.NotImplementedException(),
        };


    }
}
