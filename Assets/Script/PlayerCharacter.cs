using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour, IDamagable
{
    int baseHitPoint = 30;

    [SerializeField]
    int HP;

    int maxHP;

    int baseExp = 10;

    int increment = 100;

    int needExp;

    int Exp;

    int level;

    GameManager gameManager;

    Slider slider;

    TextMeshProUGUI levelText;

    TextMeshProUGUI HPText;

    enum mode
    {
        play,
        invincible
    }

    mode eMode = mode.play;

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

        if (Input.GetKeyDown(KeyCode.F9))
        {
            eMode = mode.invincible;
        }
#endif
    }

    void SetHP()
    {
        HP = baseHitPoint + level * (level - 1);
        maxHP = HP;
        slider.maxValue = HP;
        slider.value = HP;
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
        if (eMode == mode.play)
        {
            HP = HP - value;
        }

        else if(eMode == mode.invincible)
        {
            
            return;
        }
    }

    void Death()
    {
        gameManager.isDead();
        Destroy(gameObject);
    }

    public void Healing()
    {
        HP += 20;
        if (HP > maxHP)
        {
            HP = maxHP;
        }
    }

    public void GetEXP(int getExp)
    {
        Debug.Log(getExp + "‚ðŠl“¾‚µ‚½");
        Exp += getExp;
    }
}
