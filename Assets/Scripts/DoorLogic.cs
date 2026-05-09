using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorLogic : MonoBehaviour
{
    [Header("Scene")]
    public string sceneToLoad;

    [Header("Interaction")]
    public bool requireButtonPress = false;
    public KeyCode interactKey = KeyCode.E;

    private bool playerInside = false;
    private bool alreadyTriggered = false;

    private void Update()
    {
        if (requireButtonPress && playerInside && Input.GetKeyDown(interactKey))
        {
            TryLoadScene();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        if (!requireButtonPress)
        {
            TryLoadScene();
        }
        else
        {
            Debug.Log("Нажми E, чтобы перейти");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;
    }

    private void TryLoadScene()
    {
        if (alreadyTriggered)
            return;

        alreadyTriggered = true;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("Scene To Load не заполнена!");
            alreadyTriggered = false;
            return;
        }

        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false);
        }

        SceneFadeTransition fadeTransition = FindFirstObjectByType<SceneFadeTransition>();

        if (fadeTransition != null)
        {
            fadeTransition.FadeToScene(sceneToLoad);
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}