using System;
using UnityEngine;

public class FlockChild : MonoBehaviour
{
    [HideInInspector]
    public FlockController _spawner;
    [HideInInspector]
    public Vector3 _wayPoint;
    public float _speed;
    [HideInInspector]
    public bool _dived = true;
    [HideInInspector]
    public float _stuckCounter;
    [HideInInspector]
    public float _damping;
    [HideInInspector]
    public bool _soar = true;
    [HideInInspector]
    public bool _landing;
    [HideInInspector]
    public float _targetSpeed;
    [HideInInspector]
    public bool _move = true;
    public GameObject _model;
    public Transform _modelT;
    [HideInInspector]
    public float _avoidValue;
    [HideInInspector]
    public float _avoidDistance;
    private float _soarTimer;
    private bool _instantiated;
    private static int _updateNextSeed;
    private int _updateSeed = -1;
    [HideInInspector]
    public bool _avoid = true;
    public Transform _thisT;
    public Vector3 _landingPosOffset;

    public void animationSpeed()
    {
        foreach (AnimationState state in this._model.GetComponent<Animation>())
        {
            state.speed = (this._dived || this._landing) ? this._spawner._maxAnimationSpeed : UnityEngine.Random.Range(this._spawner._minAnimationSpeed, this._spawner._maxAnimationSpeed);
        }
    }

    public unsafe bool Avoidance()
    {
        RaycastHit hitInfo = new RaycastHit();
        Vector3 forward = this._modelT.forward;
        bool flag = false;
        Quaternion identity = Quaternion.identity;
        Vector3 zero = Vector3.zero;
        Vector3 position = Vector3.zero;
        position = this._thisT.position;
        identity = this._thisT.rotation;
        zero = this._thisT.rotation.eulerAngles;
        if (Physics.Raycast(this._thisT.position, forward + (this._modelT.right * this._avoidValue), out hitInfo, this._avoidDistance, (int) this._spawner._avoidanceMask))
        {
            float* singlePtr1 = &zero.y;
            singlePtr1[0] -= (this._spawner._birdAvoidHorizontalForce * this._spawner._newDelta) * this._damping;
            identity.eulerAngles = zero;
            this._thisT.rotation = identity;
            flag = true;
        }
        else if (Physics.Raycast(this._thisT.position, forward + (this._modelT.right * -this._avoidValue), out hitInfo, this._avoidDistance, (int) this._spawner._avoidanceMask))
        {
            float* singlePtr2 = &zero.y;
            singlePtr2[0] += (this._spawner._birdAvoidHorizontalForce * this._spawner._newDelta) * this._damping;
            identity.eulerAngles = zero;
            this._thisT.rotation = identity;
            flag = true;
        }
        if (this._spawner._birdAvoidDown && (!this._landing && Physics.Raycast(this._thisT.position, -Vector3.up, out hitInfo, this._avoidDistance, (int) this._spawner._avoidanceMask)))
        {
            float* singlePtr3 = &zero.x;
            singlePtr3[0] -= (this._spawner._birdAvoidVerticalForce * this._spawner._newDelta) * this._damping;
            identity.eulerAngles = zero;
            this._thisT.rotation = identity;
            float* singlePtr4 = &position.y;
            singlePtr4[0] += (this._spawner._birdAvoidVerticalForce * this._spawner._newDelta) * 0.01f;
            this._thisT.position = position;
            flag = true;
        }
        else if (this._spawner._birdAvoidUp && (!this._landing && Physics.Raycast(this._thisT.position, Vector3.up, out hitInfo, this._avoidDistance, (int) this._spawner._avoidanceMask)))
        {
            float* singlePtr5 = &zero.x;
            singlePtr5[0] += (this._spawner._birdAvoidVerticalForce * this._spawner._newDelta) * this._damping;
            identity.eulerAngles = zero;
            this._thisT.rotation = identity;
            float* singlePtr6 = &position.y;
            singlePtr6[0] -= (this._spawner._birdAvoidVerticalForce * this._spawner._newDelta) * 0.01f;
            this._thisT.position = position;
            flag = true;
        }
        return flag;
    }

