using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpPanel : MonoBehaviour
{
    public Button helpButton;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        helpButton.onClick.AddListener(() => { ShowHelp(); });
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HideIfClicked();
    }

    private void HideIfClicked()
    {
        if (Input.anyKey)
        {
            HideHelp();
        }
    }

    public void ShowHelp()
    {
        animator.SetBool("open", true);
        //gameObject.SetActive(true);
    }



    public void HideHelp()
    {
        animator.SetBool("open", false);
        //gameObject.SetActive(false);

    }
}
