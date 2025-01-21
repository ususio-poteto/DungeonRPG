using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour, IDamagable
{
    int baseHitPoint = 30;

    int hitPoint;

    int level = 1;

    GameManager gameManager;

    Slider slider;

    TextMeshProUGUI levelText;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        slider = GameObject.Find("Slider").GetComponent<Slider>();
        levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
        SetHP();
        slider.maxValue = hitPoint;
        slider.value = hitPoint;
    }
    
    void Update()
    {
        slider.value = hitPoint;
        levelText.text = "Lv." + level;
        if (hitPoint <= 0)
        {
            Death();
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F5))
        {
            TakeDamage(5);
        }
#endif
    }

    void SetHP()
    {
        hitPoint = baseHitPoint + level * (level - 1);
    }

    public void TakeDamage(int value)
    {
        hitPoint = hitPoint - value;
    }

    public void Death()
    {
        gameManager.isDead();
        Destroy(gameObject);
    }

    public void GetEXP(int getExp)
    {
        Debug.Log(getExp + "‚ðŠl“¾‚µ‚½");
    }
}
