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
    public GameStateController gamestateController;
    public Matrix4x4 MenuGUIMatrix;
    public bool showScores = false;

	// Use this for initialization
	void Start () {
        gamestateController = GameObject.FindObjectOfType<Menu>().GameStateController;
    }

    void Update()
    {
        if (gamestateController == null)
        {
            var menu = GameObject.FindObjectOfType<Menu>();
            gamestateController = menu.GameStateController;
            menu.SetGUIScale();
            MenuGUIMatrix = GUI.matrix;
        }

        else if (!showScores && gamestateController.CurrentGameState == GameState.Playing)
        {
            foreach (var p in gamestateController.PlayerController.Players)
                AllPlayerScores.Add(new PlayerScoreObject() { Player = p });
            showScores = true;
        }
    }
    
    void OnGUI()
    {
        if (showScores)
        {
            GUI.matrix = MenuGUIMatrix;

            var tempSkin = GUI.skin;
            if (ScoreGUISkin != null)
                GUI.skin = ScoreGUISkin;

            ScoreBoardRect.height = AllPlayerScores.Count * RowHeight + RowHeight;

            GUILayout.BeginArea(ScoreBoardRect);
            GUILayout.Box("SCORES");

            foreach (var score in AllPlayerScores.OrderBy(m => m.Player.Score))
                GUILayout.Box(score.ToString());

            GUILayout.EndArea();

            if (ScoreGUISkin != null)
                GUI.skin = tempSkin;

            // test
            if (Network.isServer && GUI.Button(new Rect(Screen.width - 60, 10, 50, 20), "+"))
                gamestateController.NetworkMessageController.AddToPlayerScoreOnServer(Network.player, 50);
        }
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
