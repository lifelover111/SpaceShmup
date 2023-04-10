using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private int _score;
    private float time;
    public int highScore = 0;
    public int highTime = 0;
    public Text scoreText;
    public Text timeText;
    public static UI S;
    public int score
    { get { return _score; } set { _score = value; } }
    private void Awake()
    {
        S = this;
        if (PlayerPrefs.HasKey("HighScore"))
        { // b
            highScore = PlayerPrefs.GetInt("HighScore");
        }
        PlayerPrefs.SetInt("HighScore", highScore);
        if (PlayerPrefs.HasKey("HighTime"))
        { // b
            highTime = PlayerPrefs.GetInt("HighTime");
        }
        PlayerPrefs.SetInt("HighTime", highTime);
    }
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        time = 0;
        scoreText.text = "Score: " + score;
        timeText.text = "Time: 00:00" ;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        scoreText.text = "Score: " + score;
        if(time%60 < 10)
            timeText.text = "Time: " + (int)time/60 + ":" + "0" + (int)time%60;
        else
            timeText.text = "Time: " + (int)time / 60 + ":" + (int)time % 60;

        if (score > PlayerPrefs.GetInt("HighScore"))
        { // d
            PlayerPrefs.SetInt("HighScore", score);
        }
        if (time > PlayerPrefs.GetInt("HighTime"))
        { // d
            PlayerPrefs.SetInt("HighTime", (int)time);
        }
    }
}
