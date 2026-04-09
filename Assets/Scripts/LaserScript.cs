using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserScript : MonoBehaviour
{

    public LineRenderer linerender;
    public string SolarPanelBeingHitName;
    private GameObject logic;
    public GameObject Barell;
    [SerializeField] private bool inverse;
    void Start()
    {
        logic = GameObject.Find("ActivatesLogicManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (logic.GetComponent<ActivateLogicScript>().CheckActiveStatus(gameObject.name) != inverse) {
            linerender.enabled = true;
            if (Physics2D.Raycast(transform.GetChild(0).transform.position, transform.right)) {
                RaycastHit2D hit = Physics2D.Raycast(transform.GetChild(0).transform.position, transform.right);
                linerender.SetPosition(0, transform.GetChild(0).transform.position);
                linerender.SetPosition(1, hit.point);

                if (hit.transform.gameObject.CompareTag("Player")) {
                    hit.transform.gameObject.GetComponent<MovementSampleScript>().restart();

                }
                if (hit.transform.gameObject.CompareTag("SolarPanel"))
                {
                    SolarPanelBeingHitName = hit.transform.gameObject.name;
                }
                else
                {
                    SolarPanelBeingHitName = "nothing";
                }
            } else
            {
                linerender.SetPosition(0, transform.GetChild(0).transform.position);
                linerender.SetPosition(1, transform.right * 100);
            }
        }
        else
        {
            SolarPanelBeingHitName = "nothing";
            linerender.enabled = false;
        }
    }
}
