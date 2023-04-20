using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeManager : MonoBehaviour
{
    public static SwipeManager instance;

    public delegate void SwipeDelegate(bool[] swipes);
    public SwipeDelegate SwipeEvent;

    public delegate void ClickDelegate(Vector2 position);
    public ClickDelegate ClickEvent;

    public enum Direction { Left, Right, Up, Down };
    bool[] swipe = new bool[4];

    const float SWIPE_THREASHOLD = 50;

    Vector2 startTouch;
    bool touchMoved;
    Vector3 swipeDelta;

    Vector2 TouchPosition() { return (Vector2)Input.mousePosition; }
    bool TouchBegan() { return Input.GetMouseButtonDown(0); }
    bool TouchEnded() { return Input.GetMouseButtonUp(0); }
    bool GetTouch() { return Input.GetMouseButton(0); }

    private void Awake()
    {
        instance = this;
    }


    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        // START / FINISH
        if (TouchBegan())
        {
            startTouch = TouchPosition();
            touchMoved = true;
        }
        else if (TouchEnded() && touchMoved == true)
        {
            SendSwipe();
            touchMoved = false;
        }

        // CALC DISTANCE
        swipeDelta = Vector2.zero;
        if (touchMoved && GetTouch())
        {
            swipeDelta = TouchPosition() - startTouch;
        }

        // CHECK SWIPE
        if (swipeDelta.magnitude > SWIPE_THREASHOLD)
        {
            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                // LEFT / RIGHT
                swipe[(int)Direction.Left] = swipeDelta.x < 0;
                swipe[(int)Direction.Right] = swipeDelta.x > 0;
            }
            else
            {
                // UP / DOWN
                swipe[(int)Direction.Down] = swipeDelta.y < 0;
                swipe[(int)Direction.Up] = swipeDelta.y > 0;
            }

            SendSwipe();
        }
    }

    private void SendSwipe()
    {
        if (swipe[0] || swipe[1] || swipe[2] || swipe[3])
        {
            Debug.Log(swipe[0] + "|" + swipe[1] + "|" + swipe[2] + "|" + swipe[3]);
            SwipeEvent?.Invoke(swipe);
        }
        else
        {
            Debug.Log("Click");
        }
        Reset();
    }

    private void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        touchMoved = false;
        for (int i = 0; i < 4; i++) swipe[i] = false;
    }
}
