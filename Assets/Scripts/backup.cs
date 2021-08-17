using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class backup : MonoBehaviour {

	public static Character c, c2, c3;
    public static List<Vector3> spawnPositions = new List<Vector3>();
    public static List<Character> playerTeam = new List<Character>();

    public TextMeshPro testText;
    public static TextMeshPro testText2;

	public void Awake() {
        foreach(Transform child in GameObject.Find("Positions").transform)
        {
            spawnPositions.Add(child.transform.position);
        }

		Program.Main();
	}

	void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "I am a button"))
        {
			playerTeam[0].SetHp(-90);
        }
    }

	public static class Program
    {
        public static void Main()
        {
            playerTeam.Add(Character.Create("Char1", spawnPositions[0]));
            playerTeam.Add(Character.Create("Char2", spawnPositions[1]));
        }
    }

	int score = 0;


    public class Character
    {
        // Client code can subscribe to this event
        public event System.EventHandler<System.EventArgs> HpChanged;

        public string Name;
        public int Hp;
        public int HpMax;

    	public TextMeshPro m_Text;
        public TextContainer m_TextContainer;
        public RectTransform m_RectTransform;

		public static Character Create(string name, Vector3 position)
		{
			var result = new Character();

			// Safe to call a virtual method here
			result.Initialize(name, position);

			// Now you can do any other post-constructor stuff
			return result;
		}

		public virtual void Initialize(string name, Vector3 position)
		{
            this.Hp = 100;
            this.HpMax = 100;

            this.Name = name;

            // define handler with lambda function (preferred)
            this.HpChanged += (sender, e) =>
            {
                Debug.Log(Name + "'s Hp changed! New value: " + Hp);
				m_Text.text = Hp.ToString();
            };

            // Loads Model and Instantiates it
 			GameObject characterModel = (GameObject)Resources.Load("Prefabs/Character", typeof(GameObject));			
			var Model = Instantiate(characterModel);
            Model.name = "Char1";

            var HealthText = new GameObject("Health");
            m_Text = HealthText.AddComponent<TextMeshPro>();

            HealthText.transform.parent = Model.transform;

            m_TextContainer = HealthText.AddComponent<TextContainer>();

            m_RectTransform = HealthText.GetComponent<RectTransform>();
            m_RectTransform.sizeDelta = new Vector2(3, 1);
            m_RectTransform.position = new Vector3(-1, 2, -0.5f);

			m_Text.text = Hp.ToString();
            m_Text.fontSize = 5;
            m_Text.alignment = TextAlignmentOptions.Center;

            Model.transform.position = position;

		}

        // Child classes can override and raise the event as needed
        // This is not public facing
        protected void OnHpChanged(System.EventArgs e)
        {
            var handler = HpChanged;
            if (handler != null)
                handler(this, e);
        }

        public void SetHp(int value)
        {
            int hp_old = Hp;
            Hp += value;

            if (Hp != hp_old)
            {
                OnHpChanged(System.EventArgs.Empty);
            }
        }
    }
}
