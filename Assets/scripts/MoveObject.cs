using UnityEngine;

[DisallowMultipleComponent]
public class MoveObject : MonoBehaviour
{
    [SerializeField] private Vector3 movePosition = new Vector3(0, 35, 0);
    [SerializeField] [Range(0,1)] private float moveProgress;
    private Vector3 startPosition;
    [SerializeField] private float moveSpeed = 0.3f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        moveProgress = Mathf.PingPong(Time.time * moveSpeed, 1);

        Vector3 offset = movePosition * moveProgress;
        transform.position = startPosition + offset;
    }
}
