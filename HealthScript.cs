using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HealthScript : MonoBehaviour {

	private GameMaster gm;
	//finds player
	private PlayerScript2 playerPos;
	//finds enemy




	public bool isEnemy = true;
	public float max_Health = 100f; 
	public float cur_Health = 0f; 
	public GameObject healthBar; 
	PolygonCollider2D polygonCollider2D;
	public bool invulnerable = false;
	public bool hit = false; 


	void Start()
	{ 
		playerPos = FindObjectOfType<PlayerScript2> ();
		cur_Health = max_Health;
		InvokeRepeating ("decreaseHealth", 1f, 1f);
	 	gm = GameObject.FindGameObjectWithTag ("GameMaster").GetComponent<GameMaster> (); 

	}

	void decreaseHealth()
	{
		float calc_Health = cur_Health / max_Health;
		SetHealthBar (calc_Health);

	}

	public void SetHealthBar(float myHealth)
	{
		healthBar.transform.localScale = new Vector3 (myHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
	}

	// When an object enters this object's trigger collider... 
	public void TakeDamage(float damageAmount){
		if (!invulnerable) {
			cur_Health -= damageAmount;

			// ...and if current health is <= to 0 and the object is an enemy, destroy the object.
			if (cur_Health <= 0 && gameObject.tag == "Enemy") {
				Destroy (gameObject);
  			    gm.points += 1;
			}
		}
	}

	public bool GetIsInvulnerable(){
		if (invulnerable)
			return true;
		else
			return false;
	}

	void OnCollisionEnter2D (Collision2D col)
	{
		if (col.gameObject.tag == "Enemy"){
			cur_Health = cur_Health - Mathf.Round(Random.Range(3.0f, 5.0f)); 
			} 
		if (cur_Health <= 0 && gameObject.tag == "Player") {
			SceneManager.LoadScene ("YouLose"); 
		}
	}

}
