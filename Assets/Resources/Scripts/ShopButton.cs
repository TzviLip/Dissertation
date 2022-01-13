using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private int cost;

    public void setIcon(Sprite sprite)
    {
        icon.sprite = sprite; //Set Avatar Icon
    }

    public void setCost(int cost)
    {
        this.cost = cost; //Set Avatar Cost
    }
}
