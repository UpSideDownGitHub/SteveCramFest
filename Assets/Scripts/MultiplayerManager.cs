using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public CameraController CameraController;
    public int currentPlayerCount;
    public int startPlayerCount = 2;

    public void PlayerAdded()
    {
        currentPlayerCount++;
        if (currentPlayerCount == startPlayerCount)
        {
            CameraController.FindPlayers();
        }
    }
}