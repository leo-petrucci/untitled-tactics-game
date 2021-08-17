using UnityEngine;
using System.Collections;

/// <summary>
/// Creates wandering behaviour for a CharacterController.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class wander_test : MonoBehaviour
{
	public float speed = 5;
	public float directionChangeInterval = 1;
	public float maxHeadingChange = 30;

	CharacterController controller;
	UnityEngine.AI.NavMeshAgent agent;
	float heading;
	Vector3 targetRotation;

	Vector3 forward
	{
		get { return transform.TransformDirection(Vector3.forward); }
	}

	void Awake ()
	{
		controller = GetComponent<CharacterController>();
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();


		// Set random initial rotation
		heading = Random.Range(0, 360);
		transform.eulerAngles = new Vector3(0, heading, 0);

		StartCoroutine(NewHeadingRoutine());
	}

	void Update ()
	{
		transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);
		//controller.SimpleMove(forward * speed);
		agent.destination = forward * speed;
	}

	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		if (hit.gameObject.tag != "Boundary") {
			return;
		}

		// Bounce off the wall using angle of reflection
		var newDirection = Vector3.Reflect(forward, hit.normal);
		transform.rotation = Quaternion.FromToRotation(Vector3.forward, newDirection);
		heading = transform.eulerAngles.y;
		NewHeading();
	}

	/// <summary>
	/// Calculates a new direction to move towards.
	/// </summary>
	void NewHeading ()
	{
		var floor = transform.eulerAngles.y - maxHeadingChange;
		var ceil  = transform.eulerAngles.y + maxHeadingChange;
		heading = Random.Range(floor, ceil);
		targetRotation = new Vector3(0, heading, 0);
	}

	/// <summary>
	/// Repeatedly calculates a new direction to move towards.
	/// Use this instead of MonoBehaviour.InvokeRepeating so that the interval can be changed at runtime.
	/// </summary>
	IEnumerator NewHeadingRoutine ()
	{
		while (true) {
			NewHeading();
			yield return new WaitForSeconds(directionChangeInterval);
		}
	}
}