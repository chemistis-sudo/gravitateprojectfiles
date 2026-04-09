using System;
using TMPro;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class MovementSampleScript : MonoBehaviour
{
    // References
    [SerializeField] private Rigidbody2D MyRigidBody;
    private GameObject cam;


    //Gravity
    [SerializeField] private float ChangeBonus;
    [SerializeField] private int gscale = 1;
    [SerializeField] private float DownGravityMultiplier;

    //yPhysics
    public float JumpStrength;
    [SerializeField] private float MaxVel;
    [SerializeField] private float StartVel;

    //xPhysics
    [SerializeField] private float RESISTANCE;
    public float WalkPower;
    [SerializeField] private float FlyPower;


    //Timers
    private float CoyoteTimer = 0;
    [SerializeField] private float CoyoteTime;
    private float FlipTimer = 0;
    [SerializeField] private float FlipTime = 0.25f;
    private float JumpBufferTimer;
    [SerializeField] private float JumpBuffer;
    private float timer;

    //CameraMovement
    [SerializeField] private float camlastpos;
    [SerializeField] private float camfirstpos;

    //Raycast
    [SerializeField] private bool DebugLine;
    [SerializeField] private float GroundCheckRayCastSize;

    //Animation
    [SerializeField] private Animator animator;


    //Misc
    private bool GotKey;
    private Color color;
    private bool paused = false;
    private GameObject canvas;
    private GameObject timercanvas;
    private float framenum = 0;
    private float restarttimer = 0;
    float restarttime = 0.24f;
    private bool firstrestart;
    private float musictimer = 0;

    TextMeshProUGUI musicvolumedisplaytext;

    GameObject UpGates;
    GameObject DownGates;

    GameObject[] GravityChangableObjects;
    AudioSource music;

    void Start()
    {
        // Misc
        musicvolumedisplaytext = GameObject.Find("MusicVolumeDisplay").GetComponent<TextMeshProUGUI>();
        firstrestart = true;
        paused = false;
        MyRigidBody.bodyType = RigidbodyType2D.Dynamic;
        timercanvas = GameObject.Find("TimerText");
        cam = GameObject.FindWithTag("MainCamera");
        MyRigidBody.gravityScale = 1;
        GotKey = false;
        if (timercanvas != null)
        {
            timer = float.Parse(timercanvas.GetComponent<TextMeshProUGUI>().text);
            timercanvas.GetComponent<TextMeshProUGUI>().color = new Color(timercanvas.GetComponent<TextMeshProUGUI>().color.r, timercanvas.GetComponent<TextMeshProUGUI>().color.g, timercanvas.GetComponent<TextMeshProUGUI>().color.b, 1);
            if (timercanvas.transform.parent.name == "TimerFalse") {
                timercanvas.SetActive(false);
            } 
        }
        Time.timeScale = 1;

        GravityChangableObjects = GameObject.FindGameObjectsWithTag("GravityChangable");

        // PauseScreen
        canvas = GameObject.FindWithTag("Canvas");
        canvas.SetActive(false);

        // Get XDoor List
        UpGates = GameObject.FindGameObjectWithTag("UpGate");
        DownGates = GameObject.FindGameObjectWithTag("DownGate");

        // XDoor at start
        UpGates.GetComponent<TilemapCollider2D>().enabled = true;
        DownGates.GetComponent<TilemapCollider2D>().enabled = false;
        color = UpGates.GetComponent<Tilemap>().color;
        color.a = 1f;
        UpGates.GetComponent<Tilemap>().color = color;
        color = DownGates.GetComponent<Tilemap>().color;
        color.a = 0.4f;
        DownGates.GetComponent<Tilemap>().color = color;

        //StartVel

        MyRigidBody.linearVelocity = new Vector2(MyRigidBody.linearVelocity.x, StartVel);

        //Misc
        music = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();
        if (music.isPlaying == false)
        {
            music.Play();
        }
    }

    void Update()
    {
        if (DebugLine)
        {
            Debug.DrawRay(transform.position, Vector2.down * GroundCheckRayCastSize * gscale, Color.red);
        }

        // Input Management

        if (Input.GetKeyDown(KeyCode.Space) && !paused)
        {
            if (OnGround() || CoyoteTimer > 0)
            {
                animator.SetTrigger("Jump");
                MyRigidBody.linearVelocity = new Vector2(MyRigidBody.linearVelocity.x, JumpStrength * gscale);
            }
            else
            {
                JumpBufferTimer = JumpBuffer;
            }
        }

        if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) && !paused)
        {
            flip();
        }

        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !paused)
        {
            move(1);
        }
        else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !paused) 
        {
            move(-1);
        }
        else
        {
            move(0);
        }

        if (Input.GetKeyDown(KeyCode.R) && !paused)
        {
            restart();
        }

        if (Input.GetKey(KeyCode.Backspace) && !paused)
        {
            Quit();   
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                paused = true;
                music.Pause();
                canvas.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                ContinueLevel();
            }
        }

        if (Input.GetKeyDown(KeyCode.T) && !paused && timercanvas != null && SceneManager.GetActiveScene().name != "Tutorial")
        {
             if (timercanvas.activeSelf)
            {
                timercanvas.SetActive(false);
                timercanvas.transform.parent.name = "TimerFalse";
            } else
            {
                timercanvas.SetActive(true);
                timercanvas.transform.parent.name = "TimerTrue";
            }
        }

        if (Input.GetKey(KeyCode.O))
        {
            if ((framenum/10f) == Mathf.Round(framenum/10f))
            {
                music.volume += 0.01f;
            }
            musicvolumedisplaytext.text = Mathf.Round(music.volume * 100).ToString();
            musictimer = 1;
        }
        else if (Input.GetKey(KeyCode.I)) {
            if ((framenum / 10f) == Mathf.Round(framenum / 10f))
            {
                music.volume -= 0.01f;
            }
            musicvolumedisplaytext.text = Mathf.Round(music.volume * 100).ToString();
            musictimer = 1;
        }
        if (Input.GetKeyDown(KeyCode.Tab) && SceneManager.GetActiveScene().name != "Tutorial") {
            if (GameObject.Find("Hardcore"))
            {
                GameObject.Find("Hardcore").name = "HardcoreOn";
                timercanvas.GetComponent<TextMeshProUGUI>().color = new Color(1, 0.5f, 0.5f);
            }
            else {
                GameObject.Find("HardcoreOn").name = "Hardcore";
                timercanvas.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1);
            }
            timercanvas.SetActive(true);
            timer = 0;
            timercanvas.GetComponent<TextMeshProUGUI>().text = "0";
            SceneManager.LoadScene("Level 1");
        }
        if (Input.GetKeyDown(KeyCode.L) && SceneManager.GetActiveScene().name != "Tutorial")
        {
            timercanvas.SetActive(true);
            timer = 0;
            timercanvas.GetComponent<TextMeshProUGUI>().text = "0";
            SceneManager.LoadScene("Level 1");
        }
        if (timercanvas != null && SceneManager.GetActiveScene().name != "Tutorial" && SceneManager.GetActiveScene().name != "Secret")
        {
            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.K) && Input.GetKey(KeyCode.P)) {
                timercanvas.GetComponent<TextMeshProUGUI>().text = (timer + 2000).ToString("F2");
                timercanvas.SetActive(true);
                SceneManager.LoadScene("Level " + Convert.ToString(Convert.ToInt16(SceneManager.GetActiveScene().name.Split(" ")[1]) + 1));
            }
        }

        // MaxVel Limiter

        if (MyRigidBody.linearVelocity.x < (-1 * MaxVel))
        {
            MyRigidBody.linearVelocity = new Vector2(-1 * MaxVel, MyRigidBody.linearVelocity.y);
        }
        if (MyRigidBody.linearVelocity.x > MaxVel)
        {
            MyRigidBody.linearVelocity = new Vector2(MaxVel, MyRigidBody.linearVelocity.y);
        }


        // Camera Movement

        if (transform.position.x > camfirstpos && transform.position.x < camlastpos)
        {
            cam.transform.position = new Vector3(transform.position.x, cam.transform.position.y, cam.transform.position.z);
        }

        // Fall Gravity Applier

        if ((MyRigidBody.linearVelocity.y * gscale) < 0)
        {
            MyRigidBody.gravityScale = DownGravityMultiplier * gscale;
        }
        else
        {
            MyRigidBody.gravityScale = 1 * gscale;
        }

        // Timer Scripts

        if (CoyoteTimer > 0)
        {
            CoyoteTimer = CoyoteTimer - Time.deltaTime;
        }
        if (FlipTimer > 0)
        {
            FlipTimer = FlipTimer - Time.deltaTime;
            if (FlipTimer < 0 || OnGround())
            {
                FlipTimer = 0;
            }
        }
        if (JumpBufferTimer > 0)
        {
            JumpBufferTimer = JumpBufferTimer - Time.deltaTime;
            if (OnGround())
            {
                animator.SetTrigger("Jump");
                MyRigidBody.linearVelocity = new Vector2(MyRigidBody.linearVelocity.x, JumpStrength * gscale);
                JumpBufferTimer = 0;
            }
        }

        if (SceneManager.GetActiveScene().name != "Level 5")
        {
            timer += Time.deltaTime;
        }

        if (restarttimer > 0) {
            restarttimer -= Time.deltaTime;
        } else if (restarttimer < 0){
            if (SceneManager.GetActiveScene().name == "Tutorial")
            {
                SceneManager.LoadScene("Tutorial");
            }
            else
            {
                if (timercanvas.transform.parent.name == "TimerFalse")
                {
                    timercanvas.GetComponent<TextMeshProUGUI>().color = new Color(timercanvas.GetComponent<TextMeshProUGUI>().color.r, timercanvas.GetComponent<TextMeshProUGUI>().color.g, timercanvas.GetComponent<TextMeshProUGUI>().color.b, 0);
                }
                timercanvas.SetActive(true);
                music.Stop();
                if (GameObject.Find("Hardcore") == null || SceneManager.GetActiveScene().name == "Level 1")
                {
                    timer = 0;
                    timercanvas.GetComponent<TextMeshProUGUI>().text = "0";
                    SceneManager.LoadScene("Level 1");
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }

        if (((framenum/5) == Mathf.Floor(framenum / 5)) && SceneManager.GetActiveScene().name != "Level 5") {
            if (timercanvas != null) {
                timercanvas.GetComponent<TextMeshProUGUI>().text = timer.ToString("F2");
            } else {
                timercanvas = GameObject.Find("TimerText");
                if (timercanvas != null)
                {
                    timer = float.Parse(timercanvas.GetComponent<TextMeshProUGUI>().text);
                    timercanvas.GetComponent<TextMeshProUGUI>().color = new Color(timercanvas.GetComponent<TextMeshProUGUI>().color.r, timercanvas.GetComponent<TextMeshProUGUI>().color.g, timercanvas.GetComponent<TextMeshProUGUI>().color.b, 1);
                    if (timercanvas.transform.parent.name == "TimerFalse")
                    {
                        timercanvas.SetActive(false);
                    }
                }
            }
        }

        if (musictimer > 0)
        {
            musictimer -= Time.deltaTime;
            musicvolumedisplaytext.color = new Color(musicvolumedisplaytext.color.r, musicvolumedisplaytext.color.g, musicvolumedisplaytext.color.b, 1);
        } if (musictimer < 0) {
            musictimer = 0;
            musicvolumedisplaytext.color = new Color(musicvolumedisplaytext.color.r, musicvolumedisplaytext.color.g, musicvolumedisplaytext.color.b, 0);
        }

        if (SceneManager.GetActiveScene().name == "Level 5") {
            if (framenum / 120 == Mathf.Floor(framenum / 120))
            {
                if (timercanvas.activeSelf) {
                    timercanvas.SetActive(false);
                } else {
                    timercanvas.SetActive(true);
                }
            }
        }
        
        framenum++;
    }

    private bool OnGround()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down * gscale, GroundCheckRayCastSize, LayerMask.GetMask("Ground")))
        {
            CoyoteTimer = CoyoteTime;
            return true;
        }
        else { return false; }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Getting Key

        if (other.gameObject.CompareTag("Key"))
        {
            GotKey = true;
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("FinishLine"))
        {
            if (SceneManager.GetActiveScene().name == "Tutorial")
            {
                SceneManager.LoadScene("TitleScreen");
            } else if (SceneManager.GetActiveScene().name == "Level 5")
            {
                SceneManager.LoadScene("Secret");
            }
            else if (SceneManager.GetSceneByName("Level " + Convert.ToString(SceneManager.GetActiveScene().name.Split(" ")[1])).IsValid())
            {
                timercanvas.SetActive(true);
                SceneManager.LoadScene("Level " + Convert.ToString(Convert.ToInt16(SceneManager.GetActiveScene().name.Split(" ")[1]) + 1));
                Debug.Log(timer);
            }
            else
            {
                Debug.Log("FinalLevel");
            }

        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Game Object Detectors

        if (other.gameObject.CompareTag("Flipper"))
        {
            flip();
        }
        if (other.gameObject.CompareTag("Door"))
        {
            if (GotKey)
            {
                Destroy(other.gameObject);
            }
        }
        if (other.gameObject.CompareTag("Lava"))
        {
            restart();
        }
        if (other.gameObject.CompareTag("Button"))
        {
            other.gameObject.GetComponent<ButtonScript>().Activate();
        }
    }

    private void flip()
    {
        if (FlipTimer <= 0 || OnGround())
        {
            gscale = gscale * -1;
            MyRigidBody.gravityScale = Mathf.Abs(MyRigidBody.gravityScale) * gscale;
            MyRigidBody.linearVelocity = new Vector2(MyRigidBody.linearVelocity.x,(ChangeBonus * gscale * -1));
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * -1, transform.localScale.z);
            FlipTimer = FlipTime;

            // XDoors

            if (gscale == 1)
            {
                UpGates.GetComponent<TilemapCollider2D>().enabled = true;
                DownGates.GetComponent<TilemapCollider2D>().enabled = false;
                color = UpGates.GetComponent<Tilemap>().color;
                color.a = 1f;
                UpGates.GetComponent<Tilemap>().color = color;
                color = DownGates.GetComponent<Tilemap>().color;
                color.a = 0.4f;
                DownGates.GetComponent<Tilemap>().color = color;
            }
            else {
                UpGates.GetComponent<TilemapCollider2D>().enabled = false;
                DownGates.GetComponent<TilemapCollider2D>().enabled = true;
                color = UpGates.GetComponent<Tilemap>().color;
                color.a = 0.4f;
                UpGates.GetComponent<Tilemap>().color = color;
                color = DownGates.GetComponent<Tilemap>().color;
                color.a = 1f;
                DownGates.GetComponent<Tilemap>().color = color;
            }

            // GravityChangeBlocks

            for (int i = 0; i < GravityChangableObjects.Length; i++) {
                GravityChangableObjects[i].GetComponent<Rigidbody2D>().gravityScale =  GravityChangableObjects[i].GetComponent<Rigidbody2D>().gravityScale * -1;
            }
        }
    }
   

    private void move(int direction)
    {

        if (direction != 0)
        {
            animator.SetBool("IsRunning", true);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);
            if (OnGround())
            {
                MyRigidBody.linearVelocity += new Vector2(WalkPower * direction, 0);
            }
            else
            {
                MyRigidBody.linearVelocity += new Vector2(FlyPower * direction, 0);
            }

        }
        else
        {
            animator.SetBool("IsRunning", false);
            MyRigidBody.linearVelocity = MyRigidBody.linearVelocity * new Vector2(RESISTANCE, 1);
        }
    }

    public void restart()
    {
        if (firstrestart) {
            firstrestart = false;
            paused = true;
            MyRigidBody.bodyType = RigidbodyType2D.Static;
            if (gameObject.GetComponent<AudioSource>() != null)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }
            animator.SetTrigger("FadePlayer");
            restarttimer = restarttime;
        }
    }

    public void Quit()
    {
        Application.Quit();
        // Destroy(GameObject.Find("BackgroundMusic"));
        // Destroy(timercanvas.transform.parent.gameObject);
        // SceneManager.LoadScene("TitleScreen");
    }

    public void ContinueLevel()
    {
        paused = false;
        music.UnPause();
        canvas.SetActive(false);
        Time.timeScale = 1;
    }
}
