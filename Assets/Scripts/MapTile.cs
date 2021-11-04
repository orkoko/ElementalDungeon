using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {

    public static float mapWidth = 2.275f;
    public static float mapHeight = 2.275f;


    public Chest chest1;
    public Chest chest2;

    public enum MapType
    {
        MAP_1_2_3 = 0,
        MAP_1_3_2 = 1,
        MAP_2_1_3 = 2,
        MAP_2_3_1 = 3,
        MAP_3_1_2 = 4,
        MAP_3_2_1 = 5,
        STARTING_MAP = 6
    }

    public enum Path
    {
        LEFT_TURN,
        LEFT_TURN_LONG,
        STRAIGHT,
        RIGHT_TURN,
        RIGHT_TURN_LONG
    }

    public MapType Type
    {
        get{
            return m_mapType;
        }
        set
        {
            m_mapType = value;
            DrawMap();
            GenerateChests();
        }
    }

    private float FIX_TO_MIDDLE = -0.17f;

    private void GenerateChests()
    {
        float middleOfMap = gameObject.transform.position.y + FIX_TO_MIDDLE;
        switch (Type)
        {
            case MapType.MAP_1_2_3:
                chest1.ResetChest(middleOfMap, Game.Position.MID);
                chest2.ResetChest(middleOfMap, Game.Position.RIGHT);
                break;
            case MapType.MAP_1_3_2:
                chest1.ResetChest(middleOfMap, Game.Position.MID);
                chest2.ResetChest(middleOfMap, Game.Position.RIGHT);
                break;
            case MapType.MAP_2_1_3:
                chest1.ResetChest(middleOfMap, Game.Position.LEFT);
                chest2.ResetChest(middleOfMap, Game.Position.RIGHT);
                break;
            case MapType.MAP_2_3_1:
                chest1.ResetChest(middleOfMap, Game.Position.LEFT);
                chest2.ResetChest(middleOfMap, Game.Position.MID);
                break;
            case MapType.MAP_3_1_2:
                chest1.ResetChest(middleOfMap, Game.Position.MID);
                chest2.ResetChest(middleOfMap, Game.Position.RIGHT);
                break;
            case MapType.MAP_3_2_1:
                chest1.ResetChest(middleOfMap, Game.Position.LEFT);
                chest2.ResetChest(middleOfMap, Game.Position.MID);
                break;
        }

    }

    public Sprite StartingMap;
    public Sprite Map1;
    public Sprite Map2;
    public Sprite Map3;
    public Sprite Map4;
    public Sprite Map5;
    public Sprite Map6;

    void Awake()
    {
        m_renderer = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void DrawMap()
    {

        Sprite sprite;

        switch (m_mapType)
        {
            case (MapType.MAP_1_2_3):
                sprite = Map1;
                break;
            case (MapType.MAP_1_3_2):
                sprite = Map5;
                break;
            case (MapType.MAP_2_1_3):
                sprite = Map3;
                break;
            case (MapType.MAP_2_3_1):
                sprite = Map6;
                break;
            case (MapType.MAP_3_1_2):
                sprite = Map4;
                break;
            case (MapType.MAP_3_2_1):
                sprite = Map2;
                break;
            default:
                sprite = StartingMap;
                break;

        }

        m_renderer.sprite = sprite;
    }

    private bool m_isPlayerMoving;
    private SpriteRenderer m_renderer;
    private MapType m_mapType;

}
