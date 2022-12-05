using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class UnitResources : MonoBehaviour
    {
        [SerializeField]
        public readonly string CharacterClipPath = "Unit/Characters";
        [SerializeField]

        private GameObject spritePrefab;
        private readonly string spriteName="CharacterSprite";
        private enum ClipType
        {
            Idle, Attack, Interact, Move
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public GameObject CreateCharacter(Character c)
        {
            var obj = Instantiate(spritePrefab, Vector3.zero, Quaternion.identity);
            var animator = obj.transform.Find(spriteName).GetComponent<Animator>();
            AnimatorOverrideController o = new(animator.runtimeAnimatorController);
            foreach (ClipType clipType in (ClipType[])Enum.GetValues(typeof(ClipType)))
            {
                o[clipType.ToString()]=GetClip(c,clipType);//o[clipType.ToString()]
            }
            animator.runtimeAnimatorController=o;
            return obj;
        }
        private AnimationClip GetClip(Character c, ClipType t)
        {
            var path=Path.Combine(CharacterClipPath,c.ToString(),$"{t.ToString()}.anim");
            return Resources.Load<AnimationClip>(path);
        }
    }
}
