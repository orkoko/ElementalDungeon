using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * TODO list
 * tips for active/passive/combining
 * add sound effects
 * add music
 * Implement online score board
 * create nerrative + in game intro to game based on nerrative
 * game over screen - contains show of top 5 score board
 * draggable side help menu (currently is activated by click)
 * add player's animation - when looting or encountering trap a trap
 * try machanic change - make player's path decision more meaningful
 * Score Board
 *  
 * // Lower priority
 * add visual effects 
 * make move speed slower (maybe use lerp) in order to help user follow his character
 * refractor player elements to have child classes for each element
 * refractor code - use events instead of calling functions - will help adding more game mechanics later on
 * add map's art (background and tiles)
 * dragable elements (for combine/pick)
 */
public class Game : MonoBehaviour {

    #region Public Members
    public int numberOfChestToLootThisLevel;
    public int numberOfPlayerElement;

    public SpriteHelper spriteHelper;

    public Text levelText;
    public Text bestLevelText;
    public Text hpText;
    public Text trapDamageText;
    public Text chaosDamageText;
    public Text regenText;

    public int level; // TODO MAKE PRIVATE
    public int bestLevel; // TODO MAKE PRIVATE
    public int hp; // TODO MAKE PRIVATE
    public int maxHp; // TODO MAKE PRIVATE
    public int trapChance; // TODO MAKE PRIVATE
    public int trapEvadeChance; // TODO MAKE PRIVATE
    public int recycleChance; // TODO MAKE PRIVATE
    public int mistChance; // TODO MAKE PRIVATE

    internal void liftFog()
    {
        if (!isFogLifted)
        {
            isFogLifted = true;
            MapFog.transform.Translate(0, MapTile.mapHeight/2, 0);
            if (random.Next(100) < trapChance)
            {
                currentMap.chest1.DiscoverTrap();
            }
            else
            {
                currentMap.chest1.DiscoverElements();
            }
            if (random.Next(100) < trapChance)
            {
                currentMap.chest2.DiscoverTrap();
            }
            else
            {
                currentMap.chest2.DiscoverElements();
            }
        }
    }
    internal void lowerFog()
    {
        if (isFogLifted)
        {
            isFogLifted = false;
            MapFog.transform.Translate(0, -MapTile.mapHeight/2, 0);
        }
    }

    public int trapDamage; // TODO MAKE PRIVATE
    public int trapDamageReduction; // TODO MAKE PRIVATE
    public bool isFogLifted = false;

    internal void activateVoid()
    {
        updateMaxHp(15);
        foreach (PlayerElemental element in elements)
        {
            element.TryToActivateElement();
            updateHp(-maxHp);
        }
    }

    // buffs
    public bool isIceArmorActive;
    public GameObject iceArmor;
    public bool isTransformationActive;
    public GameObject transformation;
    public bool isGrowthActive;
    public GameObject growth;
    public bool isCyborgActive;
    public GameObject cyborg;
    public bool isSteamActive;
    public GameObject steam;
    public bool isImmortalityActive;
    public GameObject immortality;

    public Player player;

    public MapTile previousMap;
    public MapTile currentMap;
    public MapTile nextMap;

    public GameObject MapFog;
    
    public PlayerElemental playerElemental1;
    public PlayerElemental playerElemental2;
    public PlayerElemental playerElemental3;
    public PlayerElemental playerElemental4;
    public PlayerElemental playerElemental5;
    public List<PlayerElemental> elements;

    public PlayerElemental selectedElement1;
    public PlayerElemental selectedElement2;

    public PickableElemental pickedElemental1;
    public PickableElemental pickedElemental2;

    public PickableElemental pickedElemental3;
    public PickableElemental pickedElemental4;



    #endregion

    #region Public Enums

    /// <summary>
    /// objects' available positions
    /// </summary>
    public enum Position
    {
        LEFT,
        MID,
        RIGHT
    }

    #endregion

