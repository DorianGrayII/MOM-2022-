// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.Formation
using System.Collections;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using MHUtils.UI;
using MOM;
using UnityEngine;
using WorldCode;

public class Formation : MonoBehaviour, IAttentionController
{
    public IPlanePosition owner;

    public BaseUnit source;

    public float formationSpeed = 0.5f;

    private List<CharacterActor> characters = new List<CharacterActor>();

    public Vector3 direction = Vector3.back;

    public float formationLazines = 0.3f;

    public CharacterActor dyingCharacter;

    public int standingActors;

    public bool isVisible = true;

    public string modelName;

    public List<Vector3> storagePosition = new List<Vector3>();

    public int unitsMoving;

    public bool isActivelyBuilding;

    public bool detached;

    public bool obsoleteModelPositioning;

    public GameObject raftModel;

    private GameObject enchantmentSparkles;

    private EEnchantmentCategory enchantmentCategory;

    private int lastIterationUpdate = -1;

    public static Formation CreateFormation(BaseUnit source, Vector3i position, IPlanePosition owner, Vector3 lookAtDir)
    {
        if (owner.GetPlane() == null)
        {
            Debug.LogError("Creating Formation for Unit that is not assigned to plane");
            return null;
        }
        Chunk chunk = owner.GetPlane().GetPlaneData().GetChunk(position);
        GameObject obj = new GameObject("Formation " + source.GetDBName());
        obj.transform.parent = chunk.go.transform;
        obj.transform.localPosition = Vector3.zero;
        Formation formation = obj.AddComponent<Formation>();
        formation.direction = lookAtDir;
        formation.owner = owner;
        formation.source = source;
        bool flag = source.IsHumanPlayerFocusedOnPlane();
        obj.SetActive(flag);
        if (flag)
        {
            formation.UpdateFigureCount();
            if (owner is global::MOM.Group)
            {
                bool value = (owner as global::MOM.Group).isActivelyBuilding;
                formation.SetAcivelyBuilding(value);
            }
        }
        return formation;
    }

    private void OnEnable()
    {
        this.UpdateFigureCount();
        if (this.obsoleteModelPositioning)
        {
            this.obsoleteModelPositioning = false;
            this.unitsMoving = 0;
            this.MovementCleanup(freeFocus: false);
            this.InitializeModelPositions();
        }
        bool value = (this.owner as global::MOM.Group)?.isActivelyBuilding ?? false;
        this.SetAcivelyBuilding(value);
    }

    private void OnDisable()
    {
        if (this.unitsMoving != 0)
        {
            while (this.unitsMoving > 0)
            {
                this.MovementFinished(updatePosition: false);
            }
            this.unitsMoving = 0;
            this.obsoleteModelPositioning = true;
            GameManager.Get().FreeFocus(this, throwErrors: false);
            if (this.owner == null)
            {
                this.Destroy();
            }
        }
    }

