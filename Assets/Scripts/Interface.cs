using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface : MonoBehaviour {

	// Use this for initialization
	void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "Check HP Change"))
        {
			CharacterLogic.playerTeam[0].SetHp(-90);
        }
        if (GUI.Button(new Rect(160, 10, 150, 100), "Test Action"))
        {
        }

        GUI.Label(new Rect(10, 115, 100, 20), "Hello World!");
        GUI.Label(new Rect(10, 135, 100, 20), "Hello World!");

        // Action progress bar
        DrawQuad(new Rect(10, Screen.height - 25, 120, 15), new Color(0, 0, 0));
        DrawQuad(new Rect(10, Screen.height - 15, CharacterLogic.playerTeam[0].ActionProgress, 5), new Color(0,191,255));
        // Action progress bar
        DrawQuad(new Rect(150, Screen.height - 25, 120, 15), new Color(0, 0, 0));
        DrawQuad(new Rect(150, Screen.height - 15, CharacterLogic.playerTeam[1].ActionProgress, 5), new Color(0,191,255));

    }
    void DrawQuad(Rect position, Color color) {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0,0,color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;

        GUI.Box(position, GUIContent.none);
    }

}
