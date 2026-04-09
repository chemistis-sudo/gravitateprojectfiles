using UnityEngine;

public class ActivateLogicScript : MonoBehaviour
{

    GameObject[] Buttons;
    GameObject[] Panels;

    void Start()
    {
        Buttons = GameObject.FindGameObjectsWithTag("Button");
        Panels = GameObject.FindGameObjectsWithTag("SolarPanel");
    }

    void Update()
    {
        
    }

    public bool CheckActiveStatus(string name)
    {
        for (int i = 0; i < Buttons.Length; i++) {
            if (Buttons[i].name == name) {
                return Buttons[i].GetComponent<ButtonScript>().Active;
            }
        }
        for (int i = 0; i < Panels.Length; i++)
        {
            if (Panels[i].name == name)
            {
                return Panels[i].GetComponent<SolarPanelScript>().Active;
            }
        }
        return true;
    }
}
