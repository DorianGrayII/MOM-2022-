using System;
using UnityEngine;
using UnityEngine.Events;
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

    private void Less()
    {
        this.slider.value -= this.delta;
    }

    private void More()
    {
        this.slider.value += this.delta;
    }

    private void OnDisable()
    {
        this.LessButton.onClick.RemoveListener(new UnityAction(this.Less));
        this.MoreButton.onClick.RemoveListener(new UnityAction(this.More));
        this.slider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnValueChanged));
    }

    private void OnEnable()
    {
        this.LessButton.onClick.AddListener(new UnityAction(this.Less));
        this.MoreButton.onClick.AddListener(new UnityAction(this.More));
        this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnValueChanged));
        this.OnValueChanged(this.slider.value);
    }

    private void OnValueChanged(float value)
    {
        this.LessButton.interactable = value > this.slider.minValue;
        this.MoreButton.interactable = value < this.slider.maxValue;
    }
}

