using MHUtils;
using MHUtils.UI;
using MOM;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using WorldCode;

public class CameraController : MonoBehaviour
{
    private static CameraController instance;
    public const int TERRAIN_LAYER = 9;
    public static UnityEngine.Plane clickUnityPlane = new UnityEngine.Plane(Vector3.up, Vector3.zero);
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

    public static unsafe void CenterAt(Vector3 pos, float verticalOffset)
    {
        if (pos.z > 0f)
        {
            Vector3 vector4 = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A11);
            pos.z = Mathf.Min(pos.z, vector4.z - 3f);
        }
        else if (pos.z < 0f)
        {
            Vector3 vector5 = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A00);
            pos.z = Mathf.Max(pos.z, vector5.z + 3f);
        }
        if (World.GetActivePlane().area.horizontalWrap)
        {
            if (pos.x > 0f)
            {
                Vector3 vector6 = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A11);
                pos.x = Mathf.Min(pos.x, vector6.x - 3f);
            }
            else if (pos.x < 0f)
            {
                Vector3 vector7 = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A00);
                pos.x = Mathf.Max(pos.x, vector7.x + 3f);
            }
        }
        Vector3 vector = GetClickWorldPosition(true, false, false);
        float* singlePtr1 = &vector.z;
        singlePtr1[0] -= verticalOffset;
        Vector3 vector2 = vector - Get().transform.position;
        Vector3 vector3 = pos - vector2;
        vector3.y = 0f;
        Get().transform.position = vector3;
        Get().mouseNavigationBuffer = Vector2.zero;
    }

    public static void CenterAt(Vector3i pos, bool instant, float verticalOffset)
    {
        CameraController controller = Get();
        float num = Time.realtimeSinceStartup - controller.makeNextCenterInstant;
        if (!instant && (num >= 0.2f))
        {
            controller.targetPos = HexCoordinates.HexToWorld3D(pos);
        }
        else
        {
            controller.makeNextCenterInstant = 0f;
            CenterAt(HexCoordinates.HexToWorld3D(pos), verticalOffset);
        }
    }

    private unsafe void FixedUpdate()
    {
        if (FSMCoreGame.instance != null)
        {
            if ((this.focusPoint != Vector3i.zero) && (((this.focusPoint.x + this.focusPoint.y) + this.focusPoint.z) == 0))
            {
                CenterAt(this.focusPoint, false, 0f);
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
                zero += new Vector3(-this.keysDragSpeed * Time.deltaTime, 0f, 0f);
            }
            if (SettingsBlock.IsKey(Settings.KeyActions.UI_MOVE_CAMERA_SOUTH) && (UIManager.IsTopForInput(HUD.Get()) || UIManager.IsTopForInput(BattleHUD.Get())))
            {
                zero += new Vector3(0f, 0f, -this.keysDragSpeed * Time.deltaTime);
            }
            if (SettingsBlock.IsKey(Settings.KeyActions.UI_MOVE_CAMERA_NORTH) && (UIManager.IsTopForInput(HUD.Get()) || UIManager.IsTopForInput(BattleHUD.Get())))
            {
                zero += new Vector3(0f, 0f, this.keysDragSpeed * Time.deltaTime);
            }
            if (UIManager.IsTopForInput(HUD.Get()) || UIManager.IsTopForInput(BattleHUD.Get()))
            {
                if (Input.GetMouseButton(2))
                {
                    if (!Screen.safeArea.Contains(this.oldMousePosition))
                    {
                        this.mouseNavigationBuffer = Vector2.zero;
                    }
                    else
                    {
                        Vector2 vector4 = new Vector2(this.oldMousePosition.x - Input.mousePosition.x, this.oldMousePosition.y - Input.mousePosition.y);
                        Vector2 vector5 = (Vector2) ((0.01f * this.mouseDragSpeed) * vector4);
                        if (Mathf.Abs(vector5.x) > Mathf.Abs(this.mouseNavigationBuffer.x))
                        {
                            this.mouseNavigationBuffer.x = vector5.x * this.dragScrollMultiplier;
                        }
                        if (Mathf.Abs(vector5.y) > Mathf.Abs(this.mouseNavigationBuffer.y))
                        {
                            this.mouseNavigationBuffer.y = vector5.y * this.dragScrollMultiplier;
                        }
                    }
                }
                if (Settings.GetData().GetEdgescrolling() && this.IsMouseWithinGameWindow())
                {
                    float num5 = 15f;
                    float num6 = 0f;
                    if (Input.mousePosition.x >= (Screen.width - num5))
                    {
                        float* singlePtr1 = &this.mouseNavigationBuffer.x;
                        singlePtr1[0] += this.dragScrollMultiplier;
                    }
                    if (Input.mousePosition.x <= num5)
                    {
                        float* singlePtr2 = &this.mouseNavigationBuffer.x;
                        singlePtr2[0] -= this.dragScrollMultiplier;
                    }
                    if (Input.mousePosition.y >= ((Screen.height - num5) - num6))
                    {
                        float* singlePtr3 = &this.mouseNavigationBuffer.y;
                        singlePtr3[0] += this.dragScrollMultiplier;
                    }
                    if (Input.mousePosition.y <= num5)
                    {
                        float* singlePtr4 = &this.mouseNavigationBuffer.y;
                        singlePtr4[0] -= this.dragScrollMultiplier;
                    }
                }
            }
            this.oldMousePosition = Input.mousePosition;
            if ((this.mouseNavigationBuffer.x != 0f) || (this.mouseNavigationBuffer.y != 0f))
            {
                if (this.mouseNavigationBuffer.sqrMagnitude < 0.0004f)
                {
                    float* singlePtr5 = &zero.x;
                    singlePtr5[0] += this.mouseNavigationBuffer.x;
                    float* singlePtr6 = &zero.z;
                    singlePtr6[0] += this.mouseNavigationBuffer.y;
                    this.mouseNavigationBuffer = Vector2.zero;
                }
                else
                {
                    Vector2 vector6 = (Vector2) (0.13f * this.mouseNavigationBuffer);
                    float* singlePtr7 = &zero.x;
                    singlePtr7[0] += vector6.x;
                    float* singlePtr8 = &zero.z;
                    singlePtr8[0] += vector6.y;
                    this.mouseNavigationBuffer -= vector6;
                }
            }
            if (!(zero != Vector3.zero))
            {
                this.UpdateCameraSmooth();
            }
            else
            {
                this.StopSmooth();
                float num7 = GetClickWorldPosition(true, false, false).z - position.z;
                if (zero.z > 0f)
                {
                    Vector3 vector7 = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A11);
                    zero.z = Mathf.Max(0f, Mathf.Min(zero.z, ((vector7.z - position.z) - num7) - 3f));
                }
                else if (zero.z < 0f)
                {
                    Vector3 vector8 = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A00);
                    zero.z = Mathf.Min(0f, Mathf.Max(zero.z, ((vector8.z - position.z) - num7) + 3f));
                }
                if (!World.GetActivePlane().area.horizontalWrap)
                {
                    if (zero.x > 0f)
                    {
                        Vector3 vector9 = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A11);
                        zero.x = Mathf.Max(0f, Mathf.Min(zero.x, (vector9.x - position.x) - 3f));
                    }
                    else if (zero.x < 0f)
                    {
                        Vector3 vector10 = HexCoordinates.HexToWorld3D(World.GetActivePlane().area.A00);
                        zero.x = Mathf.Min(0f, Mathf.Max(zero.x, (vector10.x - position.x) + 3f));
                    }
                }
                base.transform.Translate(zero);
            }
            if ((Input.mouseScrollDelta.y != 0f) && (!UIManager.IsOverUI() && this.IsMouseWithinGameWindow()))
            {
                this.zoom = Mathf.Clamp(Mathf.RoundToInt(this.zoom - Input.mouseScrollDelta.y), 0, 5);
                this.zoomOut = ((float) this.zoom) / 5f;
            }
            float targetMinHeight = this.targetMinHeight;
            float targetMaxHeight = this.targetMaxHeight;
            if (this.forcedHeight > 0f)
            {
                targetMinHeight = this.forcedHeight;
            }
            if (SettingsBlock.IsKey(Settings.KeyActions.UI_ZOOM_IN))
            {
                float num8 = (targetMaxHeight - targetMinHeight) / 6f;
                if (MathF.Abs((targetMinHeight + ((targetMaxHeight - targetMinHeight) * this.zoomOut)) - localPosition.y) < num8)
                {
                    this.zoom = Mathf.Clamp(Mathf.RoundToInt((float) (this.zoom + 1)), 0, 5);
                    this.zoomOut = ((float) this.zoom) / 5f;
                }
            }
            if (SettingsBlock.IsKey(Settings.KeyActions.UI_ZOOM_OUT))
            {
                float num9 = (targetMaxHeight - targetMinHeight) / 6f;
                if (MathF.Abs((targetMinHeight + ((targetMaxHeight - targetMinHeight) * this.zoomOut)) - localPosition.y) < num9)
                {
                    this.zoom = Mathf.Clamp(Mathf.RoundToInt((float) (this.zoom - 1)), 0, 5);
                    this.zoomOut = ((float) this.zoom) / 5f;
                }
            }
            float single1 = this.targetMaxHeight;
            float maxAngle = this.maxAngle;
            float minAngle = this.minAngle;
            float forcedHeight = targetMinHeight + ((this.targetMaxHeight - targetMinHeight) * this.zoomOut);
            if (this.forcedHeight > 0f)
            {
                forcedHeight = this.forcedHeight;
            }
            float f = forcedHeight - localPosition.y;
            if (Mathf.Abs(f) > 0.01f)
            {
                if (this.forcedHeight == 0f)
                {
                    f = Mathf.Clamp(f, -0.3f, 0.3f);
                }
                float num10 = Mathf.Clamp(localPosition.y + f, targetMinHeight, this.targetMaxHeight);
                this.SetCameraHeight(num10);
            }
            if (Input.GetMouseButtonUp(0) && !UIManager.IsOverUI())
            {
                Vector3i hexCoordAt = HexCoordinates.GetHexCoordAt(GetClickWorldPosition(false, true, false));
                WorldCode.Plane activePlane = World.GetActivePlane();
                Hex hexAt = activePlane.GetHexAt(hexCoordAt);
                if (activePlane != null)
                {
                    hexCoordAt = activePlane.area.KeepHorizontalInside(hexCoordAt);
                }
                if (hexAt == null)
                {
                    Debug.Log("Hex click: " + hexCoordAt.ToString());
                }
                else
                {
                    string[] textArray1 = new string[] { "Hex click: ", hexCoordAt.ToString(), " Terrain type: ", hexAt.GetTerrain().ToString(), " Terrain food: ", hexAt.GetFood().ToString() };
                    Debug.Log(string.Concat(textArray1));
                }
            }
        }
    }

    public static CameraController Get()
    {
        return instance;
    }

    public static Vector3 GetCameraPosition()
    {
        return Get().mainCamera.transform.parent.position;
    }

    public static Ray GetCameraRay(Vector2 position)
    {
        float x = position.x / ((float) Screen.width);
        return instance.mainCamera.ViewportPointToRay(new Vector3(x, position.y / ((float) Screen.height)));
    }

    public static Vector3 GetClickWorldPosition(bool flat, bool mousePosition, bool checkForUILock)
    {
        Vector2 vector;
        if (mousePosition)
        {
            vector = Input.mousePosition;
        }
        else
        {
            vector = new Vector2((float) (Screen.width / 2), (float) (Screen.height / 2));
        }
        return GetClickWorldPosition(vector, flat, checkForUILock);
    }

    public static Vector3 GetClickWorldPosition(Vector2 position, bool flat, bool checkForUILock)
    {
        RaycastHit hit;
        if (checkForUILock && UIManager.IsOverUI())
        {
            return -Vector3.one;
        }
        float maxDistance = 100f;
        Ray cameraRay = GetCameraRay(position);
        return ((flat || (!Physics.Raycast(cameraRay, out hit, maxDistance, 0x200) || (hit.point.y < 0f))) ? (!clickUnityPlane.Raycast(cameraRay, out maxDistance) ? Vector3.zero : cameraRay.GetPoint(maxDistance)) : hit.point);
    }

    public static unsafe Vector3 GetClickWorldPositionUV(Vector2 position, bool flat, bool checkForUILock)
    {
        float* singlePtr1 = &position.x;
        singlePtr1[0] *= Screen.width;
        float* singlePtr2 = &position.y;
        singlePtr2[0] *= Screen.height;
        return GetClickWorldPosition(position, flat, checkForUILock);
    }

    public static bool IsAlreadyTargeted(Vector3i pos)
    {
        float num = 0.01f;
        CameraController controller = Get();
        Vector3 vector = HexCoordinates.HexToWorld3D(pos);
        if (Vector3.Magnitude(controller.targetPos - vector) < num)
        {
            return true;
        }
        Vector3 vector2 = GetClickWorldPosition(true, false, false) - controller.transform.position;
        return (Vector3.Magnitude((controller.transform.position + vector2) - vector) < num);
    }

    private bool IsMouseWithinGameWindow()
    {
        Vector3 mousePosition = Input.mousePosition;
        return ((mousePosition.x >= 0f) && ((mousePosition.y >= 0f) && ((mousePosition.x <= Screen.width) && (mousePosition.y <= Screen.height))));
    }

    public static void MakeNextCenterInstant()
    {
        Get().makeNextCenterInstant = Time.realtimeSinceStartup;
    }

    public static void MoveInstantlyCameraPosition(Vector3 pos)
    {
        CameraController controller1 = Get();
        controller1.mainCamera.transform.parent.position = pos;
        controller1.StopSmooth();
    }

    private void SetCameraHeight(float value)
    {
        Vector3 localPosition = Camera.main.transform.localPosition;
        localPosition.y = value;
        float num = this.targetMaxHeight - this.targetMinHeight;
        float num2 = this.maxAngle - this.minAngle;
        float t = Mathf.Sin(((localPosition.y - this.targetMinHeight) / num) * 1.570796f);
        float f = Mathf.Lerp(this.minFOV, this.maxFOV, t) - this.mainCamera.fieldOfView;
        if (Mathf.Abs(f) > 0.01f)
        {
            f = Mathf.Clamp(f, -1f, 1f);
            this.mainCamera.fieldOfView += f;
        }
        Quaternion quaternion = Quaternion.Euler(new Vector3((t * num2) + this.minAngle, 0f, 0f));
        if (quaternion != Quaternion.identity)
        {
            this.mainCamera.transform.rotation = quaternion;
            this.mainCamera.transform.localPosition = localPosition;
        }
    }

    public void SetForcedZoom(bool enable, Vector3i pos)
    {
        this.forcedHeight = enable ? this.targetForcedHeight : 0f;
        if (enable)
        {
            this.SetCameraHeight(this.forcedHeight);
            CenterAt(pos, true, 0f);
        }
    }

    private void Start()
    {
        instance = this;
        this.mainCamera = Camera.main;
        Vector3 localPosition = this.mainCamera.transform.localPosition;
        localPosition.y = this.targetMinHeight;
        this.mainCamera.transform.localPosition = localPosition;
        this.mainCamera.fieldOfView = this.minFOV;
        this.mainCamera.transform.rotation = Quaternion.Euler(new Vector3(this.minAngle, 0f, 0f));
    }

    private void StopSmooth()
    {
        this.targetPos = Vector3.negativeInfinity;
        this.smoothVelocity = Vector3.zero;
    }

    public static void Target(Vector3 pos)
    {
        Get().targetPos = pos;
    }

    private void UpdateCameraSmooth()
    {
        if (this.targetPos.x != float.NegativeInfinity)
        {
            Vector3 vector = GetClickWorldPosition(true, false, false) - base.transform.position;
            Vector3 vector2 = Vector3.SmoothDamp(base.transform.position + vector, this.targetPos, ref this.smoothVelocity, this.smoothTime, this.smoothMaxSpeed);
            base.transform.position = vector2 - vector;
            if (this.smoothVelocity.magnitude <= this.smoothStopMagnitude)
            {
                this.StopSmooth();
            }
        }
    }
}

