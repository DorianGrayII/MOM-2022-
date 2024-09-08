using UnityEngine.Networking;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("WWW")]
    [Tooltip("Gets data from a url and store it in variables. See Unity WWW docs for more details.")]
    public class WWWObject : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Url to download data from.")]
        public FsmString url;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("Gets text from the url.")]
        public FsmString storeText;

        [UIHint(UIHint.Variable)]
        [Tooltip("Gets a Texture from the url.")]
        public FsmTexture storeTexture;

        [UIHint(UIHint.Variable)]
        [Tooltip("Error message if there was an error during the download.")]
        public FsmString errorString;

        [UIHint(UIHint.Variable)]
        [Tooltip("How far the download progressed (0-1).")]
        public FsmFloat progress;

        [ActionSection("Events")]
        [Tooltip("Event to send when the data has finished loading (progress = 1).")]
        public FsmEvent isDone;

        [Tooltip("Event to send if there was an error.")]
        public FsmEvent isError;

        private UnityWebRequest uwr;

        private DownloadHandlerBuffer d;

        public override void Reset()
        {
            this.url = null;
            this.storeText = null;
            this.storeTexture = null;
            this.errorString = null;
            this.progress = null;
            this.isDone = null;
        }

        public override void OnEnter()
        {
            if (string.IsNullOrEmpty(this.url.Value))
            {
                base.Finish();
                return;
            }
            if (!this.storeTexture.IsNone)
            {
                this.uwr = UnityWebRequestTexture.GetTexture(this.url.Value);
            }
            else
            {
                this.uwr = new UnityWebRequest(this.url.Value);
                this.d = new DownloadHandlerBuffer();
                this.uwr.downloadHandler = this.d;
            }
            this.uwr.SendWebRequest();
        }

        public override void OnUpdate()
        {
            if (this.uwr == null)
            {
                this.errorString.Value = "Unity Web Request is Null!";
                base.Finish();
                return;
            }
            this.errorString.Value = this.uwr.error;
            if (!string.IsNullOrEmpty(this.uwr.error))
            {
                base.Finish();
                base.Fsm.Event(this.isError);
                return;
            }
            this.progress.Value = this.uwr.downloadProgress;
            if (this.progress.Value.Equals(1f))
            {
                if (!this.storeText.IsNone)
                {
                    this.storeText.Value = this.uwr.downloadHandler.text;
                }
                if (!this.storeTexture.IsNone)
                {
                    this.storeTexture.Value = ((DownloadHandlerTexture)this.uwr.downloadHandler).texture;
                }
                this.errorString.Value = this.uwr.error;
                base.Fsm.Event(string.IsNullOrEmpty(this.errorString.Value) ? this.isDone : this.isError);
                base.Finish();
            }
        }
    }
}
