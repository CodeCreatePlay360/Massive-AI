using UnityEngine;


namespace CodeCreatePlay.Utils
{
	public static class CollisionDetection
	{
		/// <summary>
		/// 
		/// </summary>
		public static bool LineLine(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out Vector3 intersectionPoint)
		{
			intersectionPoint = Vector3.zero;

			// Calculate the distance to intersection point
			float uA = ((p4.x - p3.x) * (p1.z - p3.z) - (p4.z - p3.z) * (p1.x - p3.x)) / ((p4.z - p3.z) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.z - p1.z));
			float uB = ((p2.x - p1.x) * (p1.z - p3.z) - (p2.z - p1.z) * (p1.x - p3.x)) / ((p4.z - p3.z) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.z - p1.z));

			// If uA and uB are between 0-1, lines are colliding
			if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
			{
				// Calculate the intersection point
				intersectionPoint = new Vector3(p1.x + (uA * (p2.x - p1.x)), p1.y, p1.z + (uA * (p2.z - p1.z)));
				return true;
			}

			return false;
		}


		/// <summary>
		/// Sphere into sphere collision test.
		/// </summary>
		public static bool SphereCollision(float r1, Vector3 p1, float r2, Vector3 p2)
		{
			if (Vector3.Dot(p2 - p1, p2 - p1) < (r1 + r2) * (r1 + r2))
				return true;

			return false;
		}
		
		
		/// <summary>
		/// Returns true if point is withing 2D bounds given bottom left and top right coordinates.
		/// </summary>
		public static bool IsPosWithinBounds2d(Vector3 btmLeftPoint, Vector3 topRightPoint, Vector3 pos)
		{
			return (pos.x > btmLeftPoint.x && pos.x < topRightPoint.x) && (pos.z > btmLeftPoint.z && pos.z < topRightPoint.z);
		}
	}


	public static class Maths
	{
		public static Quaternion RandQuat(Vector3 originalRotation)
		{
			return Quaternion.Euler(originalRotation.x, Random.Range(originalRotation.y, 360f), originalRotation.z);
		}


		public static float GetNormalizedValue(float value, float min_val, float max_val)
		{
			return (value - min_val) / (max_val - min_val);
		}
		
		
		/// <summary>
		/// Converts a value to 0-1 range.
		/// </summary>
		public static float ConvertToRange(float oldMin, float oldMax, float newMin, float newMax, float value)
		{
			if ((oldMax-oldMin) == 0)
				return newMin;
			else
				return (((value - oldMin) * (newMax - newMin)) / (oldMax - oldMin)) + newMin;
		}


		/// <summary>
		/// Returns the position of a tile in a 2d array, given a world position, tile size and tile radius.
		/// </summary>
		public static Vector3 GetTilePos(Vector3 pos, float tileSize, float tileRadius)
		{
			return Vector3.right * ((pos.x / tileSize) * tileSize + tileRadius) +
				Vector3.forward * ((pos.z / tileSize) * tileSize + tileRadius);
		}


		/// <summary>
		/// Returns 1D index of an element in a 2D array, given world position on tile, tile size and total tiles count. 
		/// </summary>
		public static int GetTileIdxFromPos(Vector3 pos, int tileSize, int tilesCount_1D)
		{
			return (int)(pos.x / tileSize) * tilesCount_1D + (int)(pos.z / tileSize);
		}


		/// <summary>
		/// Maps a world position to a pixel on image and validates if the pixel color value on specified image channel is greater than threshold. 
		/// </summary>
		//public static bool ValidateSpawnOnTex(Texture2D tex,
		//    float worldScale, Vector3 pos,
		//    FoliageSystem.ImageChannel channel,
		//    float threshold = 0.01f)
		//{
		//    if (!tex)
		//        return true;

		//    switch(channel)
		//    {
		//        case FoliageSystem.ImageChannel.Red:
		//            if (GetPixel(tex, worldScale, pos).r > threshold)
		//                return true;
		//            break;

		//        case FoliageSystem.ImageChannel.Green:
		//            if (GetPixel(tex, worldScale, pos).g > threshold)
		//                return true;
		//            break;

		//        case FoliageSystem.ImageChannel.Blue:
		//            if (GetPixel(tex, worldScale, pos).b > threshold)
		//                return true;
		//            break;

		//        case FoliageSystem.ImageChannel.Alpha:
		//            if (GetPixel(tex, worldScale, pos).a > threshold)
		//                return true;
		//            break;
		//    }

		//    return false;
		//}


		/// <summary>
		/// Maps a world position to a pixel on image and returns the pixel.
		/// </summary>
		public static Color GetPixel(Texture2D tex, float worldScale, Vector3 position)
		{
			if (!tex)
				return Color.black;

			/*
			Vector3 point = spawnPoint * (tex.width / boundsScale);
			return tex.GetPixel((int)(point.x), (int)(point.z));
			*/

			return tex.GetPixel((int)((position * (tex.width / worldScale)).x), (int)((position * (tex.width / worldScale)).z));
		}


		public static string ConvertColorToHex(float r, float g, float b)
		{
			// Convert RGB values to hexadecimal
			int r_ = (int)(r * 255f);
			int g_ = (int)(g * 255f);
			int b_ = (int)(b * 255f);

			// Create hex string
			string hex = string.Format("#{0:X2}{1:X2}{2:X2}", r_, g_, b_);

			return hex;
		}


		public static Color ConvertHexToColor(string hex)
		{
			// Remove '#' if present
			if (hex.StartsWith("#"))
				hex = hex[1..];

			// Convert hexadecimal string to Color
			Color color = new Color32(
				(byte)System.Convert.ToUInt32(hex.Substring(0, 2), 16), // Red component
				(byte)System.Convert.ToUInt32(hex.Substring(2, 2), 16), // Green component
				(byte)System.Convert.ToUInt32(hex.Substring(4, 2), 16), // Blue component
				255 // Alpha component (255 = fully opaque)
			);

			return color;
		}


		/// <summary>
		/// Weighted choice with a list of floats as input.
		/// </summary>
		private static System.Collections.Generic.List<float> totals = new();
		private static float runningTotal = 0;
		private static float randVal;
		public static int WeightedChoice(System.Collections.Generic.List<float> weights)
		{
			totals = new System.Collections.Generic.List<float>();
			runningTotal = 0;

			for (int i = 0; i < weights.Count; i++)
			{
				runningTotal += weights[i];
				totals.Add(runningTotal);
			}

			randVal = UnityEngine.Random.Range(0f, 1f) * runningTotal;

			for (int i = 0; i < totals.Count; i++)
			{
				if (randVal < totals[i])
					return i;
			}

			return -1;
		}


		/// <summary>
		/// Weighted choice with an array of floats as input.
		/// </summary>
		public static int WeightedChoice(float[] weights)
		{
			totals = new System.Collections.Generic.List<float>();
			runningTotal = 0;

			for (int i = 0; i < weights.Length; i++)
			{
				runningTotal += weights[i];
				totals.Add(runningTotal);
			}

			randVal = UnityEngine.Random.Range(0f, 1f) * runningTotal;

			for (int i = 0; i < totals.Count; i++)
			{
				if (randVal < totals[i])
					return i;
			}

			return -1;
		}
	}
}
