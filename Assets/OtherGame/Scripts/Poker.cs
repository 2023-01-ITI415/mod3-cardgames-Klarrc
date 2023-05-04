using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    public TMP_Text currentScoreText;
    public TMP_Text highScoreText;
    public int score = 0;
    public int highscore = 0;
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
        highscore = PlayerPrefs.GetInt("High Score", highscore);
        highScoreText.text = $"High Score: {highscore:#,###}";
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
        if(drawPile.Count > 0)
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
        for(int i = 0; i < 5; i++){
            if(S.cols[i] == 5){
                S.cols[1] ++;
                List<Card> hand = new List<Card>();
                hand.Add(S.cards[i, 0]);
                hand.Add(S.cards[i, 1]);
                hand.Add(S.cards[i, 2]);
                hand.Add(S.cards[i, 3]);
                hand.Add(S.cards[i, 4]);
                S.score += Evaluate(hand);
            }
            if(S.rows[i] == 5){
                S.rows[1] ++;
                List<Card> hand = new List<Card>();
                hand.Add(S.cards[0, i]);
                hand.Add(S.cards[1, i]);
                hand.Add(S.cards[2, i]);
                hand.Add(S.cards[3, i]);
                hand.Add(S.cards[4, i]);
                S.score += Evaluate(hand);
            }
        }
        S.currentScoreText.text = $"Score: {S.score:#,###}";
        if(S.drawPile.Count == 0){
            S.Game_End();
        }
    }
    void Game_End(){
        if(highscore < score){
            highscore = score;

        }
        PlayerPrefs.SetInt("High Score", highscore);
        highScoreText.text = $"High Score: {highscore:#,###}";
        Invoke("Reload", 4f);
    }
    void Reload(){
        SceneManager.LoadScene("PokerSolitaire");
    }
    //royal flush: 200
    //straight flush: 150
    //four: 100
    //full house: 50
    //flush: 40
    //straight: 30
    //three: 20
    //two pair: 10
    //one pair: 4
    static int Evaluate(List<Card> hand){
        Card low = hand[0];
        List<Card> sorted = new List<Card>();
        while (hand.Count != 0)
        {
            low = hand[0];
            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i].rank < low.rank)
                {
                    low = hand[i];
                }
            }
            sorted.Add(low);
            hand.Remove(low);

        }
        bool flush = false;
        char suit = sorted[0].suit;
        for(int i = 0; i < sorted.Count; i++){
            if(sorted[i].suit != suit){
                break;
            }
            if(i == sorted.Count - 1){
                flush = true;
            }
        }
        if(sorted[0].rank == 1 && sorted[1].rank == 10 && sorted[2].rank == 11 && sorted[3].rank == 12 && sorted[4].rank == 13 && flush){
            return 200;
        }
        if((flush && (sorted[1].rank == sorted[0].rank + 1 && sorted[2].rank == sorted[1].rank + 1 && sorted[3].rank == sorted[2].rank + 1 && sorted[4].rank == sorted[3].rank + 1) ||
        (sorted[0].rank == 1 && sorted[1].rank == 2 && sorted[4].rank == 13 && sorted[3].rank == 12 && sorted[2].rank == 11) ||
        (sorted[0].rank == 1 && sorted[1].rank == 2 && sorted[4].rank == 13 && sorted[3].rank == 12 && sorted[2].rank == 11) ||
        (sorted[0].rank == 1 && sorted[1].rank == 2 && sorted[2].rank == 3 && sorted[4].rank == 13 && sorted[3].rank == 12) ||
        (sorted[0].rank == 1 && sorted[1].rank == 2 && sorted[2].rank == 3 && sorted[3].rank == 4 && sorted[4].rank == 13)))
            return 150;
        if((sorted[0].rank == sorted[1].rank && sorted[1].rank == sorted[2].rank && sorted[2].rank == sorted[3].rank) ||
        (sorted[1].rank == sorted[2].rank && sorted[2].rank == sorted[3].rank && sorted[3].rank == sorted[4].rank))
            return 100;
        if((sorted[0].rank == sorted[1].rank && sorted[1].rank == sorted[2].rank && sorted[3].rank == sorted[4].rank) ||
        (sorted[0].rank == sorted[1].rank && sorted[2].rank == sorted[3].rank && sorted[3].rank == sorted[4].rank))
            return 50;
        if(flush)
            return 40;
        if((sorted[1].rank == sorted[0].rank + 1 && sorted[2].rank == sorted[1].rank + 1 && sorted[3].rank == sorted[2].rank + 1 && sorted[4].rank == sorted[3].rank + 1) ||
        (sorted[0].rank == 1 && sorted[1].rank == 2 && sorted[4].rank == 13 && sorted[3].rank == 12 && sorted[2].rank == 11) ||
        (sorted[0].rank == 1 && sorted[1].rank == 2 && sorted[4].rank == 13 && sorted[3].rank == 12 && sorted[2].rank == 11) ||
        (sorted[0].rank == 1 && sorted[1].rank == 2 && sorted[2].rank == 3 && sorted[4].rank == 13 && sorted[3].rank == 12) ||
        (sorted[0].rank == 1 && sorted[1].rank == 2 && sorted[2].rank == 3 && sorted[3].rank == 4 && sorted[4].rank == 13))
            return 30;
        if((sorted[0].rank == sorted[1].rank && sorted[1].rank == sorted[2].rank) ||
        (sorted[2].rank == sorted[3].rank && sorted[3].rank == sorted[4].rank) ||
        (sorted[1].rank == sorted[2].rank && sorted[2].rank == sorted[3].rank))
            return 20;
        if((sorted[0].rank == sorted[1].rank && sorted[2].rank == sorted[3].rank) ||
        (sorted[0].rank == sorted[1].rank && sorted[3].rank == sorted[4].rank) ||
        (sorted[1].rank == sorted[2].rank && sorted[3].rank == sorted[4].rank))
            return 10;
        if((sorted[0].rank == sorted[1].rank) ||
        (sorted[1].rank == sorted[2].rank) ||
        (sorted[2].rank == sorted[3].rank) ||
        (sorted[3].rank == sorted[4].rank))
            return 4;
        return 0;
    }
}
