using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nano
{
    public class MainPanel : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                var animator = gameObject.GetComponent<Animator>();
                animator.SetBool("MainOpen", !animator.GetBool("MainOpen"));
            }
        }
    }
}