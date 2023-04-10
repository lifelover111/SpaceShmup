using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text record;
    public GameObject heroModel;
    public GameObject background;
    public GameObject canvas;
    private Quaternion startRot;
    private float p = -1;
    private int highScore;
    private int highTime;
    // Start is called before the first frame update
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
        if (p != -1)
        {
            p += Time.deltaTime;
            if (heroModel.transform.rotation == Quaternion.Euler(-90, 360, 0))
            {
                SceneManager.LoadScene("_Scene_0");
                record.gameObject.SetActive(false);
            }
            else
            {
                heroModel.transform.rotation = Quaternion.Slerp(startRot, Quaternion.Euler(-90, 0, 0), p / 3);
                heroModel.transform.localScale = Vector3.Lerp(new Vector3(2.3f, 2.3f, 2.3f), new Vector3(0.3f, 0.3f, 0.3f), p / 3);
                background.GetComponent<Parallax>().scrollspeed = (p * p / 9 - 1) - 30 * p * p / 9;
            }
        }
        record.text = "Highscore:" + highScore + " Record Time:" + highTime/60 + ":" + highTime%60;
    }

    public void Survive()
    {
        p = 0;
        heroModel.GetComponent<HeroPreview>().loadingCalled = true;
        startRot = heroModel.transform.rotation;
        canvas.SetActive(false);
    }
}
