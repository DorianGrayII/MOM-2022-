using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
[ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Detect collisions between Game Objects that have RigidBody2D/Collider2D components.")]
public class Collision2dEvent : FsmStateAction
{
        [Tooltip("The GameObject to detect collisions on.")]
    public FsmOwnerDefault gameObject;

        [Tooltip("The type of collision to detect.")]
    public Collision2DType collision;

    [UIHint(UIHint.TagMenu)]
        [Tooltip("Filter by Tag.")]
    public FsmString collideTag;

        [Tooltip("Event to send if a collision is detected.")]
    public FsmEvent sendEvent;

    [UIHint(UIHint.Variable)]
        [Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
    public FsmGameObject storeCollider;

    [UIHint(UIHint.Variable)]
        [Tooltip("Store the force of the collision. NOTE: Use Get Collision 2D Info to get more info about the collision.")]
    public FsmFloat storeForce;

    private PlayMakerProxyBase cachedProxy;

    public override void Reset()
    {
        this.collision = Collision2DType.OnCollisionEnter2D;
        this.collideTag = "";
        this.sendEvent = null;
        this.storeCollider = null;
        this.storeForce = null;
    }

    public override void OnPreprocess()
    {
        if (this.gameObject == null)
        {
            this.gameObject = new FsmOwnerDefault();
        }
        if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
        {
            switch (this.collision)
            {
            case Collision2DType.OnCollisionEnter2D:
                base.Fsm.HandleCollisionEnter2D = true;
                break;
            case Collision2DType.OnCollisionStay2D:
                base.Fsm.HandleCollisionStay2D = true;
                break;
            case Collision2DType.OnCollisionExit2D:
                base.Fsm.HandleCollisionExit2D = true;
                break;
            case Collision2DType.OnParticleCollision:
                base.Fsm.HandleParticleCollision = true;
                break;
            }
        }
        else
        {
            this.GetProxyComponent();
        }
    }

    public override void OnEnter()
    {
        if (this.gameObject.OwnerOption != 0)
        {
            if (this.cachedProxy == null)
            {
                this.GetProxyComponent();
            }
            this.AddCallback();
            this.gameObject.GameObject.OnChange += UpdateCallback;
        }
    }

    public override void OnExit()
    {
        if (this.gameObject.OwnerOption != 0)
        {
            this.RemoveCallback();
            this.gameObject.GameObject.OnChange -= UpdateCallback;
        }
    }

    private void UpdateCallback()
    {
        this.RemoveCallback();
        this.GetProxyComponent();
        this.AddCallback();
    }

    private void GetProxyComponent()
    {
        this.cachedProxy = null;
        GameObject value = this.gameObject.GameObject.Value;
        if (!(value == null))
        {
            switch (this.collision)
            {
            case Collision2DType.OnCollisionEnter2D:
                this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionEnter2D>(value);
                break;
            case Collision2DType.OnCollisionStay2D:
                this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionStay2D>(value);
                break;
            case Collision2DType.OnCollisionExit2D:
                this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionExit2D>(value);
                break;
            case Collision2DType.OnParticleCollision:
                this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerParticleCollision>(value);
                break;
            }
        }
    }

    private void AddCallback()
    {
        if (!(this.cachedProxy == null))
        {
            switch (this.collision)
            {
            case Collision2DType.OnCollisionEnter2D:
                this.cachedProxy.AddCollision2DEventCallback(CollisionEnter2D);
                break;
            case Collision2DType.OnCollisionStay2D:
                this.cachedProxy.AddCollision2DEventCallback(CollisionStay2D);
                break;
            case Collision2DType.OnCollisionExit2D:
                this.cachedProxy.AddCollision2DEventCallback(CollisionExit2D);
                break;
            case Collision2DType.OnParticleCollision:
                this.cachedProxy.AddParticleCollisionEventCallback(ParticleCollision);
                break;
            }
        }
    }

    private void RemoveCallback()
    {
        if (!(this.cachedProxy == null))
        {
            switch (this.collision)
            {
            case Collision2DType.OnCollisionEnter2D:
                this.cachedProxy.RemoveCollision2DEventCallback(CollisionEnter2D);
                break;
            case Collision2DType.OnCollisionStay2D:
                this.cachedProxy.RemoveCollision2DEventCallback(CollisionStay2D);
                break;
            case Collision2DType.OnCollisionExit2D:
                this.cachedProxy.RemoveCollision2DEventCallback(CollisionExit2D);
                break;
            case Collision2DType.OnParticleCollision:
                this.cachedProxy.RemoveParticleCollisionEventCallback(ParticleCollision);
                break;
            }
        }
    }

    private void StoreCollisionInfo(Collision2D collisionInfo)
    {
        this.storeCollider.Value = collisionInfo.gameObject;
        this.storeForce.Value = collisionInfo.relativeVelocity.magnitude;
    }

    public override void DoCollisionEnter2D(Collision2D collisionInfo)
    {
        if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
        {
            this.CollisionEnter2D(collisionInfo);
        }
    }

    public override void DoCollisionStay2D(Collision2D collisionInfo)
    {
        if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
        {
            this.CollisionStay2D(collisionInfo);
        }
    }

    public override void DoCollisionExit2D(Collision2D collisionInfo)
    {
        if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
        {
            this.CollisionExit2D(collisionInfo);
        }
    }

    public override void DoParticleCollision(GameObject other)
    {
        if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
        {
            this.ParticleCollision(other);
        }
    }

    private void CollisionEnter2D(Collision2D collisionInfo)
    {
        if (this.collision == Collision2DType.OnCollisionEnter2D && FsmStateAction.TagMatches(this.collideTag, collisionInfo))
        {
            this.StoreCollisionInfo(collisionInfo);
            base.Fsm.Event(this.sendEvent);
        }
    }

    private void CollisionStay2D(Collision2D collisionInfo)
    {
        if (this.collision == Collision2DType.OnCollisionStay2D && FsmStateAction.TagMatches(this.collideTag, collisionInfo))
        {
            this.StoreCollisionInfo(collisionInfo);
            base.Fsm.Event(this.sendEvent);
        }
    }

    private void CollisionExit2D(Collision2D collisionInfo)
    {
        if (this.collision == Collision2DType.OnCollisionExit2D && FsmStateAction.TagMatches(this.collideTag, collisionInfo))
        {
            this.StoreCollisionInfo(collisionInfo);
            base.Fsm.Event(this.sendEvent);
        }
    }

    private void ParticleCollision(GameObject other)
    {
        if (this.collision == Collision2DType.OnParticleCollision && FsmStateAction.TagMatches(this.collideTag, other))
        {
            if (this.storeCollider != null)
            {
                this.storeCollider.Value = other;
            }
            this.storeForce.Value = 0f;
            base.Fsm.Event(this.sendEvent);
        }
    }

    public override string ErrorCheck()
    {
        return ActionHelpers.CheckPhysics2dSetup(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
        }
    }
}
