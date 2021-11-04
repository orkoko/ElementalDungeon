using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ElementalBase : MonoBehaviour {
   
    public enum Elements
    {
        FIRE,
        WATER,
        EARTH,
        LIGHTNING,
        ARCANE,
        NATURE,
        FOG,
        METAL,
        LIGHT,
        DARKNESS,
        CHAOS,
        VOID,
        NONE
    }

    #region Private Members
    protected Button myButton;
    #endregion

    #region Public Members
    public SpriteHelper spriteHelper;
    public Elements Type
    {
        get
        {
            return m_elementalType;
        }
        set
        {
            m_elementalType = value;
            DrawElemental();
        }
    }
    #endregion




    void Awake()
    {
        m_renderer = GetComponent<Image>();
    }

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
		
	}

    private void DrawElemental()
    {
        switch (m_elementalType)
        {
            case Elements.EARTH:
                m_renderer.overrideSprite = spriteHelper.earthSprite;
                break;
            case Elements.FIRE:
                m_renderer.overrideSprite = spriteHelper.fireSprite;
                break;
            case Elements.WATER:
                m_renderer.overrideSprite = spriteHelper.waterSprite;
                break;
            case Elements.LIGHTNING:
                m_renderer.overrideSprite = spriteHelper.lightningSprite;
                break;
            case Elements.ARCANE:
                m_renderer.overrideSprite = spriteHelper.arcaneSprite;
                break;
            case Elements.NATURE:
                m_renderer.overrideSprite = spriteHelper.natureSprite;
                break;
            case Elements.FOG:
                m_renderer.overrideSprite = spriteHelper.fogSprite;
                break;
            case Elements.METAL:
                m_renderer.overrideSprite = spriteHelper.metalSprite;
                break;
            case Elements.LIGHT:
                m_renderer.overrideSprite = spriteHelper.lightSprite;
                break;
            case Elements.DARKNESS:
                m_renderer.overrideSprite = spriteHelper.darknessSprite;
                break;
            case Elements.CHAOS:
                m_renderer.overrideSprite = spriteHelper.chaosSprite;
                break;
            case Elements.VOID:
                m_renderer.overrideSprite = spriteHelper.voidSprite;
                break;
            default: // incase null or bad input, put empty
                m_renderer.overrideSprite = spriteHelper.basicSprite;
                break;
        }
                
    }
    protected Image m_renderer;
    protected Elements m_elementalType;
}
