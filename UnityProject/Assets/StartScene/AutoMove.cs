using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class AutoMove : MonoBehaviour
    {
        [SerializeField]
        private float speed = 0.05f;
        [SerializeField]
        private int destroyWhenBelow = 0;
        public bool Enabled = true;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (Enabled)
            {
                gameObject.transform.Translate(new Vector3(0, -speed, 0));
                if(gameObject.transform.position.y<destroyWhenBelow){
                    Destroy(gameObject);
                }
            }
        }
    }
}
