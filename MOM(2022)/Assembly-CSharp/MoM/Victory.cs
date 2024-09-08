using DBUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine.UI;

namespace MOM
{
    public class Victory : ScreenBase
    {
        public TextMeshProUGUI victoryText;

        public Button btEndGame;

        public Button btKeepPlaying;

        public string text;

        protected override void Start()
        {
            this.btKeepPlaying.gameObject.SetActive(value: false);
            base.Start();
            if (this.text != null && string.IsNullOrEmpty(this.victoryText?.text))
            {
                this.victoryText.text = Localization.Get(this.text, true);
            }
            PlayMusic.Play("SOUND_LIST-EMPTY", this);
            AudioLibrary.RequestSFX("Victory");
        }

        public void SetMessage(string t)
        {
            this.text = t;
            if (this.victoryText != null)
            {
                this.victoryText.text = Localization.Get(t, true);
            }
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (this.btEndGame == s)
            {
                TurnManager.StopTurnLoop();
                HallOfFame.Popup(endGame: true);
                UIManager.Close(this);
            }
            if (this.btKeepPlaying == s)
            {
                TurnManager.StopTurnLoop();
                HallOfFame.Popup(endGame: false);
                UIManager.Close(this);
            }
        }
    }
}
