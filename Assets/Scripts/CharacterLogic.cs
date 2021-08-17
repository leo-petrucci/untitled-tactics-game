using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using System;

public class CharacterLogic : MonoBehaviour
{

    public static List<Vector3> spawnPositions = new List<Vector3>();
    public static List<Character> playerTeam = new List<Character>();

    public TextMeshPro testText;
    public static TextMeshPro testText2;

    public void Awake()
    {

        // This finds the available positions and instantiates our characters there
        // To be changed, placeholder for tests
        foreach (Transform child in GameObject.Find("Positions").transform)
        {
            spawnPositions.Add(child.transform.position);
        }

        Program.Main();
    }

    public static class Program
    {
        public static void Main()
        {
            // This is just for testing, it will need to be redone better
            playerTeam.Add(Character.Create(spawnPositions[0]));
            playerTeam.Add(Character.Create(spawnPositions[1]));

            playerTeam[0].OnHpChange += playerTeam[0].a_HpChange;
            playerTeam[0].OnHpChange += playerTeam[1].a_HpChange;

            playerTeam[1].OnHpChange += playerTeam[0].a_HpChange;
            playerTeam[1].OnHpChange += playerTeam[1].a_HpChange;


            // This is also part of the testing, but it will be enabled on all characters 
            // regardless of their AI
            playerTeam[0].OnMove += playerTeam[0].a_Move;
            playerTeam[0].OnMove += playerTeam[1].a_Move;
            playerTeam[1].OnMove += playerTeam[0].a_Move;
            playerTeam[1].OnMove += playerTeam[1].a_Move;

        }
    }

    public class Character
    {
        public string Name;
        public int Hp;
        public int HpMax;

        public TextMeshPro m_Text;
        public TextContainer m_TextContainer;
        public RectTransform m_RectTransform;
        public NavMeshAgent agent;
        public WeaponCollider weaponCollider;
        public GameObject CharacterGameObject;

        // I use this function instead of "new" to create instances so I can also Initialize each instance
        public static Character Create(Vector3 position)
        {
            var result = new Character();

            // Safe to call a virtual method here
            result.Initialize(position);

            return result;
        }

        // Just getting and setting components and variables, nothing special.
        public virtual void Initialize(Vector3 position)
        {
            this.Hp = 100;
            this.HpMax = 100;

            this.Name = "Char" + (playerTeam.Count + 1);

            // Loads Model and Instantiates it
            GameObject characterModel = (GameObject)Resources.Load("Prefabs/Character", typeof(GameObject));
            CharacterGameObject = Instantiate(characterModel);
            CharacterGameObject.name = "Char" + (playerTeam.Count + 1);

            // All of this is for the HP, some is obsolete, needs to be updated and maybe made more simple
            // Might add a method exclusively to handle HP later
            var HealthText = new GameObject("Health");
            m_Text = HealthText.AddComponent<TextMeshPro>();
            HealthText.transform.parent = CharacterGameObject.transform;
            m_TextContainer = HealthText.AddComponent<TextContainer>();
            m_RectTransform = HealthText.GetComponent<RectTransform>();
            m_RectTransform.sizeDelta = new Vector2(3, 1);
            m_RectTransform.position = new Vector3(-1.5f, 2, 0);
            m_Text.text = Hp.ToString();
            m_Text.fontSize = 5;
            m_Text.alignment = TextAlignmentOptions.Center;

            // Getting the NavmeshAgent
            agent = CharacterGameObject.GetComponent<NavMeshAgent>();

            // Getting the script associated with the weapon's collider
            weaponCollider = CharacterGameObject.transform.Find("Weapon").transform.Find("AttackCollider").GetComponent<WeaponCollider>();

            // Moves model to spawn position
            CharacterGameObject.transform.position = position;

        }

        public GameObject CurrentTarget;

        // This is where we keep all the event-related stuff
        public event EventHandler<EventArgs> CurrentAction, OnMove;
        public event EventHandler<HpEventArgs> OnHpChange;

        // Sets HP and triggers HP event
        public void SetHp(int value)
        {
            int hp_old = Hp;
            Hp += value;

            m_Text.text = Hp.ToString();

            HpEventArgs args = new HpEventArgs();
            args.Name = Name;

            if (Hp != hp_old)
            {
                OnHpChange(this, args);
            }
        }

        // We use this to change the Character's position, this way we can also call the movement event
        // At the same time. Might be bad for performance, need to do more testing.

        // Might just replace this with something that is triggered by a third collider
        private Vector3 characterPosition;
        public Vector3 CharacterPosition
        {
            set
            {
                agent.SetDestination(value);
                OnMove(this, EventArgs.Empty);
            }
        }

        // Response to one of the character's HP going critical, useless now, but it works
        public void a_HpChange(object sender, HpEventArgs e)
        {
            Debug.Log(e.Name + "'s Health Points are critical!");
        }
        // Response to characters moving
        public void a_Move(object sender, EventArgs e)
        {
            //Debug.Log("Toggle movement");
        }

        public float actionProgress = 5;
        public float ActionProgress
        {
            get { return actionProgress; }
            set
            {
                actionProgress = value;
                if (actionProgress > 120)
                {
                    CurrentAction(this, EventArgs.Empty);
                    CurrentAction = null;
                }
            }
        }

        public void ResetProgressBar()
        {
            ActionProgress = 5;
        }

        public bool HasCurrentAction
        {
            get
            {
                bool a = CurrentAction != null;
                return a;
            }
        }

        public bool CoroutineStartCheck = false;

        // This is tied to the framerate and needs to be fixed
        public IEnumerator ProressBar()
        {
            while (CurrentAction != null)
            {
                ActionProgress += 1;
                if (ActionProgress > 120)
                {
                    ActionProgress = 120;
                }
                yield return null;
            }
            CoroutineStartCheck = false;
        }

        public class HpEventArgs : EventArgs
        {
            public string Name { get; set; }
        }
    }


    // This is where the Actions will be contained, it's a mess but it's easier to keep them here.
    public void Attack(object sender, EventArgs e)
    {
        var sendingObject = (Character)sender;
        StartCoroutine(AttackCoroutine(sendingObject));
    }
    public IEnumerator AttackCoroutine(Character sender)
    {
        while (!sender.weaponCollider.GameobjectIDs.Exists(x => x == sender.CurrentTarget.GetInstanceID()))
        {
            //sender.agent.destination = sender.CurrentTarget.transform.position;
            //sender.agent.SetDestination(sender.CurrentTarget.transform.position);
            sender.CharacterPosition = sender.CurrentTarget.transform.position;
            yield return null;
        }
        //Debug.Log("Execute Attack");
        sender.ResetProgressBar();
        StepBack(sender);
    }

    public void StepBack(Character sender)
    {
        Vector3 StepBack = -sender.CharacterGameObject.transform.forward / 5f;
        Debug.DrawRay(-sender.CharacterGameObject.transform.forward / 5f, Vector3.up * 20, Color.red);
        sender.agent.SetDestination(sender.CharacterGameObject.transform.position - sender.CharacterGameObject.transform.forward);
    }

}
