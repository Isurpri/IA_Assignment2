
using UnityEngine;

public class DustSpawner : MonoBehaviour
{
	private GameObject sample;

	public float interval = 5f; // one seed every interval seconds

	private float elapsedTime = 0f; // time elapsed since last generation

	void Start()
	{
		sample = Resources.Load<GameObject>("DUST");
	}

	// Update is called once per frame
	void Update()
	{
		GameObject clone;
		if (elapsedTime >= interval)
		{
			// spawn creating an instance...
			clone = Instantiate(sample);
			clone.transform.position = LocationHelper.RandomWalkableLocation();
			clone.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
			elapsedTime = 0;
		}
		else
		{
			elapsedTime += Time.deltaTime;
		}

	}
}
