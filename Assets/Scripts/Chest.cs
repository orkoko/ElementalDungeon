using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {

    enum LootLocation
    {
        Top,
        Bottom
    }

    public static int lootCounter = 0;
    public SpriteHelper spriteHelper;
    public SpriteRenderer m_rend;

    // Use this for initialization
    void Awake () {
        m_rend = GetComponent<SpriteRenderer>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player")
        {
            return;
        }
        
        m_isLooted = true;

    }

    public void ResetChest(float mapY, Game.Position pos)
    {
        m_rend.sprite = spriteHelper.chest;
        switch (pos)
        {
            case Game.Position.LEFT:
                this.transform.position = new Vector3(-MapTile.mapWidth/2, mapY, transform.position.z);
                break;
            case Game.Position.MID:
                this.transform.position = new Vector3(0, mapY, transform.position.z);
                break;
            case Game.Position.RIGHT:
                this.transform.position = new Vector3(MapTile.mapWidth/2, mapY, transform.position.z);
                break;
        }
    }

    bool m_isLooted = false;

    internal void DiscoverTrap()
    {
        m_rend.sprite = spriteHelper.trapChest;
    }

    internal void DiscoverElements()
    {
        m_rend.sprite = spriteHelper.elementChest;
    }
}
