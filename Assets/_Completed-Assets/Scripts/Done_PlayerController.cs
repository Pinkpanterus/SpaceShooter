using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UIElements;

public class Done_PlayerController : MonoBehaviour
{
	Vector2 screenBounds;

	public float speed;
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	public int burstCount = 3;
	 
	private float nextFire;
	private FixedJoystick joystick;
	public Action<GameObject, SoundController.SoundCase> onFire;
	

	private void Start()
	{
		#if UNITY_EDITOR
			joystick = null;
		#else
			joystick = FindObjectOfType<FixedJoystick>();
		#endif

		screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
		

	}

	

	void Update ()
	{
		Fire();
	}

	public void Fire() 
	{
#if UNITY_EDITOR
		if (Input.GetButton("Fire1") && Time.time > nextFire)
		{
			nextFire = Time.time + fireRate;

			onFire?.Invoke(this.gameObject, SoundController.SoundCase.Fire);

			Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
			//GetComponent<AudioSource>().Play();
		}
#endif
	}

	public void UI_ButtonFire() 
	{
		StartCoroutine("StartFiring");

		//if (Time.time > nextFire)
		//{
		//	nextFire = Time.time + fireRate;
		//	onFire?.Invoke(this.gameObject, SoundController.SoundCase.Fire);
		//	Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
		//}
	}

	public IEnumerator StartFiring()
	{
		var shootToMake = burstCount;
		while (shootToMake > 0)
		{
			yield return new WaitForSeconds(fireRate);
			if (Time.time > nextFire)
			{
				nextFire = Time.time + fireRate;
				onFire?.Invoke(this.gameObject, SoundController.SoundCase.Fire);
				Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
				shootToMake--;
			}
		}
		if (shootToMake <= 0)
			StopCoroutine("StartFiring");
	}

	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		Vector3 movement;

		if (joystick)
			movement = new Vector3(joystick.Horizontal, 0.0f, joystick.Vertical);
		else
			movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		

		GetComponent<Rigidbody>().velocity = movement * speed;
		
	

		GetComponent<Rigidbody>().position = new Vector3
		(
			Mathf.Clamp(GetComponent<Rigidbody>().position.x, -screenBounds.x + 0.5f, screenBounds.x - 0.5f),
			0.0f,
			Mathf.Clamp(GetComponent<Rigidbody>().position.z, -screenBounds.y + 6, screenBounds.y*3 - 4)
		);
	}
}
