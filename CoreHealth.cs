using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CoreHealth : MonoBehaviour {

	public float max_Health = 100000f; 
	public float cur_Health = 0f; 
	public GameObject healthBar; 
	HitBoxObject incomingHitBox;

	void Start(){
		cur_Health = max_Health;
		InvokeRepeating ("decreaseHealth", 1f, 1f);
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

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Beam") {
			SceneManager.LoadScene ("WinScreen");
		}
	}

	void OnTriggerStay2D(Collider2D other){
		Debug.Log ("Hit.");
		if (other.tag == "HitBox") {
			HitBoxObject incomingHitBox = other.GetComponent<HitBoxObject> ();
			cur_Health -= incomingHitBox.damageAmount;
		}
		if (cur_Health <= 0) {
			SceneManager.LoadScene ("WinScreen");
		}
	}
}
