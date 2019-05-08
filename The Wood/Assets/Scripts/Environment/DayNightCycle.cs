using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicFogAndMist;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle instance;

    public delegate void TimeOfDayHandler();
    public static event TimeOfDayHandler OnDaytime;
    public static event TimeOfDayHandler OnNightfall;

    [SerializeField]
    private Transform directionalLight;
    private Vector3 offset;
    [SerializeField, Range(0, 23)]
    private int initialHour = 18;
    [SerializeField, Range(1f, 60f)]
    private float rate = 1f;

    private float hourProgress = 0;
    [SerializeField] private float hour = -1;
    public float Hour
    {
        get { return hour; }
        private set
        {
            if ((int)hour == (int)value) return;
            hour = value;
            if ((int)hour == 5)
                OnDaytime?.Invoke();
            if ((int)hour == 18)
                OnNightfall?.Invoke();
        }
    }

    private Light sunLightSource;
    float baseIntensity;
    [SerializeField] ParticleSystemRenderer stars;

    [SerializeField] DynamicFogProfile fogProfile;
    float baseFogAlpha;
    float baseSkyFogAlpha;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        SaveLoadController.OnSaveGame += SaveLoadController_OnSaveGame;
        SaveLoadController.OnLoadGame += SaveLoadController_OnLoadGame;

        if (directionalLight != null)
        {
            sunLightSource = directionalLight.GetComponent<Light>();
            baseIntensity = sunLightSource.intensity;
        }
        baseFogAlpha = 0.7f;
        baseSkyFogAlpha = 0.7f;
    }

    private void OnDestroy()
    {
        instance = null;
        SaveLoadController.OnSaveGame -= SaveLoadController_OnSaveGame;
        SaveLoadController.OnLoadGame -= SaveLoadController_OnLoadGame;
    }

    // Start is called before the first frame update
    void Start()
    {
        offset = directionalLight.localEulerAngles;
        Hour = initialHour;
        InitializeTimeOfDayEvents();
    }

    // Update is called once per frame
    void Update()
    {
        rate = Mathf.Clamp(rate, 1f, 60f);
        hourProgress += Time.deltaTime / 50f * rate;
        if (hourProgress >= 1)
        {
            Hour++;
            hourProgress = 0;
        }

        if (Hour >= 24)
            Hour = 0;

        UpdateDirectionalLight();
        UpdateFog();
        UpdateStars();
    }

    private void InitializeTimeOfDayEvents()
    {
        if ((int)hour == 5 || (int)hour == 18)
            return;

        if ((int)hour >= 5 && (int)hour <= 18)
            OnDaytime?.Invoke();
        else
            OnNightfall?.Invoke();
    }

    private void UpdateDirectionalLight()
    {
        if (directionalLight == null)
            return;

        directionalLight.localEulerAngles = (Vector3.right * (Hour + hourProgress) * 15f) + new Vector3(-90, offset.y, 0f);

        // Change the sun's intensity based on position
        // If it's daytime then increase the intensity
        // If it's nighttime then decrease the intensity
        sunLightSource.intensity = GetIntensity(baseIntensity) + 0.05f;
        // If the sun is in the sky, allow shadows
        sunLightSource.shadows = (GetIntensity(baseIntensity) < (baseIntensity * 0.5f)) ? LightShadows.None : LightShadows.Soft;
    }

    private void UpdateFog()
    {
        if (DynamicFog.instance == null)
            return;

        // Set the DynamicFog's intensity based on time of day
        fogProfile.alpha = (baseFogAlpha - (GetIntensity(baseFogAlpha)) + 0.4f);
        fogProfile.skyAlpha = (baseSkyFogAlpha - GetIntensity(baseSkyFogAlpha)) + 0.4f;
        DynamicFog.instance.SetTargetProfile(fogProfile, Time.deltaTime);
    }

    private void UpdateStars()
    {
        Color targetColor = new Color(1f, 1f, 1f, 1f - GetIntensity(1f));
        stars.material.color = Color.Lerp(stars.material.color, targetColor, Time.deltaTime);
    }

    public void SetRate(float newRate)
    {
        rate = newRate;
    }

    public float GetRate()
    {
        return rate;
    }

    public int GetCurrentHour()
    {
        return Mathf.RoundToInt(Hour);
    }

    public float GetCurrentHourRaw()
    {
        return Hour + hourProgress;
    }

    public string GetStandardTime(int hour)
    {
        hour %= 24;

        if (hour >= 12)
            return (hour == 12 ? hour : (hour % 12)) + ":00 PM";
        else if (hour == 0)
            return "12:00 AM";
        else
            return hour + ":00 AM";
    }
    
    private float GetIntensity(float baseIntensity)
    {
        return ((Mathf.Sin(((Hour + hourProgress) - 6) * Mathf.PI / 12) / 2f) + 0.5f) * baseIntensity;
    }

    // Event Listeners
    private void SaveLoadController_OnSaveGame()
    {
        SaveSystem.SaveDayNightCycle(this, "/daynight.dat");
    }

    private void SaveLoadController_OnLoadGame()
    {
        DayNightCycleSaveData data = SaveSystem.LoadData<DayNightCycleSaveData>("/daynight.dat");

        if (data == null)
            return;

        initialHour = (int)data.hour;
        Hour = data.hour;

        InitializeTimeOfDayEvents();
    }

}
