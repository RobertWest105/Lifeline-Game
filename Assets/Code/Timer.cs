using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour {

    Text timerText;
    static string finalTimerText = "";

    float theTime = 0.0f;
    public float speed;

	// Use this for initialization
	void Start () {
        timerText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if(Player.isDead()) {
            finalTimerText = timerText.text.Substring(15);
            gameObject.SetActive(false);
            return;
        }

        if(!Player.getRotatingLine()) {
            theTime += Time.deltaTime * speed;
        }
            
        //string hours = Mathf.Floor((theTime % 216000) / 3600).ToString("00");
        string minutes = ((int)theTime / 60).ToString();
        string seconds = (theTime % 60).ToString("f2");
        timerText.text = "Time Survived: " + minutes + ":" + seconds;
        
	}

    //void OnDisable() {
    //    finalTimerText = timerText.text.Substring(15);
    //}

    //IEnumerator findFinalTime() {
    //    finalTimerText = timerText.text.Substring(15);
    //    yield return new WaitForSeconds(1);
    //}

    public static string getFinalTimerText() {
        return finalTimerText;
    }
}
