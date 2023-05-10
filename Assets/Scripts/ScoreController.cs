using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public static ScoreController control;
    private int score;

    private void Awake()
    {

        if (control == null)
        {
            control = this;
            DontDestroyOnLoad(gameObject);
        } else if (control != this)
        {
            Destroy(gameObject);
        }
    }

    public void addScore(int amount)
    {
        score += amount;
    }

    public void resetScore()
    {
        score = 0;
    }

    public int getScore()
    {
        return score;
    }
}
