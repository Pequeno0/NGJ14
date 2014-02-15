using UnityEngine;
using System.Collections;

public class MenuIngame : MonoBehaviour {

    private Rect windowRect;
    public int WindowWidth;
    public int WindowHeight;
    private bool showMenu = false;
    public GUISkin MenuSkin;
    public Rect MenuButtonRect;
    public GameStateController gamestateController;
    private Matrix4x4 MenuGUIMatrix;


	// Use this for initialization
	void Start () {
        MenuButtonRect = new Rect(Screen.width - 90, 10, 80, 40);
    }
	
	// Update is called once per frame
	void Update () {
        if (gamestateController == null)
        {
            var menu = gameObject.GetComponent<Menu>();
            gamestateController = menu.GameStateController;
            var scale = new Vector3(menu.GuiRatio, menu.GuiRatio, 1f);
            MenuGUIMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
            windowRect = new Rect((Screen.width / 2f) / menu.GuiRatio - WindowWidth / 2f, (Screen.height / 2f) / menu.GuiRatio - WindowHeight / 2f, WindowWidth, WindowHeight);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            showMenu = !showMenu;
	}

    void OnGUI()
    {
        var tempSkin = GUI.skin;

        if (MenuSkin != null)
            GUI.skin = MenuSkin;

        if (!showMenu && GUI.Button(MenuButtonRect, "OPTIONS"))
            showMenu = true;

        var tempMatrix = GUI.matrix;
        GUI.matrix = MenuGUIMatrix;

        if (showMenu)
            GUI.Window(2, windowRect, IngameMenuWindow, "NGJ 2014");

        if( MenuSkin !=  null)
            GUI.skin = tempSkin;    
        GUI.matrix = tempMatrix;
    }

    void IngameMenuWindow(int id)
    {
        if (GUILayout.Button("RESUME"))
            showMenu = false;

        if (gamestateController.CurrentGameState == GameState.Playing && GUILayout.Button("LOBBY"))
        {
            if( Network.peerType != NetworkPeerType.Disconnected)
                Network.Disconnect();
            Application.LoadLevel(0);
        }

        if (GUILayout.Button("EXIT"))
            Application.Quit();
    }
}
