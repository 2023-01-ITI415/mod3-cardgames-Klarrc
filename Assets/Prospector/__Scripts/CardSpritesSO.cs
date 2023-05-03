using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu( fileName = "CardSprites", menuName = "ScriptableObjects/CardSpritesSO")]
public class CardSpritesSO : ScriptableObject
{
    [Header("Card Stock")]
    public Sprite cardBack;
    public Sprite cardBackGold;
    public Sprite cardFront;
    public Sprite cardFrontGold;
    public Sprite cardBackSilver;
    public Sprite cardFrontSilver;

    [Header("Suits")]
    public Sprite suitClub;
    public Sprite suitDiamond;
    public Sprite suitHeart;
    public Sprite suitSpade;

    [Header("Pip Sprites")]
    public Sprite[] faceSprites;
    public Sprite[] rankSprites;

    private static CardSpritesSO S;

    public static Dictionary<char, Sprite> SUITS {get; private set;}

    public void Init(){
        INIT_STATISTICS(this);
    }
    static void INIT_STATISTICS(CardSpritesSO cSSO) {
        if (S != null) {
            Debug.LogError("CardSpritesSO.S can't be set a 2nd time!");
            return;
        }
        S = cSSO;
        SUITS = new Dictionary<char, Sprite>(){
            {'C', S.suitClub},
            {'D', S.suitDiamond},
            {'H', S.suitHeart},
            {'S', S.suitSpade}
        };
    }
    public static Sprite[] RANKS {
        get {return S.rankSprites;}
    }
    public static Sprite GET_FACE(string name) {
        foreach (Sprite spr in S.faceSprites) {
            if (spr.name == name) return spr;
        }
        return null;
    }
    public static Sprite BACK {
        get { return S.cardBack; }
    }
    public static Sprite BACKGOLD
    {
        get { 


            return S.cardBackGold;

            }
    }
    public static Sprite BACKSILVER
    {
        get { 


            return S.cardBackSilver;

            }
    }
    public static Sprite FRONT
    {
        get { 

            return S.cardFront; 
            }
    }
    public static Sprite FRONTSILVER
    {
        get { 

            return S.cardFrontSilver; 
            }
    }
    public static Sprite FRONTGOLD
    {
        get { 

            return S.cardFrontGold; 
            }
    }







    public static void RESET() {
        S = null;
    }
}
