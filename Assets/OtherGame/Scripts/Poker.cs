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
        Layout();
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
