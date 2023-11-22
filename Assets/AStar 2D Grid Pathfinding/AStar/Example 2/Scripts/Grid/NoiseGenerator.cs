using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace demo2
{
	public class NoiseGenerator
	{
		private float scale = 1f;
		private Vector2 offset = Vector2.zero;

		public NoiseGenerator(float scale = 1f)
		{
			this.scale = scale;
			this.offset = Vector2.zero;
		}

		public NoiseGenerator(Vector2 offset, float scale = 1f)
		{
			this.scale = scale;
			this.offset = offset;
		}

		public NoiseGenerator(bool randomOffest, float scale = 1f)
		{
			this.scale = scale;
			offset = Vector2.zero;

			float randomRange = 1000;

			if (randomOffest)
			{
				float randomX = Random.Range(-randomRange, randomRange);
				float randomY = Random.Range(-randomRange, randomRange);
				offset.x += randomX;
				offset.y += randomY;
			}
		}

		public float GetPerlinNoise(float xCordinate, float yCordinate)
		{
			return Mathf.PerlinNoise(xCordinate * scale + offset.x, yCordinate * scale + offset.y);
		}
	}
}