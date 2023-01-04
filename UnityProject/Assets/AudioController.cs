using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class AudioController : MonoBehaviour
    {
        public static AudioController Instance{get;private set;}
        public AudioSource Source{get;private set;}
        [SerializeField]
        private GameObject bgmObject;
        private AudioSource bgmSource;
        // Start is called before the first frame update
        void Start()
        {
            Instance=this;
            Source=gameObject.GetComponent<AudioSource>();
            bgmSource=bgmObject.GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        public void PlayBGM(AudioClip clip){
            bgmSource.clip=clip;
            bgmSource.Play();
        }
    }
}
