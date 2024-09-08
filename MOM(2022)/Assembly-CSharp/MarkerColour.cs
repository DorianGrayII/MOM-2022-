using UnityEngine;
using UnityEngine.UI;

public class MarkerColour : MonoBehaviour
{
    public Image colouredImage;

    public Color PColor
    {
        get
        {
            return this.colouredImage.color;
        }
        set
        {
            this.colouredImage.color = value;
        }
    }
}
