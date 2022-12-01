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
        public static Main Instance { get; private set; }
        public bool Connected
        {
            get => ws != null && ws.IsAlive;
        }
        private WebSocket ws;
        [SerializeField]
        private NetHandler handler;
        public readonly string IP = "localhost";
        public readonly int port = 7777;
        // Start is called before the first frame update
        void Start()
        {
            NetInit();
            Instance = this;
            NetSend(new NetStartGame(){
                SaveName="default.txt",
            });
        }

        private void NetInit() => Connect(IP, port);

        private void Connect(string ip, int port)
        {
            ws = new WebSocket($"ws://{ip}:{port}");
            ws.Connect();
            ws.OnMessage += (sender, e) => NetReceive(e.Data);
            print($"connected to {ip} {port}");
        }

        void NetReceive(string raw)
        {
            lock (handler.MessageList)
            {
                handler.MessageList.Add(raw);
            }
        }

        void NetSend<T>(T data)
        {
            if (Connected)
            {
                ws.Send($"{data.GetType().Name}@{JsonConvert.SerializeObject(data)}");
            }
            else
            {
                print("NetSend: connection closed");
            }
        }

        // Update is called once per frame
        void Update()
        {

            if (Connected)
            {
                if (Input.GetKeyDown(KeyCode.O))
                {
                    var msg = new ServerPrint()
                    {
                        content = "test msg"
                    };
                    NetSend(msg);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    NetInit();
                }
            }
        }
    }
}
