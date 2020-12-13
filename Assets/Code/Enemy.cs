using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    [SerializeField]
    float startSpeed;

    [SerializeField]
    float acceleration;

    [SerializeField]
    float accMultStopped;

    [SerializeField]
    float accMultOnGrow;

    [SerializeField]
    float accMultOnShrink;

    float speed;

    float sizeMultiplier = 1;

    [SerializeField]
    float sizeIncrease;

    [SerializeField]
    float maxSize;

    [SerializeField]
    float minSize;

    [SerializeField]
    int shrinkThreshold;

    SphereCollider sCollider;

    GameObject player;

    Rect cameraRect;

    //Rigidbody rb;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        sCollider = GetComponent<SphereCollider>();
        //rb = GetComponent<Rigidbody>();
        speed = startSpeed;

        cameraRect = Player.getCameraRect();
        Vector2 spawnAreaBottomLeft = player.transform.position + 3 * (new Vector3(-1,1));
        Rect spawnArea = new Rect(spawnAreaBottomLeft, cameraRect.max - spawnAreaBottomLeft - new Vector2(0.5f,0.5f));
        transform.position = new Vector3(Random.Range(spawnArea.min.x, spawnArea.max.x), Random.Range(spawnArea.min.y, spawnArea.max.y));
    }
	
	// Update is called once per frame
	void Update () {
        bool playerMoving = Player.getMoving();
        bool playerRotating = Player.getRotatingLine();

        //Change acceleration if player rotating
        float accelerationMultiplier = 1.0f;
        //extraAcceleration += playerMoving ? 1 : 0;
        accelerationMultiplier = playerRotating ? accMultStopped : 1.0f;
        

        //Grow
        if(Player.getLoopedScreen()) {
            accelerationMultiplier = accMultOnGrow;
            sizeMultiplier = 1 + sizeIncrease;
            if(transform.localScale.x > maxSize) sizeMultiplier = 1;
            transform.localScale = sizeMultiplier * transform.localScale;
            //sCollider.radius *= sizeMultiplier;
        }

        //Shrink
        if(Player.getStoppedRotating()) {
            //accelerationMultiplier = accMultOnShrink;
            sizeMultiplier = 1 - sizeIncrease;
            if(transform.localScale.x <= minSize) sizeMultiplier = 1;
            transform.localScale = sizeMultiplier * transform.localScale;
            //sCollider.radius *= sizeMultiplier;
            Player.resetMovesSinceLastLineChange();
        }

        //Accelerate
        StartCoroutine(accelerate(accelerationMultiplier));

        //Move
        if(!playerRotating) {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * speed);
        }
    }

    //IEnumerator move(Vector3 target) {
    //    float sqDistanceLeft = (rb.position - target).sqrMagnitude;

    //    while(sqDistanceLeft > float.Epsilon) {
    //        Vector3 nextPosition = Vector3.MoveTowards(rb.position, target, speed * Time.deltaTime);
    //        rb.MovePosition(nextPosition);
    //        sqDistanceLeft = (rb.position - target).sqrMagnitude;
    //        yield return null;
    //    }
    //}

    IEnumerator accelerate(float modifier) {
        yield return new WaitForSeconds(1);
        speed += (acceleration * modifier) * Time.deltaTime;
        if(speed < startSpeed) speed = startSpeed;
        
    }
}
