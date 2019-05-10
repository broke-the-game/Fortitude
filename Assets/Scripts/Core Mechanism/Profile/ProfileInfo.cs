using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBank;

public class ProfileInfo : MonoBehaviour
{
    public ProfileEntity ProfileEntity => profileEntity;
    ProfileEntity profileEntity;
    public static ProfileInfo Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetProfile(ProfileEntity pe)
    {
        profileEntity = pe;
    }
}
