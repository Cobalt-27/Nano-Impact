using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Nano
{
    public class Map : MonoBehaviour
    {
        public static Map Instance;
        public Block[,] BlockSet = new Block[0, 0];
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
        [SerializeField]
        private GameObject overlayPrefab;

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
            gameObject.tag = Main.MapTag;
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }




        private Vector3 GetPosition(int row, int col, float height)
        {
            return new Vector3(UnitLength * row, height, col * UnitLength);
        }

        public void NetUpdate(ServerSetMap args)
        {
            Debug.Assert(args.Blocks.Length == args.Row * args.Col);
            Clear();
            BlockSet = new Block[args.Row, args.Col];
            foreach (var b in args.Blocks)
            {
                BlockSet[b.Row, b.Col] = CreateBlock(Type2Prefab(b.Type), b.Row, b.Col, b.Height, b.Type);
            }
        }

        private void Clear()
        {
            foreach (Transform child in gameObject.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private Block CreateBlock(GameObject prefab, int row, int col, float height, BlockType type)
        {
            var instance = Instantiate(prefab, GetPosition(row, col, 0), Quaternion.identity);
            if (type != BlockType.Empty)
                instance.transform.RotateAround(instance.transform.position, Vector3.up, 90f * Random.Range(0, 4));
            instance.transform.SetParent(gameObject.transform);
            var block = instance.AddComponent<Block>();
            block.Init(row, col,type);
            if (type != BlockType.Empty)
            {
                var overlay = Instantiate(overlayPrefab, block.Top, Quaternion.identity);
                overlay.transform.SetParent(gameObject.transform);
                overlay.SetActive(false);
                block.Overlay = overlay;
            }
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
