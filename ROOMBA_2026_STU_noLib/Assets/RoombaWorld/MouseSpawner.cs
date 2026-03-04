
using UnityEngine;

public class MosueSpawner : MonoBehaviour
{
	private GameObject sample;

	public float interval = 5f;// one seed every interval seconds

	private float elapsedTime = 0f; // time elapsed since last generation

	void Start()
	{
		sample = Resources.Load<GameObject>("MOUSE");
		interval = Random.Range(20f, 30f);
	}

	// Update is called once per frame
	void Update()
	{
		GameObject clone;
		if (elapsedTime >= interval)
		{
			// spawn creating an instance...
			clone = Instantiate(sample);
			clone.transform.position = LocationHelper.RandomEntryExitPoint().transform.position;
			elapsedTime = 0;
			interval = Random.Range(20f, 30f);
		}
		else
		{
			elapsedTime += Time.deltaTime;
		}

	}
}
