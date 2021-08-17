using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour {

	public List<int> GameobjectIDs;

	// This is essentially adding and removing enemies to a List
	// if one of them is the same as the Character.CurrentTarget, they'll execute an attack
	void OnTriggerEnter(Collider other) {
        	GameobjectIDs.Add(other.gameObject.GetInstanceID());
   	}

	void OnTriggerExit(Collider other) {
        	GameobjectIDs.Remove(other.gameObject.GetInstanceID());
	}

}