    #region Unity Flow
    // Use this for initialization
    void Start()
    {
        // intialize
        InitializeMapStepToPath();
        InitializeElementsCombinations();
        level = 1;
        bestLevel = PlayerPrefs.GetInt("bestLevel", bestLevel);
        bestLevelText.text = "Best: " + bestLevel.ToString();
        hp = maxHp = 10;
        m_lootCounter = 0;
        numberOfChestToLootThisLevel = 0;
        numberOfPlayerElement = 0;
        trapChance = 50;
        trapEvadeChance = 0;
        trapDamage = 1;
        trapDamageReduction = 0;
        recycleChance = 0;
        mistChance = 0;
        disableIceArmor();
        disableTransformation();
        disableGrowth();
        disableCyborg();
        disableSteam();
        disableImmortality();
        elements = new List<PlayerElemental>();
        elements.Add(playerElemental1);
        elements.Add(playerElemental2);
        elements.Add(playerElemental3);
        elements.Add(playerElemental4);
        elements.Add(playerElemental5);

        currentMap.Type = MapTile.MapType.STARTING_MAP;
        Array values = Enum.GetValues(typeof(MapTile.MapType));
        MapTile.MapType randomMap;

        randomMap = (MapTile.MapType)values.GetValue(random.Next(values.Length - 1));
        previousMap.Type = randomMap;

        randomMap = (MapTile.MapType)values.GetValue(random.Next(values.Length - 1));
        nextMap.Type = randomMap;

        pickedElemental1.Hide();
        pickedElemental2.Hide();
        pickedElemental3.Hide();
        pickedElemental4.Hide();

        // update UI
        SetLevelText();
        setHpText();


        // assign events
        player.OnPlayerPickChest += generateLoot;

    }

    // Update is called once per frame
    void Update ()
    {
        SetTrapDamageText();
        SetChaosDamageText();
        SetRegenText();
        setHpText();
    }

    #endregion

    #region Public Methods

    public MapTile.Path StepRules(MapTile.MapType map, Player.Step step)
    {
        return m_stepVsPath[map][step];
    }

    public MapTile.Path GetPathFromStep(Player.Step step)
    {
        return m_stepVsPath[currentMap.Type][step];
    }

    public bool TryToPickElements()
    {
        if (pickedElemental1.isClicked)
        {
            pickedElemental1.isClicked = false;
            if (TryToChooseElement(pickedElemental1))
            {
                numberOfChestToLootThisLevel--;
                pickedElemental1.Hide();
                pickedElemental2.Hide();
            }
        }
        else if (pickedElemental2.isClicked)
        {
            pickedElemental2.isClicked = false;
            if (TryToChooseElement(pickedElemental2))
            {
                numberOfChestToLootThisLevel--;
                pickedElemental1.Hide();
                pickedElemental2.Hide();
            }
        }

        if (pickedElemental3.isClicked)
        {
            pickedElemental3.isClicked = false;
            if (TryToChooseElement(pickedElemental3))
            {
                numberOfChestToLootThisLevel--;
                pickedElemental3.Hide();
                pickedElemental4.Hide();
            }
        }
        else if (pickedElemental4.isClicked)
        {
            pickedElemental4.isClicked = false;
            if (TryToChooseElement(pickedElemental4))
            {
                // TODO - make the picked element 4 go to slot
                numberOfChestToLootThisLevel--;
                pickedElemental3.Hide();
                pickedElemental4.Hide();
            }
        }
        // we cant return true until player picked all elements
        if (numberOfChestToLootThisLevel == 0)
            return true;

        return false;
    }

    public void TryToGetRandomElement()
    {
        // if player got less then 5 elements, pick element
        PlayerElemental nextAvailable = GetNextAvailableElementSlot();
        if (nextAvailable == null)
            return;
        Array values = new ElementalBase.Elements[12] {
            ElementalBase.Elements.FIRE,
            ElementalBase.Elements.LIGHTNING,
            ElementalBase.Elements.WATER,
            ElementalBase.Elements.EARTH,
            ElementalBase.Elements.FOG,
            ElementalBase.Elements.NATURE,
            ElementalBase.Elements.ARCANE,
            ElementalBase.Elements.METAL,
            ElementalBase.Elements.LIGHT,
            ElementalBase.Elements.DARKNESS,
            ElementalBase.Elements.CHAOS,
            ElementalBase.Elements.VOID};
        nextAvailable.Type = (ElementalBase.Elements)values.GetValue(random.Next(values.Length));
        nextAvailable.onPickPassiveEffect();
    }

