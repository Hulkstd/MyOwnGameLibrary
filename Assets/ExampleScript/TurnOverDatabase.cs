using System;
using System.Collections;
using System.Collections.Generic;
using ExampleScript;
using UnityEngine;

public class TurnOverDatabase : MonoBehaviour
{
    public TurnOver _turnOver;

    private void Start()
    {
        _turnOver.SetData(new List<TurnOverContent.TurnoverContentData>()
        {
            new TurnOverContent.TurnoverContentData(){Title = "1"},
            new TurnOverContent.TurnoverContentData(){Title = "2"},
            new TurnOverContent.TurnoverContentData(){Title = "3"},
            new TurnOverContent.TurnoverContentData(){Title = "4"},
            new TurnOverContent.TurnoverContentData(){Title = "5"},
            new TurnOverContent.TurnoverContentData(){Title = "6"},
            new TurnOverContent.TurnoverContentData(){Title = "7"},
            new TurnOverContent.TurnoverContentData(){Title = "8"},
        });
    }
}
