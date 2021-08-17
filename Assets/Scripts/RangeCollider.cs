using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeCollider : MonoBehaviour {

    private SphereCollider RangeTrigger;
	public CharacterLogic.Character ParentObject;
	private CharacterLogic cl;

	private Vector3 TargetRotation;

	// We don't know the specific object that used this collider
	// To fix this we get the name of the Gameobject and 
	// then use it to find our character object in the character list
	void Start () {
		RangeTrigger = this.gameObject.GetComponent<SphereCollider>();
		string ParentObjectName = transform.root.gameObject.name;
		ParentObject = CharacterLogic.playerTeam.Find(e => e.Name == ParentObjectName);
		cl = GameObject.Find("Main Camera").GetComponent<CharacterLogic>();
	}

	void Update() {
		// Looks Towards the target, definitely will have to make it look better.
		if(ParentObject.CurrentTarget != null) {
			ParentObject.CharacterGameObject.transform.LookAt(ParentObject.CurrentTarget.transform.position);
		}
	}
	
	// This makes sure that action and Coroutine are started once,
	// However it also gets launched again when the current action has been done
	void OnTriggerStay(Collider other) {
		if(!ParentObject.HasCurrentAction && other.tag == "Enemy") {
			// This will be where the function to check the behavior will be
			// it'll be used to assign the current action to the event handler
			// For now it's just for attacks as a placeholder

			// It will also contain logic about what to target first
			ParentObject.CurrentTarget = other.gameObject;
			ParentObject.CurrentAction += cl.Attack;

			// The if statememt is a failsafe to make sure only one Coroutine is 
			// started at a time.
			if (!ParentObject.CoroutineStartCheck) {
				StartCoroutine(ParentObject.ProressBar());
			}
			ParentObject.CoroutineStartCheck = true;
		}
	}


}
