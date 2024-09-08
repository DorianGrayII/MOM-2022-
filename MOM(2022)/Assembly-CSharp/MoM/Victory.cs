namespace MOM
{
    using DBUtils;
    using MHUtils.UI;
    using System;
    using TMPro;
    using UnityEngine.UI;

    public class Victory : ScreenBase
    {
        public TextMeshProUGUI victoryText;
        public Button btEndGame;
        public Button btKeepPlaying;
        public string text;

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (this.btEndGame == s)
            {
                TurnManager.StopTurnLoop();
                HallOfFame.Popup(true);
                UIManager.Close(this);
            }
            if (this.btKeepPlaying == s)
            {
                TurnManager.StopTurnLoop();
                HallOfFame.Popup(false);
                UIManager.Close(this);
            }
        }

        public void SetMessage(string t)
        {
            this.text = t;
            if (this.victoryText != null)
            {
                this.victoryText.text = Localization.Get(t, true, Array.Empty<object>());
            }
        }

        protected override void Start()
        {
            this.btKeepPlaying.gameObject.SetActive(false);
            base.Start();
            if (this.text != null)
            {
                string text;
                if (this.victoryText != null)
                {
                    text = this.victoryText.text;
                }
                else
                {
                    TextMeshProUGUI victoryText = this.victoryText;
                    text = null;
                }
                if (string.IsNullOrEmpty(text))
                {
                    this.victoryText.text = Localization.Get(this.text, true, Array.Empty<object>());
                }
            }
            PlayMusic.Play("SOUND_LIST-EMPTY", this);
            AudioLibrary.RequestSFX("Victory", 0f, 0f, 1f);
        }
    }
}

