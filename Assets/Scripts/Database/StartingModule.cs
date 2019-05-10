using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using DataBank;
using UnityEngine.SceneManagement;

public class StartingModule : MonoBehaviour
{

    private static StartingModule _instance;
    public static StartingModule Instance { get { return _instance; } }
    private const string database_name = "Fortitude_db_v1.db";

    List<ProfileEntity> m_profileList = new List<ProfileEntity>();

    public ProfileEntity[] ProfileList => m_profileList.ToArray();

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        FetchProfileData();
        }

    public void CreateNewGameDB()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/Fortitude.db"))
        {
            System.IO.File.Delete(Application.persistentDataPath + "/Fortitude.db");
        }
        if (System.IO.File.Exists(Application.persistentDataPath + "/StateLoading.json"))
        {
            System.IO.File.Delete(Application.persistentDataPath + "/StateLoading.json");
        }
    }

    public void GoToPlayScene()
    {
        int profile = PlayerPrefs.GetInt("Profile");
        for (int i = 0; i < m_profileList.Count; i++)
        {
            if (int.Parse(m_profileList[i].GetField(ProfileEntity.DataField.ID)) == profile)
            {
                ProfileInfo.Instance.SetProfile(m_profileList[i]);
                break;
            }
        }
        SceneManager.LoadScene("HomePage");
    }

    public void FetchProfileData()
    {
        StartCoroutine(loadProfileData());
       
    }


    public void SetProfile(int index)
    {
        PlayerPrefs.SetInt("Profile", index);
        PlayerPrefs.SetInt("Ended", 0);
    }

    public void StartANewProfile(int index)
    {
        CreateNewGameDB();
        SetProfile(index);
        GoToPlayScene();
    }

    public void CheckProfileInfo(int ProfileId)
    {
        for (int i = 0; i < ProfileList.Length; i++)
        {
            if (int.Parse(ProfileList[i].GetField(ProfileEntity.DataField.ID)) == ProfileId)
            {
                StartingPageControl.Instance.ShowProfileInfo(ProfileList[i]);
                return;
            }
        }
    }

    private IEnumerator<WWW> loadProfileData()
    {
#if UNITY_ANDROID
        string db_connection_string = "jar:file://" + Application.dataPath + "!/assets/" + database_name;
        //db_connection_string = "file:///" + Application.dataPath + "/StreamingAssets/" + database_name;
        Debug.Log("Start Loading");
        WWW www = new WWW(db_connection_string);
        yield return www;
        Byte[] result = www.bytes;
        Debug.Log(www.error);
        Debug.Log("Loaded DB: " + result.Length);
        if (System.IO.File.Exists(Application.persistentDataPath + "/Fortitude_db_androidProfile.db"))
        {
            Debug.Log("File Exists");
        }
        else
        {
            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/Fortitude_db_androidProfile.db", result);
        }
#endif

		yield return null;
        LocalDb ProfileDB = new LocalDb("Profile", true);
        System.Data.IDataReader reader = ProfileDB.getAllProfile();
        while (reader.Read())
        {
            ProfileEntity _profile = new ProfileEntity();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Debug.Log(reader[i].ToString());
                _profile.SetField(i, reader[i].ToString());
            }
            m_profileList.Add(_profile);
        }
        ProfileDB.close();
        StartingPageControl.Instance.InitializeButtons();

    }


}
