using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyClass
{
    public string username;
    public string score;
    public string test;
}

public class leaderBoard : MonoBehaviour
{
    public Text Line1;
    public Text Line2;
    public Text Line3;
    public Text Line4;
    public Text Line5;
    public Text Score1;
    public Text Score2;
    public Text Score3;
    public Text Score4;
    public Text Score5;

    public Button SubmitButton;
    public InputField FieldUsername;
    public Game game;

    public bool firstClick = true;

    //public Object[] tiles = {}
    // Use this for initialization
    void Start()
    {
        Button btn = SubmitButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);

        MyClass myFireObject = new MyClass();
        myFireObject.test = "new user";
        string fireobject = JsonUtility.ToJson(myFireObject);

        void TaskOnClick()
        {
            if (!firstClick)
            {
                return;
            }
            firstClick = false;
            var usernametext = FieldUsername.text;// this would be set somewhere else in the code
            var scoretext = game.level.ToString();
            MyClass myObject = new MyClass();
            myObject.username = FieldUsername.text;
            myObject.score = game.level.ToString();
            string json = JsonUtility.ToJson(myObject);

        }
    }
}
