using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class SubCamera : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var c=gameObject.GetComponent<Camera>();
            c.clearFlags=CameraClearFlags.Depth;
            c.depth=10;
            // Camera.main.clearFlags=CameraClearFlags.Depth;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
