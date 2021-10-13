using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllHorizontal : AllyBubble
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        CheackHighestPosition();
    }

    protected override void RemoveBubble()
    {
        
    }
}
