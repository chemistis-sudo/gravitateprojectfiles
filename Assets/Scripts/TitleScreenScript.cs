using System;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TitleScreenScript : MonoBehaviour
{

    [SerializeField] private GameObject canvas;
    private Animator animator;
    private GameObject music;
    private float y = 0;
    [SerializeField]private float x = 0;
    private float xtimer = 0;
    private float ytimer = 0;
    private GameObject timer;
    [SerializeField]private GameObject titlemusic;

    GameObject cam;
    void Start()
    {
        DontDestroyOnLoad(GameObject.Find("Hardcore"));

        // BackgroundMusic
        music = GameObject.Find("BackgroundMusic");
        music.SetActive(false);

        // Animator
        animator = canvas.GetComponent<Animator>();
        animator.SetTrigger("Fade");

        // Timer
        timer = GameObject.Find("TimerText");
        timer.SetActive(false);

        cam = GameObject.Find("Space");

    }
    void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0f , y +0.5f, -16.1f));
        transform.position = new Vector3(transform.position.x + (4f * Time.deltaTime), transform.position.y + (1f * Time.deltaTime), transform.position.z);

        if ((transform.position.x > -40) || (transform.position.y > 25))
        {
            transform.position = new Vector3(-90, UnityEngine.Random.Range(-7, 15), transform.position.z);
        }
        y += 0.5f;
        if (cam.transform.position.x > -1381)
        {
            cam.transform.position = new Vector3(cam.transform.position.x - (Time.deltaTime / 2), cam.transform.position.y, cam.transform.position.z);
        }

        if (xtimer > 0)
        {
            xtimer -= Time.deltaTime;
        } if (xtimer < 0) {
            music.SetActive(true);
            timer.SetActive(true);
            timer.GetComponent<TextMeshProUGUI>().text = "0";
            timer.GetComponent<TextMeshProUGUI>().color = new Color(timer.GetComponent<TextMeshProUGUI>().color.r, timer.GetComponent<TextMeshProUGUI>().color.g, timer.GetComponent<TextMeshProUGUI>().color.b, 0);
            SceneManager.LoadScene("Level 1");
        }
        if (ytimer > 0)
        {
            xtimer -= Time.deltaTime;
        }
        if (ytimer < 0)
        {
            music.SetActive(true);
            timer.SetActive(true);
            SceneManager.LoadScene("Tutorial");
        }
    }
    
    public void play() {
        DontDestroyOnLoad(music);
        DontDestroyOnLoad(timer.transform.parent);
        titlemusic.SetActive(false);
        animator.SetTrigger("FadeIn");
        xtimer = x;
    }

    public void tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void quit() {
        Application.Quit();
    }
}
