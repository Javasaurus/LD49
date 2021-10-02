using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState instance;
    private void Awake()
    {
        if (instance != null)
        {
            GameObject.Destroy(instance.gameObject);
        }
        instance = this;
    }
    public enum State
    {
        PAUSED, CAMERA, PLACING, WALKING, PHYSICS
    }
    [SerializeField]
    private State _currentState;
    [SerializeField]
    private State _previousState;
    public State currentState
    {
        set { _previousState = _currentState; _currentState = value; }
        get { return _currentState; }
    }

    public void ResetState()
    {
        currentState = _previousState;
    }





}
