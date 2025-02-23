using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class CardInstance
{
    public int cardID;
    public CardPiles currentPile;

    public void TransferPile(CardPiles newPile)
    {
        currentPile = newPile;
    }
    public void InitCard(int ID)
    {
        cardID = ID;
        currentPile = CardPiles.Draw;
    }
}
