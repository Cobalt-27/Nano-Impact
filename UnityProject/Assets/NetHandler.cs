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
        private GameObject buildingPrefab;
        public List<string> MessageList;
        // Start is called before the first frame update
        private GameObject supervisedRoot;
        void Start()
        {
            supervisedRoot = new GameObject("Supervised");
        }

        // Update is called once per frame
        void Update()
        {
            lock (MessageList)
            {
                foreach (var raw in MessageList)
                {
                    try
                    {
                        int idx = raw.IndexOf('@');
                        string type = raw.Substring(0, idx);
                        string json = raw.Substring(idx + 1);
                        print($"@ {type}");
                        print($"< {json}");
                        Dispatch(type, json);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
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
                    Main.Instance.GameSceneSetActive(false);
                    mapInstance.GetComponent<Map>().NetUpdate(To<ServerSetMap>(json));
                    break;
                case "ServerSetUnits":
                    OnSetUnits(To<ServerSetUnits>(json));
                    break;
                case "ServerSetBuildings":
                    OnSetBuildings(To<ServerSetBuildings>(json));
                    break;
                case "ClientPrint":
                    print(To<ClientPrint>(json).Content);
                    break;
                case "ServerEndGame":
                    GameEndPage.Instance.ShowGameEnd(To<ServerEndGame>(json).Winner);
                    Main.Instance.GameSceneSetActive(false);
                    ClearSupervised();
                    break;
                case "ClientShow":
                    var fac = To<ClientShow>(json).Content;
                    string msg = null;
                    msg = fac;
                    UIController.Instance.SetNetBar(msg);
                    break;
                case "NetPlayAttack":
                    OnAttack(To<NetPlayAttack>(json).ID);
                    break;
                case "NetSetSaveInfo":
                    StartSceneController.Instance.SetSaveInfo(To<NetSetSaveInfo>(json));
                    break;
                case "ServerStartGame":
                    Main.Instance.GameSceneSetActive(true);
                    var start = To<ServerStartGame>(json);
                    Main.Instance.MyFaction = start.Faction;
                    Main.Instance.GameMode = start.GameMode;
                    print($"Game start, mode={start.GameMode}, faction={start.Faction}");
                    break;
                case "PopMessage":
                    var data = To<PopMessage>(json);
                    UIController.Instance.PopMessage(data.Row, data.Col, data.Content);
                    break;
                default:
                    return;
                    throw new NotImplementedException();
            }
        }

        private void OnAttack(string id)
        {
            Unit.All.First(u => u.name == id).PlayAttackAnim();
        }

        private void Remove(string tag, IEnumerable<string> exclude)
        {
            var objList = GameObject.FindGameObjectsWithTag(tag).ToList();
            objList.Where(o => !exclude.Contains(o.name)).ToList().ForEach(o => Destroy(o));
        }

        private IEnumerable<GameObject> CreateIfNeeded(string tag, IEnumerable<string> names, GameObject prefab)
        {
            var inited = new List<GameObject>();
            var objList = GameObject.FindGameObjectsWithTag(tag).ToList();
            var toInit = names.Except(objList.Select(o => o.name));
            foreach (var name in toInit)
            {
                var o = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                o.name = name;
                o.transform.SetParent(supervisedRoot.transform);
                inited.Add(o);
            }
            return inited;
        }
        private void ClearSupervised()
        {
            var empty = new List<string>();
            Remove(Main.UnitTag, empty);
            Remove(Main.BuildingTag,empty);
            // Remove(Main.bui)
        }
        private void OnSetUnits(ServerSetUnits args)
        {
            var names = args.Units.Select(u => u.ID);
            Remove(Main.UnitTag, names);
            CreateIfNeeded(Main.UnitTag, names, unitPrefab);
            foreach (var u in args.Units)
            {
                GameObject.Find(u.ID).GetComponent<Unit>().NetUpdate(u);
            }
        }

        private void OnSetBuildings(ServerSetBuildings args)
        {
            var names = args.Buildings.Select(u => u.ID).ToList();
            Remove(Main.BuildingTag, names);
            CreateIfNeeded(Main.BuildingTag, names, buildingPrefab);
            foreach (var b in args.Buildings)
            {
                GameObject.Find(b.ID).GetComponent<Building>().NetUpdate(b);
            }
        }
    }
}
