using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using WebSocketSharp;
using System.Linq;

namespace Nano
{
    public class Main : MonoBehaviour
    {
        private WebSocket ws;
        // Start is called before the first frame update
        void Start()
        {
            NetInit("localhost", 7777);
        }

        private void NetInit(string ip, int port)
        {
            ws = new WebSocket($"ws://{ip}:{port}");
            ws.Connect();
            ws.OnMessage += (sender, e) => NetHandle(e.Data);
        }

        void NetHandle(string raw)
        {
            // print($"> {raw}");
            int idx=raw.IndexOf('@');
            string type=raw.Substring(0,idx);
            string data=raw.Substring(idx+1);
            print($"@ {type}");
            print($"> {data}");
        }

        void NetSend<T>(T data){
            ws.Send($"{data.GetType().Name}@{JsonConvert.SerializeObject(data)}");
        }

        // Update is called once per frame
        void Update()
        {

            if (ws == null)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var msg=new ServerPrint(){
                    content="test msg"
                };
                NetSend(msg);
            }
        }
    }
}
