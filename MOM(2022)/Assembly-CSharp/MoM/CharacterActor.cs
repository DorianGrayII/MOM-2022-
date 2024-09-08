// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.CharacterActor
using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using MOM;
using UnityEngine;

public class CharacterActor : Actor
{
    public Formation parent;

    public float objectTranslationSpeed = 2f;

    public float animationSpeedMultiplier = 1.5f;

    private Coroutine currentAction;

    private Coroutine buildDelayed;

    private float idleAnimationSpeedVariable = 0.2f;

    private Animator animator;

    public List<Renderer> renderers;

    private bool startMovement;

    private bool setAnimatorState = true;

    private float originalSpeed;

    private bool modelVisible;

    private bool rangedTriggerInitialized;

    private bool noPitch;

    private bool isHit;

    private Vector3 rangedAttackVector;

    private Callback targetHit;

    private bool isActivelyBuilding;

    private Queue<IEnumerator> animationStack = new Queue<IEnumerator>();

    private List<MovementData> movementDataPack = new List<MovementData>();

    private int movementDataIndexer;

    public static CharacterActor CreateCharacter(string name, Formation parent)
    {
        GameObject gameObject = AssetManager.Get<GameObject>(name);
        if (gameObject == null)
        {
            Debug.LogWarning("Model " + name + " not found in asset manager for CharacterActor instantiation");
            gameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            gameObject.transform.localScale = Vector3.one * 0.2f;
        }
        GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject, parent.gameObject.transform);
        if (gameObject2 == null)
        {
            return null;
        }
        if (gameObject2.transform.childCount != 1)
        {
            if (gameObject2.transform.childCount == 0)
            {
                Debug.LogWarning("Character " + name + " Is empty, possibly placeholder!");
            }
            else
            {
                Debug.LogError("Character " + name + " have different than expected number of wrapped children! Ask A'vee!");
            }
        }
        CharacterActor characterActor = gameObject2.AddComponent<CharacterActor>();
        characterActor.modelVisible = true;
        characterActor.parent = parent;
        characterActor.plane = parent.owner.GetPlane();
        characterActor.isActivelyBuilding = parent.isActivelyBuilding;
        return characterActor;
    }

    private void Start()
    {
        Animator[] componentsInChildren = base.GetComponentsInChildren<Animator>(includeInactive: true);
        this.animator = ((componentsInChildren.Length != 0) ? componentsInChildren[0] : null);
        if (this.animator != null && !this.animator.isInitialized)
        {
            this.animator.Rebind();
        }
        Renderer[] componentsInChildren2 = base.GetComponentsInChildren<Renderer>(includeInactive: true);
        if (this.animator != null)
        {
            this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            this.animator.enabled = this.setAnimatorState;
            if (this.originalSpeed == 0f)
            {
                this.originalSpeed = this.animator.speed;
                this.animator.speed = 1f + global::UnityEngine.Random.Range(0f - this.idleAnimationSpeedVariable, this.idleAnimationSpeedVariable);
            }
        }
        this.renderers = new List<Renderer>();
        Renderer[] array = componentsInChildren2;
        foreach (Renderer renderer in array)
        {
            if (this.parent != null)
            {
                renderer.enabled = this.parent.isVisible;
            }
            this.renderers.Add(renderer);
        }
        if (this.parent.direction == Vector3.zero)
        {
            this.parent.direction = Vector3.back;
        }
        this.LookAt(this.parent.direction, 1f);
        this.CopyLeaderDirection();
    }

    public void CopyLeaderDirection()
    {
        List<CharacterActor> list = this.parent?.GetCharacterActors();
        if ((list?.Count ?? 0) > 0)
        {
            base.transform.rotation = list[0].transform.rotation;
        }
    }

    public void UpdateVisibility()
    {
        if (this.parent.isVisible == this.modelVisible)
        {
            return;
        }
        foreach (Renderer renderer in this.renderers)
        {
            renderer.enabled = this.parent.isVisible;
        }
        this.modelVisible = this.parent.isVisible;
    }

    public void SetAnimatorState(bool value)
    {
        this.setAnimatorState = value;
        if (this.animator != null)
        {
            this.animator.enabled = value;
        }
    }

    public void AnimateViaPath(List<Vector3> path, float laziness)
    {
        this.movementDataIndexer++;
        MovementData movementData = new MovementData();
        movementData.path = path;
        movementData.laziness = laziness;
        movementData.ID = this.movementDataIndexer;
        this.movementDataPack.Add(movementData);
        this.NotBuild();
        this.AddAnimationToStack(this.Movement(movementData.ID));
    }

    public void StopMovementViaPath()
    {
        this.movementDataPack.Clear();
    }

    private IEnumerator Movement(int movementID)
    {
        yield return null;
        MHEventSystem.TriggerEvent<CharacterActor>(this, "Start");
        if (this.animator != null)
        {
            this.animator.SetBool("Move", value: true);
            this.animator.speed = this.originalSpeed * this.animationSpeedMultiplier * (float)Settings.GetData().GetAnimationSpeed();
            while (!this.animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
            {
                yield return null;
            }
        }
        MovementData pathData = this.movementDataPack.Find((MovementData o) => o.ID == movementID);
        if (pathData != null)
        {
            pathData.processingNow = true;
            List<Vector3> navPath = pathData.path;
            int pathIndex = 1;
            while (pathIndex < navPath.Count)
            {
                yield return null;
                float num = Time.deltaTime * this.objectTranslationSpeed * (float)Settings.GetData().GetAnimationSpeed();
                bool flag = false;
                if (this.movementDataPack.Find((MovementData o) => o.ID == movementID) == null)
                {
                    if (this.animator != null)
                    {
                        this.animator.SetBool("Move", value: false);
                    }
                    this.animator.speed = 1f + global::UnityEngine.Random.Range(0f - this.idleAnimationSpeedVariable, this.idleAnimationSpeedVariable);
                    this.currentAction = null;
                    yield break;
                }
                if (this.parent?.owner is global::MOM.Group group && group.GetOwnerID() == PlayerWizard.HumanID() && FSMSelectionManager.Get()?.GetSelectedGroup() != group)
                {
                    num = 1000f;
                    flag = true;
                }
                if (Settings.GetData().GetAnimationSpeed() > 100)
                {
                    num = 1000f;
                    flag = true;
                }
                Vector3 localPosition = base.transform.localPosition;
                localPosition.y = 0f;
                Vector3 vector = localPosition;
                Vector3 vector2 = Vector3.zero;
                _ = Vector3.zero;
                while (num > 0f)
                {
                    if (pathIndex < navPath.Count)
                    {
                        Vector3 vector3 = navPath[pathIndex];
                        vector2 = vector3 - vector;
                        float magnitude = vector2.magnitude;
                        if (true)
                        {
                            if (magnitude <= num)
                            {
                                vector = vector3;
                                num -= magnitude;
                                pathIndex++;
                            }
                            else
                            {
                                vector += vector2.normalized * num;
                                num = 0f;
                            }
                        }
                        continue;
                    }
                    num = 0f;
                    break;
                }
                if (!flag && !this.parent.MoveAllowed(this, vector))
                {
                    vector = Vector3.Lerp(localPosition, vector, 0.5f);
                }
                base.SetWorldHeightPosition(vector, allowUnderwater: false);
                vector2.y = 0f;
                if (vector2 != Vector3.zero)
                {
                    this.LookAt(vector2, 0.3f);
                }
                if (num > 0f)
                {
                    break;
                }
            }
        }
        if (pathData != null && this.movementDataPack.Count > 0 && this.movementDataPack.Contains(pathData))
        {
            this.movementDataPack.Remove(pathData);
        }
        this.parent.MovementFinished();
        MHEventSystem.TriggerEvent<CharacterActor>(this, "End");
        if (this.animator != null)
        {
            this.animator.SetBool("Move", value: false);
            while (!this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                yield return null;
            }
        }
        this.animator.speed = 1f + global::UnityEngine.Random.Range(0f - this.idleAnimationSpeedVariable, this.idleAnimationSpeedVariable);
        this.currentAction = null;
    }

    public void ResetLookAt()
    {
        if (!(this.parent == null))
        {
            this.LookAt(this.parent.direction, 1f);
        }
    }

    private void LookAt(Vector3 direction, float stepShare)
    {
        Quaternion b = Quaternion.identity;
        if (direction != Vector3.zero)
        {
            b = Quaternion.LookRotation(direction);
        }
        Quaternion rotation = Quaternion.Lerp(base.transform.rotation, b, stepShare);
        base.transform.rotation = rotation;
    }

    public bool IsAnimating()
    {
        if (this.animationStack.Count <= 0)
        {
            return this.currentAction != null;
        }
        return true;
    }

    public bool IsAnimatingNotYetHit()
    {
        if (this.isHit || this.animationStack.Count <= 0)
        {
            return this.currentAction != null;
        }
        return true;
    }

    public void PlayDeath()
    {
        this.noPitch = this.parent.GetCharacterActors().Count == 1;
        this.AddAnimationToStack(this.DieAnim());
    }

    private void PlayBuildIfNeeded()
    {
        if (this.animator != null && this.isActivelyBuilding)
        {
            this.animator.SetBool("Build", value: true);
        }
        else if (this.isActivelyBuilding && this.buildDelayed == null)
        {
            this.buildDelayed = base.StartCoroutine(this.StartBuild());
        }
    }

    private void NotBuild()
    {
        if (this.animator != null)
        {
            this.animator.SetBool("Build", value: false);
        }
    }

    public void SetAcivelyBuilding(bool value)
    {
        this.isActivelyBuilding = value;
        if (!this.IsAnimating())
        {
            if (value)
            {
                this.PlayBuildIfNeeded();
            }
            else
            {
                this.NotBuild();
            }
        }
    }

    public void PlayAttackByVector(Vector3 vector)
    {
        this.noPitch = this.parent.GetCharacterActors().Count == 1;
        this.AddAnimationToStack(this.Animate("AttackMelee", this.parent.source.dbSource.Get().audio?.attackMelee, vector));
    }

    public void PlayRangedAttackByVector(Vector3 vector, Callback targetHit)
    {
        this.targetHit = targetHit;
        this.rangedAttackVector = vector;
        vector.y = 0f;
        AnimatorTrigger animatorTrigger = this.animator?.gameObject.GetComponent<AnimatorTrigger>();
        if (animatorTrigger != null && !this.rangedTriggerInitialized)
        {
            animatorTrigger.AddListener(StartProjectileAnimation);
            this.rangedTriggerInitialized = true;
        }
        if (animatorTrigger == null)
        {
            targetHit(this);
        }
        this.AddAnimationToStack(this.Animate("AttackRanged", this.parent.source.dbSource.Get().audio?.attackRanged, vector));
    }

    public void PlayMagicAttack()
    {
        this.AddAnimationToStack(this.Animate("AttackMagic", null));
    }

    public void PlayGetHit()
    {
        this.noPitch = this.parent.GetCharacterActors().Count == 1;
        this.AddAnimationToStack(this.Animate("GetHit", this.parent.source.dbSource.Get().audio?.getHit));
    }

    private IEnumerator DieAnim()
    {
        this.isHit = true;
        if (this.noPitch)
        {
            AudioLibrary.RequestSFX(this.parent.source.dbSource.Get().audio?.getHit);
            AudioLibrary.RequestSFX(this.parent.source.dbSource.Get().audio?.die);
        }
        else
        {
            AudioLibrary.RequestSFX(this.parent.source.dbSource.Get().audio?.getHit, 0.3f, 200f);
            AudioLibrary.RequestSFX(this.parent.source.dbSource.Get().audio?.die, 0.3f, 200f);
        }
        this.animator.SetBool("Die", value: true);
        Subrace subrace = this.parent.source?.dbSource?.Get();
        if (subrace != null && !subrace.fixedDeathDir)
        {
            Vector3 randomDir = new Vector3(global::UnityEngine.Random.Range(-1f, 1f), 0f, global::UnityEngine.Random.Range(-1f, 1f));
            for (int i = 0; i < 15; i++)
            {
                this.LookAt(randomDir, 0.02f);
                yield return null;
            }
        }
        AnimatorStateInfo currentAnimatorStateInfo;
        while (true)
        {
            currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
            if (currentAnimatorStateInfo.IsName("Die"))
            {
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(currentAnimatorStateInfo.length / (float)Settings.GetData().GetAnimationSpeed());
    }

    private IEnumerator Animate(string animationName, string audioEffect)
    {
        yield return this.Animate(animationName, audioEffect, Vector3.zero);
    }

    private IEnumerator Animate(string animationName, string audioEffect, Vector3 dir)
    {
        while (!base.isDead && !this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            yield return null;
        }
        if (dir != Vector3.zero)
        {
            this.LookAt(dir, 1f);
        }
        this.animator.speed = this.originalSpeed * (float)Settings.GetData().GetAnimationSpeed();
        yield return new WaitForSeconds(global::UnityEngine.Random.Range(0f, 0.25f));
        this.animator.SetBool(animationName, value: true);
        while (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            yield return null;
        }
        if (this.noPitch)
        {
            AudioLibrary.RequestSFX(audioEffect);
        }
        else
        {
            AudioLibrary.RequestSFX(audioEffect, 0.3f, 300f);
        }
        this.animator.SetBool(animationName, value: false);
        while (!this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            yield return null;
        }
        this.animator.speed = 1f + global::UnityEngine.Random.Range(0f - this.idleAnimationSpeedVariable, this.idleAnimationSpeedVariable);
    }

    private void StartProjectileAnimation(AnimatorTrigger at)
    {
        base.StartCoroutine(this.Projectile(at, this.rangedAttackVector));
    }

    private IEnumerator Projectile(AnimatorTrigger at, Vector3 dir)
    {
        Vector3 start = at.location.position;
        Vector3 vector = HexCoordinates.HexToWorld3D(HexCoordinates.GetHexCoordAt(start)) - start;
        float num = 0.1f;
        Vector3 vector2 = new Vector3(global::UnityEngine.Random.Range(0f - num, num), 0f, global::UnityEngine.Random.Range(0f - num, num));
        Vector3 destination = start + vector + dir + vector2;
        float heightAt = base.plane.GetHeightAt(destination);
        destination.y = heightAt + dir.y;
        GameObject go2 = GameObjectUtils.Instantiate(at.instanceSource, this.parent.transform.parent);
        float step = 0.15f * (float)Settings.GetData().GetAnimationSpeed();
        float total = (destination - start).magnitude;
        float half = total / 2f;
        float v = step * 2f;
        while (v < total)
        {
            v += step * 60f * Time.deltaTime;
            float num2 = v / total;
            float num3 = Mathf.Sin(num2 * (float)Math.PI);
            Vector3 vector3 = Vector3.Lerp(start, destination, num2);
            Vector3 position = go2.transform.position;
            float num4 = (at.highLobProjectile ? (num3 * half * 0.5f) : ((!at.lowLobProjectile) ? 0f : (num3 * half * 0.25f)));
            Vector3 vector4 = vector3 + Vector3.up * num4;
            go2.transform.position = vector4;
            Vector3 forward = vector4 - position;
            go2.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            yield return null;
        }
        this.targetHit?.Invoke(this);
        this.isHit = true;
        global::UnityEngine.Object.Destroy(go2);
        AudioLibrary.RequestSFX(this.parent.source.dbSource.Get().audio?.attackRangedHit, 0.15f, 60f);
        if (at.onHitEffect != null)
        {
            go2 = GameObjectUtils.Instantiate(at.onHitEffect, this.parent.transform.parent);
            go2.transform.position = destination;
        }
    }

    public void UpdateReposition(Vector3 repositionDistance)
    {
        if (this.movementDataPack == null)
        {
            return;
        }
        foreach (MovementData item in this.movementDataPack)
        {
            if (item.path != null)
            {
                for (int i = 0; i < item.path.Count; i++)
                {
                    item.path[i] -= repositionDistance;
                }
            }
        }
    }

    private void OnDisable()
    {
        this.movementDataPack.Clear();
        this.currentAction = null;
        this.animationStack.Clear();
    }

    private void AddAnimationToStack(IEnumerator animation)
    {
        this.animationStack.Enqueue(animation);
        if (this.currentAction == null)
        {
            this.isHit = false;
            if (!base.isDead)
            {
                this.currentAction = base.StartCoroutine(this.AnimateCharacter());
            }
        }
    }

    private IEnumerator StartBuild()
    {
        while (this.animator == null)
        {
            if (!this.isActivelyBuilding)
            {
                this.buildDelayed = null;
                yield break;
            }
            yield return null;
        }
        this.buildDelayed = null;
        this.PlayBuildIfNeeded();
    }

    private IEnumerator AnimateCharacter()
    {
        this.NotBuild();
        int wait = 10;
        while (this.animator == null && wait > 0 && !base.isDead)
        {
            wait--;
            yield return null;
        }
        while (this.animationStack.Count > 0 && !base.isDead)
        {
            yield return this.animationStack.Dequeue();
        }
        if (!base.isDead)
        {
            this.PlayBuildIfNeeded();
        }
        this.currentAction = null;
    }

    private void DiscardAnimationErrors()
    {
        if (!(this.animator == null))
        {
            this.animator.applyRootMotion = false;
            this.animator.transform.localPosition = Vector3.zero;
            this.animator.transform.localRotation = Quaternion.identity;
            this.animator.applyRootMotion = true;
        }
    }

    private string GetIdentificator()
    {
        return this.parent.modelName + " ID " + (this.parent.owner as Entity).GetID();
    }
}
