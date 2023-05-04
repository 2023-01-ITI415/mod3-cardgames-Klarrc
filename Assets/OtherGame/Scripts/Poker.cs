using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Deck))]
[RequireComponent(typeof(JsonParseLayout))]
public class Poker : MonoBehaviour
{
    private static Poker S;
    Card[,] cards = new Card[5,5];
    public List<Card> drawPile;
    private JsonLayout jsonLayout;
    private Deck deck;
    private Transform layoutAnchor;
    public GameObject Blank_Card;
    static public int x = -6;
    static public int y = -7;
    public int[] rows = new int[5];
    public int[] cols = new int[5];
    // Start is called before the first frame update
    void Start()
    {
        if (S != null) Debug.LogError("Attempted to set S more than once!"); // b
        S = this;
        if(layoutAnchor == null){
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
        }

        jsonLayout = GetComponent<JsonParseLayout>().layout;

        deck = GetComponent<Deck>();
        // These two lines replace the Start() call we commented out in Deck
        deck.InitDeck();
        Deck.Shuffle(ref deck.cards);
        drawPile = deck.cards;
        while(drawPile.Count > 25){
            drawPile[25].gameObject.SetActive(false);
            drawPile.RemoveAt(25);
        }
        Layout();

        for(int i = 0; i < 5; i ++){
            for(int j = 0; j <5; j ++){
                GameObject bc = Instantiate(Blank_Card, new Vector3(i * 4 + x, j * 4 + y, 0), Quaternion.identity);
                bc.GetComponent<Blank_Card>().x = i;
                bc.GetComponent<Blank_Card>().y = j;
            }
        }
    }
    void Layout(){
        Card cp;
        // Go through all the cards of the drawPile
        for (int i = 0; i < drawPile.Count; i++)
        {
            cp = drawPile[i];
            cp.transform.SetParent(layoutAnchor);

            // Position it correctly with the layout.drawPile.stagger
            Vector3 cpPos = new Vector3();
            cpPos.y = jsonLayout.multiplier.y * jsonLayout.drawPile.y;
            // Add the staggering for the drawPile
            cpPos.y += jsonLayout.drawPile.yStagger * i;
            cpPos.x = jsonLayout.multiplier.x * jsonLayout.drawPile.x;
            cpPos.z = 0.1f * i;
            cp.SetLocalPos(cpPos);

            cp.faceUp = false; // DrawPile Cards are all face-down
            // Set depth sorting


            cp.SetSpriteSortingLayer(jsonLayout.drawPile.layer);
            cp.SetSortingOrder(-10 * i);
        }
        drawPile[0].faceUp = true;
    }
    // Update is called once per frame
    Card Draw()
    {
        Card card = drawPile[0];
        drawPile.RemoveAt(0);
        Layout();
        return card;
    }
    static public void CARD_CLICKED(Blank_Card card){
        Card c = S.Draw();
        c.transform.position = new Vector3(card.x * 4 + x, card.y * 4 + y, -1);
        S.cards[card.x, card.y] = c;
        S.cols[card.x] ++;
        S.rows[card.y] ++;
    }
}
