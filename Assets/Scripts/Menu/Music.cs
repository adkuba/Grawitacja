using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Music : MonoBehaviour
{
    public AudioClip[] audioClips;
    private AudioSource musicSource;
    private float silenceTime;
    private float delay;
    private float maxDelay = 180;
    private float minDelay = 60;
    private List<int> playedMusic = new List<int>();

    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        //max moze byc jeden kontroler muzyki
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Music");
        if (gameObjects.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        silenceTime = Time.time;
        delay = Random.Range(minDelay, maxDelay);
    }

   
    void Update()
    {
        //jesli muzyka nie gra przez wyznaczony czas
        if (!musicSource.isPlaying && Time.time - silenceTime > delay && PlayerPrefs.GetInt("canPlayMusic", 1) == 1)
        {
            //jesli odtworzylismy wszystkie utwory to zaczynamy od poczatku
            if (playedMusic.Count == audioClips.Length)
            {
                playedMusic.Clear();
            }
            //odtwarzamy losowy utwor ktory jeszcze nie gralismy
            int index = Random.Range(0, audioClips.Length);
            while (playedMusic.Contains(index))
            {
                //przy intach koncowej wartosci nie wylosuje
                index = Random.Range(0, audioClips.Length);
            }
            musicSource.clip = audioClips[index];
            musicSource.Play();
            playedMusic.Add(index);
            delay = Random.Range(minDelay, maxDelay);
        }
        if (musicSource.isPlaying)
        {
            silenceTime = Time.time;
        }
        if (musicSource.isPlaying && PlayerPrefs.GetInt("canPlayMusic", 1) == 0)
        {
            musicSource.Stop();
        }
    }
}
