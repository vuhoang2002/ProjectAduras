using UnityEngine;

public abstract class State
{
    public abstract void _OnEnter();
    public abstract void _OnUpdate(float tick); //deltatime
    public abstract void _OnExit();

}
