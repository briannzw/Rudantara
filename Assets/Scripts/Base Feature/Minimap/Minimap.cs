using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minimap : MonoBehaviour
{
    public RectTransform marker; //player pointer image
    public RectTransform mapImage;//Map screenshot used in canvas
    public RectTransform mapMaskImage;
    public Transform playerReference;//player
    public Vector2 offset;//Adjust the value to match you map

    private Camera minimapCam;
    private Vector3[] mapBound = new Vector3[4];

    private Vector2 mapDimensions;
    [SerializeField] private Vector2 areaDimensions;

    private PlayerAction playerControls;

    private void Start()
    {
        playerControls = InputManager.PlayerAction;
        RegisterInputCallback();
        mapDimensions = new Vector2(mapImage.sizeDelta.x, mapImage.sizeDelta.y);
    }

    #region Callback
    private void OnEnable()
    {
        RegisterInputCallback();
    }

    private void OnDisable()
    {
        UnregisterInputCallback();
    }

    private void RegisterInputCallback()
    {
        if (playerControls == null) return;
        playerControls.Gameplay.Minimap.performed += ToggleMinimap;
    }

    private void UnregisterInputCallback()
    {
        if (playerControls == null) return;
        playerControls.Gameplay.Minimap.performed -= ToggleMinimap;
    }
    #endregion

    private void ToggleMinimap(InputAction.CallbackContext context)
    {
        if (!playerControls.Gameplay.Minimap.enabled) return;
        mapMaskImage.gameObject.SetActive(!mapMaskImage.gameObject.activeSelf);
    }

    public void Initialize(Camera cam)
    {
        minimapCam = cam;
        mapBound[0] = minimapCam.ScreenToWorldPoint(new Vector3(0, 0, minimapCam.nearClipPlane));
        mapBound[1] = minimapCam.ScreenToWorldPoint(new Vector3(minimapCam.pixelWidth, 0, minimapCam.nearClipPlane));
        mapBound[2] = minimapCam.ScreenToWorldPoint(new Vector3(minimapCam.pixelWidth, minimapCam.pixelHeight, minimapCam.nearClipPlane));
        areaDimensions.x = Mathf.Abs(mapBound[1].x - mapBound[0].x);
        areaDimensions.y = Mathf.Abs(mapBound[2].z - mapBound[1].z);
    }

    private void Update()
    {
        if (minimapCam == null) return;
        SetMarkerPosition();
    }

    private void SetMarkerPosition()
    {
        Vector3 distance = playerReference.position - mapBound[1];
        Vector2 coordinates = new Vector2(distance.x / areaDimensions.x, distance.z / areaDimensions.y);
        //mapImage.anchoredPosition = new Vector2(coordinates.x * mapDimentions.x, coordinates.y * mapDimentions.y) + offset;// - mapMaskImage.sizeDelta / 2;
        marker.anchoredPosition = new Vector2(coordinates.x * mapDimensions.x, coordinates.y * mapDimensions.y) + offset;
        marker.rotation = Quaternion.Euler(new Vector3(0, 0, -playerReference.eulerAngles.y - 90f));
        //Debug.Log(new Vector2(coordinates.x * mapDimentions.x, coordinates.y * mapDimentions.y)); Hasil posisi x dan y dari kanan bawah
        // Coordinate - center of mapImage = mapImage position
    }
}
