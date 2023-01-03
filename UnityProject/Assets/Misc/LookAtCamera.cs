using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class LookAtCamera : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            transform.rotation=Camera.main.transform.rotation;
            // transform.rotation=Quaternion.Euler(0f,Camera.main.transform.rotation.eulerAngles.y,0f);
        }
    }
}
