
using UnityEngine;

public class MosueSpawner : MonoBehaviour
{
	private GameObject sample;

	public float interval = 5f;
	private float elapsedTime = 0f; 

	void Start()
	{
		sample = Resources.Load<GameObject>("MOUSE");
		interval = Random.Range(20f, 30f);
	}

	
	void Update()
	{
		GameObject clone;
		if (elapsedTime >= interval)
		{
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