    public void CheckForDistanceToWaypoint()
    {
        if (!this._landing && ((this._thisT.position - this._wayPoint).magnitude < (this._spawner._waypointDistance + this._stuckCounter)))
        {
            this.Wander(0f);
            this._stuckCounter = 0f;
        }
        else if (!this._landing)
        {
            this._stuckCounter += this._spawner._newDelta;
        }
        else
        {
            this._stuckCounter = 0f;
        }
    }

    public unsafe void Dive()
    {
        if (this._spawner._soarAnimation != null)
        {
            this._model.GetComponent<Animation>().CrossFade(this._spawner._soarAnimation, 1.5f);
        }
        else
        {
            foreach (AnimationState state in this._model.GetComponent<Animation>())
            {
                if (this._thisT.position.y < (this._wayPoint.y + 25f))
                {
                    state.speed = 0.1f;
                }
            }
        }
        this._wayPoint = this.findWaypoint();
        float* singlePtr1 = &this._wayPoint.y;
        singlePtr1[0] -= this._spawner._diveValue;
        this._dived = true;
    }

    public void FindRequiredComponents()
    {
        if (this._thisT == null)
        {
            this._thisT = base.transform;
        }
        if (this._model == null)
        {
            this._model = this._thisT.Find("Model").gameObject;
        }
        if (this._modelT == null)
        {
            this._modelT = this._model.transform;
        }
    }

    public Vector3 findWaypoint()
    {
        Vector3 zero = Vector3.zero;
        zero.x = UnityEngine.Random.Range(-this._spawner._spawnSphere, this._spawner._spawnSphere) + this._spawner._posBuffer.x;
        zero.z = UnityEngine.Random.Range(-this._spawner._spawnSphereDepth, this._spawner._spawnSphereDepth) + this._spawner._posBuffer.z;
        zero.y = UnityEngine.Random.Range(-this._spawner._spawnSphereHeight, this._spawner._spawnSphereHeight) + this._spawner._posBuffer.y;
        return zero;
    }

    public void Flap()
    {
        if (this._move)
        {
            if (this._model != null)
            {
                this._model.GetComponent<Animation>().CrossFade(this._spawner._flapAnimation, 0.5f);
            }
            this._soar = false;
            this.animationSpeed();
            this._wayPoint = this.findWaypoint();
            this._dived = false;
        }
    }

    public void InitAvoidanceValues()
    {
        this._avoidValue = UnityEngine.Random.Range((float) 0.3f, (float) 0.1f);
        if (this._spawner._birdAvoidDistanceMax != this._spawner._birdAvoidDistanceMin)
        {
            this._avoidDistance = UnityEngine.Random.Range(this._spawner._birdAvoidDistanceMax, this._spawner._birdAvoidDistanceMin);
        }
        else
        {
            this._avoidDistance = this._spawner._birdAvoidDistanceMin;
        }
    }

    public void LimitRotationOfModel()
    {
        Quaternion identity = Quaternion.identity;
        Vector3 zero = Vector3.zero;
        identity = this._modelT.localRotation;
        zero = identity.eulerAngles;
        if ((((this._soar && this._spawner._flatSoar) || (this._spawner._flatFly && !this._soar)) && (this._wayPoint.y > this._thisT.position.y)) || this._landing)
        {
            zero.x = Mathf.LerpAngle(this._modelT.localEulerAngles.x, -this._thisT.localEulerAngles.x, this._spawner._newDelta * 1.75f);
            identity.eulerAngles = zero;
            this._modelT.localRotation = identity;
        }
        else
        {
            zero.x = Mathf.LerpAngle(this._modelT.localEulerAngles.x, 0f, this._spawner._newDelta * 1.75f);
            identity.eulerAngles = zero;
            this._modelT.localRotation = identity;
        }
    }

    public void OnDisable()
    {
        base.CancelInvoke();
        this._spawner._activeChildren--;
    }

