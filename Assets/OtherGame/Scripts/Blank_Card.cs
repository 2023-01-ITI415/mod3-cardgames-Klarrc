using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blank_Card : MonoBehaviour
{
    public int x = 0;
    public int y = 0;
    public void OnMouseUpAsButton(){
        Debug.Log(x + " " + y);
        Poker.CARD_CLICKED(this);
    }
}