using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour, IDamagable
{
    int baseHitPoint = 30;

    [SerializeField]
    int HP;

    int maxHP;

    int baseExp = 50;

    int increment = 100;

    int needExp;

    int Exp;

    int level;

    GameManager gameManager;

    Slider slider;

    TextMeshProUGUI levelText;

    TextMeshProUGUI HPText;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        slider = GameObject.Find("Slider").GetComponent<Slider>();
        levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
        HPText = GameObject.Find("HPText").GetComponent<TextMeshProUGUI>();
        HP = gameManager.GetPlayerHP();
        level = gameManager.GetPlayerLevel();
        SetHP();
        SetExp();
        slider.maxValue = HP;
        slider.value = HP;
    }
    
    void Update()
    {
        slider.value = HP;
        levelText.text = "Lv." + level;
        HPText.text = HP + "/" + maxHP;
        if (HP <= 0)
        {
            Death();
        }

        if (needExp <= Exp)
        {
            LevelUp();
            SetExp();
            SetHP();
            gameManager.SetPlayerLevel(level);
            gameManager.SetPlayerHP(HP);
            Debug.Log(needExp);
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F5))
        {
            TakeDamage(5);
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            GetEXP(99999);
        }
#endif
    }

    void SetHP()
    {
        HP = baseHitPoint + level * (level - 1);
        maxHP = HP;
    }

    void SetExp()
    {
        needExp = baseExp + (level - 1) * increment;
    }

    void LevelUp()
    {
        level++;
        Exp = 0;
    }

    public void TakeDamage(int value)
    {
        HP = HP - value;
    }

    public void Death()
    {
        gameManager.isDead();
        Destroy(gameObject);
    }

    public void GetEXP(int getExp)
    {
        Debug.Log(getExp + "‚ðŠl“¾‚µ‚½");
        Exp += getExp;
    }
}
