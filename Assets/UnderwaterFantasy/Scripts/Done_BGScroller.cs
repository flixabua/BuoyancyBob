// scrolls a quad object
using UnityEngine;
using System.Collections;

public class Done_BGScroller : MonoBehaviour
{
	public float scrollSpeed;
	public float tileWidth;
	public bool repeat;
	private Vector3 startPosition;
	private bool movingRight;

	void Start ()
	{
		startPosition = transform.position;
		movingRight = true;
	}

	void Update ()
	{
		if (!repeat)
		{
			float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileWidth);
			transform.position = startPosition + Vector3.left * newPosition;
		}
		else
		{
			if (movingRight)
			{
				float newPosition = Time.deltaTime * scrollSpeed;
				transform.position += Vector3.left * newPosition;
				if (transform.position.x < -tileWidth) movingRight = false;
			}
			else
			{
				float newPosition = Time.deltaTime * scrollSpeed;
				transform.position += Vector3.right * newPosition;
				if (transform.position.x > tileWidth) movingRight = true;
			}


		}
	}
}