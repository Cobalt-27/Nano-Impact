using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
namespace Nano
{
    public class NetHandler : MonoBehaviour
    {
        [SerializeField]
        private GameObject Map;
        public List<string> MessageList;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            lock (MessageList)
            {
                foreach (var raw in MessageList)
                {
                    int idx = raw.IndexOf('@');
                    string type = raw.Substring(0, idx);
                    string json = raw.Substring(idx + 1);
                    print($"@ {type}");
                    print($"> {json}");
                    Dispatch(type,json);
                }
                MessageList.Clear();
            }
        }
        private T To<T>(string data) => JsonConvert.DeserializeObject<T>(data);
        private void Dispatch(string name, string json)
        {
            switch (name)
            {
                case "ServerSetMap":
                    Map.GetComponent<Map>().NetUpdate(To<ServerSetMap>(json));
                    break;
                case "ServerSetUnits":

                    break;
                case "ServerSetRelics":
                    break;
                case "ServerSetBuildings":
                    break;
                case "ClientPrint":
                    var data = To<ClientPrint>(json);
                    print(data.content);
                    break;
                case "ServerEndGame":
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void Remove(string tag, IEnumerable<string> except)
        {
            var objList = GameObject.FindGameObjectsWithTag(tag).ToList();
            objList.Where(o => !except.Contains(o.name)).ToList().ForEach(o => Destroy(o));
        }


        private void HandleSetUnits(ServerSetUnits args)
        {
            Remove("Unit", args.Units.Select(u => u.ID));
        }
        private void HandleSetRelics(ServerSetRelics args)
        {
            Remove("Relic", args.Relics.Select(u => u.ID));
            // Clean("Relic",netRelics.Select(r=>r.ID));
        }

        private void HandleSetBuildings(ServerSetBuildings args)
        {
            Remove("Building", args.Buildings.Select(u => u.ID));
            // Clean("Building",netBuildings.Select(b=>b.ID));
        }
    }
}
