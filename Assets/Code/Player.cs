using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    Vector3 outOfBounds = new Vector3(1000, 1000);

    GameObject line;

    Vector3 directionToMove;

    [SerializeField]
    float moveSpeed;

    float angle;

    static bool moving = false;
    static int movesSinceLastLineChange = 0;

    static bool rotatingLine = false;
    static int rotatingFrameCount = 0;

    static bool loopedScreen = false;

    static bool stoppedRotating = false;

    Camera gameCamera;
    static Rect cameraRect;

    Vector3 startOfLine;
    //bool startDefined = false;

    Vector3 endOfLine;
    //bool endDefined = false;

    static bool dead;

    [SerializeField]
    GameObject blood;

    [SerializeField]
    GameObject gameOverScreen;

    AudioManager audioManager;

    //[SerializeField]
    //GameObject timerTextObject;

	// Use this for initialization
	void Awake () {
        directionToMove = Vector3.right;
        angle = 0.0f;
        line = GameObject.Find("Line");
        dead = false;

        gameCamera = Camera.main;
        Vector3 bottomLeft = gameCamera.ScreenToWorldPoint(Vector3.zero);
        Vector3 topRight = gameCamera.ScreenToWorldPoint(new Vector3(gameCamera.pixelWidth, gameCamera.pixelHeight));
        cameraRect = new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);

        findStartAndEndOfLine();
	}
	
    void Start() {
        audioManager = FindObjectOfType<AudioManager>();
    }

	// Update is called once per frame
	void Update () {
        moving = false;
        rotatingLine = false;
        loopedScreen = false;
        stoppedRotating = false;
        if(Input.GetKey(KeyCode.UpArrow)) {
            rotatingLine = true;
            rotatingFrameCount++;
            angle = (angle + 1)%360;
            //Debug.Log("Up and angle is: " + angle);
            Vector3 targetAngle = new Vector3(0, 0, angle);
            //line.transform.eulerAngles = Vector3.Lerp(line.transform.rotation.eulerAngles, targetAngle, Time.deltaTime);
            line.transform.position = transform.position;
            line.transform.eulerAngles = targetAngle;
            directionToMove = new Vector3(Mathf.Cos(angle * Mathf.PI / 180f), Mathf.Sin(angle * Mathf.PI / 180f));
            findStartAndEndOfLine();
        } else if(Input.GetKey(KeyCode.DownArrow)) {
            rotatingLine = true;
            rotatingFrameCount++;
            //Debug.Log("Frames spent moving: " + movesSinceLastLineChange);
            angle = angle - 1;
            if(angle < 0) angle = 360 + angle;
            //Debug.Log("Down and angle is: " + angle);
            Vector3 targetAngle = new Vector3(0, 0, angle);
            //line.transform.eulerAngles = Vector3.Lerp(line.transform.rotation.eulerAngles, targetAngle, Time.deltaTime);
            line.transform.position = transform.position;
            line.transform.eulerAngles = targetAngle;
            directionToMove = new Vector3(Mathf.Cos(angle * Mathf.PI / 180f), Mathf.Sin(angle * Mathf.PI / 180f));
            findStartAndEndOfLine();
        }else if(Input.GetKey(KeyCode.A)) {
            moving = true;
            movesSinceLastLineChange++;
            //Debug.Log("Frames spent moving: " + movesSinceLastLineChange);
            if(!(angle >= 90 && angle <= 270)){
                transform.position -= directionToMove * moveSpeed * Time.deltaTime;
            }else {
                transform.position += directionToMove * moveSpeed * Time.deltaTime;
            }
            //Loop the line if player goes outside camera rect
            if(offCamera()) {
                if(findClosestLineEnd().Equals(startOfLine)) {
                    transform.position = endOfLine;
                } else transform.position = startOfLine;
                loopedScreen = true;
            }
        } else if(Input.GetKey(KeyCode.D)) {
            moving = true;
            movesSinceLastLineChange++;
            if(!(angle >= 90 && angle <= 270)) {
                transform.position += directionToMove * moveSpeed * Time.deltaTime;
            } else {
                transform.position -= directionToMove * moveSpeed * Time.deltaTime;
            }
            //Loop the line if player goes outside camera rect
            if(offCamera()) {
                if(findClosestLineEnd().Equals(startOfLine)) {
                    transform.position = endOfLine;
                } else transform.position = startOfLine;
                loopedScreen = true;
            }
        }

        if(Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow)) {
            stoppedRotating = true;
            rotatingFrameCount = 0;
        }

        //Between 90 and 270, directions should be reversed

    }

    bool offCamera() {
        if(!cameraRect.Contains(transform.position)) return true;
        else return false;
    }

    Vector3 intersectionPoint(Vector3 start1, Vector3 direction1, Vector3 start2, Vector3 direction2) {
        Vector3 diffStartPoints = start2 - start1;
        float det = direction2.x * direction1.y - direction2.y * direction1.x;
        if(det == 0) {
            //Debug.Log("No intersection between line in direction " + direction1 + " and " + direction2 + " or infinitely many.");
            return outOfBounds;
        } else {
            float noDirectionsToIntersection = (direction2.x * diffStartPoints.y - direction2.y * diffStartPoints.x) / det;
            Vector3 intersectionPoint = start1 + noDirectionsToIntersection * direction1;
            return intersectionPoint;
        }
    }

    void findStartAndEndOfLine() { //Must ensure start & end are in camera bounds
        Vector3 leftIntersection = intersectionPoint(transform.position, directionToMove, cameraRect.min, Vector3.up);
        Vector3 bottomIntersection = intersectionPoint(transform.position, directionToMove, cameraRect.min, Vector3.right);
        Vector3 rightIntersection = intersectionPoint(transform.position, directionToMove, cameraRect.max, Vector3.down);
        Vector3 topIntersection = intersectionPoint(transform.position, directionToMove, cameraRect.max, Vector3.left);

        if(leftIntersection.y >= cameraRect.min.y && leftIntersection.y <= cameraRect.max.y) {
            //Start of line is at left of screen so end could be at bot, right or top
            startOfLine = leftIntersection;
            if(bottomIntersection.x >= cameraRect.min.x && bottomIntersection.x <= cameraRect.max.x) {
                endOfLine = bottomIntersection;
            }else if(rightIntersection.y >= cameraRect.min.y && rightIntersection.y <= cameraRect.max.y) {
                endOfLine = rightIntersection;
            }else if(topIntersection.x >= cameraRect.min.x && topIntersection.x <= cameraRect.max.x) {
                endOfLine = topIntersection;
            }
        }else if(bottomIntersection.x >= cameraRect.min.x && bottomIntersection.x <= cameraRect.max.x) {
            //Start of line is at bot of screen so end could be at right or top
            startOfLine = bottomIntersection;
            if(rightIntersection.y >= cameraRect.min.y && rightIntersection.y <= cameraRect.max.y) {
                endOfLine = rightIntersection;
            }else if(topIntersection.x >= cameraRect.min.x && topIntersection.x <= cameraRect.max.x) {
                endOfLine = topIntersection;
            }
        }else if(topIntersection.x >= cameraRect.min.x && topIntersection.x <= cameraRect.max.x) {
            startOfLine = topIntersection;
            if(rightIntersection.y >= cameraRect.min.y && rightIntersection.y <= cameraRect.max.y) {
                endOfLine = rightIntersection;
            }
        }

        //if(leftIntersection.Equals(outOfBounds)) {
        //    Debug.Log("No intersection with left wall");
        //}else {
        //    startOfLine = leftIntersection;
        //    startDefined = true;
        //}

        //if(bottomIntersection.Equals(outOfBounds)) {
        //    Debug.Log("No intersection with bottom wall");
        //} else {
        //    if(!startDefined) {
        //        startOfLine = bottomIntersection;
        //        startDefined = true;
        //    }

        //}
    }

    Vector3 findClosestLineEnd() {
        float distanceToStart = (transform.position - startOfLine).sqrMagnitude;
        float distanceToEnd = (transform.position - endOfLine).sqrMagnitude;
        if(Mathf.Min(distanceToStart, distanceToEnd) == distanceToStart) {
            //Closest to start of line
            return startOfLine;
        } else return endOfLine;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log("Hit detected");
        if(other.tag.Equals("Enemy")) {
            StartCoroutine(die());
        }

        if(other.tag.Equals("SpeedUp")) {
            float speedIncrease = other.gameObject.GetComponent<PowerUp>().getSpeedIncrease();
            Destroy(other.gameObject);
            increaseSpeed(speedIncrease);
        }
    }

    void increaseSpeed(float amount) {
        moveSpeed += amount;
        audioManager.play("PowerUp");
        Debug.Log("Speed is: " + moveSpeed);
    }

    IEnumerator die() {
        Debug.Log("You died!");
        dead = true;
        //timerTextObject.SetActive(false);
        audioManager.stop("MainTheme");
        audioManager.play("Die");
        Instantiate(blood, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.07f);
        gameOverScreen.SetActive(true);
        gameObject.SetActive(false);
    }

    public static bool getMoving() {
        return moving;
    }

    public static int getMovesSinceLastLineChange() {
        return movesSinceLastLineChange;
    }

    public static void resetMovesSinceLastLineChange() {
        movesSinceLastLineChange = 0;
    }

    public static bool getRotatingLine() {
        return rotatingLine;
    }

    public static int getRotatingFrameCount() {
        return rotatingFrameCount;
    }

    public static void resetRotatingFrameCount() {
        rotatingFrameCount = 0;
    }

    public static bool getLoopedScreen() {
        return loopedScreen;
    }

    public static bool getStoppedRotating() {
        return stoppedRotating;
    }

    public static Rect getCameraRect() {
        return cameraRect;
    }

    public static bool isDead() {
        return dead;
    }
}
