using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    public GameObject pefabOfItself { get; private set; }
    [SerializeField] public Sprite spriteOfItself;

    private void Awake()
    {
        if (pefabOfItself == null)
        {
            pefabOfItself = gameObject;
        }
    }
}
