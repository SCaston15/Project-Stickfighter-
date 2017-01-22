using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneTransferElevator : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			SceneManager.LoadScene ("BossScene");
		}
	}
}
