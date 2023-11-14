using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideRail : MonoBehaviour
{
    [SerializeField] GripRail gripRail;
    [SerializeField] float waitDelay;
    [SerializeField] float rideDuration;
    float rideRatio = 0;
    int i = 0;
    
    private void Start()
    {
        StartCoroutine(Ride());
    }

    IEnumerator Ride()
    {
        
        while (true)
        {
            if (rideRatio == rideDuration)
            {
                yield return new WaitForSeconds(waitDelay);
                rideRatio = 0;
                i = (i + 1) % gripRail.corners.Length;
            }
            yield return null;
            rideRatio += Time.deltaTime;
            if (rideRatio >= rideDuration) rideRatio = rideDuration;
            gripRail.gripPoint.transform.position = (1 - (rideRatio / rideDuration)) * gripRail.corners[i].transform.position + (rideRatio / rideDuration) * gripRail.corners[(i+1)%gripRail.corners.Length].transform.position - new Vector3(0, 0, 0.5f); ;

        }
    }
    


}
