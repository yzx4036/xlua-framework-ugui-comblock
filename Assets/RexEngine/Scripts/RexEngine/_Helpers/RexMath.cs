/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;

public class RexMath:MonoBehaviour 
{
	public static float Percentage(int lower, int higher)
	{
		float percentage = (float)lower / (float)higher;

		return percentage;
	}

	public static bool DoCollidersOverlap(BoxCollider2D col1, BoxCollider2D col2)
	{
		Rect r1 = new Rect(col1.transform.position.x + col1.offset.x - col1.size.x / 2, col1.transform.position.y + col1.offset.y - col1.size.y / 2, col1.size.x, col1.size.y);
		Rect r2 = new Rect(col2.transform.position.x + col2.offset.x - col2.size.x / 2, col2.transform.position.y + col2.offset.y - col2.size.y / 2, col2.size.x, col2.size.y);

		return r1.Overlaps(r2);
	}

	public static Vector2 GetColliderOverlapCenter(BoxCollider2D col1, BoxCollider2D col2)
	{
		Rect r1 = new Rect(col1.transform.position.x + col1.offset.x - col1.size.x / 2, col1.transform.position.y + col1.offset.y - col1.size.y / 2, col1.size.x, col1.size.y);
		Rect r2 = new Rect(col2.transform.position.x + col2.offset.x - col2.size.x / 2, col2.transform.position.y + col2.offset.y - col2.size.y / 2, col2.size.x, col2.size.y);
		Rect area = new Rect();
		
		float x1 = Mathf.Min(r1.xMax, r2.xMax);
		float x2 = Mathf.Max(r1.xMin, r2.xMin);
		float y1 = Mathf.Min(r1.yMax, r2.yMax);
		float y2 = Mathf.Max(r1.yMin, r2.yMin);
		area.x = Mathf.Min(x1, x2);
		area.y = Mathf.Min(y1, y2);
		area.width = Mathf.Max(0.0f, x1 - x2);
		area.height = Mathf.Max(0.0f, y1 - y2);
		
		return area.center;
	}

    public static float AngleFromPoint(Vector3 transform, Vector2 point)
	{
		float angle = Mathf.Atan2(point.y - transform.y, point.x - transform.x);
		angle = angle * (180/Mathf.PI);

		return angle;
	}

	public static float AngleFromVelocity(Vector3 transform, Vector2 velocity)
	{
		Vector2 pointFromVelocity = new Vector2(transform.x + velocity.x, transform.y + velocity.y);

		return AngleFromPoint(transform, pointFromVelocity);
	}

	public static Vector2 VelocityRatioFromPoint(Vector3 transform, Vector2 point)
	{
		Vector2 velocity;
		velocity.x = point.x - transform.x;
		velocity.y = point.y - transform.y;

		float totalVelocity = Mathf.Abs(velocity.x) + Mathf.Abs(velocity.y);
		Vector2 velocityRatio;
		velocityRatio.x = Mathf.Abs(velocity.x) / totalVelocity;
		velocityRatio.y = 1.0f - velocityRatio.x;

		if(velocity.x < 0)
		{
			velocityRatio.x *= -1;
		}
		if(velocity.y < 0)
		{
			velocityRatio.y *= -1;
		}

		return velocityRatio;
	}

	public static Vector2 VelocityRatioFromAngle(float angle)
	{
		float length = 10.0f;
		Vector2 velocityRatio;
		Vector2 triangle = new Vector2(0, 0);
		triangle.x = Mathf.Sin(angle * 2.0f * Mathf.PI / 360.0f) * length;
		triangle.y = length * Mathf.Cos(angle * 2.0f * Mathf.PI / 360.0f);
		
		float totalTriangle = Mathf.Abs(triangle.x) + Mathf.Abs(triangle.y);
		velocityRatio.y = triangle.x / totalTriangle;
		velocityRatio.x = triangle.y / totalTriangle;

		return velocityRatio;
	}

	public static int RandomInt(int minNumber = 0, int maxNumber = 1)
	{
		int adjustedMin = minNumber;
		int adjustedMax = maxNumber;
	
		adjustedMax += 1;
		if(minNumber < 0)
		{
			adjustedMin = 0;
			adjustedMax += Mathf.Abs(minNumber);
		}
		int randomNumber = Random.Range(adjustedMin, adjustedMax);
		if(minNumber < 0)
		{
			randomNumber -= Mathf.Abs(minNumber);
		}

		return randomNumber;
	}

	public static float RandomFloat(float minNumber, float maxNumber)
	{
		float adjustedMin = minNumber;
		float adjustedMax = maxNumber;

		if(minNumber < 0.0f)
		{
			adjustedMin = 0.0f;
			adjustedMax += Mathf.Abs(minNumber);
		}
		float randomNumber = Random.Range(adjustedMin, adjustedMax);
		if(minNumber < 0.0f)
		{
			randomNumber -= Mathf.Abs(minNumber);
		}

		return (float)(Mathf.Round(randomNumber * 100.0f) / 100.0f);
	}

	public static bool RandomBool()
	{
		int randomNumber = Random.Range(0, 2);
		return (randomNumber == 0) ? true : false;
	}

	public static bool RandomBoolWithProbability(int percentageChanceForTrue)
	{
		int randomNumber = Random.Range(0, 100);
		return (randomNumber <= percentageChanceForTrue) ? true : false;
	}
}
