using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SaveData : MonoBehaviour
{
    public TMPro.TextMeshProUGUI myScore;
    public TMPro.TMP_InputField myName;
    public TMPro.TextMeshProUGUI errorText;
    public Button button;

    public void SendScore() 
    {
        if (myName.text.Length == 0)
        {
            StartCoroutine(ErrorText());
            return;
        }

        myName.text = myName.text.ToUpper();
        
        if (HighScores.instance.scoreList != null || HighScores.instance.scoreList.Length > 0 || Convert.ToInt32(myScore.text) >= HighScores.instance.scoreList[^1].score) 
        {
            HighScores.UploadScore(myName.text, Convert.ToInt32(myScore.text));
            StartCoroutine(GoBackToTitle());
        }
    }

    private void OnEnable()
    {
        button.interactable = true;
    }

    private IEnumerator ErrorText()
    {
        errorText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        errorText.gameObject.SetActive(false);
    }

    private IEnumerator GoBackToTitle()
    {
        button.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        GameManager.Instance.BackToTitle();
    }
}
