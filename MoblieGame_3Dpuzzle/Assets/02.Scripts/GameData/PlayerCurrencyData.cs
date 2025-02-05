using System;

[Serializable]
public class PlayerCurrencyData
{
    public int gold;
    public int wood;
    public int stone;

    public PlayerCurrencyData()
    {
        gold = 0;
        wood = 0;
        stone = 0;
    }

    public void AddCurrency(int goldAmount, int woodAmount, int stoneAmount)
    {
        gold += goldAmount;
        wood += woodAmount;
        stone += stoneAmount;
    }

    public bool SpendCurrency(int goldAmount, int woodAmount, int stoneAmount)
    {
        if (gold >= goldAmount && wood >= woodAmount && stone >= stoneAmount)
        {
            gold -= goldAmount;
            wood -= woodAmount;
            stone -= stoneAmount;
            return true;
        }
        return false;
    }
}