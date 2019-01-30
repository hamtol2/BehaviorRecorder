using UnityEngine;
using UnityEngine.Events;

public class MOCCAEventListener : MonoBehaviour
{
    public MOCCAEvent Event;
    public UnityEvent response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        response.Invoke();
    }
}