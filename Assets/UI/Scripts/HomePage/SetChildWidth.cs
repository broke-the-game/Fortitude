using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class SetChildWidth : MonoBehaviour
{

    [SerializeField]
    GridLayoutGroup Grid_lg;

    [SerializeField, Range(1, 10)]
    int AppNumsInRow = 1;


    // Start is called before the first frame update
    void Start()
    {
        float cellwidth = ((RectTransform)transform).rect.width / AppNumsInRow;
        Grid_lg.cellSize = new Vector2(cellwidth, Grid_lg.cellSize.y);
    }

    // Update is called once per frame
    void Update()
    {
        float cellwidth = ((RectTransform)transform).rect.width / AppNumsInRow;
        Grid_lg.cellSize = new Vector2(cellwidth, Grid_lg.cellSize.y);
    }
}
