using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class GameMaster : MonoBehaviour {

	public int points;
	public Text pointsText; 

	void Update()
	{
		pointsText.text = ("Kills: " + points);
	}
}
