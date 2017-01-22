using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {

	public GameObject enemy; 
	public int numOfEnemies = 0; 
	public bool isTriggered = false; 
	float num;

	// Use this for initialization
	void Start () {
	}

	void OnTriggerEnter2D (Collider2D collider) {

		if(!isTriggered && collider.tag == "Player"){
			for (int i = 0; i < numOfEnemies; i++) {

				num = Random.Range(-2.0f, 2.0f);

				//generate the transform by creating a new vector3 and multiplying its x value
				//by a randomly generated number.
				//INSERT CODE HERE
				//then instantiate the enemy with the position set to the vector3 you made.
				Vector3 vector = new Vector3((transform.position.x + num), transform.position.y, transform.position.z);
				Instantiate (enemy, vector, Quaternion.identity);
			}
			isTriggered = true; 
		}
	}
}

