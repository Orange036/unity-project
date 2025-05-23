using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotBar : MonoBehaviour
{

    [SerializeField] GameObject playerInteractionGameObject;

    RectTransform rectTransform;

    private void Awake()
    {
        PlayerInteraction playerInteraction = playerInteractionGameObject.GetComponent<PlayerInteraction>();
        playerInteraction.CurrentPositionChanged += OnCurrentPositionChange;

        rectTransform = GetComponent<RectTransform>();
    }

    private void OnCurrentPositionChange(object currPos, EventArgs a)
    {
        switch (currPos)
        {
            case 0: rectTransform.localPosition = new Vector3(-84, -180, 0); break;
            case 1: rectTransform.localPosition = new Vector3(-28, -180, 0); break;
            case 2: rectTransform.localPosition = new Vector3(28, -180, 0); break;
            case 3: rectTransform.localPosition = new Vector3(84, -180, 0); break;
            default: break;
        }
    }

}

