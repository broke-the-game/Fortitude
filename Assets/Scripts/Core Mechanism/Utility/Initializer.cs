using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    #region Singleton
    public static Initializer Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    [SerializeField]
    MonoBehaviour[] initializable;


    // Start is called before the first frame update
    public void Init()
    {
        for (int i = 0; i < initializable.Length; i++)
        {
            if (!initializable[i])
            {
                Debug.LogError("initializable[" + i +"]Not found");
                continue;
            }
            Initializable init = initializable[i].GetComponent<Initializable>();
            if (init == null)
            {
                Debug.LogError("initializable[" + i + "] Init Script Not found");
                continue;
            }
            init.Init();
        }
    }
}
