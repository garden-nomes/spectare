using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// loosely based on https://github.com/prime31/CharacterController2D
public class MovementController : MonoBehaviour
{
	public event Action<Collider2D> OnLand;

	[SerializeField] private LayerMask platformLayer;
	[SerializeField] private LayerMask oneWayPlatformLayer;

	[Range(2, 20)][SerializeField] private int horizontalRays = 8;
	[Range(2, 20)][SerializeField] private int verticalRays = 4;
	[SerializeField] private float skinWidth = 0.02f;

	private BoxCollider2D boxCollider;
	private bool isGrounded = false;
	public bool IsGrounded => isGrounded;
	public bool isLeftTouching = false;
	public bool IsLeftTouching => isLeftTouching;
	public bool isRightTouching = false;
	public bool IsRightTouching => isRightTouching;

	void DrawRay(Vector3 start, Vector3 dir, Color color)
	{
		Debug.DrawRay(start, dir, color);
	}

	void Awake()
	{
		boxCollider = GetComponent<BoxCollider2D>();
	}

	public Vector2 Move(Vector2 delta)
	{
		isGrounded = false;
		isLeftTouching = false;
		isRightTouching = false;

		if (delta.x != 0f)
			delta = CastHorizontally(delta);

		if (delta.y != 0f)
			delta = CastVertically(delta);

		transform.Translate(delta);
		return delta;
	}

	private Vector3 CastHorizontally(Vector3 delta)
	{
		bool isGoingRight = delta.x > 0f;
		float rayDistance = Mathf.Abs(delta.x) + skinWidth;
		Vector2 rayDirection = isGoingRight ? Vector2.right : Vector2.left;
		float distanceBetweenRays = (boxCollider.bounds.size.y - skinWidth * 2f) / ((float) horizontalRays - 1);
		var layer = platformLayer & ~oneWayPlatformLayer;

		Vector2 origin = new Vector2(
			isGoingRight ? boxCollider.bounds.max.x - skinWidth : boxCollider.bounds.min.x + skinWidth,
			boxCollider.bounds.min.y + skinWidth);

		for (int i = 0; i < horizontalRays; i++)
		{
			Vector2 ray = origin + Vector2.up * distanceBetweenRays * i;

			DrawRay(ray, rayDirection * rayDistance, Color.red);
			RaycastHit2D hit = Physics2D.Raycast(ray, rayDirection, rayDistance, layer);

			if (hit)
			{
				rayDistance = Mathf.Abs(hit.point.x - ray.x);
				delta.x = hit.point.x - ray.x + (isGoingRight ? -skinWidth : skinWidth);

				if (isGoingRight)
					isRightTouching = true;
				else
					isLeftTouching = true;

				if (rayDistance < skinWidth + float.Epsilon)
					break;
			}
		}

		return delta;
	}

	private Vector3 CastVertically(Vector3 delta)
	{
		bool isGoingUp = delta.y > 0f;
		float rayDistance = Mathf.Abs(delta.y) + skinWidth;
		Vector2 rayDirection = isGoingUp ? Vector2.up : Vector2.down;
		float distanceBetweenRays = (boxCollider.bounds.size.x - skinWidth * 2f) / ((float) verticalRays - 1);

		var layer = platformLayer;
		if (isGoingUp)
			layer &= ~oneWayPlatformLayer;

		Vector2 origin = new Vector2(
			boxCollider.bounds.min.x + skinWidth,
			isGoingUp ? boxCollider.bounds.max.y - skinWidth : boxCollider.bounds.min.y + skinWidth);

		for (int i = 0; i < verticalRays; i++)
		{
			Vector2 ray = origin + Vector2.right * distanceBetweenRays * i;

			DrawRay(ray, rayDirection * rayDistance, Color.red);
			RaycastHit2D hit = Physics2D.Raycast(ray, rayDirection, rayDistance, layer);

			if (hit)
			{
				rayDistance = Mathf.Abs(hit.point.y - ray.y);
				delta.y = hit.point.y - ray.y + (isGoingUp ? -skinWidth : skinWidth);

				if (!isGoingUp)
				{
					isGrounded = true;
					OnLand?.Invoke(hit.collider);
				}

				if (rayDistance < skinWidth + float.Epsilon)
					break;
			}
		}

		return delta;
	}
}
