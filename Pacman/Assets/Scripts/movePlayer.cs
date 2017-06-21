using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class movePlayer : MonoBehaviour {
    private Rigidbody rb;
    public float speed;
    private int points;
    public Text scoreText;
    public Text highScoreText;
    public Text PowerUpText;
    public Text loseText;
    public Text ateEnemyText;
    public int hightscore;
    public bool gameOver;
    public bool invulnerabilityActive;
    private int invulnerableTime;
    private Vector3[] teleportLocations = new Vector3[15];
    private bool teleportationActive;
    private int tagTeleport;
    private bool walkWalls;
    public GameObject[] walls;
    public float time;
    public int canTeleport;
    bool rotateLeft, rotatedRight, rotateFront, rotateBack;

    public AudioSource general;
    public AudioSource win;
    public AudioSource lose;
    public AudioSource[] money = new AudioSource [10];
    public AudioSource eat;

    private int invulnerabilityLeft;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        //points = 0;
        points = GlobalControl.Instance.points;
        if(GlobalControl.Instance.here == 1)
            transform.position = GlobalControl.Instance.player_pos;

        setTextScore();
        highScoreText.text = "Highscore : " + hightscore.ToString();
        setLoseText(0);
        setPowerUpText(0);
        gameOver = false;
        invulnerabilityActive = false;
        invulnerableTime = 6;
        setTeleportLocations();
        teleportationActive = false;
        tagTeleport = 0;
        walkWalls = false;
        time = 3.0f;
        setateEnemyText(0);
        canTeleport = 1;
        rotateBack = rotatedRight = rotateLeft = false;
        rotateFront = true;

        general.Play();
        
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SavePlayer();
            SceneManager.LoadScene("menu");
        }
        if (gameOver)
            return;
        if (!general.isPlaying)
            general.Play();

       if (SceneManager.GetActiveScene().name.CompareTo("main") == 0)
             if(points > 40)
                    SceneManager.LoadScene("level2");
       if (SceneManager.GetActiveScene().name.CompareTo("level2") == 0)
                if (points > 65)
                    SceneManager.LoadScene("level3");
        if (SceneManager.GetActiveScene().name.CompareTo("level3") == 0)
            if (points > 80)
            {
                setLoseText(-6);
                gameOver = true;
            }


        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 misc = rb.position;
        misc.x = misc.x - vertical * speed;
        misc.z = misc.z + horizontal*speed;
        rb.MovePosition(misc);
        /*
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
        Vector3 pivotTransVector;
        pivotTransVector.x = transform.position.x;
        pivotTransVector.y = transform.position.y;
        pivotTransVector.z = transform.position.z;
        //transform.rotation =  transform.RotateAround(pivotTransVector, Vector3.right, x);
        //transform.Translate(0, 0, z);
        */


        if (canTeleport > 0)
        {
            if (Input.GetKeyDown(KeyCode.T) && teleportationActive)
            {
                canTeleport = -1;
                transform.position = teleportLocations[tagTeleport];
                teleportationActive = false;
                StartCoroutine(deactivateTeleportation(time));
            } /*else
                if (tagTeleport >= 8)
                {
                    canTeleport = -1;
                    transform.position = teleportLocations[tagTeleport];
                    teleportationActive = false;
                    StartCoroutine(deactivateTeleportation(time));
                }*/
            
        }

        if (Input.GetKeyDown(KeyCode.P) )
        {
            SavePlayer();
            SceneManager.LoadScene("menu");
        }
        // down
        if (Input.GetKeyDown(KeyCode.S) && !rotateBack)
        {
            transform.Rotate(0, -180, 0);
            rotateBack = true;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            rotateBack = false;
            transform.Rotate(0, 180, 0);
        }

        // left
        if (Input.GetKeyDown(KeyCode.A) && !rotateLeft)
        {
            transform.Rotate(0, -90, 0);
            rotateLeft = true;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            rotateLeft = false;
            transform.Rotate(0, 90, 0);
        }

        // right
        if (Input.GetKeyDown(KeyCode.D) && !rotatedRight)
        {
            transform.Rotate(0, 90, 0);
            rotatedRight = true;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            rotatedRight = false;
            transform.Rotate(0, -90, 0);
        }
    }


    public void SavePlayer()
    {
        GlobalControl.Instance.points = points;
        GlobalControl.Instance.player_pos = transform.position;
        GlobalControl.Instance.here = 1;
        GlobalControl.Instance.level = SceneManager.GetActiveScene().name;
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.CompareTag("points"))
        {
            money[0].Play();
            other.gameObject.SetActive(false);
            points = points + 1;
            
            setTextScore();
            /*
            for (int i= 0; i<10; i++)
            {
                if (!money[i].isPlaying)
                {
                    money[i].Play();
                    i = 11;
                }
            }*/

        
        }
       
        //power1 = invulnerability
        if (other.gameObject.CompareTag("power1"))
        {
            other.gameObject.SetActive(false);
            invulnerabilityLeft = 6;
            invulnerabilityActive = true;
            StartCoroutine(loseInvurnelability(invulnerableTime));
            setPowerUpText(1);
        }
        //power2 = walk through walls
        if (other.gameObject.CompareTag("power2"))
        {
            walkWalls = true;
            invulnerabilityLeft = 6;
            other.gameObject.SetActive(false);
            StartCoroutine(loseWalkWalls(invulnerableTime));
            setPowerUpText(2); 
            
        }
        

        if (other.gameObject.CompareTag("enemy"))
        {
            if (!invulnerabilityActive)
            {
                gameOver = true;
                general.Stop();
                lose.Play();
                setLoseText(1);
                
            }
            else
            {
                eat.Play();
                other.gameObject.SetActive(false);
                points = points + 50;
                setateEnemyText(1);
            }                   
        }


        //highScoreText.text = other.gameObject.tag;
        checkTeleports(other);

    }

    private void OnTriggerExit(Collider other)
    {
        teleportationActive = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("wall"))
            if (walkWalls) { 
                collision.collider.isTrigger = true;
                StartCoroutine(resetWalls(2, collision));
            }
            
    }

    void checkTeleports(Collider other)
    {
        
        if (other.gameObject.CompareTag("teleport1_1"))
        {
            teleportationActive = true;
            tagTeleport = 1;
           // StartCoroutine(deactivateTeleportation(time));
        }
        if (other.gameObject.CompareTag("teleport1_2") )
        {
            teleportationActive = true;
            tagTeleport = 0;
           // StartCoroutine(deactivateTeleportation(time));
        }
        ////////////////////////////
        if (other.gameObject.CompareTag("teleport2_1"))
        {
            teleportationActive = true;
            tagTeleport = 3;
            //StartCoroutine(deactivateTeleportation(time));
        }
        if (other.gameObject.CompareTag("teleport2_2"))
        {
            teleportationActive = true;
            tagTeleport = 2;
           // StartCoroutine(deactivateTeleportation(time));
        }
        //////////////////////////////////
        if (other.gameObject.CompareTag("teleport3_1"))
        {
            teleportationActive = true;
            tagTeleport = 5;
           // StartCoroutine( deactivateTeleportation(time));
        }
        if (other.gameObject.CompareTag("teleport3_2"))
        {
            teleportationActive = true;
            tagTeleport = 4;
           // StartCoroutine(deactivateTeleportation(time));
        }
        if (other.gameObject.CompareTag("teleport4_1"))
        {
            teleportationActive = true;
            tagTeleport = 7;
            //StartCoroutine(deactivateTeleportation(time));
        }
        if (other.gameObject.CompareTag("teleport4_2"))
        {
            teleportationActive = true;
            tagTeleport = 6;
           // StartCoroutine(deactivateTeleportation(time));
        }

        /////////////////////////////////////////////////////
        // sides
        if (other.gameObject.CompareTag("GR1"))
        {
            teleportationActive = true;
            tagTeleport = 8;
            // StartCoroutine(deactivateTeleportation(time));
        }
        if (other.gameObject.CompareTag("GR2"))
        {
            teleportationActive = true;
            tagTeleport = 9;
            // StartCoroutine(deactivateTeleportation(time));
        }
        if (other.gameObject.CompareTag("GL1"))
        {
            teleportationActive = true;
            tagTeleport = 10;
            // StartCoroutine(deactivateTeleportation(time));
        }
        if (other.gameObject.CompareTag("GL2"))
        {
            teleportationActive = true;
            tagTeleport = 11;
            // StartCoroutine(deactivateTeleportation(time));
        }

    }

    void setTextScore()
    {
        scoreText.text = "Score : " + points.ToString();
    }

    void setateEnemyText(int i)
    {
        if(i == 0)
        {   
            ateEnemyText.text = "";
        }
        else
        {
            ateEnemyText.text = "NOM NOM NOM";
            //eat.Play();
            StartCoroutine(ateEnemy(3));
            
        }
    }
    void setLoseText(int i)
    {
        if (i == 0)
            loseText.text = "";
        else
        if(i == 1)
        {
            loseText.text = "YOU LOSE";
            general.Stop();
            lose.Play();
        }

        if (i == -6)
        {
            loseText.text = "OH ....YOU WIN";
            general.Stop();
            win.Play();
        }
    }

    void setPowerUpText(int power)
    {
        if (power == 0)
            PowerUpText.text = "";
        if(power == 1)
        {   if (invulnerabilityLeft > 0)
            {
                PowerUpText.text = "INVULNERABILITY : " + invulnerabilityLeft.ToString();
                invulnerabilityLeft--;
                StartCoroutine(erasePowerUpText(1, power));
            }
            else setPowerUpText(0);
        }
        if(power == 2)
        {
            if (invulnerabilityLeft > 0)
            {
                PowerUpText.text = "GHOST RUN : " + invulnerabilityLeft.ToString();
                invulnerabilityLeft--;
                StartCoroutine(erasePowerUpText(1, power));
               
            }
            else setPowerUpText(0);
        }
    }

    IEnumerator erasePowerUpText(float time, int power)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
        setPowerUpText(power);
    }

    IEnumerator loseInvurnelability(float time)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
        invulnerabilityActive = false;
    }

    IEnumerator loseWalkWalls(float time)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
        walkWalls = false;
    }

    IEnumerator deactivateTeleportation(float time)
    {
        yield return new WaitForSeconds(2);

        // Code to execute after the delay
        canTeleport = 2;
    }

    IEnumerator resetWalls(float time, Collision collision)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay 
        collision.collider.isTrigger = false;
    }

    IEnumerator ateEnemy(float time)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay 
        setateEnemyText(0);
    }




    void setTeleportLocations()
    {
        teleportLocations[0] = new Vector3(GameObject.FindGameObjectWithTag("teleport1_1").transform.position.x,
            GameObject.FindGameObjectWithTag("Player").transform.position.y,
            GameObject.FindGameObjectWithTag("teleport1_1").transform.position.z);
        teleportLocations[1] = new Vector3(GameObject.FindGameObjectWithTag("teleport1_2").transform.position.x,
            GameObject.FindGameObjectWithTag("Player").transform.position.y,
            GameObject.FindGameObjectWithTag("teleport1_2").transform.position.z);

        teleportLocations[2] = new Vector3(GameObject.FindGameObjectWithTag("teleport2_1").transform.position.x,
             GameObject.FindGameObjectWithTag("Player").transform.position.y,
             GameObject.FindGameObjectWithTag("teleport2_1").transform.position.z);
        teleportLocations[3] = new Vector3(GameObject.FindGameObjectWithTag("teleport2_2").transform.position.x,
            GameObject.FindGameObjectWithTag("Player").transform.position.y,
            GameObject.FindGameObjectWithTag("teleport2_2").transform.position.z);

        teleportLocations[4] = new Vector3(GameObject.FindGameObjectWithTag("teleport3_1").transform.position.x,
            GameObject.FindGameObjectWithTag("Player").transform.position.y,
            GameObject.FindGameObjectWithTag("teleport3_1").transform.position.z);
        teleportLocations[5] = new Vector3(GameObject.FindGameObjectWithTag("teleport3_2").transform.position.x,
            GameObject.FindGameObjectWithTag("Player").transform.position.y,
            GameObject.FindGameObjectWithTag("teleport3_2").transform.position.z);

        teleportLocations[6] = new Vector3(GameObject.FindGameObjectWithTag("teleport4_1").transform.position.x,
            GameObject.FindGameObjectWithTag("Player").transform.position.y,
            GameObject.FindGameObjectWithTag("teleport4_1").transform.position.z);
        teleportLocations[7] = new Vector3(GameObject.FindGameObjectWithTag("teleport4_2").transform.position.x,
            GameObject.FindGameObjectWithTag("Player").transform.position.y,
            GameObject.FindGameObjectWithTag("teleport4_2").transform.position.z);

        // sides
        teleportLocations[8] = new Vector3(GameObject.FindGameObjectWithTag("GL1").transform.position.x,
           GameObject.FindGameObjectWithTag("Player").transform.position.y,
           GameObject.FindGameObjectWithTag("GL1").transform.position.z);
        teleportLocations[9] = new Vector3(GameObject.FindGameObjectWithTag("GL2").transform.position.x,
            GameObject.FindGameObjectWithTag("Player").transform.position.y,
            GameObject.FindGameObjectWithTag("GL2").transform.position.z );

        teleportLocations[10] = new Vector3(GameObject.FindGameObjectWithTag("GR1").transform.position.x,
           GameObject.FindGameObjectWithTag("Player").transform.position.y,
           GameObject.FindGameObjectWithTag("GR1").transform.position.z);
        teleportLocations[11] = new Vector3(GameObject.FindGameObjectWithTag("GR2").transform.position.x,
            GameObject.FindGameObjectWithTag("Player").transform.position.y,
            GameObject.FindGameObjectWithTag("GR2").transform.position.z);
    }

}
