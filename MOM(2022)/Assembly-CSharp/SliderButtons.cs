using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderButtons : MonoBehaviour
{
    public Button LessButton;

    public Button MoreButton;

    [Tooltip("The amount to incremen / decrement by when pressing the buttons")]
    public float delta = 1f;

    private Slider slider;

    private void Awake()
    {
        this.slider = base.GetComponent<Slider>();
    }

    private void OnEnable()
    {
        this.LessButton.onClick.AddListener(Less);
        this.MoreButton.onClick.AddListener(More);
        this.slider.onValueChanged.AddListener(OnValueChanged);
        this.OnValueChanged(this.slider.value);
    }

    private void More()
    {
        this.slider.value += this.delta;
    }

    private void Less()
    {
        this.slider.value -= this.delta;
    }

    private void OnDisable()
    {
        this.LessButton.onClick.RemoveListener(Less);
        this.MoreButton.onClick.RemoveListener(More);
        this.slider.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(float value)
    {
        this.LessButton.interactable = value > this.slider.minValue;
        this.MoreButton.interactable = value < this.slider.maxValue;
    }
}
