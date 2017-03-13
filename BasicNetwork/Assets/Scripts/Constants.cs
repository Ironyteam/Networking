using UnityEngine;

public static class Constants
{
   public const string commandDivider  = ":";
   public const string gameDivider     = "~";
   public const string gameListDivider = ";";
   public const string addGame         = "1";
   public const string addPlayer       = "2";
   public const string requestGameList = "3";
   public const string cancelGame      = "4"; 
   public const string gameStarted     = "5";
   public const string gameEnded       = "6";
   public const string characterSelect = "7";
   public const string characterResult = "8";
   public const string diceRoll        = "9";
   public const string buildSettlement = "10";
   public const string upgradeToCity   = "11";
   public const string buildRoad       = "12";
   public const string buildArmy       = "13";
   public const string attackCity      = "14";
   public const string moveRobber      = "15";
   public const string endTurn         = "16";
   public const string startTurn       = "17";
   public const string sendChat        = "18";
   public const string networkError    = "19";
   public const string lobbyFull       = "20";
   public const string serverKillCode  = "1234";



    // Imported constants from Luke
    public const int StartingGold = 150;
    public const int StartingResources = 0;
    public const int GoldPerSettlement = 50;
    public const int GoldPerCity = 50;

    public const int BricksPerRoad = 0;
    public const int OrePerRoad = 0;
    public const int WheatPerRoad = 0;
    public const int WoodPerRoad = 0;
    public const int WoolPerRoad = 0;

    public const int BricksPerSettlement = 0;
    public const int OrePerSettlement = 0;
    public const int WheatPerSettlement = 0;
    public const int WoodPerSettlement = 0;
    public const int WoolPerSettlement = 0;

    public const int BricksPerCity = 0;
    public const int OrePerCity = 0;
    public const int WheatPerCity = 0;
    public const int WoodPerCity = 0;
    public const int WoolPerCity = 0;

    public const int ArmyGoldCost = 100;

    public const int MaxRoads = 10;
    public const int MaxSettlements = 10;
    public const int MaxCities = 10;
    public const int MaxPlayers = 6;

    public const int MinXCoordinate = 0;
    public const int MaxXCoordinate = 6;
    public const int MinYCoordinate = 0;
    public const int MaxYCoordinate = 6;

    public const float TurnTime = 120.0f;
    public const float StandardDiscount = 0.75f;
    public const float SpecificDiscount = 0.50f;
    public const float NoDiscount = 1.00f;
    public const float ResourceCost = 100.0f;
}