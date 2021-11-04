using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    #region Player's Enums

    /// <summary>
    /// Player's available steps
    /// </summary>
    public enum Step
    {
        LEFT,
        STRAIGHT,
        RIGHT
    }
    #endregion

    #region Public Fields

    // game items
    public Game game;
    public GameObject canvas;
    public GameObject leadBoard;
    public Button leftBtn;
    public Button strightBtn;
    public Button rightBtn;

    // player's art/animations
    public GameObject body;
    public SpriteRenderer sprRend;
    public Animator anm;

    #endregion

    #region Public Events
    public delegate void PlayerPickChestEvent(GameObject chest);
    public event PlayerPickChestEvent OnPlayerPickChest;
    #endregion

    // Use this for initialization
    void Start ()
    {
        m_isMoving = false;
        sprRend = body.GetComponent<SpriteRenderer>();
        anm = body.GetComponent<Animator>();
        anm.SetBool("IsMoving", false);
        m_playerPos = Game.Position.MID;
        InitalizePathToSteps();
        startPosition = target = transform.position;

        // get player's animation for death
        m_playerAnim = body.GetComponent<Animator>();

        // get UI animation for game over
        m_canvasAnim = canvas.GetComponent<Animator>();
        m_leaderBoardAnim = leadBoard.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update ()
    {
        // while player chooses element(s), no other action can be taken and the player cant move
        // until element(s) where chosen.
        if (m_isLooting)
        {
            if (game.TryToPickElements() && game.hp > 0)
            {
                m_isLooting = false;
            }
        }
        
        // while moving no action can be taken
        else if (m_isMoving)
        {
            // we do not want the user to see the next map while walking
            game.lowerFog();
            // decide on direction for the player
            bool flipSprite = (target.x-transform.position.x > 0 && sprRend.flipX) || (target.x-transform.position.x < 0 && !sprRend.flipX);
            if (flipSprite)
            {
                sprRend.flipX = !sprRend.flipX;
            }

            // using Lerp to make the current step of the player
            t += Time.deltaTime / timeToReachTarget;
            transform.position = Vector3.Lerp(startPosition, target, t);
            // if player has reached target
            if (transform.position == target)
            {
                // if player is in a path, continue to the next step
                if (m_currentSteps.Count > 0)
                    SetStepDestination(m_currentSteps.Dequeue());
                // players finished his steps

                // update game progress
                else
                {
                    m_isLooting = true;
                    game.UpdateGameProgress();
                    m_isMoving = false;
                    anm.SetBool("IsMoving", false);
                }
            }

            return;
        }
        // if player is not moving inside a path, pick one and update next map
        else
        {
            Step PlayerChoise;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                PlayerChoise = Step.LEFT;
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                PlayerChoise = Step.RIGHT;
            }
            else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                PlayerChoise = Step.STRAIGHT;
            }
            // no action is taken - do not choose path
            else
                return;

            m_isMoving = true;
            anm.SetBool("IsMoving", true);


            MapTile.Path currentPath;

            // create steps queue
            // in case the player position will indicate the path (if player chooses stright)
            if (PlayerChoise == Step.STRAIGHT)
            {
                if (m_playerPos == Game.Position.LEFT)
                    currentPath = game.GetPathFromStep(Step.LEFT);
                else if (m_playerPos == Game.Position.RIGHT)
                    currentPath = game.GetPathFromStep(Step.RIGHT);
                else
                    currentPath = game.GetPathFromStep(Step.STRAIGHT);
            }
            else
                currentPath = game.GetPathFromStep(PlayerChoise);
            m_currentSteps = GetSteps(currentPath, PlayerChoise);

            Step currentStep = m_currentSteps.Dequeue();
            SetStepDestination(currentStep);
        }
        
    }

    public void killPlayer()
    {
        m_playerAnim.SetTrigger("GameOver");
        m_canvasAnim.SetTrigger("GameOver");
        m_leaderBoardAnim.SetTrigger("GameOver");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Chest")
        {
            OnPlayerPickChest(other.gameObject);
        }
            
    }


    private void TaskOnClickLeft()
    {
        WhenStepChosen(Step.LEFT);
    }

    private void TaskOnClickStright()
    {
        WhenStepChosen(Step.STRAIGHT);
    }

    private void TaskOnClickRight()
    {
        WhenStepChosen(Step.RIGHT);
    }

    public void WhenStepChosen(Step PlayerChoise)
    {
        if (m_isMoving || m_isLooting)
        {
            return;
        }
        anm.SetBool("IsMoving", true);
        m_isMoving = true;

        MapTile.Path currentPath;

        // create steps queue
        // in case the player position will indicate the path (if player chooses stright)
        if (PlayerChoise == Step.STRAIGHT)
        {
            if (m_playerPos == Game.Position.LEFT)
                currentPath = game.GetPathFromStep(Step.LEFT);
            else if (m_playerPos == Game.Position.RIGHT)
                currentPath = game.GetPathFromStep(Step.RIGHT);
            else
                currentPath = game.GetPathFromStep(Step.STRAIGHT);
        }
        else
            currentPath = game.GetPathFromStep(PlayerChoise);
        m_currentSteps = GetSteps(currentPath, PlayerChoise);

        Step currentStep = m_currentSteps.Dequeue();
        SetStepDestination(currentStep);
        
    }

    private void InitalizePathToSteps()
    {
        m_pathToSteps = new Dictionary<MapTile.Path, List<Step>>();

        m_pathToSteps[MapTile.Path.LEFT_TURN] = new List<Step>() { Step.STRAIGHT, Step.LEFT, Step.STRAIGHT };
        m_pathToSteps[MapTile.Path.LEFT_TURN_LONG] = new List<Step>() { Step.STRAIGHT, Step.LEFT, Step.LEFT, Step.STRAIGHT };
        m_pathToSteps[MapTile.Path.RIGHT_TURN] = new List<Step>() { Step.STRAIGHT, Step.RIGHT, Step.STRAIGHT };
        m_pathToSteps[MapTile.Path.RIGHT_TURN_LONG] = new List<Step>() { Step.STRAIGHT, Step.RIGHT, Step.RIGHT, Step.STRAIGHT };
        m_pathToSteps[MapTile.Path.STRAIGHT] = new List<Step>() { Step.STRAIGHT, Step.STRAIGHT };
    }

    private Queue<Step> GetSteps(MapTile.Path path, Step step)
    {
        Queue<Step> retSteps = new Queue<Step>();

        if (step == Step.LEFT)
        {
            if (m_playerPos == Game.Position.RIGHT)
            {
                retSteps.Enqueue(Step.LEFT);
                retSteps.Enqueue(Step.LEFT);
            }
            if (m_playerPos == Game.Position.MID)
                retSteps.Enqueue(Step.LEFT);
        }
        else if (step == Step.RIGHT)
        {
            if (m_playerPos == Game.Position.LEFT)
            {
                retSteps.Enqueue(Step.RIGHT);
                retSteps.Enqueue(Step.RIGHT);
            }
            if (m_playerPos == Game.Position.MID)
                retSteps.Enqueue(Step.RIGHT);
        }
        foreach (Step curSteps in m_pathToSteps[path])
            retSteps.Enqueue(curSteps);
        
        return retSteps;
    }

    private void SetStepDestination(Step step)
    {

        Vector3 position = transform.position;
        
        switch (step)
        {
            case Step.LEFT:
                position.x -= MapTile.mapWidth / 2;
                break;
            case Step.RIGHT:
                position.x += MapTile.mapWidth / 2;
                break;
            case Step.STRAIGHT:
                position.y += MapTile.mapHeight / 2;
                break;
        }
        SetDestination(position, 0.2f);
        UpdatePlayerPos(step);
    }

    // this method uses Linear interpolation (LERP) - to move player
    public void SetDestination(Vector3 destination, float time)
    {

        t = 0;
        startPosition = transform.position;
        timeToReachTarget = time;
        target = destination;
    }

    private void UpdatePlayerPos(Step step)
    {

        switch (step)
        {
            case Step.LEFT:
                if (m_playerPos == Game.Position.MID)
                    m_playerPos = Game.Position.LEFT;
                else
                    m_playerPos = Game.Position.MID;
                break;
            case Step.RIGHT:
                if (m_playerPos == Game.Position.MID)
                    m_playerPos = Game.Position.RIGHT;
                else
                    m_playerPos = Game.Position.MID;
                break;
        }
    }

    private void PickChest()
    {
        //Debug.Log("yey player picked chest");
    }

    private Dictionary<MapTile.Path, List<Step>> m_pathToSteps;
    private Game.Position m_playerPos;
    private bool m_isMoving;
    private bool m_isLooting;
    private Queue<Step> m_currentSteps;
    private Animator m_playerAnim;
    private Animator m_canvasAnim;
    private Animator m_leaderBoardAnim;
    private float m_restartTimer;
    private float m_restartDelay = 5f;

    // fields for using Linear interpolation (LERP) - to move player
    float t;
    Vector3 startPosition;
    Vector3 target;
    float timeToReachTarget;
}
