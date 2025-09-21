using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIAmmoIndicator : MonoBehaviour
{
    [SerializeField] private playerInventory inventory;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image barFill;
    [SerializeField] private Image hexOutline;

    [Header("Thresholds (percent)")]
    [Range(0f, 1f)] public float medCutOff = 0.75f;
    [Range(0f, 1f)] public float lowCutOff = 0.50f;
    [Range(0f, 1f)] public float criticalCutoff = 0.25f;

    [Header("Colors")]
    public Color highColor = Color.white;
    public Color mediumColor = Color.yellow;
    public Color lowColor = Color.orange;
    public Color criticalColor = Color.red;
    public Color emptyColor = Color.red;
    public bool showNumbers = false;


    [SerializeField] private float updatesPerSecond = 10f;

    float _nextTick;

   void Awake()
    {
        if (!ammoText) ammoText = GetComponentInChildren<TextMeshProUGUI>(true);
        if (!inventory) inventory = FindFirstObjectByType<playerInventory>();
    }

    void Update()
    {
        if (Time.unscaledTime < _nextTick) return;
        _nextTick = Time.unscaledTime + 1f / Mathf.Max(1f, updatesPerSecond);

        var held = gamemanager.instance?.throwScript?.throwable ?? inventory?.equippedWeapon;
        if(!held)
        {
            ClearUI(); return;
        }

        var td = held.GetComponentInChildren<throwableDamage>(true) ?? held.GetComponent<throwableDamage>();

        if (!td || td.maxAmmo <= 0)
        { ClearUI(); return; }

        int cur = Mathf.Max(0, td.curAmmo);
        int max = Mathf.Max(0, td.maxAmmo);
        if (max == 0)
        {
            ClearUI(); return;
        }

        float pct = Mathf.Clamp01((float)cur / max);

        Color c =
            (cur <= 0) ? emptyColor :
            (pct <= criticalCutoff) ? criticalColor :
            (pct <= lowCutOff) ? lowColor :
            (pct <= medCutOff) ? mediumColor :
                                  highColor;

        if (hexOutline) hexOutline.color = c;
        if (barFill)
        {
            barFill.fillAmount = pct;

            barFill.color = c;
        }
        if (ammoText)
        {
            ammoText.text = showNumbers ? $"{cur}/{max}" : "";
        }
        
    }
    void ClearUI()
    {
        if (ammoText) ammoText.text = "";
        if (barFill) { barFill.fillAmount = 0f; }
        if (hexOutline) hexOutline.color = new Color(hexOutline.color.r, hexOutline.color.g, hexOutline.color.b, 0f);

    }
}

   