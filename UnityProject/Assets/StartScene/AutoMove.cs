using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class AutoMove : MonoBehaviour
    {
        [SerializeField]
        private float speed=0.05f;
        [SerializeField]
        private int lifeSpan=600;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            gameObject.transform.Translate(new Vector3(0,0,speed));
            lifeSpan--;
            if(lifeSpan<=0){
                Destroy(gameObject);
            }
        }
    }
}
