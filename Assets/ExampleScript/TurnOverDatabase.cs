using System;
using System.Collections;
using System.Collections.Generic;
using ExampleScript;
using UnityEngine;

public class TurnOverDatabase : MonoBehaviour
{
    public TurnOver turnOver;

    private void Start()
    {
        turnOver.SetData(new List<TurnOverContent.TurnoverContentData>()
        {
            new TurnOverContent.TurnoverContentData(){title = "1"},
            new TurnOverContent.TurnoverContentData(){title = "2"},
            new TurnOverContent.TurnoverContentData(){title = "3"},
            new TurnOverContent.TurnoverContentData(){title = "4"},
            new TurnOverContent.TurnoverContentData(){title = "5"},
            new TurnOverContent.TurnoverContentData(){title = "6"},
            new TurnOverContent.TurnoverContentData(){title = "7"},
            new TurnOverContent.TurnoverContentData(){title = "8"},
        });
    }
}
