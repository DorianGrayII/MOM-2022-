using System;
using MHUtils;
using MHUtils.UI;
using MOM;
using UnityEngine;
using WorldCode;

public class CameraController : MonoBehaviour
{
    private static CameraController instance;

    public const int TERRAIN_LAYER = 9;

    public static global::UnityEngine.Plane clickUnityPlane = new global::UnityEngine.Plane(Vector3.up, Vector3.zero);

    public float keysDragSpeed = 15f;

    public float mouseDragSpeed = 30f;

    public float dragScrollMultiplier = 0.2f;

    public float targetHeight = 7f;

    public float targetMaxHeight = 12f;

    public float targetMinHeight = 7f;

    public float targetForcedHeight = 4f;

    private int zoom;

    private float zoomIn;

    private float zoomOut;

    private float forcedHeight;

    public float maxAngle = 50f;

    public float minAngle = 40f;

    public float minFOV = 50f;

    public float maxFOV = 40f;

    public Vector3i focusPoint;

    public float smoothMaxSpeed = 10000f;

    public float smoothStopMagnitude = 0.1f;

    public float smoothTime = 0.1f;

    private Vector2 mouseNavigationBuffer;

    private Vector2 oldMousePosition;

    private Vector3 targetPos = Vector3.negativeInfinity;

    private Camera mainCamera;

    private float makeNextCenterInstant;

    private Vector3 smoothVelocity = Vector3.zero;

    private void Start()
    {
        CameraController.instance = this;
        this.mainCamera = Camera.main;
        Vector3 localPosition = this.mainCamera.transform.localPosition;
        localPosition.y = this.targetMinHeight;
        this.mainCamera.transform.localPosition = localPosition;
        this.mainCamera.fieldOfView = this.minFOV;
        this.mainCamera.transform.rotation = Quaternion.Euler(new Vector3(this.minAngle, 0f, 0f));
    }

