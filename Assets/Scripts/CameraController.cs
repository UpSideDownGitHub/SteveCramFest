using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    public Camera cam;

    public GameObject[] players;
    public float minXBound;
    public float maxXBound;
    public float minYBound;
    public float maxYBound;
    public float camMaxSize;
    public float camMinSize;

    public float smoothTime = 0.15f; 
    private Vector3 cameraVelocity;
    private float velocity;

    public void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (players.Length == 2)
        {
            // Find the center point between the two players
            Vector3 centerPoint = (players[0].transform.position + players[1].transform.position) * 0.5f;

            // Calculate extents of players relative to center point
            float extentX = Mathf.Max(Mathf.Abs(players[0].transform.position.x - centerPoint.x), Mathf.Abs(players[1].transform.position.x - centerPoint.x));
            float extentY = Mathf.Max(Mathf.Abs(players[0].transform.position.y - centerPoint.y), Mathf.Abs(players[1].transform.position.y - centerPoint.y));

            // Calculate required zoom to fit players with some buffer
            float desiredOrthographicSize = Mathf.Max(extentX, extentY) + camMinSize; // Add a buffer of 1 unit

            // Clamp zoom to avoid extreme values
            desiredOrthographicSize = Mathf.Clamp(desiredOrthographicSize, cam.nearClipPlane + 0.1f, cam.farClipPlane - 0.1f);

            // Calculate desired camera position based on center point and bounds
            Vector3 desiredPosition = new Vector3(
                Mathf.Clamp(centerPoint.x, minXBound + GetCameraHalfWidth(desiredOrthographicSize), maxXBound - GetCameraHalfWidth(desiredOrthographicSize)),
                Mathf.Clamp(centerPoint.y, minYBound + GetCameraHalfHeight(desiredOrthographicSize), maxYBound - GetCameraHalfHeight(desiredOrthographicSize)),
                transform.position.z // Maintain current Z position
            );

            // Smoothly lerp towards the desired position and zoom

            var temp = Mathf.SmoothDamp(cam.orthographicSize, desiredOrthographicSize, ref velocity, smoothTime);
            cam.orthographicSize = temp > camMaxSize ? camMaxSize : temp;

            if (temp < camMaxSize)
                transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref cameraVelocity, smoothTime);
        }
        else if (players.Length == 1)
        {
            Vector3 centerPoint = players[0].transform.position;

            // Calculate extents of players relative to center point
            float extentX = Mathf.Abs(players[0].transform.position.x - centerPoint.x);
            float extentY = Mathf.Abs(players[0].transform.position.y - centerPoint.y);

            // Calculate required zoom to fit players with some buffer
            float desiredOrthographicSize = Mathf.Max(extentX, extentY) + camMinSize; // Add a buffer of 1 unit

            // Clamp zoom to avoid extreme values
            desiredOrthographicSize = Mathf.Clamp(desiredOrthographicSize, cam.nearClipPlane + 0.1f, cam.farClipPlane - 0.1f);

            // Calculate desired camera position based on center point and bounds
            Vector3 desiredPosition = new Vector3(
                Mathf.Clamp(centerPoint.x, minXBound + GetCameraHalfWidth(desiredOrthographicSize), maxXBound - GetCameraHalfWidth(desiredOrthographicSize)),
                Mathf.Clamp(centerPoint.y, minYBound + GetCameraHalfHeight(desiredOrthographicSize), maxYBound - GetCameraHalfHeight(desiredOrthographicSize)),
                transform.position.z // Maintain current Z position
            );

            // Smoothly lerp towards the desired position and zoom
            var temp = Mathf.SmoothDamp(cam.orthographicSize, desiredOrthographicSize, ref velocity, smoothTime);
            cam.orthographicSize = temp > camMaxSize ? camMaxSize : temp;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref cameraVelocity, smoothTime);
        }
    }

    private float GetCameraHalfWidth(float orthographicSize)
    {
        return orthographicSize * cam.aspect;
    }

    private float GetCameraHalfHeight(float orthographicSize)
    {
        return orthographicSize;
    }
}