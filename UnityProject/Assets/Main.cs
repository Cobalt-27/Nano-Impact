using System;
using System.Net.NetworkInformation;
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
        #region Tags
        public static readonly string MapTag="Map";
        public static readonly string UnitTag="Unit";
        public static readonly string BuildingTag="Building";
        public static readonly string RelicTag="Relic";
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            InitGame();
        }

        void InitGame(){
            Connect(IP, port);
            NetSend(new NetStartGame(){
                SaveName="default.txt",
            });
        }


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

        public void NetSend<T>(T data)
        {
            if (Connected)
            {
                string type=data.GetType().Name;
                string json=JsonConvert.SerializeObject(data);
                print($"@ {type}");
                print($"< {json}");
                ws.Send($"{type}@{JsonConvert.SerializeObject(data)}");
            }
            else
            {
                print("NetSend failed: connection closed");
            }
        }
        public static int Distance(int row0,int col0,int row1,int col1){
            return Math.Abs(row0-row1)+Math.Abs(col0-col1);
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
                if(Input.GetKeyDown(KeyCode.Delete)){
                    NetSend(new NetEndRound());
                }
                if(Input.GetKeyDown(KeyCode.Backspace)){
                    NetSend(new NetRollback());
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    InitGame();
                }
            }
        }
    }
}
