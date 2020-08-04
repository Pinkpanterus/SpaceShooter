using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu_Controller : MonoBehaviour
{

	[Tooltip("_sceneToLoadOnPlay is the name of the scene that will be loaded when users click play")]
	public string _sceneToLoadOnPlay = "Level";
	[Tooltip("_webpageURL defines the URL that will be opened when users click on your branding icon")]
	public string _webpageURL = "http://www.alpaca.studio";
	[Tooltip("_soundButtons define the SoundOn[0] and SoundOff[1] Button objects.")]
	public Button[] _soundButtons;
	[Tooltip("_audioClip defines the audio to be played on button click.")]
	public AudioClip _audioClip;
	[Tooltip("_audioSource defines the Audio Source component in this scene.")]
	public AudioSource _audioSource;

	public GameObject mainMenuPanel;
	public GameObject loadLevelPanel;
	public LevelData[] levelsDatas;
	public Button[] levelsButtons;
	//public IEnumerable<LevelData> maxOpenedLevel;

	public Action<int> onLevelLoad;


	//The private variable 'scene' defined below is used for example/development purposes.
	//It is used in correlation with the Escape_Menu script to return to last scene on key press.
	UnityEngine.SceneManagement.Scene scene;

	void Awake()
	{
		UpdateLevelData_SO();

		if (!PlayerPrefs.HasKey("_Mute"))
		{
			PlayerPrefs.SetInt("_Mute", 0);
		}

		scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
		PlayerPrefs.SetString("_LastScene", scene.name.ToString());
		//Debug.Log(scene.name);


		DontDestroyOnLoad(this);
		if (FindObjectsOfType(GetType()).Length > 1)
			Destroy(gameObject);

	}

	void UpdateLevelData_SO()
	{
		var SO_ToLoad = levelsDatas.Where(x => x.isNotEmpty == true);

		foreach (var so in SO_ToLoad)
			UnityEngine.Debug.Log(so);

		foreach (var so in SO_ToLoad)
		{
			var number = so.levelNumber;
			levelsDatas[number] = LoadData(so.levelNumber);
			//UnityEngine.Debug.Log(LoadData(so.levelNumber));
		}

		
	}

	LevelData LoadData(int levelNumber)
	{
		LevelData data = null;
		if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "LevelData" + (levelNumber - 1) + ".txt"))
		{
			data = ScriptableObject.CreateInstance<LevelData>();
			string json = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "LevelData" + (levelNumber - 1) + ".txt");
			JsonUtility.FromJsonOverwrite(json, data);
		}
		else
		{
			data = levelsDatas[(levelNumber - 1)];
		}
		return data;
	}

	public void OpenLevelLoadMenu()
	{
		mainMenuPanel.active = false;
		loadLevelPanel.active = true;

		MarkButtons();
	}

	private void MarkButtons()
	{
		foreach (var b in levelsButtons)
			b.GetComponent<Button>().interactable = false;

		for (int i = 0; i <= levelsButtons.Length - 1; i++)
		{

			if (levelsDatas[i].levelStatus != LevelData.LevelStatus.Closed)
				levelsButtons[i].GetComponent<Button>().interactable = true;

			if (levelsDatas[i].levelStatus == LevelData.LevelStatus.Complited)
				levelsButtons[i].GetComponentInChildren<Text>().color = Color.green;
		}

		/*
		maxOpenedLevel = from ld in levelsDatas
							 where ld.levelStatus == LevelData.LevelStatus.Opened
							 select ld;

		foreach(var q in maxOpenedLevel)
			UnityEngine.Debug.Log("List contains: " + q);

		var maxNumber = maxOpenedLevel.OrderBy(x => x.levelNumber).Last().levelNumber;
		UnityEngine.Debug.Log(maxNumber);*/

		//var x = levelsDatas.Select(ld => ld.levelStatus == LevelData.LevelStatus.Closed).OrderBy(x => x.).Last().levelNumber;


		//List<LevelData> openedLevels = new List<LevelData>();
		//foreach (var ld in levelsDatas)
		//	if (ld.levelStatus == LevelData.LevelStatus.Closed)
		//		openedLevels.Add(ld);
		//foreach (var q in openedLevels)
		//	UnityEngine.Debug.Log("List contains: " + q);

		//var maxNumber = openedLevels.OrderByDescending(x => x.levelNumber).Reverse().ToList();
		//UnityEngine.Debug.Log(maxNumber[0]);


		//for (int i = (maxNumber[0].levelNumber - 1); i >= 0; i--) 
		//{
		//	if (levelsDatas[i].levelStatus != LevelData.LevelStatus.Opened)
		//		levelsDatas[i].levelStatus = LevelData.LevelStatus.Opened;

		//	levelsButtons[i].GetComponent<Button>().interactable = true;
		//}

		//for (int i = (maxNumber[0].levelNumber - 2); i >= 0; i--)
		//{
		//	if (levelsDatas[i].levelStatus != LevelData.LevelStatus.Complited)
		//	{
		//		levelsDatas[i].levelStatus = LevelData.LevelStatus.Complited;				
		//	}
		//	levelsButtons[i].GetComponentInChildren<Text>().color = Color.green;
		//}





	}

	public void LoadLevel()
	{
		

		//var gc = FindObjectOfType<Done_GameController>();
		int levelNumber = int.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(EventSystem.current.currentSelectedGameObject.name.Length - 1, 1));
		//onLevelLoad?.Invoke(levelNumber);
		PlayerPrefs.SetInt("LevelToLoad", levelNumber);
		UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoadOnPlay);
		
		
		UnityEngine.Debug.Log("LoadLevel done");

		//var ld = gc.levelDataStorage[levelNumber - 1];
		//UnityEngine.Debug.Log(ld);
		//gc.LoadLevelData(ld);
		//GameObject.FindWithTag("GameController").GetComponent<Done_GameController>().LoadLevelData(ld); ;
	}

	public void ReturnToMainMenu()
	{
		mainMenuPanel.active = true;
		loadLevelPanel.active = false;
	}

	public void OpenWebpage()
	{
		_audioSource.PlayOneShot(_audioClip);
		Application.OpenURL(_webpageURL);
	}

	public void PlayGame()
	{
		levelsDatas[0].levelStatus = LevelData.LevelStatus.Opened;

		foreach (var x in levelsDatas.Skip(1)) 
		{
			x.levelStatus = LevelData.LevelStatus.Closed;
			x.isNotEmpty = false;
			x.asteroids = null;
			x.score = 0;
			x.shieldEnergy = 3;
			x.playerShip = null;

			string json = JsonUtility.ToJson(levelsDatas[x.levelNumber - 1]);
			File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "LevelData" + x.levelNumber + ".txt", json);

			PlayerPrefs.SetInt("LevelToLoad", 1);
			UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoadOnPlay);
		}
			

		//onLevelLoad?.Invoke(1);

			//_audioSource.PlayOneShot(_audioClip);
			//PlayerPrefs.SetString("_LastScene", scene.name);
			//UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoadOnPlay);

	}

	public void Mute()
	{
		_audioSource.PlayOneShot(_audioClip);
		_soundButtons[0].interactable = true;
		_soundButtons[1].interactable = false;
		PlayerPrefs.SetInt("_Mute", 1);
	}

	public void Unmute()
	{
		_audioSource.PlayOneShot(_audioClip);
		_soundButtons[0].interactable = false;
		_soundButtons[1].interactable = true;
		PlayerPrefs.SetInt("_Mute", 0);
	}

	public void QuitGame()
	{
		_audioSource.PlayOneShot(_audioClip);
#if !UNITY_EDITOR
			Application.Quit();
#endif

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}
