using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JsonParseDeck))]
public class Deck : MonoBehaviour
{
    [Header("Inscribed")]
    public CardSpritesSO cardSprites;
    public GameObject prefabCard;
    public GameObject prefabSprite;
    public bool startFaceUp = true;
    public int GoldCards = 6;
    public int SilverCards = 6;
    public static int silvers = 6;
    public static int golds = 6;

    [Header("Dynamic")]
    public Transform deckAnchor;
    public List<Card> cards;

    private JsonParseDeck jsonDeck;
    static public GameObject SPRITE_PREFAB { get; private set; }







    public void InitDeck()
    {

        SPRITE_PREFAB = prefabSprite;

        cardSprites.Init();


        jsonDeck = GetComponent<JsonParseDeck>();



        if (GameObject.Find("_Deck") == null)
        {

            GameObject anchorGO = new GameObject("_Deck");
            deckAnchor = anchorGO.transform;
        }

        MakeCards();
    }



    bool contains(int[] arr, int x)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == x)
                return true;
        }
        return false;
    }



    void MakeCards()
    {
        cards = new List<Card>();
        Card c;
        int[] gold = new int[GoldCards];

        int[] silver = new int[SilverCards];
        int[] used = new int[GoldCards + SilverCards];

        for (int i = 0; i < used.Length; i++)
            used[i] = -1;

        for (int i = 0; i < gold.Length; i++)
        {
            int n = Random.Range(0, 52);
            while (contains(used, n))
                n = Random.Range(0, 52);
            gold[i] = n;
            used[i] = n;

        }




        for (int i = 0; i < silver.Length; i++)
        {
            int n = Random.Range(0, 52);
            while (contains(used, n))
                n = Random.Range(0, 52);
            silver[i] = n;
            used[i + gold.Length] = n;
        }
        int count = 0;


        string suits = "CDHS";
        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j <= 13; j++)
            {

                Card.type cardtype;

                if (contains(gold, count))
                    cardtype = Card.type.gold;
                else if (contains(silver, count))
                    cardtype = Card.type.silver;
                else
                    cardtype = Card.type.normal;

                c = MakeCard(suits[i], j, cardtype);

                cards.Add(c);


                c.transform.position = new Vector3((j - 7) * 3, (i - 1.5f) * 4, 0);

                count++;
            }
        }
    }


    Card MakeCard(char suit, int rank, Card.type type)
    {
        GameObject go = Instantiate<GameObject>(prefabCard, deckAnchor); // f
        Card card = go.GetComponent<Card>();

        card.Init(suit, rank, type, startFaceUp);

        return card;
    }



    static public void Shuffle(ref List<Card> refCards)
    {

        Shuffle2(ref refCards);

        List<Card> cards1 = new List<Card>();


        List<int> gscards = new List<int>();
        
        for (int i = 0; i < refCards.Count; i++)
        {
            if (refCards[i].cardtype == Card.type.gold || refCards[i].cardtype == Card.type.silver)
            {
                gscards.Add(i);
                
            }

        }
        gscards.Sort();
        gscards.Reverse();

        
        foreach (int i in gscards)
        {
            cards1.Add(refCards[i]);
            refCards.RemoveAt(i);
        }

        while (refCards.Count >= 25)
        {
            cards1.Add(refCards[0]);
            refCards.RemoveAt(0);
        }
        foreach(Card x in refCards){
            if(x.cardtype == Card.type.gold || x.cardtype == Card.type.silver)
                Debug.Log("here");
        }
        Shuffle2(ref cards1);
        foreach (Card c in refCards)
        {
            cards1.Add(c);
        }
        refCards = cards1;

    }

    


    static public void Shuffle2(ref List<Card> refCards)
    {

        List<Card> tCards = new List<Card>();

        int ndx; 
        while (refCards.Count > 0)
        {

            ndx = Random.Range(0, refCards.Count);

            tCards.Add(refCards[ndx]);

            refCards.RemoveAt(ndx);
        }

        refCards = tCards;
        return;
    }


}
