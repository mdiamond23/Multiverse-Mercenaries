using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public List<Transform> waypoints;
    public float moveSpeed;
    public int target;
    public float waitSeconds = .5f;

    private PlatformEffector2D effector;
    private bool playerNearPlatformer;
    private float savedSurfaceArc;

    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
        playerNearPlatformer = false;
        savedSurfaceArc = effector.surfaceArc;
    }
   void Update()
   {
    transform.position = Vector3.MoveTowards(transform.position, waypoints[target].position, moveSpeed * Time.deltaTime);
   }
   private void FixedUpdate()
   {
    if(transform.position == waypoints[target].position){
        if(target == waypoints.Count-1){
            target = 0;
        }
        else{
            target+=1;
        }
    }
   }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "Player")
        {
            playerNearPlatformer = true;
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.name == "Player")
        {
            playerNearPlatformer = false;
            other.transform.SetParent(null);
        }
    }

    IEnumerator Disable()
    {
        yield return new WaitForSeconds(waitSeconds);
        effector.surfaceArc = savedSurfaceArc;
    }  
}
