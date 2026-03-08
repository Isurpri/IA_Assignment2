
using UnityEngine;

public class DustSpawner : MonoBehaviour
{
	private GameObject sample;

	public float interval = 5f;

	private float elapsedTime = 0f;

	void Start()
	{
		sample = Resources.Load<GameObject>("DUST");
	}

	void Update()
	{
		GameObject clone;
		if (elapsedTime >= interval)
		{
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
