using System;
using UnityEngine;

public class SolarPanelScript : MonoBehaviour
{
    private GameObject[] Lasers;
    private bool forloopram = false;
    public bool Active = false;
    private Sprite origsprite;
    public Sprite ActiveSprite;
    private SpriteRenderer rendererofsprite;

    void Start()
    {

        Lasers = GameObject.FindGameObjectsWithTag("Laser");
        rendererofsprite = gameObject.GetComponent<SpriteRenderer>();
        origsprite = rendererofsprite.sprite;

    }

    void Update()
    {
        forloopram = false;
        for (int i = 0; i < Lasers.Length; i++) {
            if (Lasers[i].GetComponent<LaserScript>().SolarPanelBeingHitName == gameObject.name)
            {
                forloopram = true;
                break;
            }
        }
        Active = forloopram;

        if (Active) {
            rendererofsprite.sprite = ActiveSprite;
        } else
        {
            rendererofsprite.sprite = origsprite;
        }
    }
}
