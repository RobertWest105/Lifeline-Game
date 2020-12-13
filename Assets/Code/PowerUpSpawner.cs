using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour {

    [SerializeField]
    GameObject speedUp;

    Rect cameraRect;

    [SerializeField]
    int spawnAreaOffset;

    static bool isPowerUpSpawned = false;

    [SerializeField]
    int framesBetweenSpawns;

    int frameCount;

    Rect spawnArea;

    // Use this for initialization
    void Start () {
        frameCount = 0;
        cameraRect = Player.getCameraRect();
        spawnArea = new Rect(cameraRect.min + new Vector2(spawnAreaOffset, spawnAreaOffset), cameraRect.size - new Vector2(2 * spawnAreaOffset, 2 * spawnAreaOffset));
    }
	
	// Update is called once per frame
	void Update () {
        if(!Player.getRotatingLine() && !Player.isDead()) {
            if(!isPowerUpSpawned) frameCount++;
            if((frameCount >= framesBetweenSpawns) && !isPowerUpSpawned) {
                Vector3 spawnPos = new Vector3(Random.Range(spawnArea.min.x, spawnArea.max.x), Random.Range(spawnArea.min.y, spawnArea.max.y));
                Instantiate(speedUp, spawnPos, Quaternion.identity);
                isPowerUpSpawned = true;
                frameCount = 0;
            }
        }
    }

    public static void powerUpDespawned() {
        isPowerUpSpawned = false;
    }
}
