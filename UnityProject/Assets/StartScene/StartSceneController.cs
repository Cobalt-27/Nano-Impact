using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class StartSceneController : MonoBehaviour
    {
        [SerializeField]
        private GameObject block;
        [SerializeField]
        private int interval=120;
        private int counter=0;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        void FixedUpdate()
        {
            counter++;
            if(counter%interval==0){
                Instantiate(block,Vector3.zero,Quaternion.identity);
            }
        }
    }
}
