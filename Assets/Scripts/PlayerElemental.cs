using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerElemental : ElementalBase {

    #region Public Members
    public bool isClicked;
    public Game game;
    public int chaosCounter;

    public Animator selectionAnm;
    #endregion

    // Use this for initialization
    void Start()
    {
        Type = ElementalBase.Elements.NONE;
        isClicked = false;
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(() => { OnClick(); });
        chaosCounter = 1;
        selectionAnm = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update () {
        if (isClicked)
        {
            isClicked = false;
            selectThisElement();           
            
            game.selectElement(this);
        }
	}

    public void selectThisElement()
    {
        selectionAnm.SetBool("Selected", true);
    }

    public void unselectThisElement()
    {
        selectionAnm.SetBool("Selected", false);
    }

    public bool TryToActivateElement()
    {
        onRemovePassiveEffect();
        Elements elementType = Type;
        Type = Elements.NONE;

        switch (elementType)
        {
            case Elements.EARTH:
                game.updateHp(-3);
                break;
            case Elements.FIRE:
                game.maxHp++;
                game.setHpText();
                break;
            case Elements.WATER:
                return game.activateIceArmor();
            case Elements.LIGHTNING:
                game.liftFog();
                break;
            case Elements.ARCANE:
                return game.activateTransformation();
            case Elements.NATURE:
                return game.activateGrowth();
            case Elements.METAL:
                return game.activateCyborg();
            case Elements.FOG:
                return game.activateSteam();
            case Elements.LIGHT:
                game.updateHp(-game.maxHp);
                break;
            case Elements.DARKNESS:
                return game.activateImmortality();
            case Elements.CHAOS:
                game.TryToGetRandomElement();
                game.TryToGetRandomElement();
                break;
            case Elements.VOID:
                game.activateVoid();
                break;
            default:
                break;
        }
        return true;

    }


    public void onEndTurnEffect()
    {
        switch (Type)
        {
            case Elements.EARTH:
                break;
            case Elements.FIRE:
                game.updateHp(-1);
                break;
            case Elements.WATER:
                break;
            case Elements.LIGHTNING:
                break;
            case Elements.ARCANE:
                break;
            case Elements.NATURE:
                break;
            case Elements.METAL:
                break;
            case Elements.FOG:
                break;
            case Elements.LIGHT:
                game.updateHp(-2);
                break;
            case Elements.DARKNESS:
                break;
            case Elements.CHAOS:
                game.updateHp(chaosCounter);
                chaosCounter++;
                break;
            case Elements.VOID:
                break;
            default:
                break;
        }
    }

    public void onRemovePassiveEffect()
    {
        unselectThisElement();
        switch (Type)
        {
            case Elements.EARTH:
                game.updateMaxHp(-2);
                game.setHpText();
                break;
            case Elements.FIRE:
                break;
            case Elements.WATER:
                game.trapDamageReduction--;
                game.setHpText();
                break;
            case Elements.LIGHTNING:
                game.trapEvadeChance -= 10;
                break;
            case Elements.ARCANE:
                break;
            case Elements.NATURE:
                game.updateMaxHp(-5);
                game.setHpText();
                break;
            case Elements.METAL:
                game.recycleChance -= 20;
                break;
            case Elements.FOG:
                game.mistChance -= 15;
                break;
            case Elements.LIGHT:
                break;
            case Elements.DARKNESS:
                game.trapDamageReduction-=2;
                break;
            case Elements.CHAOS:
                chaosCounter = 1;
                break;
            case Elements.VOID:
                break;
            default:
                break;
        }
    }

    public void onPickPassiveEffect()
    {
        switch (Type)
        {
            case Elements.EARTH:
                game.updateMaxHp(2);
                game.setHpText();
                break;
            case Elements.FIRE:
                break;
            case Elements.WATER:
                game.trapDamageReduction++;
                game.setHpText();
                break;
            case Elements.LIGHTNING:
                game.trapEvadeChance += 10;
                break;
            case Elements.ARCANE:
                break;
            case Elements.NATURE:
                game.updateMaxHp(5);
                game.setHpText();
                break;
            case Elements.METAL:
                game.recycleChance += 20;
                break;
            case Elements.FOG:
                game.mistChance += 15;
                break;
            case Elements.LIGHT:
                break;
            case Elements.DARKNESS:
                game.trapDamageReduction+=2;
                break;
            case Elements.CHAOS:
                break;
            case Elements.VOID:
                TryToActivateElement();
                break;
            default:
                break;
        }
    }

    public void onLootingTwoChestsEffect()
    {
        switch (Type)
        {
            case Elements.EARTH:
                break;
            case Elements.FIRE:
                break;
            case Elements.WATER:
                break;
            case Elements.LIGHTNING:
                break;
            case Elements.ARCANE:
                game.updateHp(-4);
                break;
            case Elements.NATURE:
                break;
            case Elements.METAL:
                break;
            case Elements.FOG:
                break;
            case Elements.LIGHT:
                break;
            case Elements.DARKNESS:
                break;
            case Elements.CHAOS:
                break;
            case Elements.VOID:
                break;
            default:
                break;
        }
    }

    public void OnClick()
    {
        if (Type != Elements.NONE)
        {
            isClicked = true;
        }
    }

}