    private void FixedUpdate()
    {
        if (FSMCoreGame.instance == null)
        {
            return;
        }
        if (this.focusPoint != Vector3i.zero && this.focusPoint.x + this.focusPoint.y + this.focusPoint.z == 0)
        {
            CameraController.CenterAt(this.focusPoint);
            this.focusPoint = Vector3i.zero;
        }
        Vector3 zero = Vector3.zero;
        Vector3 localPosition = this.mainCamera.transform.localPosition;
        Vector3 position = this.mainCamera.transform.position;
        if (SettingsBlock.IsKey(Settings.KeyActions.UI_MOVE_CAMERA_EAST) && (UIManager.IsTopForInput(HUD.Get()) || UIManager.IsTopForInput(BattleHUD.Get())))
        {
            zero += new Vector3(this.keysDragSpeed * Time.deltaTime, 0f, 0f);
        }
        if (SettingsBlock.IsKey(Settings.KeyActions.UI_MOVE_CAMERA_WEST) && (UIManager.IsTopForInput(HUD.Get()) || UIManager.IsTopForInput(BattleHUD.Get())))
        {
            zero += new Vector3((0f - this.keysDragSpeed) * Time.deltaTime, 0f, 0f);
        }
        if (SettingsBlock.IsKey(Settings.KeyActions.UI_MOVE_CAMERA_SOUTH) && (UIManager.IsTopForInput(HUD.Get()) || UIManager.IsTopForInput(BattleHUD.Get())))
        {
            zero += new Vector3(0f, 0f, (0f - this.keysDragSpeed) * Time.deltaTime);
        }
        if (SettingsBlock.IsKey(Settings.KeyActions.UI_MOVE_CAMERA_NORTH) && (UIManager.IsTopForInput(HUD.Get()) || UIManager.IsTopForInput(BattleHUD.Get())))
        {
            zero += new Vector3(0f, 0f, this.keysDragSpeed * Time.deltaTime);
        }
        if (UIManager.IsTopForInput(HUD.Get()) || UIManager.IsTopForInput(BattleHUD.Get()))
        {
            if (Input.GetMouseButton(2))
            {
                if (Screen.safeArea.Contains(this.oldMousePosition))
                {
                    Vector2 vector = new Vector2(this.oldMousePosition.x - Input.mousePosition.x, this.oldMousePosition.y - Input.mousePosition.y);
                    Vector2 vector2 = 0.01f * this.mouseDragSpeed * vector;
                    if (Mathf.Abs(vector2.x) > Mathf.Abs(this.mouseNavigationBuffer.x))
                    {
                        this.mouseNavigationBuffer.x = vector2.x * this.dragScrollMultiplier;
                    }
                    if (Mathf.Abs(vector2.y) > Mathf.Abs(this.mouseNavigationBuffer.y))
                    {
                        this.mouseNavigationBuffer.y = vector2.y * this.dragScrollMultiplier;
                    }
                }
                else
                {
                    this.mouseNavigationBuffer = Vector2.zero;
                }
            }
            if (Settings.GetData().GetEdgescrolling() && this.IsMouseWithinGameWindow())
            {
                float num = 15f;
                float num2 = 0f;
                if (Input.mousePosition.x >= (float)Screen.width - num)
                {
                    this.mouseNavigationBuffer.x += this.dragScrollMultiplier;
                }
                if (Input.mousePosition.x <= num)
                {
                    this.mouseNavigationBuffer.x -= this.dragScrollMultiplier;
                }
                if (Input.mousePosition.y >= (float)Screen.height - num - num2)
                {
                    this.mouseNavigationBuffer.y += this.dragScrollMultiplier;
                }
                if (Input.mousePosition.y <= num)
                {
                    this.mouseNavigationBuffer.y -= this.dragScrollMultiplier;
                }
            }
        }
        this.oldMousePosition = Input.mousePosition;
        if (this.mouseNavigationBuffer.x != 0f || this.mouseNavigationBuffer.y != 0f)
        {
            if (this.mouseNavigationBuffer.sqrMagnitude < 0.0004f)
            {
                zero.x += this.mouseNavigationBuffer.x;
                zero.z += this.mouseNavigationBuffer.y;
                this.mouseNavigationBuffer = Vector2.zero;
            }
            else
            {
                Vector2 vector3 = 0.13f * this.mouseNavigationBuffer;
                zero.x += vector3.x;
                zero.z += vector3.y;
                this.mouseNavigationBuffer -= vector3;
            }
        }
        if (zero != Vector3.zero)
        {
            this.StopSmooth();
            float num3 = CameraController.GetClickWorldPosition(flat: true, mousePosition: false).z - position.z;
            bool horizontalWrap = World.GetActivePlane().area.horizontalWrap;
            if (zero.z > 0f)
            {
                Vector3 vector4 = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A11);
                zero.z = Mathf.Max(0f, Mathf.Min(zero.z, vector4.z - position.z - num3 - 3f));
            }
            else if (zero.z < 0f)
            {
                Vector3 vector5 = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A00);
                zero.z = Mathf.Min(0f, Mathf.Max(zero.z, vector5.z - position.z - num3 + 3f));
            }
            if (!horizontalWrap)
            {
                if (zero.x > 0f)
                {
                    Vector3 vector6 = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A11);
                    zero.x = Mathf.Max(0f, Mathf.Min(zero.x, vector6.x - position.x - 3f));
                }
                else if (zero.x < 0f)
                {
                    Vector3 vector7 = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A00);
                    zero.x = Mathf.Min(0f, Mathf.Max(zero.x, vector7.x - position.x + 3f));
                }
            }
            base.transform.Translate(zero);
        }
        else
        {
            this.UpdateCameraSmooth();
        }
        if (Input.mouseScrollDelta.y != 0f && !UIManager.IsOverUI() && this.IsMouseWithinGameWindow())
        {
            this.zoom = Mathf.Clamp(Mathf.RoundToInt((float)this.zoom - Input.mouseScrollDelta.y), 0, 5);
            this.zoomOut = (float)this.zoom / 5f;
        }
        float num4 = this.targetMinHeight;
        float num5 = this.targetMaxHeight;
        if (this.forcedHeight > 0f)
        {
            num4 = this.forcedHeight;
        }
        if (SettingsBlock.IsKey(Settings.KeyActions.UI_ZOOM_IN))
        {
            float num6 = num4 + (num5 - num4) * this.zoomOut;
            float num7 = (num5 - num4) / 6f;
            if (Mathf.Abs(num6 - localPosition.y) < num7)
            {
                this.zoom = Mathf.Clamp(Mathf.RoundToInt(this.zoom + 1), 0, 5);
                this.zoomOut = (float)this.zoom / 5f;
            }
        }
        if (SettingsBlock.IsKey(Settings.KeyActions.UI_ZOOM_OUT))
        {
            float num8 = num4 + (num5 - num4) * this.zoomOut;
            float num9 = (num5 - num4) / 6f;
            if (Mathf.Abs(num8 - localPosition.y) < num9)
            {
                this.zoom = Mathf.Clamp(Mathf.RoundToInt(this.zoom - 1), 0, 5);
                this.zoomOut = (float)this.zoom / 5f;
            }
        }
        _ = this.targetMaxHeight;
        _ = this.maxAngle;
        _ = this.minAngle;
        float num10 = num4 + (this.targetMaxHeight - num4) * this.zoomOut;
        if (this.forcedHeight > 0f)
        {
            num10 = this.forcedHeight;
        }
        float num11 = num10 - localPosition.y;
        if (Mathf.Abs(num11) > 0.01f)
        {
            if (this.forcedHeight == 0f)
            {
                num11 = Mathf.Clamp(num11, -0.3f, 0.3f);
            }
            float cameraHeight = Mathf.Clamp(localPosition.y + num11, num4, this.targetMaxHeight);
            this.SetCameraHeight(cameraHeight);
        }
        if (Input.GetMouseButtonUp(0) && !UIManager.IsOverUI())
        {
            Vector3i vector3i = HexCoordinates.GetHexCoordAt(CameraController.GetClickWorldPosition());
            global::WorldCode.Plane activePlane = World.GetActivePlane();
            Hex hexAt = activePlane.GetHexAt(vector3i);
            if (activePlane != null)
            {
                vector3i = activePlane.area.KeepHorizontalInside(vector3i);
            }
            Vector3i vector3i2;
            if (hexAt == null)
            {
                vector3i2 = vector3i;
                Debug.Log("Hex click: " + vector3i2.ToString());
                return;
            }
            string[] obj = new string[6] { "Hex click: ", null, null, null, null, null };
            vector3i2 = vector3i;
            obj[1] = vector3i2.ToString();
            obj[2] = " Terrain type: ";
            obj[3] = hexAt.GetTerrain().ToString();
            obj[4] = " Terrain food: ";
            obj[5] = hexAt.GetFood().ToString();
            Debug.Log(string.Concat(obj));
        }
    }

    private void SetCameraHeight(float value)
    {
        Vector3 localPosition = Camera.main.transform.localPosition;
        localPosition.y = value;
        float num = this.targetMaxHeight - this.targetMinHeight;
        float num2 = this.maxAngle - this.minAngle;
        float num3 = Mathf.Sin((localPosition.y - this.targetMinHeight) / num * ((float)Math.PI / 2f));
        float num4 = Mathf.Lerp(this.minFOV, this.maxFOV, num3) - this.mainCamera.fieldOfView;
        if (Mathf.Abs(num4) > 0.01f)
        {
            num4 = Mathf.Clamp(num4, -1f, 1f);
            this.mainCamera.fieldOfView += num4;
        }
        Quaternion quaternion = Quaternion.Euler(new Vector3(num3 * num2 + this.minAngle, 0f, 0f));
        if (quaternion != Quaternion.identity)
        {
            this.mainCamera.transform.rotation = quaternion;
            this.mainCamera.transform.localPosition = localPosition;
        }
    }

    public static CameraController Get()
    {
        return CameraController.instance;
    }

    public static Vector3 GetCameraPosition()
    {
        return CameraController.Get().mainCamera.transform.parent.position;
    }

    public static void MoveInstantlyCameraPosition(Vector3 pos)
    {
        CameraController cameraController = CameraController.Get();
        cameraController.mainCamera.transform.parent.position = pos;
        cameraController.StopSmooth();
    }

    public static void MakeNextCenterInstant()
    {
        CameraController.Get().makeNextCenterInstant = Time.realtimeSinceStartup;
    }

    public static void CenterAt(Vector3i pos, bool instant = false, float verticalOffset = 0f)
    {
        CameraController cameraController = CameraController.Get();
        float num = Time.realtimeSinceStartup - cameraController.makeNextCenterInstant;
        if (instant || num < 0.2f)
        {
            cameraController.makeNextCenterInstant = 0f;
            CameraController.CenterAt(HexCoordinates.HexToWorld3D(pos), verticalOffset);
        }
        else
        {
            cameraController.targetPos = HexCoordinates.HexToWorld3D(pos);
        }
    }

    public static bool IsAlreadyTargeted(Vector3i pos)
    {
        float num = 0.01f;
        CameraController cameraController = CameraController.Get();
        Vector3 vector = HexCoordinates.HexToWorld3D(pos);
        if (Vector3.Magnitude(cameraController.targetPos - vector) < num)
        {
            return true;
        }
        Vector3 vector2 = CameraController.GetClickWorldPosition(flat: true, mousePosition: false) - cameraController.transform.position;
        if (Vector3.Magnitude(cameraController.transform.position + vector2 - vector) < num)
        {
            return true;
        }
        return false;
    }

    public static void Target(Vector3 pos)
    {
        CameraController.Get().targetPos = pos;
    }

    private void UpdateCameraSmooth()
    {
        if (this.targetPos.x != float.NegativeInfinity)
        {
            Vector3 vector = CameraController.GetClickWorldPosition(flat: true, mousePosition: false) - base.transform.position;
            Vector3 vector2 = Vector3.SmoothDamp(base.transform.position + vector, this.targetPos, ref this.smoothVelocity, this.smoothTime, this.smoothMaxSpeed);
            base.transform.position = vector2 - vector;
            if (this.smoothVelocity.magnitude <= this.smoothStopMagnitude)
            {
                this.StopSmooth();
            }
        }
    }

    private void StopSmooth()
    {
        this.targetPos = Vector3.negativeInfinity;
        this.smoothVelocity = Vector3.zero;
    }

    public static void CenterAt(Vector3 pos, float verticalOffset = 0f)
    {
        bool horizontalWrap = World.GetActivePlane().area.horizontalWrap;
        if (pos.z > 0f)
        {
            Vector3 vector = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A11);
            pos.z = Mathf.Min(pos.z, vector.z - 3f);
        }
        else if (pos.z < 0f)
        {
            Vector3 vector2 = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A00);
            pos.z = Mathf.Max(pos.z, vector2.z + 3f);
        }
        if (horizontalWrap)
        {
            if (pos.x > 0f)
            {
                Vector3 vector3 = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A11);
                pos.x = Mathf.Min(pos.x, vector3.x - 3f);
            }
            else if (pos.x < 0f)
            {
                Vector3 vector4 = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A00);
                pos.x = Mathf.Max(pos.x, vector4.x + 3f);
            }
        }
        Vector3 clickWorldPosition = CameraController.GetClickWorldPosition(flat: true, mousePosition: false);
        clickWorldPosition.z -= verticalOffset;
        Vector3 vector5 = clickWorldPosition - CameraController.Get().transform.position;
        Vector3 position = pos - vector5;
        position.y = 0f;
        CameraController.Get().transform.position = position;
        CameraController.Get().mouseNavigationBuffer = Vector2.zero;
    }

    public static Vector3 GetClickWorldPosition(bool flat = false, bool mousePosition = true, bool checkForUILock = false)
    {
        Vector2 position = ((!mousePosition) ? new Vector2(Screen.width / 2, Screen.height / 2) : ((Vector2)Input.mousePosition));
        return CameraController.GetClickWorldPosition(position, flat, checkForUILock);
    }

    public static Vector3 GetClickWorldPositionUV(Vector2 position, bool flat = false, bool checkForUILock = false)
    {
        position.x *= Screen.width;
        position.y *= Screen.height;
        return CameraController.GetClickWorldPosition(position, flat, checkForUILock);
    }

    public static Vector3 GetClickWorldPosition(Vector2 position, bool flat = false, bool checkForUILock = false)
    {
        if (checkForUILock && UIManager.IsOverUI())
        {
            return -Vector3.one;
        }
        float enter = 100f;
        Ray cameraRay = CameraController.GetCameraRay(position);
        if (!flat && Physics.Raycast(cameraRay, out var hitInfo, enter, 512) && hitInfo.point.y >= 0f)
        {
            return hitInfo.point;
        }
        if (CameraController.clickUnityPlane.Raycast(cameraRay, out enter))
        {
            return cameraRay.GetPoint(enter);
        }
        return Vector3.zero;
    }

    public static Ray GetCameraRay(Vector2 position)
    {
        float x = position.x / (float)Screen.width;
        float y = position.y / (float)Screen.height;
        return CameraController.instance.mainCamera.ViewportPointToRay(new Vector3(x, y));
    }

    public void SetForcedZoom(bool enable, Vector3i pos)
    {
        this.forcedHeight = (enable ? this.targetForcedHeight : 0f);
        if (enable)
        {
            this.SetCameraHeight(this.forcedHeight);
            CameraController.CenterAt(pos, instant: true);
        }
    }

    private bool IsMouseWithinGameWindow()
    {
        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition.x < 0f || mousePosition.y < 0f || mousePosition.x > (float)Screen.width || mousePosition.y > (float)Screen.height)
        {
            return false;
        }
        return true;
    }
}
