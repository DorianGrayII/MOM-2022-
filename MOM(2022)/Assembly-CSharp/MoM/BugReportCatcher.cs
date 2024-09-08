// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.BugReportCatcher
using System.Collections;
using MHUtils;
using MHUtils.UI;
using MOM;
using UnityEngine;

public class BugReportCatcher : MonoBehaviour
{
    private static BugReportCatcher instance;

    public static byte[] screenShot;

    public static Texture2D screenShotTexture;

    private Coroutine bugCatcher;

    private void Start()
    {
        BugReportCatcher.instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F8))
        {
            BugReportCatcher.Open();
        }
    }

    public static void Open()
    {
        if (BugReportCatcher.instance != null && BugReportCatcher.instance.bugCatcher == null)
        {
            BugReportCatcher.instance.bugCatcher = BugReportCatcher.instance.StartCoroutine(BugReportCatcher.OpenBugCatcher());
        }
    }

    public static IEnumerator OpenBugCatcher()
    {
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
        yield return new WaitForEndOfFrame();
        screenImage.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();
        BugReportCatcher.screenShotTexture = screenImage;
        BugReportCatcher.screenShot = screenImage.EncodeToJPG(100);
        BugReport report = UIManager.GetOrOpen<BugReport>(UIManager.Layer.Popup);
        while (report != null && report.stateStatus < State.StateStatus.PreClose)
        {
            yield return null;
        }
        if (screenImage != null)
        {
            Object.Destroy(screenImage);
        }
        BugReportCatcher.screenShot = null;
        BugReportCatcher.instance.bugCatcher = null;
    }
}
