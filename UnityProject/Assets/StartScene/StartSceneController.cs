using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Nano
{
    public class StartSceneController : MonoBehaviour
    {
        public static StartSceneController Instance;
        [SerializeField]
        private GameObject block;
        [SerializeField]
        private int interval = 120;
        [SerializeField]
        private GameObject input;
        [SerializeField]
        private GameObject infoName;
        private int counter = 0;
        private readonly int port = 7777;
        private List<NetSaveInfo> infoList = new List<NetSaveInfo>();
        private NetSaveInfo selected
        {
            get
            {
                int n=infoList.Count;
                if (infoList.Count == 0)
                    return null;
                return infoList[((pointer % n)+n)%n];
            }
        }
        private int pointer;
        // Start is called before the first frame update
        void Start()
        {
            Instance = this;

        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                string ip = input.GetComponent<TextMeshProUGUI>().text;
                ip = ip.Substring(0, ip.Length - 1);
                Main.Instance.Connect(ip, port);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                pointer--;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                pointer++;
            }
            if (Input.GetKeyDown(KeyCode.F1) && selected != null)
            {
                Main.Instance.NetSend<NetStartGame>(new NetStartGame
                {
                    SaveName = selected.Name,
                    Load = true,
                });
            }
            var name = selected == null ? "..." : selected.Name;
            infoName.GetComponent<TextMeshProUGUI>().text = name;
        }

        public void SetSaveInfo(NetSetSaveInfo info)
        {
            print($"save count: {info.SaveInfoList.Length}");
            infoList.Clear();
            foreach (var x in info.SaveInfoList)
            {
                infoList.Add(x);
            }
        }

        void FixedUpdate()
        {
            counter++;
            if (counter % interval == 0)
            {
                Instantiate(block, gameObject.transform.position, Quaternion.identity);
            }
        }
    }
}
