using System.IO;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public class Unit : MonoBehaviour
    {
        public string ID => gameObject.name;
        public Character Character { get; private set; }
        public UnitType Type { get; private set; }
        public int Row { get; private set; }
        public int Col { get; private set; }
        public int Strength { get; private set; }
        public int Defence { get; private set; }
        public int Life { get; private set; }
        public int Range { get; private set; }
        public int Speed { get; private set; }
        public int Level { get; private set; }
        public bool CanMove { get; private set; }
        public bool CanAttack { get; private set; }
        public string RelicID {get;private set;}
        private bool firstUpdate = true;
        public Faction Faction { get; private set; }
        private GameObject unitSprite = null;
        private GameObject healthBar = null;
        private Animator animator => unitSprite.GetComponent<Animator>();
        [SerializeField]
        private float yOffset = 2.6f;
        [SerializeField]
        private float speed = 0.1f;

        private readonly string animSelected = "touch";
        private readonly string animMove = "move";
        private readonly string animAttack = "attack";
        private UnitResources resources => UnitResources.Instance;
        public Block Block => Map.Instance.BlockSet[Row, Col];

        public static IEnumerable<Unit> All => GameObject.FindObjectsOfType<Unit>();


        // Start is called before the first frame update
        void Start()
        {
            gameObject.tag = Main.UnitTag;
        }

        // Update is called once per frame
        void Update()
        {
            var moveTo = Block.Top;
            var delta = moveTo - gameObject.transform.position;
            if (delta.magnitude > speed)
            {
                gameObject.transform.Translate(delta.normalized * speed);
                animator.SetBool(animMove, true);
                unitSprite.GetComponent<SpriteRenderer>().flipX = delta.z < 0;
            }
            else
            {
                animator.SetBool(animMove, false);
                // unitSprite.GetComponent<SpriteRenderer>().flipX=false;
            }
        }
        public void NetUpdate(NetUnit data)
        {
            Character = data.Character;
            Type = data.Type;
            Row = data.Row;
            Col = data.Col;
            Strength = data.Strength;
            Defence = data.Defence;
            Life = data.Life;
            Range = data.Range;
            Level = data.Level;
            CanMove = data.CanMove;
            CanAttack = data.CanAttack;
            RelicID = data.RelicID;
            Faction = data.Faction;
            Speed = data.Speed;
            if (firstUpdate)
            {
                firstUpdate = false;
                gameObject.transform.position = Block.Top;
            }

            if (unitSprite == null)
            {
                SetupSprite();
            }
            Debug.Assert(healthBar != null);
            Debug.Assert(healthBar.TryGetComponent<HealthBar>(out var c) != false);
            healthBar.GetComponent<HealthBar>().Health = Life;

        }
        private void SetupSprite()
        {
            var pos = gameObject.transform.position + Vector3.up * yOffset;
            var wrapped = Instantiate(resources.GetSprite(Character), pos, Quaternion.identity);
            wrapped.transform.SetParent(gameObject.transform);
            unitSprite = wrapped.transform.Find("CharacterSprite").gameObject;
            healthBar = Instantiate(resources.GetHealthBar(Faction), pos, Quaternion.identity);
            healthBar.transform.SetParent(gameObject.transform);
        }
        public string RelicVerbose(string relic)=> relic switch{
            "R0"=>"Circles of Logos",
            "R1"=>"Flower of Life",
            "R2"=>"Goblet of Eonothem",
            _=>throw new NotImplementedException(),
        };
        public void OnSelected()
        {
            UIHandler.Instance.Select(this);
            var leftNames = new List<string>{
                $"Level:{Strength}",
                $"Life:{Life}",
                $"Range:{Range}",
                $"Speed:{Speed}",
            };
            var leftValues = new List<float>{
                Strength/50f,
                Life/100f,
                Range/15f,
                Speed/15f,
            };
            var rightNames = new List<string>{
                $"Relic: {RelicVerbose(RelicID)}",
                $"CanMove {CanMove}",
                $"CanAttack {CanAttack}",
                $"Character {Character.ToString()}",
            };
            var rightValues = new List<float> { 0, 0, 0, 0 };
            UIController.Instance.SetLeftList(leftNames, leftValues);
            UIController.Instance.SetRightList(rightNames, rightValues);
            UIController.Instance.SetTitle(Character.ToString());
            UIController.Instance.SetBar(Character.ToString());
            UIController.Instance.SetPortrait(UnitResources.Instance.GetPortrait(Character));
            animator.SetTrigger(animSelected);
            PlaySound(UnitResources.VoiceType.Interact);
        }
        public void OnSetAsTarget()
        {
            throw new NotImplementedException();// units are not targetted, blocks are
        }
        private void PlaySound(UnitResources.VoiceType t) => AudioController.Instance.Source.PlayOneShot(UnitResources.Instance.GetVoice(Character, t));
        public void PlayAttackAnim()
        {
            animator.SetTrigger(animAttack);
            PlaySound(UnitResources.VoiceType.Attack);
        }
    }
}