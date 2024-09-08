namespace HutongGames.PlayMaker
{
    using System;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public class FsmProcessor
    {
        private static bool AddEventHandlerComponent(PlayMakerFSM fsm, System.Type type)
        {
            if (type == null)
            {
                return false;
            }
            PlayMakerProxyBase eventHandlerComponent = GetEventHandlerComponent(fsm.gameObject, type);
            if (eventHandlerComponent == null)
            {
                return false;
            }
            eventHandlerComponent.AddTarget(fsm);
            if (!PlayMakerGlobals.IsEditor && PlayMakerPrefs.LogPerformanceWarnings)
            {
                Debug.Log("AddEventHandlerComponent: " + type.FullName);
            }
            return true;
        }

        private static void AddUiEventHandler<T>(PlayMakerFSM fsm) where T: PlayMakerUiEventBase
        {
            T component = fsm.GetComponent<T>();
            if (component == null)
            {
                component = fsm.gameObject.AddComponent<T>();
                if (!PlayMakerPrefs.ShowEventHandlerComponents)
                {
                    component.hideFlags = HideFlags.HideInInspector;
                }
            }
            component.AddTargetFsm(fsm);
        }

        public static PlayMakerProxyBase GetEventHandlerComponent(GameObject go, System.Type type)
        {
            if (go == null)
            {
                return null;
            }
            Component component = go.GetComponent(type);
            if (component == null)
            {
                component = go.AddComponent(type);
                if (!PlayMakerPrefs.ShowEventHandlerComponents)
                {
                    component.hideFlags = HideFlags.HideInInspector;
                }
            }
            return (component as PlayMakerProxyBase);
        }

        private static void HandleUiEvent<T>(PlayMakerFSM fsm, UiEvents uiEvent) where T: PlayMakerUiEventBase
        {
            if ((fsm.Fsm.HandleUiEvents & uiEvent) != UiEvents.None)
            {
                AddUiEventHandler<T>(fsm);
            }
        }

        public static void OnPreprocess(PlayMakerFSM fsm)
        {
            if (fsm.Fsm.HandleLegacyNetworking && !AddEventHandlerComponent(fsm, ReflectionUtils.GetGlobalType("PlayMakerLegacyNetworking")))
            {
                Debug.LogError("Could not add PlayMakerLegacyNetworking proxy!");
            }
            if (fsm.Fsm.HandleUiEvents != UiEvents.None)
            {
                HandleUiEvent<PlayMakerUiClickEvent>(fsm, UiEvents.Click);
                HandleUiEvent<PlayMakerUiDragEvents>(fsm, UiEvents.DragEvents);
                HandleUiEvent<PlayMakerUiDropEvent>(fsm, UiEvents.Drop);
                HandleUiEvent<PlayMakerUiPointerEvents>(fsm, UiEvents.PointerEvents);
                HandleUiEvent<PlayMakerUiBoolValueChangedEvent>(fsm, UiEvents.BoolValueChanged);
                HandleUiEvent<PlayMakerUiFloatValueChangedEvent>(fsm, UiEvents.FloatValueChanged);
                HandleUiEvent<PlayMakerUiIntValueChangedEvent>(fsm, UiEvents.IntValueChanged);
                HandleUiEvent<PlayMakerUiVector2ValueChangedEvent>(fsm, UiEvents.Vector2ValueChanged);
                HandleUiEvent<PlayMakerUiEndEditEvent>(fsm, UiEvents.EndEdit);
            }
        }
    }
}

