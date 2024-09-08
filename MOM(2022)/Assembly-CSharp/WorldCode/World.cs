// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// WorldCode.World
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using MOM;
using UnityEngine;
using WorldCode;

public class World : MonoBehaviour
{
    public static World instance;

    private List<global::WorldCode.Plane> planes = new List<global::WorldCode.Plane>();

    private global::WorldCode.Plane activePlane;

    private global::WorldCode.Plane previouslyActive;

    public int seed;

    public int gameID;

    public int worldSizeSetting;

    public GameObject waterMyrror;

    public GameObject waterArcanus;

    public bool DEBUG_MODE;

    public Vector3i debugVector;

    public int debugIndex;

    public bool GetDebugMode()
    {
        return false;
    }

    private void Start()
    {
        World.instance = this;
    }

    public static World Get()
    {
        return World.instance;
    }

    public static void AddPlane(global::WorldCode.Plane p)
    {
        World.Get().planes.Add(p);
    }

    public static void RemovePlane(global::WorldCode.Plane p)
    {
        if (World.Get().planes.Contains(p))
        {
            World.Get().planes.Remove(p);
        }
    }

    public static void ActivatePlane(global::WorldCode.Plane p, bool forceUpdate = false)
    {
        if (!forceUpdate && World.Get().activePlane == p)
        {
            return;
        }
        IPlanePosition planePosition = FSMSelectionManager.Get()?.GetSelectedGroup();
        if (planePosition != null && planePosition.GetPlane() != p)
        {
            FSMSelectionManager.Get().Select(null, focus: false);
        }
        CameraController.MakeNextCenterInstant();
        VerticalMarkerManager.Get().Clearmarkers();
        global::WorldCode.Plane plane = World.Get().activePlane;
        World.Get().activePlane = p;
        if (plane?.battlePlane != p.battlePlane)
        {
            Settings.GetData().UpdateAnimationSpeed();
        }
        if (plane == null || plane.battlePlane)
        {
            Material material = World.GetWater(plane?.arcanusType ?? true)?.GetComponent<MeshRenderer>()?.material;
            if (material != null)
            {
                material.SetFloat("_UseBorderWave", 1f);
            }
            LightController.SetInstensity();
        }
        VerticalMarkerManager.Get().InitializeMarkers(p);
        if (p.planeSource.Get() == (global::DBDef.Plane)PLANE.ARCANUS)
        {
            PosProcessingLibrary.SetArcanusMode();
            World.Get().waterArcanus.SetActive(value: true);
            World.Get().waterMyrror.SetActive(value: false);
            if (p.battlePlane)
            {
                List<SoundList> list = DataBase.GetType<SoundList>().FindAll((SoundList o) => o.identifier == "Battle");
                PlayMusic.Play(list[Random.Range(0, list.Count)].dbName, Battle.GetBattle());
            }
            else
            {
                PlayMusic.Play("SOUND_LIST-MAP_ARCANUS", GameManager.Get());
            }
        }
        else
        {
            PosProcessingLibrary.SetMyrrorMode();
            World.Get().waterArcanus.SetActive(value: false);
            World.Get().waterMyrror.SetActive(value: true);
            if (p.battlePlane)
            {
                List<SoundList> list2 = DataBase.GetType<SoundList>().FindAll((SoundList o) => o.identifier == "Battle");
                PlayMusic.Play(list2[Random.Range(0, list2.Count)].dbName, Battle.GetBattle());
            }
            else
            {
                PlayMusic.Play("SOUND_LIST-MAP_MYRROR", GameManager.Get());
            }
        }
        MHEventSystem.TriggerEvent<World>(World.Get(), p);
        List<global::MOM.Group> groupsOfPlane = GameManager.GetGroupsOfPlane(p);
        for (int i = 0; i < groupsOfPlane.Count; i++)
        {
            global::MOM.Group group = groupsOfPlane[i];
            if (group.IsModelVisible())
            {
                group.GetMapFormation();
            }
            else
            {
                _ = group.GetMapFormation(createIfMissing: false) != null;
            }
        }
        List<global::MOM.Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(p);
        if (locationsOfThePlane == null)
        {
            return;
        }
        foreach (global::MOM.Location item in locationsOfThePlane)
        {
            if (item.IsModelVisible() && item.model == null)
            {
                item.InitializeModel();
            }
        }
    }

    public static List<global::WorldCode.Plane> GetPlanes()
    {
        return World.Get().planes;
    }

    public static global::WorldCode.Plane GetActivePlane()
    {
        return World.Get().activePlane;
    }

    public static GameObject GetArcanusWater()
    {
        return World.Get().waterArcanus;
    }

    public static GameObject GetMyrrorWater()
    {
        return World.Get().waterMyrror;
    }

    public static GameObject GetWater(bool arcanus)
    {
        if (!arcanus)
        {
            return World.GetMyrrorWater();
        }
        return World.GetArcanusWater();
    }

    public static global::WorldCode.Plane GetMyrror()
    {
        global::DBDef.Plane p = (global::DBDef.Plane)PLANE.MYRROR;
        return World.Get().planes.Find((global::WorldCode.Plane o) => !o.battlePlane && o.planeSource.Get() == p);
    }

    public static global::WorldCode.Plane GetArcanus()
    {
        global::DBDef.Plane p = (global::DBDef.Plane)PLANE.ARCANUS;
        return World.Get().planes.Find((global::WorldCode.Plane o) => !o.battlePlane && o.planeSource.Get() == p);
    }

    public static global::WorldCode.Plane GetOtherPlane(global::WorldCode.Plane p)
    {
        return World.Get().planes.Find((global::WorldCode.Plane o) => !o.battlePlane && o.planeSource.Get() != p.planeSource.Get());
    }

    public void Update()
    {
        if (this.activePlane == null)
        {
            return;
        }
        foreach (global::WorldCode.Plane plane in this.planes)
        {
            if (this.previouslyActive != this.activePlane)
            {
                plane.UpdateVisibility(plane == this.activePlane);
            }
            plane.UpdatePlane(plane == this.activePlane);
        }
        this.previouslyActive = this.activePlane;
    }

    public static void CleanupSequence()
    {
        foreach (global::WorldCode.Plane item in new List<global::WorldCode.Plane>(World.GetPlanes()))
        {
            item.Destroy();
        }
        World.Get().planes.Clear();
        World.Get().activePlane = null;
        World.Get().previouslyActive = null;
    }

    public static void PartialCleanup()
    {
        foreach (global::WorldCode.Plane item in new List<global::WorldCode.Plane>(World.GetPlanes()))
        {
            if (item.battlePlane)
            {
                item.Destroy();
            }
            else
            {
                item.Cleanup();
            }
        }
    }
}
