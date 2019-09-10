using UnityEngine;

public class PlatformMover : MonoBehaviour
{

	public MovingPlatform platform;
	public Transform[] waypoints;

	public enum MovementType
	{
		CYCLE,
		OSCILATE
	}

	public MovementType movementType;

	public float speed = 1f;

	int lastWaypoint = 0;
	int nextWaypoint = 0;

	int GetRealWaypointIndex (int waypoint)
	{
		if (movementType == MovementType.OSCILATE) {
			int forward = waypoint % (waypoints.Length - 1);
			int backward = waypoints.Length - 1 - forward;
			int reversing = Mathf.FloorToInt (waypoint / (waypoints.Length - 1f)) % 2;
			return reversing * backward + (1 - reversing) * forward;
		} else {
			return waypoint % waypoints.Length;
		}
	}

	Transform GetWaypoint (int index)
	{
		return waypoints [GetRealWaypointIndex (index)];
	}

	void Update ()
	{
		Vector3 origin = GetWaypoint (lastWaypoint).position;
		Vector3 dest = GetWaypoint (nextWaypoint).position;

		Vector3 increment = (dest - origin).normalized * speed * platform.myDeltaTime;

		platform.transform.position = platform.transform.position + increment;

		float proportion = GetJourneyProportion (origin, dest, platform.transform.position);
		if (proportion >= 1f) {
			lastWaypoint = nextWaypoint++;
			platform.transform.position = dest;
		}


	}

	float GetJourneyProportion (Vector3 origin, Vector3 dest, Vector3 pos)
	{
		if (origin == dest) {
			return 1f;
		}
		Vector3 journey = dest - origin;
		Vector3 completed = pos - origin;
		return completed.magnitude / journey.magnitude;
	}

}