    private void Update()
    {
        if (this.characters.Count == 0)
        {
            if (this.enchantmentSparkles != null)
            {
                Object.Destroy(this.enchantmentSparkles);
                this.enchantmentSparkles = null;
            }
        }
        else
        {
            if (this.source == null)
            {
                return;
            }
            int num = (this.source.GetEnchantments().Count << 12) | this.source.GetAttributes().iteration;
            if (this.lastIterationUpdate != num)
            {
                bool flag = false;
                bool flag2 = false;
                foreach (EnchantmentInstance enchantmentsWithRemote in this.source.GetEnchantmentManager().GetEnchantmentsWithRemotes())
                {
                    if (!enchantmentsWithRemote.source.Get().hideEnch)
                    {
                        EEnchantmentCategory enchCategory = enchantmentsWithRemote.source.Get().enchCategory;
                        flag = flag || enchCategory != EEnchantmentCategory.Negative;
                        flag2 = flag2 || enchCategory != EEnchantmentCategory.Positive;
                        if (flag && flag2)
                        {
                            break;
                        }
                    }
                }
                bool flag3 = true;
                if (this.source is BattleUnit)
                {
                    BattleUnit battleUnit = this.source as BattleUnit;
                    flag3 = battleUnit.currentlyVisible || battleUnit.GetWizardOwnerID() == PlayerWizard.HumanID();
                }
                else if (this.source is global::MOM.Unit)
                {
                    Reference<global::MOM.Group> group = (this.source as global::MOM.Unit).group;
                    if (group != null)
                    {
                        flag3 = !group.Get().IsGroupInvisible() || group.Get().GetOwnerID() == PlayerWizard.HumanID();
                    }
                }
                if (!flag && !flag2)
                {
                    if (this.enchantmentSparkles != null)
                    {
                        Object.Destroy(this.enchantmentSparkles);
                        this.enchantmentSparkles = null;
                    }
                }
                else if (!flag && this.enchantmentCategory != EEnchantmentCategory.Negative)
                {
                    if (this.enchantmentSparkles != null)
                    {
                        Object.Destroy(this.enchantmentSparkles);
                        this.enchantmentSparkles = null;
                    }
                    GameObject original = AssetManager.Get<GameObject>("EnchantmentsNegative");
                    this.enchantmentSparkles = Object.Instantiate(original, base.transform);
                    this.enchantmentCategory = EEnchantmentCategory.Negative;
                }
                else if (!flag2 && this.enchantmentCategory != EEnchantmentCategory.Positive)
                {
                    if (this.enchantmentSparkles != null)
                    {
                        Object.Destroy(this.enchantmentSparkles);
                        this.enchantmentSparkles = null;
                    }
                    GameObject original2 = AssetManager.Get<GameObject>("EnchantmentsPositive");
                    this.enchantmentSparkles = Object.Instantiate(original2, base.transform);
                    this.enchantmentCategory = EEnchantmentCategory.Positive;
                }
                else if (flag2 && flag && (this.enchantmentSparkles == null || this.enchantmentCategory != 0))
                {
                    if (this.enchantmentSparkles != null)
                    {
                        Object.Destroy(this.enchantmentSparkles);
                        this.enchantmentSparkles = null;
                    }
                    GameObject original3 = AssetManager.Get<GameObject>("EnchantmentsMixed");
                    this.enchantmentSparkles = Object.Instantiate(original3, base.transform);
                    this.enchantmentCategory = EEnchantmentCategory.None;
                }
                if (this.enchantmentSparkles != null && this.enchantmentSparkles.activeInHierarchy != flag3)
                {
                    this.enchantmentSparkles.SetActive(flag3);
                }
                this.lastIterationUpdate = num;
            }
            Vector3 zero = Vector3.zero;
            foreach (CharacterActor character in this.characters)
            {
                zero += character.transform.position;
            }
            zero /= (float)this.characters.Count;
            if (this.enchantmentSparkles != null)
            {
                this.enchantmentSparkles.transform.position = zero;
            }
        }
    }

    public void UpdateFigureCount()
    {
        if (this.source == null)
        {
            return;
        }
        if (this.detached || this.source == null)
        {
            this.Destroy();
        }
        int num = this.source.FigureCount();
        if (num < 0)
        {
            Debug.LogError("Invalid figure count! " + num);
        }
        string model3dName = this.source.GetModel3dName();
        if (model3dName != this.modelName)
        {
            if (this.unitsMoving != 0)
            {
                this.unitsMoving = 0;
                this.MovementCleanup();
            }
            this.ClearCharacters();
            this.standingActors = 0;
        }
        int num2 = num - this.characters.Count;
        if (num2 == 0)
        {
            return;
        }
        if (num2 < 0)
        {
            bool flag = true;
            if (!(this.source is global::MOM.Unit) || (this.source as global::MOM.Unit).group != null)
            {
                flag = this.source.GetPlane() != null && this.source.GetPlane().battlePlane;
            }
            while (this.characters.Count > num)
            {
                if (this.characters.Count == 0)
                {
                    Debug.LogError("Cannot remove index 0 from empty list!");
                    break;
                }
                if (this.characters[0] != null)
                {
                    this.dyingCharacter = this.characters[0];
                    if (flag)
                    {
                        this.dyingCharacter.PlayDeath();
                        this.characters.Remove(this.dyingCharacter);
                    }
                    else
                    {
                        this.dyingCharacter.DestroyObject();
                        this.characters.Remove(this.dyingCharacter);
                    }
                }
            }
        }
        else
        {
            this.modelName = model3dName;
            if (this.owner != null)
            {
                for (int i = 0; i < num2; i++)
                {
                    CharacterActor item = CharacterActor.CreateCharacter(model3dName, this);
                    this.characters.Add(item);
                }
            }
        }
        this.UpdateStorageSize();
        if (this.unitsMoving != 0)
        {
            this.unitsMoving = 0;
            this.MovementCleanup();
        }
    }