    private bool TryToChooseElement(PickableElemental pickedElemental)
    {
        // if player got less then 5 elements, pick element
        PlayerElemental nextAvailable = GetNextAvailableElementSlot();
        if (nextAvailable == null)
            return false;
        nextAvailable.Type = pickedElemental.Type;
        nextAvailable.onPickPassiveEffect();
        return true;
    }

    private PlayerElemental GetNextAvailableElementSlot()
    {
        if (playerElemental1.Type == ElementalBase.Elements.NONE)
            return playerElemental1;
        if (playerElemental2.Type == ElementalBase.Elements.NONE)
            return playerElemental2;
        if (playerElemental3.Type == ElementalBase.Elements.NONE)
            return playerElemental3;
        if (playerElemental4.Type == ElementalBase.Elements.NONE)
            return playerElemental4;
        if (playerElemental5.Type == ElementalBase.Elements.NONE)
            return playerElemental5;
        return null;
    }

    public void UpdateGameProgress()
    {

        level++;
        trapDamage = (level / 10) + 1;
        SetLevelText();
        setHpText();
        SetTrapDamageText();
        SetRegenText();
        updateBestLevel();
        if (hp <= 0)
        {
            player.killPlayer();
        }
        else
        {
            UpdateNextMap();
            EndOfLevelEffects();
            // activate passive of elements
            foreach (PlayerElemental element in elements)
            {
                element.onEndTurnEffect();
            }
            SetChaosDamageText();
            // check again after end of turn events
            if (hp <= 0)
            {
                player.killPlayer();
            }
        }
    }

    private void EndOfLevelEffects()
    {
        // check if player got random buff
        if (random.Next(100) < mistChance)
        {
            Array values = new ElementalBase.Elements[3] {
            ElementalBase.Elements.ARCANE,
            ElementalBase.Elements.NATURE,
            ElementalBase.Elements.FOG};

            ElementalBase.Elements randomElement;

            do
            {
                randomElement = (ElementalBase.Elements)values.GetValue(random.Next(values.Length));
            } while (!TryToGetRandomBuff(randomElement));
        }
    }

    public bool TryToGetRandomBuff(ElementalBase.Elements elementType)
    {
        // when all buffs are already active, do nothing.
        if (isTransformationActive && isGrowthActive && isSteamActive)
            return true;

        switch (elementType)
        {
            case ElementalBase.Elements.ARCANE:
                return activateTransformation();
            case ElementalBase.Elements.NATURE:
                return activateGrowth();
            case ElementalBase.Elements.FOG:
                return activateSteam();
            default:
                break;
        }
        return true;

    }

    public void SetLevelText()
    {
        levelText.text = "Level: " + level.ToString();
    }

    public void updateBestLevel()
    {
        if (level > bestLevel)
        {
            bestLevelText.color = Color.green;
            bestLevel = level;
            bestLevelText.text = "Best: " + level.ToString();
            PlayerPrefs.SetInt("bestLevel", bestLevel);
        }
    }

    public void SetTrapDamageText()
    {
        trapDamageText.text = "Trap Damage: " + calculateTrapDamage().ToString();
    }

    public void SetChaosDamageText()
    {
        // go over elements and sum chaos damage
        int sum = 0;
        foreach (PlayerElemental element in elements)
        {
            if (element.Type == ElementalBase.Elements.CHAOS)
                sum += element.chaosCounter;
        }
        chaosDamageText.text = "Chaos Damage: " + sum.ToString();
    }

    public void SetRegenText()
    {
        // goto over elements and sum regen
        int sum = 0;
        foreach (PlayerElemental element in elements)
        {
            if (element.Type == ElementalBase.Elements.FIRE)
                sum += 1;
            else if (element.Type == ElementalBase.Elements.LIGHT)
                sum += 2;
        }
        regenText.text = "Regen: " + sum.ToString();
    }



    public void updateHp(int damage)
    {
        if (damage > 0)
        {
            // Avoid damage if trap is avoided
            if (random.Next(100) < trapEvadeChance)
            {
                damage = 0;
            }
            // Avoid damage if ice armor is up
            if (isIceArmorActive)
            {
                disableIceArmor();
                damage = 0;
            }
            // Heal damage if cyborg
            else if (isCyborgActive)
            {
                disableCyborg();
                damage = -damage;
            }
            // Avoid damage if lethal
            else if (isImmortalityActive && (hp < damage))
            {
                disableImmortality();
                hp = 1;
                damage = 0;
            }
            // this should be triggered by event (damage taken
            // take damage and loose buffs
            else
            {
                disableTransformation();
                disableGrowth();
                disableSteam();
            }
        }
        hp -= damage;
        // if more then max, heals to max
        if (hp > maxHp)
        {
            hp = maxHp;
        }
        
        setHpText();
    }

