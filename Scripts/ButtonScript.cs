using System.Threading;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField]bool firsttime = true;
    public bool Active;
    private Sprite normalbuttonsprite;
    [SerializeField] private Sprite pusheddownbuttonsprite;
    [SerializeField] private float downtime;
    [SerializeField] private bool inverse;
    private float downtimer = 0;
    private AudioSource sound;

    void Start()
    {
        normalbuttonsprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        sound = gameObject.GetComponent<AudioSource>();
        sound.pitch = 3;
        sound.volume = 0.2f;
    }

    void Update()
    {
        if (downtime != 0) {
            if (downtimer != 0)
            {
                if (downtimer > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = pusheddownbuttonsprite;
                    downtimer -= Time.deltaTime;

                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = normalbuttonsprite;
                    Active = inverse;
                    downtimer = 0;
                    gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0.04f, -0.19f);
                    gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.72f, 0.3f);
                    firsttime = true;
                }
            }
            else
            {
                Active = inverse;
            }
        } 
    }

    public void Activate()
    {
        if (firsttime) {
            firsttime = false;
            if (inverse)
            {
                Active = false;
            } else
            {
                Active = true;
            }
            sound.Play();
            gameObject.GetComponent<SpriteRenderer>().sprite = pusheddownbuttonsprite;
            gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0.04f, -0.24f);
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.72f, 0.16f);
            if (downtime != 0) {
                downtimer = downtime; 
            }
        }     
    }
}
