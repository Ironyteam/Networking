using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds data for an individual player.
[System.Serializable]
public class Player
{
    // All of the private properies and their public accessors.
    #region Properties
    private string name;        // The player's name as entered.
    public  string ipAddress;   // ipAddress of the player, used to identify connections for networking SILAS

    private int armies;         // The number of armies deployed by this player over the entire map.
    private int character;      // The character player has chosen to play as, 0 for no character.
    private int cities;         // The number of settlements upgraded to cities by this player over the entire map.
    private int gold;           // The amount of gold a player currently owns.
    private int roads;          // The number of roads built by this player over the entire map. 
    private int settlements;    // The number of settlements built by this player over the entire map. 
    private int turnOrder;      // The place in the turn order the player is, 0 for position not set.
    private int victoryPoints;  // The amount of victory points a player currently has.
    public  int connectionID;   // Used to send a message to a player, linked with id number SILAS

    private int brick = Constants.StartingResources; // The amount of Brick the player currently owns.
    private int ore = Constants.StartingResources; // The amount of Brick the player currently owns.
    private int wheat = Constants.StartingResources; // The amount of Brick the player currently owns.
    private int wood = Constants.StartingResources; // The amount of Brick the player currently owns.
    private int wool = Constants.StartingResources; // The amount of Brick the player currently owns.

    private float brickDiscount = Constants.NoDiscount; // The discount the player receives when purchasing Brick from the market.
    private float oreDiscount = Constants.NoDiscount; // The discount the player receives when purchasing Ore from the market.
    private float wheatDiscount = Constants.NoDiscount; // The discount the player receives when purchasing Wheat from the market.
    private float woodDiscount = Constants.NoDiscount; // The discount the player receives when purchasing Wood from the market.
    private float woolDiscount = Constants.NoDiscount; // The discount the player receives when purchasing Wool from the market.

    // Constructors to instantiate Player classes
    #region Public constructors

    // Default constructor.
    public Player()
    {
        name = "";
        armies = 0;
        character = 0;
        cities = 0;
        gold = Constants.StartingGold;
        roads = 0;
        settlements = 0;
        turnOrder = 0;
        victoryPoints = 0;
    }

    // Constructor used in the game lobby.
    public Player(string name)
    {
        name = this.name;
        armies = 0;
        character = 0;
        cities = 0;
        gold = Constants.StartingGold;
        roads = 0;
        settlements = 0;
        turnOrder = 0;
        victoryPoints = 0;
    }
    #endregion Public constructors

    // Public accessors. Includes data validation.
    #region Public accessors
    public string Name { get; set; }

    public int Armies
    {
        get
        {
            return armies;
        }
        set
        {
            if (value > 0)
            {
                armies = value;
            }
            else
                throw new System.ArgumentOutOfRangeException("Armies cannot be less than zero.");
        }
    }

    public int Character
    {
        get
        {
            return character;
        }
        set
        {
            if (value > 0)
            {
                character = value;
            }
            else
                throw new System.ArgumentOutOfRangeException("Character selection cannot be less than zero.");
        }
    }

    public int Cities
    {
        get
        {
            return cities;
        }
        set
        {
            if (value >= 0)
            {
                if (value <= Constants.MaxCities)
                    cities = value;
                else
                    throw new System.ArgumentOutOfRangeException("Cities built cannot exceed the maximum number of cities.");
            }
            else
                throw new System.ArgumentOutOfRangeException("Cities built cannot be less than zero.");
        }
    }

    public int Gold
    {
        get
        {
            return gold;
        }
        set
        {
            if (value >= 0)
            {
                gold = value;
            }
            else
                throw new System.ArgumentOutOfRangeException("Gold amount cannot be less than zero.");
        }
    }

    public int Roads
    {
        get
        {
            return roads;
        }
        set
        {
            if (value >= 0)
            {
                if (value <= Constants.MaxRoads)
                    roads = value;
                else
                    throw new System.ArgumentOutOfRangeException("Roads built cannot exceed the maximum number of roads.");
            }
            else
                throw new System.ArgumentOutOfRangeException("Roads built cannot be less than zero.");
        }
    }

