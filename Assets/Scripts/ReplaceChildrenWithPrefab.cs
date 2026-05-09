using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReplaceChildrenWithPrefab : MonoBehaviour
{
    [Header("Что заменяем")]
    [SerializeField] private Transform parentWithOldStars;

    [Header("На что заменяем")]
    [SerializeField] private GameObject newStarPrefab;

    [Header("Фильтр")]
    [SerializeField] private string onlyObjectsWithNamePrefix = "Circle";

    [Header("Настройки")]
    [SerializeField] private bool usePrefabScale = true;
    [SerializeField] private bool deleteOldObjects = true;

    [ContextMenu("Replace Children With Prefab")]
    private void Replace()
    {
#if UNITY_EDITOR
        if (parentWithOldStars == null)
        {
            Debug.LogError("Не указан Parent With Old Stars. Перетащи сюда объект Pegas.", this);
            return;
        }

        if (newStarPrefab == null)
        {
            Debug.LogError("Не указан New Star Prefab. Перетащи сюда prefab новой звезды.", this);
            return;
        }

        List<Transform> oldStars = new List<Transform>();

        foreach (Transform child in parentWithOldStars)
        {
            if (!string.IsNullOrEmpty(onlyObjectsWithNamePrefix))
            {
                if (!child.name.StartsWith(onlyObjectsWithNamePrefix))
                    continue;
            }

            oldStars.Add(child);
        }

        if (oldStars.Count == 0)
        {
            Debug.LogWarning("Не найдено объектов для замены.", this);
            return;
        }

        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Replace stars with prefab");

        foreach (Transform oldStar in oldStars)
        {
            GameObject newStar = (GameObject)PrefabUtility.InstantiatePrefab(
                newStarPrefab,
                parentWithOldStars
            );

            Undo.RegisterCreatedObjectUndo(newStar, "Create new star");

            newStar.transform.position = oldStar.position;
            newStar.transform.rotation = oldStar.rotation;

            if (!usePrefabScale)
                newStar.transform.localScale = oldStar.localScale;

            newStar.name = newStarPrefab.name;

            if (deleteOldObjects)
                Undo.DestroyObjectImmediate(oldStar.gameObject);
            else
                oldStar.gameObject.SetActive(false);
        }

        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log($"Заменено объектов: {oldStars.Count}", this);
#else
        Debug.LogError("Этот скрипт работает только в редакторе Unity.");
#endif
    }
}