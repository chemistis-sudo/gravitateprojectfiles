using UnityEngine;

public class MoveUpandDownScript : MonoBehaviour
{
    [SerializeField] private float floattime;
    [SerializeField] private float speed;
    private float offset = 0.1f;
    
    bool goingup = true;
    Vector3 origpos;

     void Start()
     {
        origpos = gameObject.transform.position;
    }   

    // Update is called once per frame
    void Update()
    {
        if (offset > floattime)
        {
            goingup = false;
        }
        if (offset < (-1 * floattime)) {
            goingup = true;
        }
        if (goingup)
        {
            offset += Time.deltaTime * speed;
        }
        else {
            offset -= Time.deltaTime * speed;
        }
        transform.position = new Vector3(origpos.x, origpos.y + offset, origpos.z);
    }
}