    public int Settlements
    {
        get
        {
            return settlements;
        }
        set
        {
            if (value >= 0)
            {
                if (value <= Constants.MaxSettlements)
                    settlements = value;
                else
                    throw new System.ArgumentOutOfRangeException("Settlements built cannot exceed the maximum number of settlements.");
            }
            else
                throw new System.ArgumentOutOfRangeException("Settlements built cannot be less than zero.");
        }
    }

    public int TurnOrder
    {
        get
        {
            return turnOrder;
        }
        set
        {
            if (value > 0)
            {
                turnOrder = value;
            }
            throw new System.ArgumentOutOfRangeException("Turn order must be greater than zero.");
        }
    }

    public int VictoryPoints
    {
        get
        {
            return victoryPoints;
        }
        set
        {
            if (value >= 0)
            {
                victoryPoints = value;
            }
            else
                throw new System.ArgumentOutOfRangeException("Victory points cannot be less than zero.");
        }
    }

    // Public accessors to return the resources available.
    public int Brick { get; set; }
    public int Ore { get; set; }
    public int Wheat { get; set; }
    public int Wood { get; set; }
    public int Wool { get; set; }

    #endregion Public accessors
    #endregion Properties

    // Public methods.
    #region Methods

    // Sets the standard resource discount for when a player builds on a general port.
    public void SetStandardDiscount()
    {
        brickDiscount = Constants.StandardDiscount;
        oreDiscount = Constants.StandardDiscount;
        wheatDiscount = Constants.StandardDiscount;
        woodDiscount = Constants.StandardDiscount;
        woolDiscount = Constants.StandardDiscount;
    }

    // Sets the specific Brick discount for when a player builds on a Brick port.
    public void SetBrickDiscount() { brickDiscount = Constants.SpecificDiscount; }

    // Sets the specific Ore discount for when a player builds on an Ore port.
    public void SetOreDiscount() { oreDiscount = Constants.SpecificDiscount; }

    // Sets the specific Wheat discount for when a player builds on a Wheat port.
    public void SetWheatDiscount() { wheatDiscount = Constants.SpecificDiscount; }

    // Sets the specific Wood discount for when a player builds on a Wood port.
    public void SetWoodDiscount() { woodDiscount = Constants.SpecificDiscount; }

    // Sets the specific Wool discount for when a player builds on a Wool port.
    public void SetWoolDiscount() { woolDiscount = Constants.SpecificDiscount; }

    // Subtracts the cost for a Brick from the player's gold and adds a Brick to their inventory.
    public void BuyBrick()
    {
        float cost = brickDiscount * Constants.ResourceCost;

        // If player has enough gold, complete transaction, else throw exception.
        if (gold >= (int)cost)
        {
            gold -= (int)cost;
            brick++;
        }
        else
            throw new System.ArgumentOutOfRangeException("Player does not own enough gold to purchase a Brick.");
    }

    // Subtracts the cost for an Ore from the player's gold and adds an Ore to their inventory.
    public void BuyOre()
    {
        float cost = oreDiscount * Constants.ResourceCost;

        // If player has enough gold, complete transaction, else throw exception.
        if (gold >= (int)cost)
        {
            gold -= (int)cost;
            ore++;
        }
        else
            throw new System.ArgumentOutOfRangeException("Player does not own enough gold to purchase an Ore.");
    }

    // Subtracts the cost for a Wheat from the player's gold and adds a Wheat to their inventory.
    public void BuyWheat()
    {
        float cost = wheatDiscount * Constants.ResourceCost;

        // If player has enough gold, complete transaction, else throw exception.
        if (gold >= (int)cost)
        {
            gold -= (int)cost;
            wheat++;
        }
        else
            throw new System.ArgumentOutOfRangeException("Player does not own enough gold to purchase a Wheat.");
    }

