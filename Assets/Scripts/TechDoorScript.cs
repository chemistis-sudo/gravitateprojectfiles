using UnityEngine;
using UnityEngine.UIElements;

public class TechDoorScript : MonoBehaviour
{
   
    [SerializeField]private bool active;
    private GameObject logic;
    [SerializeField] private float h;
    [SerializeField] private float offset = 0;
    [SerializeField] private float speedmultiply = 2;
    Vector3 OrigPos;
    [SerializeField] private bool startopen;

    void Start()
    {
        logic = GameObject.Find("ActivatesLogicManager");
        h = gameObject.GetComponent<SpriteRenderer>().bounds.size.y;
        OrigPos = transform.position;
        if (startopen)
        {
            offset = h; 
        }
    }


    void Update()
    {
        active = logic.GetComponent<ActivateLogicScript>().CheckActiveStatus(gameObject.name);

        if (active) {
            if (offset < h) {
                offset += Time.deltaTime * speedmultiply;
                transform.position = OrigPos + (transform.up * offset);
            } else
            {
                offset = h;
            }
        } else
        {
            if (offset > 0)
            {
                offset -= Time.deltaTime * speedmultiply;
                transform.position = OrigPos + (transform.up * offset);
            } else
            {
                offset = 0;
            }
        }
    }
}
