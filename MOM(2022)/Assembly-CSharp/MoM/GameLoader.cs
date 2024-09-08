// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.GameLoader
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameLoader : ScreenBase
{
    public Slider progressSlider;

    public RawImage displayImage;

    public TextMeshProUGUI displayName;

    public TextMeshProUGUI quote;

    public VideoPlayer introSource;

    public float progress;

    private bool started;

    protected override void Awake()
    {
        base.Awake();
        MHEventSystem.RegisterListener<GameLoader>(ProgressUpdate, this);
    }

    public override void OnStart()
    {
        base.OnStart();
        this.progress = 0f;
        MHEventSystem.TriggerEvent(this, "Progress");
    }

    private void ProgressUpdate(object sender, object e)
    {
        if (e is float num)
        {
            if (sender is FSMLoadSettings)
            {
                float num2 = 0f;
                float num3 = 0.1f;
                this.progress = num2 + (num3 - num2) * num;
            }
            else if (sender is FSMInitializeDatabase)
            {
                float num4 = 0.1f;
                float num5 = 0.2f;
                this.progress = num4 + (num5 - num4) * num;
            }
            else if (sender is FSMInitializeScripts)
            {
                float num6 = 0.2f;
                float num7 = 0.3f;
                this.progress = num6 + (num7 - num6) * num;
            }
            else if (sender is FSMInitializeForegroundFactory)
            {
                float num8 = 0.3f;
                float num9 = 0.4f;
                this.progress = num8 + (num9 - num8) * num;
            }
            else if (sender is FSMInitializeAssetBundles)
            {
                float num10 = 0.4f;
                float num11 = 0.5f;
                this.progress = num10 + (num11 - num10) * num;
            }
            else if (sender is FSMLoadAdventures)
            {
                float num12 = 0.5f;
                float num13 = 0.6f;
                this.progress = num12 + (num13 - num12) * num;
            }
            else if (sender is string text && text == "SortByFight")
            {
                float num14 = 0.6f;
                float num15 = 0.9f;
                this.progress = num14 + (num15 - num14) * num;
            }
            else if (sender is FSMLoadAudio)
            {
                float num16 = 0.9f;
                float num17 = 1f;
                this.progress = num16 + (num17 - num16) * num;
            }
            this.progressSlider.value = this.progress * 100f;
        }
    }

    public void Update()
    {
        if (this.progress >= 1f && (this.introSource == null || !this.introSource.isPlaying) && base.stateStatus == StateStatus.Active)
        {
            AchievementManager.Send();
            MHEventSystem.TriggerEvent(this, "LoadingEnd");
        }
        if (this.introSource != null && this.introSource.isPlaying && Input.GetKeyDown(KeyCode.Space))
        {
            this.introSource.Stop();
        }
        if (!this.started && base.gameObject.activeInHierarchy && Settings.IsLoaded())
        {
            this.started = true;
        }
        if (this.introSource.isPlaying && this.displayImage.color.a < 1f)
        {
            Color color = this.displayImage.color;
            color.a = Mathf.Clamp01(color.a + 0.05f);
            this.displayImage.color = color;
        }
        else if (!this.introSource.isPlaying && this.displayImage.color.a > 0f)
        {
            Color color2 = this.displayImage.color;
            color2.a = Mathf.Clamp01(color2.a - 0.02f);
            this.displayImage.color = color2;
        }
    }
}
