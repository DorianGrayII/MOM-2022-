using System;
using System.Collections;
using System.Collections.Generic;
using DBUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnrealByte.EasyJira;

namespace MOM
{
    public class BugReport : ScreenBase
    {
        public Button btCancel;

        public Button btSend;

        public TMP_InputField bugTitle;

        public TMP_InputField bugDescription;

        public TextMeshProUGUI labelSendingProgress;

        public Image screenshot;

        public Slider sendingProgress;

        public DropDownFilters category;

        private bool send;

        public override void OnStart()
        {
            base.OnStart();
            this.sendingProgress.minValue = 0f;
            this.sendingProgress.maxValue = 100f;
            this.sendingProgress.value = 0f;
            this.sendingProgress.gameObject.SetActive(value: false);
            this.btSend.interactable = false;
            Texture2D screenShotTexture = BugReportCatcher.screenShotTexture;
            Rect rect = new Rect(0f, 0f, screenShotTexture.width, screenShotTexture.height);
            this.screenshot.sprite = Sprite.Create(screenShotTexture, rect, new Vector2(0.5f, 0.5f), 100f);
            List<string> options = new List<string> { "Crash", "Bug", "Gameplay-Balance", "Gameplay-Other", "Other" };
            this.category.SetOptions(options);
        }

        public override IEnumerator Closing()
        {
            yield return base.Closing();
            global::UnityEngine.Object.Destroy(this.screenshot.sprite);
        }

        public override void UpdateState()
        {
            base.UpdateState();
            if (!string.IsNullOrEmpty(this.bugDescription.text) && !string.IsNullOrEmpty(this.bugTitle.text))
            {
                this.btSend.interactable = true;
            }
            else
            {
                this.btSend.interactable = false;
            }
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btCancel)
            {
                UIManager.Close(this);
            }
            else if (s == this.btSend)
            {
                JiraObject component = base.gameObject.GetComponent<JiraObject>();
                string text = "v. " + GameVersion.GetGameVersion();
                string selection = this.category.GetSelection();
                if (JiraObject.jiraInstance != null)
                {
                    this.btSend.interactable = false;
                    string title = "[" + selection + " " + text + "] " + this.bugTitle.text;
                    string text2 = (TLog.Get().usedDevMenu ? ("*DEV MENU* used in the app" + Environment.NewLine) : "");
                    string text3 = ((GameManager.instance != null && GameManager.instance.usedDevMenuInThisGame) ? ("*DEV MENU* modified this game" + Environment.NewLine) : "");
                    string description = "*Version:* " + GameVersion.GetGameVersionFull() + Environment.NewLine + text2 + text3 + Environment.NewLine + "*Comments:* " + Environment.NewLine + this.bugDescription.text + Environment.NewLine + Environment.NewLine + "*Top log:* " + Environment.NewLine + TLog.firstCritical;
                    component.SendForm(title, description);
                    this.send = true;
                }
            }
        }

        private void Update()
        {
            if (this.send)
            {
                int uploadProgress = JiraConnect.uploadProgress;
                this.sendingProgress.gameObject.SetActive(value: true);
                this.sendingProgress.value = uploadProgress;
                this.labelSendingProgress.text = Localization.Get("UI_SENDING", true) + " " + uploadProgress + "%";
            }
        }
    }
}
