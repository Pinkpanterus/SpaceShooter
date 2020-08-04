using UnityEngine;
using System.Collections;
using System;

public class Done_DestroyByContact : MonoBehaviour
{
	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue;
	private Done_GameController gameController;
	public Action onPlayerCollision;

	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <Done_GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' script");
		}

		gameController.onGameOverEvent += PlayerShipExposion;
	}

	private void PlayerShipExposion()
	{
		var player = GameObject.FindGameObjectWithTag("Player");
		if (player)
		{
			Destroy(player.gameObject); // Bolt or Player
			Instantiate(playerExplosion, player.transform.position, player.transform.rotation);
		}
	}

	//private void FixedUpdate()
	//{
	//	if (gameController.gameOver)
	//	{
	//		var player= GameObject.FindGameObjectWithTag("Player");
	//		if (player)
	//		{
	//			Destroy(player.gameObject); // Bolt or Player
	//			Instantiate(playerExplosion, player.transform.position, player.transform.rotation);
	//		}
			
	//	}
	//}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Boundary") /*|| other.CompareTag("Enemy")*/)
		{
			// Ignore Boundary
			return;
		}

		if (explosion != null)
		{
			
			Instantiate(explosion, transform.position, transform.rotation);
		}

		if (other.CompareTag("Player"))
		{
			onPlayerCollision?.Invoke();
			onPlayerCollision -= gameController.ReduceShieldEnergy;
			//onPlayerCollision?.Invoke(this.gameObject, SoundController.SoundCase.Explosion);
			//gameController.ReduceShieldEnergy();
		}
		else
		{
			gameController.AddScore(scoreValue); // Only add to the score when not hitting the Player!
			Destroy(other.gameObject);
		}

		Destroy(gameObject); // Asteroid
	}

	public void TerminateAsteroid() 
	{
		Destroy(gameObject);
		Instantiate(explosion, transform.position, transform.rotation);
	}

}