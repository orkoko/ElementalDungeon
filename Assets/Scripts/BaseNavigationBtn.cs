using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseNavigationBtn : MonoBehaviour
{
    // step parameter of this button
    public Player.Step step;
    public Player player;
    public bool isPressed;


    // Start is called before the first frame update
    void Start()
    {
        isPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPressed)
            player.WhenStepChosen(step);
    }
    public void OnPress()
    {
        isPressed = true;

    }
    public void OnRelease()
    {
        isPressed = false;

    }
}
