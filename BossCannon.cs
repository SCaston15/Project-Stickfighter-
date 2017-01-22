using UnityEngine;
using System.Collections;

public class BossCannon : MonoBehaviour {
	public Transform targetTransform;
	public float speed;
	public float timeLeft = 20f;
	public AudioSource audioSource;
	public bool charged = false;
	public bool fired = false;
	public bool stoppedFiring = false;
	Animator anim;
	public GameObject beam;

	// Use this for initialization
	void Start () {
		anim = gameObject.GetComponent<Animator> ();
	}
	
	// Update is called once per frame

	//Every 10 seconds, the cannon fires. It starts by following the player while the timer is still counting down.
	//When the timer reaches zero, it charges and fires.
	void Update () {
		timeLeft -= Time.deltaTime;

		if (timeLeft > 8.5) {
			Vector3 vectorToTarget = targetTransform.position - transform.position;
			float angle = Mathf.Atan2 (vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
			Quaternion q = Quaternion.AngleAxis (angle, Vector3.forward);
			transform.rotation = Quaternion.RotateTowards (transform.rotation, q, Time.deltaTime * speed);
		}

		if (timeLeft < 10f && !charged) {
			Charge ();
			charged = true;
		}

		if (timeLeft < 6.5f && !fired) {
			Fire ();
			fired = true;
		}

		if (timeLeft < 3f && !stoppedFiring) {
			StopFiring ();
			stoppedFiring = true;
		}

		if (timeLeft < 0f) {
			Reset ();
		}
	}

	void Charge(){
		audioSource.Play ();
		anim.SetBool ("charging", true);
	}

	void Fire(){
		anim.SetBool ("charging", false);
		beam.SetActive (true);
	}

	void StopFiring(){
		beam.SetActive (false);
	}

	void Reset(){
		charged = false;
		fired = false;
		stoppedFiring = false;
		timeLeft = 20f;
	}
}
