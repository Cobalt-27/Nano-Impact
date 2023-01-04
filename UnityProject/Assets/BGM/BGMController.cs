using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class BGMController : MonoBehaviour
    {
        public enum BGMType
        {
            Menu, Game
        }
        public static BGMController Instance { get; private set; }
        [SerializeField]
        public AudioClip DreamAria, Mondstadt;
        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void PlayBGM(BGMType t)
        {
            print($"play BGM {t}");
            switch (t)
            {
                case BGMType.Menu:
                    AudioController.Instance.PlayBGM(DreamAria);
                    break;
                case BGMType.Game:
                    AudioController.Instance.PlayBGM(Mondstadt);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
