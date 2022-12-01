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
        private GameObject mapInstance;
        [SerializeField]
        private GameObject unitPrefab;
        [SerializeField]
        private GameObject relicPrefab;
        [SerializeField]
        private GameObject buildingPrefab;
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
                    mapInstance.GetComponent<Map>().NetUpdate(To<ServerSetMap>(json));
                    break;
                case "ServerSetUnits":
                    OnSetUnits(To<ServerSetUnits>(json));
                    break;
                case "ServerSetRelics":
                    OnSetRelics(To<ServerSetRelics>(json));
                    break;
                case "ServerSetBuildings":
                    OnSetBuildings(To<ServerSetBuildings>(json));
                    break;
                case "ClientPrint":
                    var content = To<ClientPrint>(json);
                    print(content.content);
                    break;
                case "ServerEndGame":
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void Remove(string tag, IEnumerable<string> exclude)
        {
            var objList = GameObject.FindGameObjectsWithTag(tag).ToList();
            objList.Where(o => !exclude.Contains(o.name)).ToList().ForEach(o => Destroy(o));
        }

        private IEnumerable<GameObject> CreateIfNeeded(string tag,IEnumerable<string> names,GameObject prefab){
            var inited=new List<GameObject>();
            var objList = GameObject.FindGameObjectsWithTag(tag).ToList();
            var toInit=names.Except(names);
            foreach(var name in toInit){
                var o=Instantiate(prefab,Vector3.zero,Quaternion.identity);
                o.name=name;
                inited.Add(o);
            }
            return inited;
        }

        private void OnSetUnits(ServerSetUnits args)
        {
            var tag="Unit";
            var names=args.Units.Select(u => u.ID);
            Remove(tag, names);
            CreateIfNeeded(tag, names,unitPrefab);
            
        }
        private void OnSetRelics(ServerSetRelics args)
        {
            var tag="Relic";
            var names=args.Relics.Select(u => u.ID);
            Remove(tag, names);
            CreateIfNeeded(tag,names,relicPrefab);
        }

        private void OnSetBuildings(ServerSetBuildings args)
        {
            var tag="Building";
            var names=args.Buildings.Select(u => u.ID);
            Remove(tag, names);
            CreateIfNeeded(tag,names,buildingPrefab);
        }
    }
}