    private void UpdateStorageSize()
    {
        bool flag = false;
        if (this.storagePosition == null)
        {
            this.storagePosition = new List<Vector3>(this.characters.Count);
            flag = true;
        }
        if (this.storagePosition.Count < this.characters.Count)
        {
            while (this.storagePosition.Count < this.characters.Count)
            {
                this.storagePosition.Add(Vector3.zero);
            }
            flag = true;
        }
        else if (this.storagePosition.Count > this.characters.Count)
        {
            while (this.storagePosition.Count > this.characters.Count)
            {
                this.storagePosition.RemoveAt(this.storagePosition.Count - 1);
            }
        }
        if (flag)
        {
            this.InitializeModelPositions();
        }
    }

    internal void PlayMagicCast()
    {
        foreach (CharacterActor character in this.characters)
        {
            character.PlayMagicAttack();
        }
    }

    internal void Attack(Vector3i pos)
    {
        Vector3 vector = HexCoordinates.HexToWorld3D(pos);
        Vector3 vector2 = HexCoordinates.HexToWorld3D(this.owner.GetPosition());
        foreach (CharacterActor character in this.characters)
        {
            character.PlayAttackByVector(vector - vector2);
        }
    }

    internal void AttackRanged(Vector3i pos, BattleUnit bu, Callback ready)
    {
        Vector3 vector = HexCoordinates.HexToWorld3D(pos);
        Vector3 vector2 = HexCoordinates.HexToWorld3D(this.owner.GetPosition());
        Vector3 vector3 = vector - vector2;
        if (bu != null && bu.battleFormation != null)
        {
            List<CharacterActor> characterActors = bu.battleFormation.GetCharacterActors();
            if (characterActors != null)
            {
                Bounds bounds = new Bounds(base.transform.position, Vector3.one * 0.01f);
                foreach (CharacterActor item in characterActors)
                {
                    Renderer[] componentsInChildren = item.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in componentsInChildren)
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                }
                vector3.y = bounds.center.y;
            }
        }
        int hitCount = 0;
        foreach (CharacterActor character in this.characters)
        {
            character.PlayRangedAttackByVector(vector3, delegate(object o)
            {
                hitCount++;
                if (hitCount >= this.characters.Count)
                {
                    ready(o);
                }
            });
        }
    }

    internal void GetHit()
    {
        foreach (CharacterActor character in this.characters)
        {
            character.PlayGetHit();
        }
    }

    internal bool IsAnimating()
    {
        foreach (CharacterActor character in this.characters)
        {
            if (character.IsAnimating())
            {
                return true;
            }
        }
        return false;
    }

    public void SetAcivelyBuilding(bool value, bool onlyIfDifferent = false)
    {
        if (value == this.isActivelyBuilding && onlyIfDifferent)
        {
            return;
        }
        this.isActivelyBuilding = value;
        if (this.characters == null)
        {
            return;
        }
        foreach (CharacterActor character in this.characters)
        {
            character.SetAcivelyBuilding(value);
        }
    }

    public void InitializeModelPositions(bool force = true)
    {
        if (this.owner == null)
        {
            return;
        }
        Vector3 vector = HexCoordinates.HexToWorld3D(this.owner.GetPosition());
        for (int i = 0; i < this.characters.Count; i++)
        {
            Vector3 formationOffset = this.GetFormationOffset(i, this.direction);
            if (force || !(HexCoordinates.GetHexCoordAt(this.storagePosition[i]) == this.owner.GetPosition()))
            {
                this.storagePosition[i] = vector + formationOffset;
            }
        }
        if (this.characters == null || this.storagePosition == null)
        {
            return;
        }
        if (this.characters.Count != this.storagePosition.Count)
        {
            Debug.LogError("characters.Count != storagePosition.Count");
            return;
        }
        for (int j = 0; j < this.characters.Count; j++)
        {
            this.characters[j].SetWorldHeightPosition(this.storagePosition[j], allowUnderwater: false);
            this.characters[j].ResetLookAt();
        }
    }

    public void ClearCharacters()
    {
        if (this.storagePosition != null)
        {
            this.storagePosition.Clear();
        }
        foreach (CharacterActor character in this.characters)
        {
            character.DestroyObject();
        }
        this.characters.Clear();
        this.modelName = null;
    }

    public void Cleanup()
    {
        this.ClearCharacters();
        this.standingActors = 0;
    }

    public void DetachFromGroup()
    {
        VerticalMarkerManager.Get().DestroyMarker(this.owner);
        this.detached = true;
        this.owner = null;
    }

    public void Destroy()
    {
        if (this.raftModel != null)
        {
            Object.Destroy(this.raftModel);
        }
        Object.Destroy(base.gameObject);
        VerticalMarkerManager.Get().DestroyMarker(this.owner);
        GameManager.Get().FreeFocus(this, throwErrors: false);
    }

    public void SetVisibility(bool v)
    {
        if (v)
        {
            if (base.gameObject.layer == 11)
            {
                return;
            }
            GameObjectUtils.ChangeLayer(base.gameObject.transform, 11);
            Light[] componentsInChildren = base.gameObject.GetComponentsInChildren<Light>();
            if (componentsInChildren != null)
            {
                Light[] array = componentsInChildren;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].gameObject.SetActive(value: true);
                }
            }
        }
        else
        {
            if (base.gameObject.layer == 14)
            {
                return;
            }
            GameObjectUtils.ChangeLayer(base.gameObject.transform, 14);
            Light[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Light>();
            if (componentsInChildren2 != null)
            {
                Light[] array = componentsInChildren2;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].gameObject.SetActive(value: false);
                }
            }
        }
    }

    public List<CharacterActor> GetCharacterActors()
    {
        return this.characters;
    }

    public Vector3 GetFormationOffset(int index, Vector3 dir)
    {
        int num = ((this.characters == null) ? 9 : this.characters.Count);
        int num2 = 1;
        switch (num)
        {
        case 1:
            num2 = 1;
            break;
        case 2:
        case 4:
            num2 = 2;
            break;
        default:
            num2 = 3;
            break;
        }
        int num3 = (num + num2 - 1) / num2;
        int num4 = index / num2;
        int num5 = index - num4 * num2;
        float num6 = 1f - (float)num2 * 0.2f;
        float num7 = 1f - (float)num3 * 0.2f;
        Vector3 vector = num6 * Vector3.left;
        Vector3 vector2 = num7 * Vector3.forward;
        Vector3 vector3 = vector * 0.5f * (num2 - 1) - vector * num5;
        Vector3 vector4 = vector2 * 0.5f * (num3 - 1) - vector2 * num4;
        Vector3 vector5 = vector3 + vector4;
        return Quaternion.FromToRotation(Vector3.forward, dir) * vector5;
    }

    public IEnumerator WaitToEndOfMovement()
    {
        while (this.unitsMoving > 0)
        {
            yield return null;
        }
    }

    public void InstantMove()
    {
        if (this.source != null && base.gameObject.activeInHierarchy)
        {
            this.InitializeModelPositions();
        }
    }

    public void Move(List<Vector3i> path, bool wastefullLastStep)
    {
        if (this.source == null || !base.gameObject.activeInHierarchy)
        {
            if (this.characters != null && this.owner is global::MOM.Group group)
            {
                group.Position = path[path.Count - 1];
                this.InitializeModelPositions();
            }
            MHEventSystem.TriggerEvent<Formation>(this, null);
            return;
        }
        if (this.unitsMoving > 0)
        {
            this.unitsMoving = 0;
            this.MovementCleanup();
        }
        if (this.owner is BaseUnit baseUnit && (baseUnit.GetWizardOwner() == null || baseUnit.GetWizardOwner().ID != PlayerWizard.HumanID()) && !baseUnit.GetPlane().battlePlane && path.Count > 0)
        {
            World.ActivatePlane(baseUnit.GetPlane());
            CameraController.CenterAt(path[path.Count - 1]);
        }
        global::WorldCode.Plane plane = this.owner.GetPlane();
        Vector3i vector3i = plane.area.KeepHorizontalInside(path[0]);
        List<Vector3> list = new List<Vector3>(path.Count);
        Vector3i vector3i2 = Vector3i.invalid;
        _ = Vector3.zero;
        for (int i = 0; i < path.Count; i++)
        {
            vector3i2 = ((i <= 0) ? vector3i : plane.area.UnvrappedNext(path[i], vector3i2));
            list.Add(HexCoordinates.HexToWorld3D(vector3i2));
        }
        if (this.characters.Count > 0)
        {
            GameManager.Get().TakeFocus(this, GameManager.FocusFlag.Movement);
            float num = 1.05f * (float)Settings.GetData().GetAnimationSpeed();
            AudioLibrary.RequestLoopingSFX(this.source.dbSource.Get().audio?.move, (float)(path.Count - 1) / num, this);
            this.unitsMoving = this.characters.Count;
            for (int j = 0; j < this.characters.Count; j++)
            {
                List<Vector3> list2 = new List<Vector3>(list.Count);
                for (int k = 0; k < list.Count; k++)
                {
                    Vector3 dir = ((k != list.Count - 1) ? (list[k + 1] - list[k]) : (list[k] - list[k - 1]));
                    list2.Add(list[k] + this.GetFormationOffset(j, dir));
                }
                List<Vector3> path2 = Bezier.InterpolateWithBezier(list2, 7);
                this.characters[j].AnimateViaPath(path2, 0f);
            }
        }
        else
        {
            this.InitializeModelPositions(this.characters.Count > 0);
            MHEventSystem.TriggerEvent<Formation>(this, null);
        }
    }

    public void MovementFinished(bool updatePosition = true)
    {
        if (this.unitsMoving <= 0)
        {
            return;
        }
        this.unitsMoving--;
        if (this.unitsMoving == 0)
        {
            this.MovementCleanup(freeFocus: true, updatePosition);
            AudioLibrary.StopLoopingSFX(this);
            if (!updatePosition)
            {
                this.obsoleteModelPositioning = true;
            }
        }
    }

    private void MovementCleanup(bool freeFocus = true, bool updatePosition = true)
    {
        if (this.detached)
        {
            this.Destroy();
            return;
        }
        if (updatePosition && this.storagePosition.Count > 0)
        {
            global::WorldCode.Plane plane = this.owner.GetPlane();
            Vector3i position = this.owner.GetPosition();
            Chunk chunk = plane.GetPlaneData().GetChunk(position);
            Transform obj = base.gameObject.transform;
            obj.parent = chunk.go.transform;
            obj.localPosition = Vector3.zero;
            if (this.characters != null)
            {
                foreach (CharacterActor character in this.characters)
                {
                    character.StopMovementViaPath();
                }
            }
            Vector3 vector = HexCoordinates.HexToWorld3D(position);
            Vector3 vector2 = this.storagePosition[0];
            if ((vector - vector2).magnitude > 15f)
            {
                this.InitializeModelPositions(force: false);
            }
        }
        if (freeFocus)
        {
            GameManager.Get().FreeFocus(this);
        }
    }

    public bool MoveAllowed(CharacterActor c, Vector3 newPos)
    {
        float num = float.MaxValue;
        float num2 = float.MaxValue;
        Vector3 localPosition = c.transform.localPosition;
        foreach (CharacterActor character in this.characters)
        {
            if (character != c)
            {
                Vector3 localPosition2 = character.transform.localPosition;
                float sqrMagnitude = (localPosition2 - localPosition).sqrMagnitude;
                if (sqrMagnitude < num)
                {
                    num = sqrMagnitude;
                }
                float sqrMagnitude2 = (localPosition2 - newPos).sqrMagnitude;
                if (sqrMagnitude2 < num2)
                {
                    num2 = sqrMagnitude2;
                }
            }
        }
        if (num2 > 0.25f || num2 > num)
        {
            return true;
        }
        return false;
    }

    public bool RequiresFocus()
    {
        if (this.dyingCharacter != null && this.dyingCharacter.IsAnimating())
        {
            return true;
        }
        if (this.characters != null)
        {
            foreach (CharacterActor character in this.characters)
            {
                if (character != null && character.IsAnimating())
                {
                    return true;
                }
            }
        }
        this.dyingCharacter = null;
        return false;
    }
}