    // Subtracts the cost for a Wood from the player's gold and adds a Wood to their inventory.
    public void BuyWood()
    {
        float cost = woodDiscount * Constants.ResourceCost;

        // If player has enough gold, complete transaction, else throw exception.
        if (gold >= (int)cost)
        {
            gold -= (int)cost;
            wood++;
        }
        else
            throw new System.ArgumentOutOfRangeException("Player does not own enough gold to purchase a Wood.");
    }

    // Subtracts the cost for a Wool from the player's gold and adds a Wool to their inventory.
    public void BuyWool()
    {
        float cost = woolDiscount * Constants.ResourceCost;

        // If player has enough gold, complete transaction, else throw exception.
        if (gold >= (int)cost)
        {
            gold -= (int)cost;
            wool++;
        }
        else
            throw new System.ArgumentOutOfRangeException("Player does not own enough gold to purchase a Wool.");
    }

    // Adds gold to the player based on the amount of settlements and cities constructed.
    public void AddTurnGold()
    {
        int goldToAdd = 0;

        goldToAdd += (settlements * Constants.GoldPerSettlement);
        goldToAdd += (cities * Constants.GoldPerCity);

        gold += goldToAdd;
    }

    // Subtracts the cost for a Road from the player's resource inventory and adds a Road to items built.
    public void BuildRoad()
    {
        if (brick < Constants.BricksPerRoad)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Brick to build a Road.");
        if (ore < Constants.OrePerRoad)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Ore to build a Road.");
        if (wheat < Constants.WheatPerRoad)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wheat to build a Road.");
        if (wood < Constants.WoodPerRoad)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wood to build a Road.");
        if (wool < Constants.WoolPerRoad)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wool to build a Road.");

        brick -= Constants.BricksPerRoad;
        ore -= Constants.OrePerRoad;
        wheat -= Constants.WheatPerRoad;
        wood -= Constants.WoodPerRoad;
        wool -= Constants.WoolPerRoad;

        roads++;
    }

    // Subtracts the cost for a Settlement from the player's resource inventory and adds a Settlement to items built.
    public void BuildSettlement()
    {
        if (brick < Constants.BricksPerSettlement)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Brick to build a Settlement.");
        if (ore < Constants.OrePerSettlement)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Ore to build a Settlement.");
        if (wheat < Constants.WheatPerSettlement)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wheat to build a Settlement.");
        if (wood < Constants.WoodPerSettlement)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wood to build a Settlement.");
        if (wool < Constants.WoolPerSettlement)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wool to build a Settlement.");

        brick -= Constants.BricksPerSettlement;
        ore -= Constants.OrePerSettlement;
        wheat -= Constants.WheatPerSettlement;
        wood -= Constants.WoodPerSettlement;
        wool -= Constants.WoolPerSettlement;

        settlements++;
    }

    // Subtracts the cost for a City from the player's resource inventory and adds a City to items built.
    public void BuildCity()
    {
        if (brick < Constants.BricksPerCity)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Brick to build a City.");
        if (ore < Constants.OrePerCity)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Ore to build a City.");
        if (wheat < Constants.WheatPerCity)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wheat to build a City.");
        if (wood < Constants.WoodPerCity)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wood to build a City.");
        if (wool < Constants.WoolPerCity)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wool to build a City.");

        brick -= Constants.BricksPerCity;
        ore -= Constants.OrePerCity;
        wheat -= Constants.WheatPerCity;
        wood -= Constants.WoodPerCity;
        wool -= Constants.WoolPerCity;

        cities++;
    }

    // Subtracts the cost to hire an Army from the player's gold amount and adds a Army to the player's total armies.
    public void HireArmy()
    {
        if (gold < Constants.ArmyGoldCost)
            throw new System.ArgumentOutOfRangeException("Player does not own enough gold to hire an Army.");

        gold -= Constants.ArmyGoldCost;
        armies++;
    }
    #endregion Methods
}