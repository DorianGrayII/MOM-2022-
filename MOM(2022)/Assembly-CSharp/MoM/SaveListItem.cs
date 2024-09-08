namespace MOM
{
    using MHUtils.UI;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class SaveListItem : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
    {
        public TextMeshProUGUI labelSaveName;
        public TextMeshProUGUI labelSaveDate;
        public TextMeshProUGUI labelWizardName;
        public TextMeshProUGUI labelTurn;
        public Button btDeleteSave;
        public GameObject warning;
        public GameObject info;
        public GameObject dlc0;
        public GameObject dlc1;
        public GameObject dlc2;
        public GameObject dlc3;
        private bool isSaveMode;
        private SaveMeta sm;
        private float lastClikTime;
        private SaveGame screen;

        public void OnPointerClick(PointerEventData eventData)
        {
            this.screen = UIManager.GetScreen<SaveGame>(UIManager.Layer.Standard);
            if (this.screen == null)
            {
                this.screen = UIManager.GetScreen<SaveGame>(UIManager.Layer.Popup);
            }
            if (!this.isSaveMode && this.warning.activeInHierarchy)
            {
                this.screen.btSave.interactable = false;
            }
            else
            {
                this.screen.btSave.interactable = true;
                if (!this.isSaveMode)
                {
                    float realtimeSinceStartup = Time.realtimeSinceStartup;
                    if ((realtimeSinceStartup - this.lastClikTime) < 0.5f)
                    {
                        if (this.screen == null)
                        {
                            this.screen = UIManager.GetScreen<SaveGame>(UIManager.Layer.Popup);
                        }
                        if (this.screen == null)
                        {
                            Debug.LogWarning("SaveScreen is null");
                        }
                        this.screen.gridSaves.SelectListItem(this);
                        SaveGame.LoadSavedGame(this.sm);
                    }
                    else
                    {
                        this.lastClikTime = realtimeSinceStartup;
                    }
                }
            }
        }

        public void SetSaveMeta(SaveMeta saveMeta)
        {
            this.sm = saveMeta;
        }

        public void SetSaveMode(bool isSave)
        {
            this.isSaveMode = isSave;
        }
    }
}

