using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.LinearAlgebra
{
	public static class UVector
	{
		public static float ToAngle(this Vector2 direction)
		{
			return Vector2.SignedAngle(Vector2.right, direction);
		}

		public static Vector2 AngleToVector2(float angle)
		{
			Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			return direction.normalized;
		}

		public static float DistanceXZ(Vector3 a, Vector3 b) { return Mathf.Sqrt(SqrDistanceXZ(a, b)); }

		public static float SqrDistanceXZ(Vector3 a, Vector3 b) { return Mathf.Pow((a.x - b.x), 2) + Mathf.Pow((a.z - b.z), 2); }

		public static Vector3 GetDirectionXZ(Vector3 fromPosition, Vector3 targetPosition)
		{
			return GetXZ(targetPosition - fromPosition).normalized;
		}

		public static Vector3 GetXZ(this Vector3 vector)
		{
			return Vector3.right * vector.x + Vector3.forward * vector.z;
		}

		public static Vector2 ElementWiseProduct(Vector2 a, Vector2 b)
		{
			return New(a.x * b.x, a.y * b.y);
		}

		public static Vector3 ElementWiseProduct(Vector3 a, Vector3 b)
		{
			return New(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public static Vector2 New(float x, float y)
		{
			return New(x, y, 0f);
		}

		public static Vector3 New(float x, float y, float z)
		{
			return Vector3.right * x + Vector3.up * y + Vector3.forward * z;
		}
	}
}
