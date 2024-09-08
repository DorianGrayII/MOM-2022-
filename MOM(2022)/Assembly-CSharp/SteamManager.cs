using AOT;
using Steamworks;
using System;
using System.Text;
using UnityEngine;

[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
    protected static bool s_EverInitialized;
    protected static SteamManager s_instance;
    protected bool m_bInitialized;
    protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

    protected virtual void Awake()
    {
        if (s_instance != null)
        {
            Destroy(base.gameObject);
        }
        else
        {
            s_instance = this;
            if (s_EverInitialized)
            {
                throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
            }
            DontDestroyOnLoad(base.gameObject);
            if (!Packsize.Test())
            {
                Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
            }
            if (!DllCheck.Test())
            {
                Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
            }
            try
            {
                if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
                {
                    Debug.Log("[Steamworks.NET] Shutting down because RestartAppIfNecessary returned true. Steam will restart the application.");
                    Application.Quit();
                    return;
                }
            }
            catch (DllNotFoundException exception)
            {
                string text1;
                if (exception != null)
                {
                    text1 = exception.ToString();
                }
                else
                {
                    DllNotFoundException local1 = exception;
                    text1 = null;
                }
                Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + text1, this);
                Application.Quit();
                return;
            }
            this.m_bInitialized = SteamAPI.Init();
            if (!this.m_bInitialized)
            {
                Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
            }
            else
            {
                s_EverInitialized = true;
            }
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void InitOnPlayMode()
    {
        s_EverInitialized = false;
        s_instance = null;
    }

    protected virtual void OnDestroy()
    {
        if (s_instance == this)
        {
            s_instance = null;
            if (this.m_bInitialized)
            {
                SteamAPI.Shutdown();
            }
        }
    }

    protected virtual void OnEnable()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        if (this.m_bInitialized && (this.m_SteamAPIWarningMessageHook == null))
        {
            this.m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamManager.SteamAPIDebugTextHook);
            SteamClient.SetWarningMessageHook(this.m_SteamAPIWarningMessageHook);
        }
    }

    [MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
    protected static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
    {
        Debug.LogWarning(pchDebugText);
    }

    protected virtual void Update()
    {
        if (this.m_bInitialized)
        {
            SteamAPI.RunCallbacks();
        }
    }

    protected static SteamManager Instance
    {
        get
        {
            return ((s_instance != null) ? s_instance : new GameObject("SteamManager").AddComponent<SteamManager>());
        }
    }

    public static bool Initialized
    {
        get
        {
            return Instance.m_bInitialized;
        }
    }
}

