using System.Collections;
using System.Collections.Generic;
using MHUtils;
using MHUtils.UI;
using MOM.Adventures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class EventEditorImage : ScreenBase
    {
        public GridItemManager gridItemManager;

        public RawImage imagePreview;

        public TextMeshProUGUI tfFileName;

        public TextMeshProUGUI tfImageSize;

        public Button buttonConfirm;

        public Button buttonCancel;

        public Button buttonLeft;

        public Button buttonRight;

        public TMP_InputField inputField;

        private Callback confirm;

        private Callback cancel;

        private List<Texture2D> textures;

        private string selectedImage;

        private Coroutine coroutine;

        protected override void Awake()
        {
            this.gridItemManager.CustomDynamicItem(GridItem, UpdateGrid);
            this.gridItemManager.onSelectionChange = OnToggleChange;
            this.coroutine = base.StartCoroutine(ImageLibrary.InitializeFullLibrary());
            this.textures = new List<Texture2D>(ImageLibrary.GetImageDictionary().Values);
        }

        public override IEnumerator PreStart()
        {
            this.UpdateGrid();
            yield return base.PreStart();
        }

        private void GridItem(GameObject itemSource, object source, object data, int index)
        {
            RawImage rawImage = GameObjectUtils.FindByNameGetComponent<RawImage>(itemSource, "Image");
            TMP_Text tMP_Text = GameObjectUtils.FindByNameGetComponent<TMP_Text>(itemSource, "Name");
            Texture2D texture2D = (Texture2D)(rawImage.texture = source as Texture2D);
            tMP_Text.text = texture2D.name;
        }

        public static void OpenImageSelection(ScreenBase parent, Callback confirm = null, Callback cancel = null)
        {
            EventEditorImage eventEditorImage = UIManager.Open<EventEditorImage>(UIManager.Layer.Popup, parent);
            eventEditorImage.confirm = confirm;
            eventEditorImage.cancel = cancel;
        }

        private void OnToggleChange(object o)
        {
            Texture2D selectedObject = this.gridItemManager.GetSelectedObject<Texture2D>();
            if (selectedObject != null)
            {
                this.selectedImage = selectedObject.name;
            }
            this.UpdateLarge();
        }

        private void UpdateGrid()
        {
            this.gridItemManager.UpdateGrid(this.textures);
        }

        private void UpdateLarge()
        {
            Texture2D texture2D = this.textures.Find((Texture2D o) => o.name == this.selectedImage);
            this.imagePreview.texture = texture2D;
            if (texture2D != null)
            {
                this.tfFileName.text = texture2D.name;
            }
            else
            {
                this.tfFileName.text = "";
            }
            if (texture2D != null)
            {
                this.tfImageSize.text = texture2D.width + "x" + texture2D.height;
            }
            else
            {
                this.tfImageSize.text = "";
            }
        }

        private void UpdateScreen()
        {
            this.gridItemManager.UpdateGrid(this.textures);
            this.UpdateLarge();
        }

        private void UpdateByFilter()
        {
            this.gridItemManager.Unselect();
            this.UpdateScreen();
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.buttonCancel)
            {
                UIManager.Close(this);
                if (this.cancel != null)
                {
                    this.cancel(null);
                }
            }
            else
            {
                if (!(s == this.buttonConfirm))
                {
                    return;
                }
                Texture2D texture2D = this.textures.Find((Texture2D o) => o.name == this.selectedImage);
                if (texture2D != null)
                {
                    if (this.confirm != null)
                    {
                        this.confirm(texture2D);
                    }
                }
                else if (this.confirm != null)
                {
                    this.confirm(null);
                }
                UIManager.Close(this);
            }
        }
    }
}
