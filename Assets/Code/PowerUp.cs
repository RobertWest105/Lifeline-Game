using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

    [SerializeField]
    float speedIncrease;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void OnDisable() {
        PowerUpSpawner.powerUpDespawned();
        Debug.Log("PowerUp gone");
    }

    public float getSpeedIncrease() {
        return speedIncrease;
    }
}
