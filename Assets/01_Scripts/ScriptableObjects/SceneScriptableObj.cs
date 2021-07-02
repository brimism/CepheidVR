using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneData", menuName = "ScriptableObjects/SceneSO", order = 1)]
public class SceneScriptableObj : ScriptableObject
{
    //increment each state by one to access different conditions/story in each scene
    //increment before loading scene


    public int tutorialState;
    public int lobbyState;
    public int examRoomState;
    public int testRoomState;


    //reset everything at tutorial scene
    public void ResetAll()
    {
        tutorialState = 0;
        lobbyState = 0;
        examRoomState = 0;
        testRoomState = 0;
    }
}
