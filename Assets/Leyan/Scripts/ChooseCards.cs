using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseCards : MonoBehaviour
{
    public int availableCardCount = 4;
    public int chosenCardCount = 2;
    public float YPos;
    public float XInterval;

    public List<HandCardUIInstance> cardUIList = new List<HandCardUIInstance>();
    public GameObject cardUI;
    public Transform canvas;
    public HandPileMainUI mainUI;

    IEnumerator DrawCards(int n)
    {
        for(int i=0;i< availableCardCount;i++)
        {
            int cardID= Random.Range(1, 21);
            CardInstance cardInstance = new CardInstance();
            cardInstance.InitCard(cardID);
            HandCardUIInstance card=Instantiate(cardUI, 
                new Vector3(i * XInterval - 1.5f * XInterval, YPos, 0), Quaternion.identity, canvas).GetComponent<HandCardUIInstance>();
        }
        yield break; ;
    }
    private void Start()
    {
       // StartCoroutine(DrawCards(availableCardCount));
    }

}