    public void updateMaxHp(int maxHpAmount)
    {
        maxHp += maxHpAmount;
        // if hp more then max, lower to max
        if (hp > maxHp)
        {
            hp = maxHp;
        }

        setHpText();

    }

    public void setHpText()
    {
        hpText.text = "HP: " + hp.ToString() + " \\ " + maxHp.ToString();
        if (hp == maxHp)
            hpText.color = Color.green;
        else if (hp <= calculateTrapDamage() * 2)
            hpText.color = Color.red;
        else
            hpText.color = Color.white;

    }

    public void DeselectAllElements()
    {
        // deselect all elements
        foreach (PlayerElemental element in elements)
        {
            element.unselectThisElement();
            selectedElement1 = null;
            selectedElement2 = null;
        }
    }
    #endregion

    #region Private Methods

    private void UpdateNextMap()
    {
        var tempCurMap = currentMap;
        var tempPreviouseMap = previousMap;
        currentMap = nextMap;
        previousMap = tempCurMap;
        nextMap = tempPreviouseMap;

        nextMap.transform.position = currentMap.transform.position + new Vector3(0, 2.275f);

        Array values = Enum.GetValues(typeof(MapTile.MapType));
        MapTile.MapType randomMap;
        randomMap = (MapTile.MapType)values.GetValue(random.Next(values.Length - 1));
        nextMap.Type = randomMap;

    }

    private void InitializeMapStepToPath()
    {
        m_stepVsPath = new Dictionary<MapTile.MapType, Dictionary<Player.Step, MapTile.Path>>();

        m_stepVsPath[MapTile.MapType.STARTING_MAP] = new Dictionary<Player.Step, MapTile.Path>();
        m_stepVsPath[MapTile.MapType.STARTING_MAP][Player.Step.LEFT] = MapTile.Path.STRAIGHT;
        m_stepVsPath[MapTile.MapType.STARTING_MAP][Player.Step.STRAIGHT] = MapTile.Path.STRAIGHT;
        m_stepVsPath[MapTile.MapType.STARTING_MAP][Player.Step.RIGHT] = MapTile.Path.STRAIGHT;

        m_stepVsPath[MapTile.MapType.MAP_1_2_3] = new Dictionary<Player.Step, MapTile.Path>();
        m_stepVsPath[MapTile.MapType.MAP_1_2_3][Player.Step.LEFT] = MapTile.Path.STRAIGHT;
        m_stepVsPath[MapTile.MapType.MAP_1_2_3][Player.Step.STRAIGHT] = MapTile.Path.STRAIGHT;
        m_stepVsPath[MapTile.MapType.MAP_1_2_3][Player.Step.RIGHT] = MapTile.Path.LEFT_TURN;

        m_stepVsPath[MapTile.MapType.MAP_1_3_2] = new Dictionary<Player.Step, MapTile.Path>();
        m_stepVsPath[MapTile.MapType.MAP_1_3_2][Player.Step.LEFT] = MapTile.Path.STRAIGHT;
        m_stepVsPath[MapTile.MapType.MAP_1_3_2][Player.Step.STRAIGHT] = MapTile.Path.RIGHT_TURN;
        m_stepVsPath[MapTile.MapType.MAP_1_3_2][Player.Step.RIGHT] = MapTile.Path.STRAIGHT;

        m_stepVsPath[MapTile.MapType.MAP_2_1_3] = new Dictionary<Player.Step, MapTile.Path>();
        m_stepVsPath[MapTile.MapType.MAP_2_1_3][Player.Step.LEFT] = MapTile.Path.STRAIGHT;
        m_stepVsPath[MapTile.MapType.MAP_2_1_3][Player.Step.STRAIGHT] = MapTile.Path.STRAIGHT;
        m_stepVsPath[MapTile.MapType.MAP_2_1_3][Player.Step.RIGHT] = MapTile.Path.LEFT_TURN_LONG;

        m_stepVsPath[MapTile.MapType.MAP_2_3_1] = new Dictionary<Player.Step, MapTile.Path>();
        m_stepVsPath[MapTile.MapType.MAP_2_3_1][Player.Step.LEFT] = MapTile.Path.STRAIGHT;
        m_stepVsPath[MapTile.MapType.MAP_2_3_1][Player.Step.STRAIGHT] = MapTile.Path.LEFT_TURN;
        m_stepVsPath[MapTile.MapType.MAP_2_3_1][Player.Step.RIGHT] = MapTile.Path.STRAIGHT;

        m_stepVsPath[MapTile.MapType.MAP_3_1_2] = new Dictionary<Player.Step, MapTile.Path>();
        m_stepVsPath[MapTile.MapType.MAP_3_1_2][Player.Step.LEFT] = MapTile.Path.RIGHT_TURN_LONG;
        m_stepVsPath[MapTile.MapType.MAP_3_1_2][Player.Step.STRAIGHT] = MapTile.Path.STRAIGHT;
        m_stepVsPath[MapTile.MapType.MAP_3_1_2][Player.Step.RIGHT] = MapTile.Path.STRAIGHT;

        m_stepVsPath[MapTile.MapType.MAP_3_2_1] = new Dictionary<Player.Step, MapTile.Path>();
        m_stepVsPath[MapTile.MapType.MAP_3_2_1][Player.Step.LEFT] = MapTile.Path.RIGHT_TURN;
        m_stepVsPath[MapTile.MapType.MAP_3_2_1][Player.Step.STRAIGHT] = MapTile.Path.STRAIGHT;
        m_stepVsPath[MapTile.MapType.MAP_3_2_1][Player.Step.RIGHT] = MapTile.Path.STRAIGHT;

    }

