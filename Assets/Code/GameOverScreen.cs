using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameOverScreen : MonoBehaviour {

    [SerializeField]
    Text finalTimeText;

    string finalTime;

    void OnEnable() {
        finalTime = Timer.getFinalTimerText();
        finalTimeText.text += finalTime;
    }

    public void restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

	public void quit() {
        Debug.Log("Quit");
        Application.Quit();
    }
}
