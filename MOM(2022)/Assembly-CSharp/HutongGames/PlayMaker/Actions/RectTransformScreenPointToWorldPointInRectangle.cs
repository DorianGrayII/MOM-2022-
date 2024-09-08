// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// HutongGames.PlayMaker.Actions.RectTransformScreenPointToWorldPointInRectangle
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.EventSystems;

[ActionCategory("RectTransform")]
[global::HutongGames.PlayMaker.Tooltip("Transform a screen space point to a world position that is on the plane of the given RectTransform. Also check if the plane of the RectTransform is hit, regardless of whether the point is inside the rectangle.")]
public class RectTransformScreenPointToWorldPointInRectangle : FsmStateAction
{
    [RequiredField]
    [CheckForComponent(typeof(RectTransform))]
    [global::HutongGames.PlayMaker.Tooltip("The GameObject target.")]
    public FsmOwnerDefault gameObject;

    [global::HutongGames.PlayMaker.Tooltip("The screenPoint as a Vector2. Leave to none if you want to use the Vector3 alternative")]
    public FsmVector2 screenPointVector2;

    [global::HutongGames.PlayMaker.Tooltip("The screenPoint as a Vector3. Leave to none if you want to use the Vector2 alternative")]
    public FsmVector3 orScreenPointVector3;

    [global::HutongGames.PlayMaker.Tooltip("Define if screenPoint are expressed as normalized screen coordinates (0-1). Otherwise coordinates are in pixels.")]
    public bool normalizedScreenPoint;

    [global::HutongGames.PlayMaker.Tooltip("The Camera. For a RectTransform in a Canvas set to Screen Space - Overlay mode, the cam parameter should be set to null explicitly (default).\nLeave to none and the camera will be the one from EventSystem.current.camera")]
    [CheckForComponent(typeof(Camera))]
    public FsmGameObject camera;

    [global::HutongGames.PlayMaker.Tooltip("Repeat every frame")]
    public bool everyFrame;

    [ActionSection("Result")]
    [global::HutongGames.PlayMaker.Tooltip("Store the world Position of the screenPoint on the RectTransform Plane.")]
    [UIHint(UIHint.Variable)]
    public FsmVector3 worldPosition;

    [global::HutongGames.PlayMaker.Tooltip("True if the plane of the RectTransform is hit, regardless of whether the point is inside the rectangle.")]
    [UIHint(UIHint.Variable)]
    public FsmBool isHit;

    [global::HutongGames.PlayMaker.Tooltip("Event sent if the plane of the RectTransform is hit, regardless of whether the point is inside the rectangle.")]
    public FsmEvent hitEvent;

    [global::HutongGames.PlayMaker.Tooltip("Event sent if the plane of the RectTransform is NOT hit, regardless of whether the point is inside the rectangle.")]
    public FsmEvent noHitEvent;

    private RectTransform _rt;

    private Camera _camera;

    public override void Reset()
    {
        this.gameObject = null;
        this.screenPointVector2 = null;
        this.orScreenPointVector3 = new FsmVector3
        {
            UseVariable = true
        };
        this.normalizedScreenPoint = false;
        this.camera = new FsmGameObject
        {
            UseVariable = true
        };
        this.everyFrame = false;
        this.worldPosition = null;
        this.isHit = null;
        this.hitEvent = null;
        this.noHitEvent = null;
    }

    public override void OnEnter()
    {
        GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
        if (ownerDefaultTarget != null)
        {
            this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
        }
        if (!this.camera.IsNone)
        {
            this._camera = this.camera.Value.GetComponent<Camera>();
        }
        else
        {
            this._camera = EventSystem.current.GetComponent<Camera>();
        }
        this.DoCheck();
        if (!this.everyFrame)
        {
            base.Finish();
        }
    }

    public override void OnUpdate()
    {
        this.DoCheck();
    }

    private void DoCheck()
    {
        if (this._rt == null)
        {
            return;
        }
        Vector2 value = this.screenPointVector2.Value;
        if (!this.orScreenPointVector3.IsNone)
        {
            value.x = this.orScreenPointVector3.Value.x;
            value.y = this.orScreenPointVector3.Value.y;
        }
        if (this.normalizedScreenPoint)
        {
            value.x *= Screen.width;
            value.y *= Screen.height;
        }
        bool flag = false;
        flag = RectTransformUtility.ScreenPointToWorldPointInRectangle(this._rt, value, this._camera, out var worldPoint);
        this.worldPosition.Value = worldPoint;
        if (!this.isHit.IsNone)
        {
            this.isHit.Value = flag;
        }
        if (flag)
        {
            if (this.hitEvent != null)
            {
                base.Fsm.Event(this.hitEvent);
            }
        }
        else if (this.noHitEvent != null)
        {
            base.Fsm.Event(this.noHitEvent);
        }
    }
}
