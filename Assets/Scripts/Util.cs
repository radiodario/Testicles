using UnityEngine;
using System.Collections;

public class Util 
{
	public static float Map(
		float value, 
		float initialRangeMin, 
		float initialRangeMax, 
		float finalRangeMin, 
		float finalRangeMax,
		bool overflow)
	{
		float initialRange = initialRangeMax - initialRangeMin;
		float finalRange = finalRangeMax - finalRangeMin;
		if (!overflow) {
			if (value > initialRangeMax) value = initialRangeMax;
			if (value < initialRangeMin) value = initialRangeMin;
		}
		return finalRangeMin + finalRange * ((value - initialRangeMin)/initialRange);
	}

	//By default Overflow is set to false, so value won't go past the min/max limits
	public static float Map(
		float value, 
		float initialRangeMin, 
		float initialRangeMax, 
		float finalRangeMin, 
		float finalRangeMax)
	{
		return Map (value, initialRangeMin, initialRangeMax, finalRangeMin, finalRangeMax, false);
	}

	//By default initial min and max range is set to 0 and 1 and overflow to false
	public static float Map(
		float value, 
		float finalRangeMin, 
		float finalRangeMax)
	{
		return Map (value, 0, 1, finalRangeMin, finalRangeMax, false);
	}

	//By default initial min and max range is set to 0 and 1 and overflow to false
	public static float Map(
		float value, 
		float finalRangeMin, 
		float finalRangeMax, 
		bool overflow)
	{
		return Map (value, 0, 1, finalRangeMin, finalRangeMax, overflow);
	}
}