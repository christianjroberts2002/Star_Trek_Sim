using UnityEngine;

public class LineCreationScript : MonoBehaviour
{

    LineRenderer lineRenderer;

    private int RandomIndecies;

    float randomX;
    float randomY;
    float randomZ = 0;

    [SerializeField] private int randMin;
    [SerializeField] private int randMax;

    [SerializeField] private float randXMin;
    [SerializeField] private float randXMax;

    [SerializeField] private float randYMin;
    [SerializeField] private float randYMax;

    private float currentX;
    private float currentY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        RandomIndecies = Random.Range(randMin, randMax);

        lineRenderer.positionCount = RandomIndecies;
        currentX = 8;
        currentY = 0;
        lineRenderer.SetPosition(0, new Vector3(currentX, currentY, 0));
        
        for (int i = 1; i < RandomIndecies; ++i)
        {
            randomX = Random.Range(randXMin, randXMax);
            randomY = Random.Range(randYMin, randYMax);

            randomX += currentX;
            randomY += currentY;

            currentX = randomX;
            //currentY = randomY;
            Vector3 thisPointVector = new Vector3(randomX, randomY, randomZ);

            lineRenderer.SetPosition(i, thisPointVector);


            
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
