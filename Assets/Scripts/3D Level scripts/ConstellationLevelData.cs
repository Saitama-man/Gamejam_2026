using UnityEngine;

public class ConstellationLevelData : MonoBehaviour
{
    [Header("Info")]
    public string constellationName;

    [TextArea(3, 8)]
    public string description;

    [Header("UI")]
    public Sprite mapIcon;
    public Sprite artImage;

    [Header("Puzzle")]
    public Transform correctViewPoint;

    [Tooltip("Угол, при котором созвездие считается найденным")]
    public float correctZoneAngle = 12f;

    [Tooltip("Угол, при котором звезда-прогресс почти пустая")]
    public float angleForEmptyStar = 70f;

    [Tooltip("Сколько секунд нужно удержать правильный ракурс")]
    public float holdTimeToSolve = 1f;

    [Header("Reward")]
    public int starPoints = 100;
}