using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepDistance : MonoBehaviour
{

    private SphereCollider DistanceTrigger;
    public CharacterLogic.Character ParentObject;

    void Start()
    {
        DistanceTrigger = this.gameObject.GetComponent<SphereCollider>();
        string ParentObjectName = transform.root.gameObject.name;
        ParentObject = CharacterLogic.playerTeam.Find(e => e.Name == ParentObjectName);
    }

    void OnTriggerStay(Collider other)
    {
        Debug.DrawLine(this.transform.position, other.gameObject.transform.position, Color.red);

        // This is probably bad, neet to create a method that returns true or false if full or not
        if (ParentObject.ActionProgress < 120)
        {
            //Debug.Log("Colliding with another Distance Collider"); 
            //ParentObject.agent.SetDestination(-other.gameObject.transform.position / 5f);
        }
    }

}
