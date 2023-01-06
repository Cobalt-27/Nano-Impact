using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nano
{
    public class MainPanel : MonoBehaviour
    {
        private Animator animator=>gameObject.GetComponent<Animator>();
        
        // Start is called before the first frame update
        void Start()
        {
            animator.SetBool("MainClose", true);
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                var closed=animator.GetBool("MainClose");
                animator.SetBool("MainClose", !closed);
                gameObject.SetActive(!closed);
            }
        }
    }
}