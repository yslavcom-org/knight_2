using UnityEngine;

public class GameTargetsOfPlayer : MonoBehaviour
{
    public static string[] playerHomingMissileObstacleTags;
    public static string[] playerTargetsTags;

    private static object _prototype;

    private void Init()
    {
        if (_prototype == null)
        {
            _prototype = this;
            playerTargetsTags = new string[2];
            if (playerTargetsTags != null)
            {
                playerTargetsTags[0] = "Enemy";
                playerTargetsTags[1] = "Target";
            }

            playerHomingMissileObstacleTags = new string[1];
            if (playerHomingMissileObstacleTags != null)
            {
                playerHomingMissileObstacleTags[0] = "ground";
            }
        }
    }

    void Awake()
    {
        Init();
    }

    public static bool IsStaticObstacle(string tagName)
    {
        return SearchInArray(tagName, ref playerHomingMissileObstacleTags);
    }

    public static bool IsValidTarget(string tagName)
    {
        return SearchInArray(tagName, ref playerTargetsTags);
    }


    private static bool SearchInArray(string tagName, ref string[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != null)
            {
                if (array[i] == tagName)
                {
                    return true;
                }
            }
        }

        return false;
    }

}
