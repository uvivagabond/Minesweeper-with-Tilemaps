using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapDimentions
{
	public int width;
	public int height;

	public MapDimentions (int width, int heigt)
	{
		this.width = width;
		this.height = heigt;
	}

	public int GetMapArea()
    {
        return this.width * this.height;
    }
}
