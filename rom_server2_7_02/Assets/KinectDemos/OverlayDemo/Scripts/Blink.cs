using UnityEngine;
using System.Collections;

/// <summary>
/// 한상우 
/// 
/// 
/// </summary>

public class Blink : MonoBehaviour
{

    public UnityEngine.UI.Image img; //반짝일 이미지
    public float second; //알파값의 진동 주기 (알파값이 다시 원상 복귀 하는데 걸리는 시간)

    private float alpha = 1.0f;
    private Color tmp;
    private float time = 0;

    // Use this for initialization
    void Start()
    {
        tmp = img.color;
        tmp.a = alpha;
        img.color = tmp;
    }

    // Update is called once per frame
    void Update()
    {
        if (time % (second) < second / 2)
        {
            alpha -= 2 * Time.deltaTime / second;
        }
        else
        {
            alpha += 2 * Time.deltaTime / second;
        }

        tmp = img.color;
        tmp.a = alpha;
        img.color = tmp;
        time += Time.deltaTime;
    }
}
