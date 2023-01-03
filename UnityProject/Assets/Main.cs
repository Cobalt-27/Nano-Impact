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
        [SerializeField]
        private GameObject gameLight;
        [SerializeField]
        private GameObject menuRoot;
        [SerializeField]
        private GameObject gameCanvas;
        [SerializeField]
        private GameObject gameCamera;
        [SerializeField]
        private GameObject menuCamera;
        [SerializeField]
        private Material menuSkyBox;
        [SerializeField]
        private Material gameSkyBox;

        private bool gameStart=false;


        public readonly string IP = "localhost";
        public readonly int port = 7777;
        public string ClientName{get;private set;}

        #region Tags
        public static readonly string MapTag="Map";
        public static readonly string UnitTag="Unit";
        public static readonly string BuildingTag="Building";
        public static readonly string RelicTag="Relic";
        public static readonly string Untagged="Untagged";
        public static readonly string MainCameraTag="MainCamera";
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            ClientName=DateTime.UtcNow.ToString();
            Instance = this;
            GameSceneSetActive(false);
        }

        void OnDestroy(){
            if(Connected){
                print("disconnect");
                ws.Close();
            }
        }

        public void GameSceneSetActive(bool active){
            gameStart=active;
            gameCamera.SetActive(active);
            gameLight.SetActive(active);
            menuRoot.SetActive(!active);
            gameCanvas.SetActive(active);
            RenderSettings.skybox=active?gameSkyBox:menuSkyBox;
        }


        public void Connect(string ip, int port)
        {
            print($"connecting to {ip} {port}");
            ws = new WebSocket($"ws://{ip}:{port}");
            ws.Connect();
            ws.OnMessage += (sender, e) => NetReceive(e.Data);
            print($"connected");
            NetSend(new NetGreet(){
                ClientName=ClientName,
            });
        }

        void NetReceive(string raw)
        {
            lock (handler.MessageList)
            {
                handler.MessageList.Add(raw);
            }
        }


        public void SetWebSocket(WebSocket ws)=>this.ws=ws;
        public void NetSend<T>(T data)
        {
            if (Connected)
            {
                string type=data.GetType().Name;
                string json=JsonConvert.SerializeObject(data);
                print($"@ {type}");
                print($"> {json}");
                ws.Send($"{type}@{json}");
            }
            else
            {
                print("NetSend failed: connection closed");
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
                if(Input.GetKeyDown(KeyCode.Delete)){
                    NetSend(new NetEndRound());
                }
                if(Input.GetKeyDown(KeyCode.Backspace)){
                    NetSend(new NetRollback());
                }
            }
        }
    }
}
