using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveData : MonoBehaviour
{
    public TMPro.TextMeshProUGUI myScore;
    public TMPro.TextMeshProUGUI myName;
    // public int currentScore;

    void Update() 
    {
        // myScore.text = $"SCORE: {PlayerPrefs.GetInt("highscore")}";
    }

    public void SendScore() 
    {
        if (Convert.ToInt32(myScore.text) > PlayerPrefs.GetInt("highscore")) 
        {
            PlayerPrefs.SetInt("highscore", Convert.ToInt32(myScore.text));
            HighScores.UploadScore(myName.text, Convert.ToInt32(myScore.text));
        }
    }
}
