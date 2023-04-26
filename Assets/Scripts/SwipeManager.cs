using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeManager : MonoBehaviourSingleton<SwipeManager> {

    public delegate void SwipeDelegate(bool[] swipes);

    public SwipeDelegate SwipeEvent;

    public delegate void ClickDelegate(Vector2 position);

    public ClickDelegate ClickEvent;

    public bool canSendSwipe = false;

    public enum Direction { Left, Right, Up, Down };

    private bool[] swipe = new bool[4];
    private const float SWIPE_THREASHOLD = 50;
    private Vector2 startTouch;
    private bool touchMoved;
    private Vector3 swipeDelta;

    private Vector2 TouchPosition()
    { return (Vector2)Input.mousePosition; }

    private bool TouchBegan()
    { return Input.GetMouseButtonDown(0); }

    private bool TouchEnded()
    { return Input.GetMouseButtonUp(0); }

    private bool GetTouch()
    { return Input.GetMouseButton(0); }

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
            if (canSendSwipe) SwipeEvent?.Invoke(swipe);
        }
        else
        {
            ClickEvent?.Invoke(Vector3.zero);
            if (GameManager.Instance.gameStarted) return;
            GameManager.Instance.StartGame();
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