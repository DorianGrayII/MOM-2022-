using System;
using System.Collections.Generic;
using UnityEngine;

public class FlockController : MonoBehaviour
{
    public FlockChild _childPrefab;
    public int _childAmount = 250;
    public bool _slowSpawn;
    public float _spawnSphere = 3f;
    public float _spawnSphereHeight = 3f;
    public float _spawnSphereDepth = -1f;
    public float _minSpeed = 6f;
    public float _maxSpeed = 10f;
    public float _minScale = 0.7f;
    public float _maxScale = 1f;
    public float _soarFrequency;
    public string _soarAnimation = "Soar";
    public string _flapAnimation = "Flap";
    public string _idleAnimation = "Idle";
    public float _diveValue = 7f;
    public float _diveFrequency = 0.5f;
    public float _minDamping = 1f;
    public float _maxDamping = 2f;
    public float _waypointDistance = 1f;
    public float _minAnimationSpeed = 2f;
    public float _maxAnimationSpeed = 4f;
    public float _randomPositionTimer = 10f;
    public float _positionSphere = 25f;
    public float _positionSphereHeight = 25f;
    public float _positionSphereDepth = -1f;
    public bool _childTriggerPos;
    public bool _forceChildWaypoints;
    public float _forcedRandomDelay = 1.5f;
    public bool _flatFly;
    public bool _flatSoar;
    public bool _birdAvoid;
    public int _birdAvoidHorizontalForce = 0x3e8;
    public bool _birdAvoidDown;
    public bool _birdAvoidUp;
    public int _birdAvoidVerticalForce = 300;
    public float _birdAvoidDistanceMax = 4.5f;
    public float _birdAvoidDistanceMin = 5f;
    public float _soarMaxTime;
    public LayerMask _avoidanceMask = -1;
    public List<FlockChild> _roamers;
    public Vector3 _posBuffer;
    public int _updateDivisor = 1;
    public float _newDelta;
    public int _updateCounter;
    public float _activeChildren;
    public bool _groupChildToNewTransform;
    public Transform _groupTransform;
    public string _groupName = "";
    public bool _groupChildToFlock;
    public Vector3 _startPosOffset;
    public Transform _thisT;

    public void AddChild(int amount)
    {
        if (this._groupChildToNewTransform)
        {
            this.InstantiateGroup();
        }
        for (int i = 0; i < amount; i++)
        {
            FlockChild item = Instantiate<FlockChild>(this._childPrefab);
            item._spawner = this;
            this._roamers.Add(item);
            this.AddChildToParent(item.transform);
        }
    }

    public void AddChildToParent(Transform obj)
    {
        if (this._groupChildToFlock)
        {
            obj.parent = base.transform;
        }
        else if (this._groupChildToNewTransform)
        {
            obj.parent = this._groupTransform;
        }
    }

    public void destroyBirds()
    {
        for (int i = 0; i < this._roamers.Count; i++)
        {
            Destroy(this._roamers[i].gameObject);
        }
        this._childAmount = 0;
        this._roamers.Clear();
    }

    public void InstantiateGroup()
    {
        if (this._groupTransform == null)
        {
            GameObject obj2 = new GameObject();
            this._groupTransform = obj2.transform;
            this._groupTransform.position = this._thisT.position;
            if (this._groupName != "")
            {
                obj2.name = this._groupName;
            }
            else
            {
                obj2.name = this._thisT.name + " Fish Container";
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (this._thisT == null)
        {
            this._thisT = base.transform;
        }
        if (!Application.isPlaying && (this._posBuffer != (this._thisT.position + this._startPosOffset)))
        {
            this._posBuffer = this._thisT.position + this._startPosOffset;
        }
        if (this._positionSphereDepth == -1f)
        {
            this._positionSphereDepth = this._positionSphere;
        }
        if (this._spawnSphereDepth == -1f)
        {
            this._spawnSphereDepth = this._spawnSphere;
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(this._posBuffer, new Vector3(this._spawnSphere * 2f, this._spawnSphereHeight * 2f, this._spawnSphereDepth * 2f));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(this._thisT.position, new Vector3((this._positionSphere * 2f) + (this._spawnSphere * 2f), (this._positionSphereHeight * 2f) + (this._spawnSphereHeight * 2f), (this._positionSphereDepth * 2f) + (this._spawnSphereDepth * 2f)));
    }

    public void RemoveChild(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            this._roamers.RemoveAt(this._roamers.Count - 1);
            Destroy(this._roamers[this._roamers.Count - 1].gameObject);
        }
    }

    public void SetFlockRandomPosition()
    {
        Vector3 zero = Vector3.zero;
        zero.x = UnityEngine.Random.Range(-this._positionSphere, this._positionSphere) + this._thisT.position.x;
        zero.z = UnityEngine.Random.Range(-this._positionSphereDepth, this._positionSphereDepth) + this._thisT.position.z;
        zero.y = UnityEngine.Random.Range(-this._positionSphereHeight, this._positionSphereHeight) + this._thisT.position.y;
        this._posBuffer = zero;
        if (this._forceChildWaypoints)
        {
            for (int i = 0; i < this._roamers.Count; i++)
            {
                this._roamers[i].Wander(UnityEngine.Random.value * this._forcedRandomDelay);
            }
        }
    }

    public void Start()
    {
        this._thisT = base.transform;
        if (this._positionSphereDepth == -1f)
        {
            this._positionSphereDepth = this._positionSphere;
        }
        if (this._spawnSphereDepth == -1f)
        {
            this._spawnSphereDepth = this._spawnSphere;
        }
        this._posBuffer = this._thisT.position + this._startPosOffset;
        if (!this._slowSpawn)
        {
            this.AddChild(this._childAmount);
        }
        if (this._randomPositionTimer > 0f)
        {
            base.InvokeRepeating("SetFlockRandomPosition", this._randomPositionTimer, this._randomPositionTimer);
        }
    }

    public void Update()
    {
        if (this._activeChildren > 0f)
        {
            if (this._updateDivisor <= 1)
            {
                this._newDelta = Time.deltaTime;
            }
            else
            {
                this._updateCounter++;
                this._updateCounter = this._updateCounter % this._updateDivisor;
                this._newDelta = Time.deltaTime * this._updateDivisor;
            }
        }
        this.UpdateChildAmount();
    }

    public void UpdateChildAmount()
    {
        if ((this._childAmount >= 0) && (this._childAmount < this._roamers.Count))
        {
            this.RemoveChild(1);
        }
        else if (this._childAmount > this._roamers.Count)
        {
            this.AddChild(1);
        }
    }
}

