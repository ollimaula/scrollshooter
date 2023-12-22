using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HighScore
{
    public string _id;
    public string _partition;
    public string name;
    public int score;
    public static HighScore Parse(string json) => JsonUtility.FromJson<HighScore>(json);
    public static List<HighScore> ParseList(string jsonArray) =>
        JsonUtility.FromJson<HighScoreWrapper>("{\"items\":" + jsonArray + "}").items;
    [Serializable] private class HighScoreWrapper { public List<HighScore> items; }
}
