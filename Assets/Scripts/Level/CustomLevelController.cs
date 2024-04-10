using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LatteGames;

public class CustomLevelController : LevelController
{    
    public enum State { Playing, Win, Lose }
    public State LevelState;

    private void Start()
    {
    }

    private void Update()
    {

    }
    public override bool IsVictory()
    {
        return LevelState == State.Win;
    }
}
