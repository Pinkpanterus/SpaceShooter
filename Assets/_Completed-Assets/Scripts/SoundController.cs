using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController instance = null;

    public AudioClip playerWeaponSoundClip;
    public AudioClip playerExplosionClip;
    public AudioClip playerEnergyShieldCollisionClip;
    public AudioClip AsteroidExplosionClip;
    public AudioClip GameOverClip;
    public AudioClip WinClip;
    public AudioClip BackgroundMusicClip;
    public enum SoundCase {Fire,Collision,Explosion};

    private Done_PlayerController playerController;
    private Done_GameController gameController;
    AudioSource au;

    //public static Action<GameObject, SoundCase> onFire;



    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this; 
        else if (instance == this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        PlayBackgroundMusic();

        playerController=FindObjectOfType<Done_PlayerController>();
        playerController.onFire += MakeSound;

        gameController = FindObjectOfType<Done_GameController>();
        gameController.onWinEvent += PlayWinClip;
        gameController.onGameOverEvent += PlayLooseClip;
    }

    public void MakeSound(GameObject go, SoundController.SoundCase sc) 
    {               
        AudioSource aus;
        aus = go.GetComponent<AudioSource>();
        if (!aus)
        {
            go.AddComponent<AudioSource>();
            aus = go.GetComponent<AudioSource>();
        }

        if (go.name.Contains("Player"))
        {           
            switch (sc) 
            {
                case SoundCase.Collision:
                    aus.clip = playerEnergyShieldCollisionClip;
                    break;
                case SoundCase.Explosion:
                    aus.clip = playerExplosionClip;
                    break;
                case SoundCase.Fire:
                    aus.clip = playerWeaponSoundClip;
                    break;
            }
        }
        else if (go.name.Contains("Asteroid"))
        {
            //var destroyByContact = go.GetComponent<Done_DestroyByContact>();
            //destroyByContact.onPlayerCollision += MakeSound;

            switch (sc)
            {
                //case SoundCase.Collision:
                //    newGO.GetComponent<AudioSource>().clip = playerEnergyShieldCollisionClip;
                //    break;
                case SoundCase.Explosion:
                    aus.clip = AsteroidExplosionClip;
                    Debug.Log("Asteroid explosion sound detected");
                    break;
                //case SoundCase.Fire:
                //    newGO.GetComponent<AudioSource>().clip = playerWeaponSoundClip;
                //    break;
            }
        }

        aus.Play();
        
    }

    void PlayBackgroundMusic() 
    {
        au = GetComponent<AudioSource>();
        au.loop = true;
        au.volume = 0.1f;
        au.clip = BackgroundMusicClip;
        au.Play();
    }

    void PlayWinClip() 
    {
        au.Stop();
        au.loop = false;
        au.volume = 1f;
        au.clip = WinClip;
        au.Play();
    }

    void PlayLooseClip()
    {
        au.Stop();
        au.loop = false;
        au.volume = 1f;
        au.clip = GameOverClip;
        au.Play();
    }



    private void OnDisable()
    {
        playerController.onFire -= MakeSound;
        gameController.onWinEvent -= PlayWinClip;
        gameController.onGameOverEvent -= PlayLooseClip;
    }
}
