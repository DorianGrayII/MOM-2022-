using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialDraggable : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
    public Transform target;
    private bool isMouseDown;
    private Vector3 startMousePosition;
    private Vector3 startPosition;
    public bool shouldReturn;

    public void OnPointerDown(PointerEventData dt)
    {
        this.isMouseDown = true;
        this.startPosition = this.target.position;
        this.startMousePosition = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData dt)
    {
        this.isMouseDown = false;
        if (this.shouldReturn)
        {
            this.target.position = this.startPosition;
        }
    }

    private void Update()
    {
        if (this.isMouseDown)
        {
            Vector3 vector = Input.mousePosition - this.startMousePosition;
            Vector3 vector2 = this.startPosition + vector;
            this.target.position = vector2;
        }
    }
}

