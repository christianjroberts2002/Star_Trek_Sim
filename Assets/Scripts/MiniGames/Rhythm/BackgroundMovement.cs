using UnityEngine;

public class BackgroundMovement: MonoBehaviour
{
    Transform backgroundPos;
    [SerializeField] float shiftSpeed = 1f;

    private Vector3 startPos;

    private float distanceTraveled;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        BackgroundShift();
        distanceTraveled = Mathf.Abs(transform.position.x - startPos.x);
    }

    private void BackgroundShift()
    {
        transform.position += Vector3.right * shiftSpeed * Time.deltaTime;
    }

    public float GetShiftSpeed()
    {
        return shiftSpeed;
    }

    public float GetDistanceTraveled()
    {
        return  distanceTraveled;
    }

}
