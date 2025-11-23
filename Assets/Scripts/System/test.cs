using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{   
    [SerializeField] private List<CardData> deckData;

    private void Start()
    {
        CardSystem.Instance.Setup(deckData);
    }
}
