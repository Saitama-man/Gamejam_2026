using UnityEngine;

public class PlayerAppearanceByStoryFlag : MonoBehaviour
{
	[Header("Animation")]
	[SerializeField] private SimpleWalkAnimation simpleWalkAnimation;

	[Header("Story Flag")]
	[SerializeField] private string noCandleFlagName = "ObservatoryCutscenePlayed";

	private void Start()
	{
		if (simpleWalkAnimation == null)
		{
			simpleWalkAnimation = GetComponentInChildren<SimpleWalkAnimation>();
		}

		if (simpleWalkAnimation == null)
		{
			Debug.LogWarning("PlayerAppearanceByStoryFlag: SimpleWalkAnimation не найден.", this);
			return;
		}

		if (StoryFlags.HasFlag(noCandleFlagName))
		{
			simpleWalkAnimation.RemoveCandle();
		}
		else
		{
			simpleWalkAnimation.GiveCandle();
		}
	}
}