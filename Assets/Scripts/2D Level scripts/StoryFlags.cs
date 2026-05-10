using System.Collections.Generic;
using UnityEngine;

public static class StoryFlags
{
	private static readonly HashSet<string> flags = new HashSet<string>();

	public static void SetFlag(string flagName)
	{
		if (string.IsNullOrEmpty(flagName))
			return;

		flags.Add(flagName);
		Debug.Log("StoryFlags: установлен флаг " + flagName);
	}

	public static bool HasFlag(string flagName)
	{
		if (string.IsNullOrEmpty(flagName))
			return false;

		return flags.Contains(flagName);
	}

	public static void ClearAll()
	{
		flags.Clear();
	}
}