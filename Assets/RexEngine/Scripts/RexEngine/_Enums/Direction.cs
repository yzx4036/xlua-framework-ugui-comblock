/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Direction
{
	public enum Horizontal
	{
		Left = -1,
		Neutral = 0,
		Right = 1
	}

	public enum Vertical
	{
		Down = -1,
		Neutral = 0,
		Up = 1
	}

	public Horizontal horizontal;
	public Vertical vertical;
}
