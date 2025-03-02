using System.Collections;
using UnityEngine;

public class LineTracking : MonoBehaviour
{
    [SerializeField] private GameObject lineFollower;
    [SerializeField] private LineRenderer lineRenderer;

    private float currentY;
    private float nextY;

    [SerializeField] private float currentX;
    [SerializeField] private float nextX;

    [SerializeField] private int currentPosInt;
    private int nextPosInt;

    [SerializeField] private BackgroundMovement backgroundMovement;

    [SerializeField] private bool hasReachedPos = true;

    private float distanceTraveled;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 startPos = new Vector3(lineRenderer.GetPosition(0).x, lineRenderer.GetPosition(0).y, lineFollower.transform.position.z);
        lineFollower.transform.position = startPos;

        Debug.Log(lineRenderer.GetPosition(currentPosInt).x);
    }

    // Update is called once per frame
    void Update()
    {
        FollowLine();
        distanceTraveled = backgroundMovement.GetDistanceTraveled();

    }

    //distance div by shift speed
    private void FollowLine()
    {
        Debug.Log(lineRenderer.positionCount);
        Debug.Log(nextPosInt + 1);
        if (nextPosInt >= lineRenderer.positionCount)
        {
            return;
        }
        if (hasReachedPos)
        {
            currentX = lineRenderer.GetPosition(currentPosInt).x;
            if(nextPosInt < lineRenderer.positionCount)
            nextPosInt = currentPosInt + 1;
            nextX = lineRenderer.GetPosition(nextPosInt).x;

            currentY = lineRenderer.GetPosition(currentPosInt).y;
            nextY = lineRenderer.GetPosition(nextPosInt).y;
        }
        
        float distanceToNextPoint = lineFollower.transform.position.x - (nextX + distanceTraveled);
        //Debug.Log(distanceToNextPoint);
        float distanceX = Mathf.Abs(nextX - currentX);
        float distanceY = Mathf.Abs(nextY - currentY);
        float speed = backgroundMovement.GetShiftSpeed();
        float increment = distanceX / speed;
        float moveY = -(distanceY / increment);
        Vector3 Movedir;
        if (nextY > currentY)
        {
            Movedir = Vector3.down;
            
        }
        else
        {
            Movedir = Vector3.up;
        }
        //Debug.Log(Movedir);
        if (distanceToNextPoint < .05f)
        {
            hasReachedPos = true;
            Debug.Log("hasreachedposition");
        }
        else
        {
            hasReachedPos = false;
        }

        if(hasReachedPos)
        {
            currentPosInt++;
        }
        

        lineFollower.transform.position += Movedir * moveY * Time.deltaTime;

    }

    IEnumerator WaitAndSwitchPosition(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
}
