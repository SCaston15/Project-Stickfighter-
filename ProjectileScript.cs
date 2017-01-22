using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {

	Rigidbody2D myRigidbody2D;

	// Use this for initialization
	void Start () {
		Destroy(gameObject, 1);

		myRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
	}
	
	void Update () {
		myRigidbody2D.velocity = new Vector2 (15, 0);
	}
}
