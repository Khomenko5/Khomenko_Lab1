using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public delegate void ZoneClickedHandler();
    public event ZoneClickedHandler OnZoneClicked;

    private void OnMouseDown()
    {
        OnZoneClicked?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}