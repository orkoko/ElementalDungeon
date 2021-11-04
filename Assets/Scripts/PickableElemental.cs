using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickableElemental : ElementalBase {
    
    #region Public Members
    public bool isClicked;
    #endregion

    #region Public Events
    public delegate void PlayerChooseElementEvent();
    public event PlayerChooseElementEvent OnPlayerChooseElement;
    #endregion

    // Use this for initialization
    void Start() {
        isClicked = false;
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(() => { OnClick(); });
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnClick()
    {
        isClicked = true;
        //OnPlayerChooseElement();
    }
    public void Hide()
    {
        GetComponentInChildren<Animator>().SetBool("Selected", false);
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        GetComponentInChildren<Animator>().SetBool("Selected", true);
        
    }
}
