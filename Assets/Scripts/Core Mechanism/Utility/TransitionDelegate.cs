using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDelegate
{
    public delegate void PreTransition();
    public delegate void PostTransition();
    public delegate void OnTransition(float progress);

}
