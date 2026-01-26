using UnityEngine;

public class PlayerGrow : MonoBehaviour
{
    private int curLevel = 0;
    private float curExp = 0f;

    private void Start()
    {
        var playData = GameInstance.Instance.SAVE_GetCurPlayData();
        curExp = playData.characterData.exp;
        curLevel = playData.characterData.level;
    } 

    public int GetLevel() { return curLevel; }
    public float GetExp() { return curExp; }

    public void AddExp(float _add)
    {
        var gameInstance = GameInstance.Instance;
        curExp += _add;

        while (true)
        {
            int requireExp = gameInstance.TABLE_GetRequireExp(curLevel);
            if (curExp < requireExp)
                break;

            curExp -= requireExp;
            LevelUp();
        }
    }
    private void LevelUp()
    {
        curLevel++;
    }
}
