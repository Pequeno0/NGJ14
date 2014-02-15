using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScoreBoard : MonoBehaviour {

    // TODO: send/recieve scores to/from network
    public List<PlayerScoreObject> AllPlayerScores = new List<PlayerScoreObject>();
    public Rect ScoreBoardRect = new Rect( 10, 10, 150, 200);
    public int RowHeight = 30;
    public GUISkin ScoreGUISkin;

	// Use this for initialization
	void Start () {
        
	}

    void OnGUI()
    {
        var tempSkin = GUI.skin;
        if (ScoreGUISkin != null)
            GUI.skin = ScoreGUISkin;

        ScoreBoardRect.height = AllPlayerScores.Count * RowHeight + RowHeight;

        GUILayout.BeginArea(ScoreBoardRect);
        GUILayout.Box("SCORES");

        foreach (var score in AllPlayerScores.OrderBy( m => m.Player.Score))
            GUILayout.Box(score.ToString());

        GUILayout.EndArea();

        if (ScoreGUISkin != null)
            GUI.skin = tempSkin;
    }

    public void AddPlayerScoreObject(Player player)
    {
        AllPlayerScores.Add(new PlayerScoreObject() { Player = player });
    }
}

public class PlayerScoreObject
{
    public Player Player;

    public override string ToString()
    {
        return string.Format("{0} : {1}", this.Player.Name, this.Player.Score);
    }
}
