using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
namespace Nano
{
    public class Dispatcher : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        private T To<T>(string data) => JsonConvert.DeserializeObject<T>(data);
        public void Dispatch(string name, string json)
        {
            switch (name)
            {
                case "SetMap":
                    FindObjectOfType<Map>().NetUpdate(To<SetMap>(json));
                    break;
                case "SetUnits":
                    break;
                case "SetRelics":
                    break;
                case "SetBuildings":
                    break;
                case "Log":
                    var data=To<Log>(json);
                    print(data.content);
                    break;
                case "EndGame":
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