    private void InitializeElementsCombinations()
    {
        // todo set instead of tuple
        m_elementsCombination = new Dictionary<Tuple<ElementalBase.Elements, ElementalBase.Elements>, ElementalBase.Elements>();

        m_elementsCombination[new Tuple<ElementalBase.Elements, ElementalBase.Elements>(ElementalBase.Elements.FIRE, ElementalBase.Elements.WATER)] = ElementalBase.Elements.FOG;
        m_elementsCombination[new Tuple<ElementalBase.Elements, ElementalBase.Elements>(ElementalBase.Elements.FIRE, ElementalBase.Elements.LIGHTNING)] = ElementalBase.Elements.ARCANE;
        m_elementsCombination[new Tuple<ElementalBase.Elements, ElementalBase.Elements>(ElementalBase.Elements.EARTH, ElementalBase.Elements.WATER)] = ElementalBase.Elements.NATURE;
        m_elementsCombination[new Tuple<ElementalBase.Elements, ElementalBase.Elements>(ElementalBase.Elements.EARTH, ElementalBase.Elements.LIGHTNING)] = ElementalBase.Elements.METAL;
        m_elementsCombination[new Tuple<ElementalBase.Elements, ElementalBase.Elements>(ElementalBase.Elements.FOG, ElementalBase.Elements.METAL)] = ElementalBase.Elements.DARKNESS;
        m_elementsCombination[new Tuple<ElementalBase.Elements, ElementalBase.Elements>(ElementalBase.Elements.NATURE, ElementalBase.Elements.ARCANE)] = ElementalBase.Elements.LIGHT;
        m_elementsCombination[new Tuple<ElementalBase.Elements, ElementalBase.Elements>(ElementalBase.Elements.LIGHT, ElementalBase.Elements.DARKNESS)] = ElementalBase.Elements.CHAOS;
        m_elementsCombination[new Tuple<ElementalBase.Elements, ElementalBase.Elements>(ElementalBase.Elements.CHAOS, ElementalBase.Elements.CHAOS)] = ElementalBase.Elements.VOID;

        m_elementsCombination[new Tuple<ElementalBase.Elements, ElementalBase.Elements>(ElementalBase.Elements.WATER, ElementalBase.Elements.FIRE)] = ElementalBase.Elements.FOG;
        m_elementsCombination[new Tuple<ElementalBase.Elements, ElementalBase.Elements>(ElementalBase.Elements.LIGHTNING, ElementalBase.Elements.FIRE)] = ElementalBase.Elements.ARCANE;
        m_elementsCombination[new Tuple<ElementalBase.Elements, ElementalBase.Elements>(ElementalBase.Elements.WATER, ElementalBase.Elements.EARTH)] = ElementalBase.Elements.NATURE;
        m_elementsCombination[new Tuple<ElementalBase.Elements, ElementalBase.Elements>(ElementalBase.Elements.LIGHTNING, ElementalBase.Elements.EARTH)] = ElementalBase.Elements.METAL;
        m_elementsCombination[new Tuple<ElementalBase.Elements, ElementalBase.Elements>(ElementalBase.Elements.METAL, ElementalBase.Elements.FOG)] = ElementalBase.Elements.DARKNESS;
        m_elementsCombination[new Tuple<ElementalBase.Elements, ElementalBase.Elements>(ElementalBase.Elements.ARCANE, ElementalBase.Elements.NATURE)] = ElementalBase.Elements.LIGHT;
        m_elementsCombination[new Tuple<ElementalBase.Elements, ElementalBase.Elements>(ElementalBase.Elements.DARKNESS, ElementalBase.Elements.LIGHT)] = ElementalBase.Elements.CHAOS;
    }

