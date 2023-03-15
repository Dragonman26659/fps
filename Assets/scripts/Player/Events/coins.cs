using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coins : MonoBehaviour
{

    public int CoinAmmount;


    public void RemoveCoins(int ammount)
    {
        CoinAmmount -= ammount;
    }

    public void AddCoins(int ammount)
    {
        CoinAmmount += ammount;
    }
}
