using System.Collections;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WorldCode;

namespace MOM
{
    public class Loading : ScreenBase
    {
        public Slider progressSlider;

        public RawImage wizardImage;

        public TextMeshProUGUI wizardName;

        public TextMeshProUGUI quote;

        public TextMeshProUGUI hint;

        public static bool ready;

        protected override void Awake()
        {
            PlayerWizard wizard = GameManager.GetWizard(PlayerWizard.HumanID());
            this.wizardImage.texture = AssetManager.Get<Texture2D>(wizard.Background);
            this.wizardName.text = wizard.name;
            this.quote.text = wizard.Quote;
            this.hint.text = this.GetRandomQuote();
            base.Awake();
            this.progressSlider.value = 0f;
        }

        public override void OnStart()
        {
            base.OnStart();
            MHEventSystem.RegisterListener<global::WorldCode.Plane>(ProgressUpdate, this);
            Loading.ready = true;
        }

        public override IEnumerator PreClose()
        {
            Loading.ready = false;
            yield return base.PreClose();
        }

        private void ProgressUpdate(object sender, object e)
        {
            float num = (float)e;
            if (sender is global::WorldCode.Plane)
            {
                num = (((sender as global::WorldCode.Plane).planeSource.Get() != (global::DBDef.Plane)PLANE.ARCANUS) ? (num * 0.5f + 0.5f) : (num * 0.5f));
            }
            this.progressSlider.value = num * 100f;
        }

        private string GetRandomQuote()
        {
            string[] array = new string[23]
            {
                "UI_LOADING_HINT1", "UI_LOADING_HINT2", "UI_LOADING_HINT4", "UI_LOADING_HINT5", "UI_LOADING_HINT6", "UI_LOADING_HINT7", "UI_LOADING_HINT8", "UI_LOADING_HINT9", "UI_LOADING_HINT10", "UI_LOADING_HINT11",
                "UI_LOADING_HINT12", "UI_LOADING_HINT13", "UI_LOADING_HINT14", "UI_LOADING_HINT15", "UI_LOADING_HINT16", "UI_LOADING_HINT17", "UI_LOADING_HINT18", "UI_LOADING_HINT19", "UI_LOADING_HINT20", "UI_LOADING_HINT21",
                "UI_LOADING_HINT22", "UI_LOADING_HINT23", "UI_LOADING_HINT24"
            };
            MHRandom mHRandom = new MHRandom(0, unsourced: true);
            return global::DBUtils.Localization.Get(array[mHRandom.GetInt(0, array.Length)], true);
        }

        public override void UpdateState()
        {
            base.UpdateState();
        }
    }
}