    private void generateLoot(GameObject chest)
    {
        // check if player looted a trap
        if (chest.GetComponent<SpriteRenderer>().sprite == spriteHelper.trapChest || (chest.GetComponent<SpriteRenderer>().sprite != spriteHelper.elementChest && random.Next(100) < trapChance))
        {
            Debug.Log("Encountered a trap");
            chest.GetComponent<SpriteRenderer>().sprite = spriteHelper.trapChest;

            // update HP according to current rules
            updateHp(calculateTrapDamage());

            // if didnt recycle trap to elemental dont continue
            if (!(random.Next(100) < recycleChance))
            {
                return;
            }
                
        }
        else 
        {
            chest.GetComponent<SpriteRenderer>().sprite = spriteHelper.elementChest;
        }

        Debug.Log("Looting");

        // check buffs
        applyLootingBuffs();

        Array values;
        if (isTransformationActive)
        {
            values = new ElementalBase.Elements[12] {
            ElementalBase.Elements.FIRE,
            ElementalBase.Elements.LIGHTNING,
            ElementalBase.Elements.WATER,
            ElementalBase.Elements.EARTH,
            ElementalBase.Elements.FOG,
            ElementalBase.Elements.NATURE,
            ElementalBase.Elements.ARCANE,
            ElementalBase.Elements.METAL,
            ElementalBase.Elements.LIGHT,
            ElementalBase.Elements.DARKNESS,
            ElementalBase.Elements.CHAOS,
            ElementalBase.Elements.VOID};
        }
        else
        {
            values = new ElementalBase.Elements[4] {
            ElementalBase.Elements.FIRE,
            ElementalBase.Elements.LIGHTNING,
            ElementalBase.Elements.WATER,
            ElementalBase.Elements.EARTH};
        }
        
        ElementalBase.Elements randomElement1;
        ElementalBase.Elements randomElement2;
        if (numberOfChestToLootThisLevel == 0)
        {
            pickedElemental1.Show();
            pickedElemental1.isClicked = false;
            pickedElemental2.Show();
            pickedElemental2.isClicked = false;

            randomElement1 = (ElementalBase.Elements)values.GetValue(random.Next(values.Length));
            pickedElemental1.Type = randomElement1;
            
            do
            {
                randomElement2 = (ElementalBase.Elements)values.GetValue(random.Next(values.Length));
            } while (randomElement1 == randomElement2);
            
            pickedElemental2.Type = randomElement2;
            
        }
        else
        {
            pickedElemental3.Show();
            pickedElemental3.isClicked = false;
            pickedElemental4.Show();
            pickedElemental4.isClicked = false;

            randomElement1 = (ElementalBase.Elements)values.GetValue(random.Next(values.Length));
            pickedElemental3.Type = randomElement1;

            do
            {
                randomElement2 = (ElementalBase.Elements)values.GetValue(random.Next(values.Length));
            } while (randomElement1 == randomElement2);

            pickedElemental4.Type = randomElement2;

            // activate passive of elements
            foreach (PlayerElemental element in elements)
            {
                element.onLootingTwoChestsEffect();
            }
        }

        numberOfChestToLootThisLevel++;
        m_lootCounter++;

    }

