// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// HutongGames.PlayMaker.Actions.CollisionEvent
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory(ActionCategory.Physics)]
[global::HutongGames.PlayMaker.Tooltip("Detect collisions between Game Objects that have RigidBody/Collider components.")]
public class CollisionEvent : FsmStateAction
{
    [global::HutongGames.PlayMaker.Tooltip("The GameObject to detect collisions on.")]
    public FsmOwnerDefault gameObject;

    [global::HutongGames.PlayMaker.Tooltip("The type of collision to detect.")]
    public CollisionType collision;

    [UIHint(UIHint.TagMenu)]
    [global::HutongGames.PlayMaker.Tooltip("Filter by Tag.")]
    public FsmString collideTag;

    [global::HutongGames.PlayMaker.Tooltip("Event to send if a collision is detected.")]
    public FsmEvent sendEvent;

    [UIHint(UIHint.Variable)]
    [global::HutongGames.PlayMaker.Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
    public FsmGameObject storeCollider;

    [UIHint(UIHint.Variable)]
    [global::HutongGames.PlayMaker.Tooltip("Store the force of the collision. NOTE: Use Get Collision Info to get more info about the collision.")]
    public FsmFloat storeForce;

    private PlayMakerProxyBase cachedProxy;

    public override void Reset()
    {
        this.gameObject = null;
        this.collision = CollisionType.OnCollisionEnter;
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
            case CollisionType.OnCollisionEnter:
                base.Fsm.HandleCollisionEnter = true;
                break;
            case CollisionType.OnCollisionStay:
                base.Fsm.HandleCollisionStay = true;
                break;
            case CollisionType.OnCollisionExit:
                base.Fsm.HandleCollisionExit = true;
                break;
            case CollisionType.OnControllerColliderHit:
                base.Fsm.HandleControllerColliderHit = true;
                break;
            case CollisionType.OnParticleCollision:
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
            case CollisionType.OnCollisionEnter:
                this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionEnter>(value);
                break;
            case CollisionType.OnCollisionStay:
                this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionStay>(value);
                break;
            case CollisionType.OnCollisionExit:
                this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerCollisionExit>(value);
                break;
            case CollisionType.OnParticleCollision:
                this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerParticleCollision>(value);
                break;
            case CollisionType.OnControllerColliderHit:
                this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerControllerColliderHit>(value);
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
            case CollisionType.OnCollisionEnter:
                this.cachedProxy.AddCollisionEventCallback(CollisionEnter);
                break;
            case CollisionType.OnCollisionStay:
                this.cachedProxy.AddCollisionEventCallback(CollisionStay);
                break;
            case CollisionType.OnCollisionExit:
                this.cachedProxy.AddCollisionEventCallback(CollisionExit);
                break;
            case CollisionType.OnParticleCollision:
                this.cachedProxy.AddParticleCollisionEventCallback(ParticleCollision);
                break;
            case CollisionType.OnControllerColliderHit:
                this.cachedProxy.AddControllerCollisionEventCallback(ControllerColliderHit);
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
            case CollisionType.OnCollisionEnter:
                this.cachedProxy.RemoveCollisionEventCallback(CollisionEnter);
                break;
            case CollisionType.OnCollisionStay:
                this.cachedProxy.RemoveCollisionEventCallback(CollisionStay);
                break;
            case CollisionType.OnCollisionExit:
                this.cachedProxy.RemoveCollisionEventCallback(CollisionExit);
                break;
            case CollisionType.OnParticleCollision:
                this.cachedProxy.RemoveParticleCollisionEventCallback(ParticleCollision);
                break;
            case CollisionType.OnControllerColliderHit:
                this.cachedProxy.RemoveControllerCollisionEventCallback(ControllerColliderHit);
                break;
            }
        }
    }

    private void StoreCollisionInfo(Collision collisionInfo)
    {
        this.storeCollider.Value = collisionInfo.gameObject;
        this.storeForce.Value = collisionInfo.relativeVelocity.magnitude;
    }

    public override void DoCollisionEnter(Collision collisionInfo)
    {
        if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
        {
            this.CollisionEnter(collisionInfo);
        }
    }

    public override void DoCollisionStay(Collision collisionInfo)
    {
        if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
        {
            this.CollisionStay(collisionInfo);
        }
    }

    public override void DoCollisionExit(Collision collisionInfo)
    {
        if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
        {
            this.CollisionExit(collisionInfo);
        }
    }

    public override void DoControllerColliderHit(ControllerColliderHit collisionInfo)
    {
        if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
        {
            this.ControllerColliderHit(collisionInfo);
        }
    }

    public override void DoParticleCollision(GameObject other)
    {
        if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
        {
            this.ParticleCollision(other);
        }
    }

    private void CollisionEnter(Collision collisionInfo)
    {
        if (this.collision == CollisionType.OnCollisionEnter && FsmStateAction.TagMatches(this.collideTag, collisionInfo))
        {
            this.StoreCollisionInfo(collisionInfo);
            base.Fsm.Event(this.sendEvent);
        }
    }

    private void CollisionStay(Collision collisionInfo)
    {
        if (this.collision == CollisionType.OnCollisionStay && FsmStateAction.TagMatches(this.collideTag, collisionInfo))
        {
            this.StoreCollisionInfo(collisionInfo);
            base.Fsm.Event(this.sendEvent);
        }
    }

    private void CollisionExit(Collision collisionInfo)
    {
        if (this.collision == CollisionType.OnCollisionExit && FsmStateAction.TagMatches(this.collideTag, collisionInfo))
        {
            this.StoreCollisionInfo(collisionInfo);
            base.Fsm.Event(this.sendEvent);
        }
    }

    private void ControllerColliderHit(ControllerColliderHit collisionInfo)
    {
        if (this.collision == CollisionType.OnControllerColliderHit && FsmStateAction.TagMatches(this.collideTag, collisionInfo))
        {
            if (this.storeCollider != null)
            {
                this.storeCollider.Value = collisionInfo.gameObject;
            }
            this.storeForce.Value = 0f;
            base.Fsm.Event(this.sendEvent);
        }
    }

    private void ParticleCollision(GameObject other)
    {
        if (this.collision == CollisionType.OnParticleCollision && FsmStateAction.TagMatches(this.collideTag, other))
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
        return ActionHelpers.CheckPhysicsSetup(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
    }
}
