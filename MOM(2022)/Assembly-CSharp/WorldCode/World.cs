using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using MOM;
using UnityEngine;

namespace WorldCode
{
    public class World : MonoBehaviour
    {
        public static World instance;

        private List<Plane> planes = new List<Plane>();

        private Plane activePlane;

        private Plane previouslyActive;

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

        public static void AddPlane(Plane p)
        {
            World.Get().planes.Add(p);
        }

        public static void RemovePlane(Plane p)
        {
            if (World.Get().planes.Contains(p))
            {
                World.Get().planes.Remove(p);
            }
        }

        public static void ActivatePlane(Plane p, bool forceUpdate = false)
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
            Plane plane = World.Get().activePlane;
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

        public static List<Plane> GetPlanes()
        {
            return World.Get().planes;
        }

        public static Plane GetActivePlane()
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

        public static Plane GetMyrror()
        {
            global::DBDef.Plane p = (global::DBDef.Plane)PLANE.MYRROR;
            return World.Get().planes.Find((Plane o) => !o.battlePlane && o.planeSource.Get() == p);
        }

        public static Plane GetArcanus()
        {
            global::DBDef.Plane p = (global::DBDef.Plane)PLANE.ARCANUS;
            return World.Get().planes.Find((Plane o) => !o.battlePlane && o.planeSource.Get() == p);
        }

        public static Plane GetOtherPlane(Plane p)
        {
            return World.Get().planes.Find((Plane o) => !o.battlePlane && o.planeSource.Get() != p.planeSource.Get());
        }

        public void Update()
        {
            if (this.activePlane == null)
            {
                return;
            }
            foreach (Plane plane in this.planes)
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
            foreach (Plane item in new List<Plane>(World.GetPlanes()))
            {
                item.Destroy();
            }
            World.Get().planes.Clear();
            World.Get().activePlane = null;
            World.Get().previouslyActive = null;
        }

        public static void PartialCleanup()
        {
            foreach (Plane item in new List<Plane>(World.GetPlanes()))
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
}
