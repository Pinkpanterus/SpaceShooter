using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Done_GameController : MonoBehaviour
{
	public LevelData[] levelDataStorage;
	public GameObject[] hazards;
	public Vector3 spawnValues;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;
	public int shiedEnergy = 3;

	public GameObject gamePanel;
	public GameObject nextLevelPanel;
	public GameObject gameOverPanel;
	public GameObject winPanel;

	public Text scoreText;
	public Text levelNumberText;
	public Text goalText;
	public Text nextLevelTimerText;

	public Image[] energyShieldIcons;

	public bool gameOver;
	public bool victory;
	//private bool restart;
	private int score;
	public int maxLevelNumber = 5;
	public int currentLevelNumber;
	private int baseGoal = 20;
	private int currentGoal;
	private bool gameIsRunning;
	float currCountdownValue = 3; // For timer for next level panel
	public List<GameObject> asteroids = new List<GameObject>();
	Coroutine spawnCoroutine;
	int levelHazardCount;
	private int startingLevel = 1;

	public Action onWinEvent;
	public Action onGameOverEvent;
	private Menu_Controller menuController;
	private bool isSpawningWaves;
	private bool isGameRunning;


	void Awake()
	{
		//DontDestroyOnLoad(transform.gameObject);
		menuController = FindObjectOfType<Menu_Controller>();
		//menuController.onLevelLoad += LoadLevelData;		
	}


	public void SaveLevelData()
	{
		LevelData ld = levelDataStorage.First(x => x.levelNumber == currentLevelNumber);
		ld.score = score;
		ld.shieldEnergy = shiedEnergy;
		ld.currentGoal = currentGoal;
		ld.isNotEmpty = true;
		//ld.levelStatus = LevelData.LevelStatus.Opened;
		//ld.playerShip = GameObject.FindGameObjectWithTag("Player");

		//var currentAsteroids = GameObject.FindGameObjectsWithTag("Enemy").ToList();
		//foreach (var ca in currentAsteroids)
		//{
		//	ld.asteroids.Add(ca, ca.transform);
		//}

		SaveData();
		Debug.Log("LevelData saved");
	}

	public void LoadLevelData(int levelNumber)
	{
		isGameRunning = true;

		LevelData ld = levelDataStorage[levelNumber - 1];

		currentLevelNumber = ld.levelNumber;

		if (ld.isNotEmpty)
		{
			score = ld.score;
			shiedEnergy = ld.shieldEnergy;
			currentGoal = ld.currentGoal;
			//if (ld.playerShip) 
			//{
			//	var player = GameObject.FindGameObjectWithTag("Player");
			//	player.transform.position = ld.playerShip.transform.position;
			//	player.transform.rotation = ld.playerShip.transform.rotation;
			//}
			
			//foreach (var ca in ld.asteroids)
			//{
			//	Instantiate(ca.Key, ca.Value.position, ca.Value.rotation);
			//}
		}

		StartLevel(currentLevelNumber);
		isGameRunning = true;
		Debug.Log("LoadLevelData done");		
	}

	void SaveData()
	{
		
		string json = JsonUtility.ToJson(levelDataStorage[currentLevelNumber - 1]);
		File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "LevelData" + currentLevelNumber + ".txt", json);
	}




	void Start()
	{
		int levelNumber = PlayerPrefs.GetInt("LevelToLoad");
		LoadLevelData(levelNumber);		
	}

	void StartLevel(int levelNumber)
	{
		currentLevelNumber = levelNumber;
		UpdateHazardCount();
		UpdateGoal();
		UpdateScoreText();
		UpdateLevelNumberText();
		currCountdownValue = 3;


		gamePanel.SetActive(true);
		nextLevelPanel.SetActive(false);
		gameOverPanel.SetActive(false);
		winPanel.SetActive(false);
		gameOver = false;
		victory = false;
		score = 0;

		if (Time.timeScale == 0)
			SwitchTimeSpeed();

		Invoke("StartSpawnWaves", 1);
		Debug.Log("StartLevel done");
	}

	void StartSpawnWaves()
	{
		spawnCoroutine = StartCoroutine(SpawnWaves());
	}


	void Update()
	{
		GameStatusCheck();
	}

	IEnumerator SpawnWaves()
	{
		isSpawningWaves = true;

		yield return new WaitForSeconds(startWait);
		while (true)
		{
			for (int i = 0; i < levelHazardCount; i++)
			{
				GameObject hazard = hazards[UnityEngine.Random.Range(0, hazards.Length)];
				Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				var asteroid = Instantiate(hazard, spawnPosition, spawnRotation);
				
				if(!asteroid.CompareTag("Enemy"))
					asteroid.tag = "Enemy";

				asteroid.GetComponent<Done_DestroyByContact>().onPlayerCollision += ReduceShieldEnergy;
				yield return new WaitForSeconds(spawnWait);
			}
			yield return new WaitForSeconds(waveWait);

			if (gameOver)
			{
				break;
			}
		}
	}

	public void AddScore(int newScoreValue)
	{
		score += newScoreValue;
		UpdateScoreText();
	}

	void UpdateScoreText()
	{
		scoreText.text = "Score: " + score;
	}


	void UpdateLevelNumberText()
	{
		levelNumberText.text = "Level: " + currentLevelNumber;
	}

	public void IncreaseLevelNumber()
	{
		levelDataStorage[currentLevelNumber - 1].levelStatus = LevelData.LevelStatus.Complited;

		if (currentLevelNumber <= levelDataStorage.Length)
			levelDataStorage[currentLevelNumber].levelStatus = LevelData.LevelStatus.Opened;

		currentLevelNumber++;
		UpdateLevelNumberText();
		UpdateGoal();
	}

	void UpdateGoalText()
	{

		goalText.text = "Goal: " + currentGoal;
	}

	void UpdateGoal()
	{
		currentGoal = baseGoal * currentLevelNumber;
		UpdateGoalText();
	}

	void UpdateHazardCount()
	{
		levelHazardCount = hazardCount * currentLevelNumber;
	}


	void UpdateNextLeVelTimer()
	{
		nextLevelTimerText.text = "" + currCountdownValue;
	}


	public void ReduceShieldEnergy()
	{
		shiedEnergy--;
		ShowEnergyShieldIcons();
	}

	private void ShowEnergyShieldIcons()
	{
		foreach (var s in energyShieldIcons)
			s.GetComponent<Image>().enabled = false;

		for (int i = 0; i <= shiedEnergy - 1; i++)
			energyShieldIcons[i].enabled = true;
	}

	void GameOver()
	{
		score = 0;
		gamePanel.SetActive(false);
		gameOverPanel.SetActive(true);
		DestroyAsteroids();
		onGameOverEvent?.Invoke();
		gameOver = true;

	}

	void GameStatusCheck()
	{
		if (isGameRunning)
		{
			if (shiedEnergy <= 0 && !gameOver)
				GameOver();

			if (score >= currentGoal && currentLevelNumber < maxLevelNumber)
				NextLevel();
			else if (score >= currentGoal && currentLevelNumber == maxLevelNumber && !victory)
				Win();
		}	
		
	}

	private void Win()
	{
		score = 0;
		gamePanel.SetActive(false);
		winPanel.SetActive(true);
		DestroyAsteroids();
		onWinEvent?.Invoke();
		victory = true;
		levelDataStorage[currentLevelNumber - 1].levelStatus = LevelData.LevelStatus.Complited;
		SaveLevelData();
	}

	private void NextLevel()
	{
		score = 0;
		gamePanel.SetActive(false);
		nextLevelPanel.SetActive(true);

		IncreaseLevelNumber();
		UpdateGoal();
		UpdateScoreText();
		UpdateLevelNumberText();
		score = 0;
		SaveLevelData();

		currCountdownValue = 3;

		DestroyAsteroids();
		StartCoroutine(StartCountdown());

	}

	void DestroyAsteroids()
	{
		if(isSpawningWaves)
			StopCoroutine(spawnCoroutine);

		asteroids = GameObject.FindGameObjectsWithTag("Enemy").ToList();
		foreach (var a in asteroids)
		{

			a.GetComponent<Done_DestroyByContact>().TerminateAsteroid();
		}
		isSpawningWaves = false;
	}

	public IEnumerator StartCountdown(float countdownValue = 3)
	{
		UpdateNextLeVelTimer();
		currCountdownValue = countdownValue;
		while (currCountdownValue > 0)
		{
			yield return new WaitForSeconds(1.0f);
			currCountdownValue--;
			UpdateNextLeVelTimer();
		}
		StartLevel(currentLevelNumber);
	}

	void SwitchTimeSpeed()
	{
		if (gameIsRunning)
		{
			Time.timeScale = 0;
			gameIsRunning = false;
		}
		else
		{
			Time.timeScale = 1;
			gameIsRunning = true;
		}
	}

	public void Quit()
	{
		SaveLevelData();
		Application.Quit();
	}

	public void LoadMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}

	//private void OnDestroy()
	//{
	//	menuController.onLevelLoad -= LoadLevelData;
	//}
}