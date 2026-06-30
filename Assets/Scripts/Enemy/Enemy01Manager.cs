using UnityEngine;

public class Enemy01Manager : MonoBehaviour
{
    public static Enemy01Manager Instance { get; private set; }
    public bool IsPanic { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    
    public void NotifyEnemyKilled()
    {
        if (IsPanic) return;
        IsPanic = true;
        Enemy01Movement[] enemies = FindObjectsByType<Enemy01Movement>(FindObjectsSortMode.None);

        foreach (Enemy01Movement enemy in enemies)
            enemy.StartPanic();
    }
}