    // move to player elemental class - onLootingActiveEffect
    private void applyLootingBuffs()
    {
        if (isGrowthActive)
        {
            updateMaxHp(2);
        }
        if (isSteamActive)
        {
            updateHp(-4);
        }
    }

    private int calculateTrapDamage()
    {
        return Math.Max(0, trapDamage-trapDamageReduction);
    }

    public void disableIceArmor()
    {
        isIceArmorActive = false;
        iceArmor.GetComponent<SpriteRenderer>().enabled = false;
    }

    public bool activateIceArmor()
    {
        if (isIceArmorActive)
            return false;
        isIceArmorActive = true;
        iceArmor.GetComponent<SpriteRenderer>().enabled = true;
        return true;
    }

    public void disableCyborg()
    {
        isCyborgActive = false;
        cyborg.GetComponent<SpriteRenderer>().enabled = false;
    }

    public bool activateCyborg()
    {
        if (isCyborgActive)
            return false;
        isCyborgActive = true;
        cyborg.GetComponent<SpriteRenderer>().enabled = true;
        return true;
    }

    public void disableSteam()
    {
        isSteamActive = false;
        steam.GetComponent<SpriteRenderer>().enabled = false;
    }

    public bool activateSteam()
    {
        if (isSteamActive)
            return false;
        isSteamActive = true;
        steam.GetComponent<SpriteRenderer>().enabled = true;
        return true;
    }

    public void disableGrowth()
    {
        isGrowthActive = false;
        growth.GetComponent<SpriteRenderer>().enabled = false;
    }

    public bool activateGrowth()
    {
        if (isGrowthActive)
            return false;
        isGrowthActive = true;
        growth.GetComponent<SpriteRenderer>().enabled = true;
        return true;
    }

    public void disableTransformation()
    {
        isTransformationActive = false;
        transformation.GetComponent<SpriteRenderer>().enabled = false;
    }

    public bool activateTransformation()
    {
        if (isTransformationActive)
            return false;
        isTransformationActive = true;
        transformation.GetComponent<SpriteRenderer>().enabled = true;
        return true;
    }

    public void disableImmortality()
    {
        isImmortalityActive = false;
        immortality.GetComponent<SpriteRenderer>().enabled = false;
    }

    public bool activateImmortality()
    {
        if (isImmortalityActive)
            return false;
        isImmortalityActive = true;
        immortality.GetComponent<SpriteRenderer>().enabled = true;
        return true;
    }


    internal void selectElement(PlayerElemental playerElemental)
    {
        if (hp <= 0)
        {
            return;
        }
        if (selectedElement1 == null)
        {
            selectedElement1 = playerElemental;
            return;
        }
        else if (selectedElement2 == null)
        {
            selectedElement2 = playerElemental;
        }
        Tuple<ElementalBase.Elements, ElementalBase.Elements> selectedCombination = new Tuple<ElementalBase.Elements, ElementalBase.Elements>(selectedElement1.Type, selectedElement2.Type);
        if (selectedElement1 == selectedElement2)
        {
            playerElemental.TryToActivateElement();
        }       
        else if (m_elementsCombination.ContainsKey(selectedCombination))
        {
            selectedElement1.onRemovePassiveEffect();
            selectedElement2.onRemovePassiveEffect();
            selectedElement1.Type = ElementalBase.Elements.NONE;
            selectedElement2.Type = ElementalBase.Elements.NONE;
            playerElemental.Type = m_elementsCombination[selectedCombination];
            playerElemental.onPickPassiveEffect();
        }
        selectedElement1.unselectThisElement();
        selectedElement2.unselectThisElement();
        selectedElement1 = null;
        selectedElement2 = null;
    }

    public void Restart()
    {
        // .. reload the currently loaded level.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    #region Private Members

    private Dictionary<MapTile.MapType, Dictionary<Player.Step, MapTile.Path>> m_stepVsPath;
    private Dictionary<Tuple<ElementalBase.Elements, ElementalBase.Elements>, ElementalBase.Elements> m_elementsCombination;
    private System.Random random = new System.Random();
    private int m_lootCounter;



    #endregion

}
