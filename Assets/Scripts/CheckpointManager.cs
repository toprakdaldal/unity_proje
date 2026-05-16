using UnityEngine;
using UnityEngine.SceneManagement;

// Sahne yüklenince checkpoint pozisyonuna ışınlar — DontDestroyOnLoad
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    Vector3 respawnPosition;
    bool    hasCheckpoint = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetCheckpoint(Vector3 position)
    {
        respawnPosition = position;
        hasCheckpoint   = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!hasCheckpoint) return;

        // Player'ı checkpoint pozisyonuna taşı
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerObj.transform.position = respawnPosition;
    }
}
