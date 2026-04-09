using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float parallaxstrength;
    private GameObject cam;
    private float origx;
    void Start()
    {
        cam = GameObject.FindWithTag("MainCamera");
        origx = transform.position.x;
    }
    void Update()
    {
        transform.position = new Vector3(origx + (cam.transform.position.x * parallaxstrength), transform.position.y, transform.position.z);
    }
}