    public void OnEnable()
    {
        if (this._instantiated)
        {
            this._spawner._activeChildren++;
            if (this._landing)
            {
                this._model.GetComponent<Animation>().Play(this._spawner._idleAnimation);
            }
            else
            {
                this._model.GetComponent<Animation>().Play(this._spawner._flapAnimation);
            }
        }
    }

    public void RandomizeStartAnimationFrame()
    {
        foreach (AnimationState state in this._model.GetComponent<Animation>())
        {
            state.time = UnityEngine.Random.value * state.length;
        }
    }

    public void RotationBasedOnWaypointOrAvoidance()
    {
        Vector3 forward = this._wayPoint - this._thisT.position;
        if ((this._targetSpeed > -1f) && (forward != Vector3.zero))
        {
            Quaternion b = Quaternion.LookRotation(forward);
            this._thisT.rotation = Quaternion.Slerp(this._thisT.rotation, b, this._spawner._newDelta * this._damping);
        }
        if (this._spawner._childTriggerPos && ((this._thisT.position - this._spawner._posBuffer).magnitude < 1f))
        {
            this._spawner.SetFlockRandomPosition();
        }
        this._speed = Mathf.Lerp(this._speed, this._targetSpeed, this._spawner._newDelta * 2.5f);
        if (this._move)
        {
            this._thisT.position += (this._thisT.forward * this._speed) * this._spawner._newDelta;
            if (this._avoid && this._spawner._birdAvoid)
            {
                this.Avoidance();
            }
        }
    }

    public void SetRandomMode()
    {
        base.CancelInvoke("SetRandomMode");
        if (!this._dived && (UnityEngine.Random.value < this._spawner._soarFrequency))
        {
            this.Soar();
        }
        else if (!this._dived && (UnityEngine.Random.value < this._spawner._diveFrequency))
        {
            this.Dive();
        }
        else
        {
            this.Flap();
        }
    }

    public void SetRandomScale()
    {
        float x = UnityEngine.Random.Range(this._spawner._minScale, this._spawner._maxScale);
        this._thisT.localScale = new Vector3(x, x, x);
    }

    public void Soar()
    {
        if (this._move)
        {
            this._model.GetComponent<Animation>().CrossFade(this._spawner._soarAnimation, 1.5f);
            this._wayPoint = this.findWaypoint();
            this._soar = true;
        }
    }

    public void SoarTimeLimit()
    {
        if (this._soar && (this._spawner._soarMaxTime > 0f))
        {
            if (this._soarTimer > this._spawner._soarMaxTime)
            {
                this.Flap();
                this._soarTimer = 0f;
            }
            else
            {
                this._soarTimer += this._spawner._newDelta;
            }
        }
    }

    public void Start()
    {
        this.FindRequiredComponents();
        this.Wander(0f);
        this.SetRandomScale();
        this._thisT.position = this.findWaypoint();
        this.RandomizeStartAnimationFrame();
        this.InitAvoidanceValues();
        this._speed = this._spawner._minSpeed;
        this._spawner._activeChildren++;
        this._instantiated = true;
        if (this._spawner._updateDivisor > 1)
        {
            int num = this._spawner._updateDivisor - 1;
            _updateNextSeed++;
            this._updateSeed = _updateNextSeed;
            _updateNextSeed = _updateNextSeed % num;
        }
    }

    public void Update()
    {
        if ((this._spawner._updateDivisor <= 1) || (this._spawner._updateCounter == this._updateSeed))
        {
            this.SoarTimeLimit();
            this.CheckForDistanceToWaypoint();
            this.RotationBasedOnWaypointOrAvoidance();
            this.LimitRotationOfModel();
        }
    }

    public void Wander(float delay)
    {
        if (!this._landing)
        {
            this._damping = UnityEngine.Random.Range(this._spawner._minDamping, this._spawner._maxDamping);
            this._targetSpeed = UnityEngine.Random.Range(this._spawner._minSpeed, this._spawner._maxSpeed);
            base.Invoke("SetRandomMode", delay);
        }
    }
}

