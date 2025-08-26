using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PV : MonoBehaviour
{
    public float waitTime = 10.0f; //何秒放っておいたら動画が流れるか（エディタ側で変更して）

    float elapsedTime = 0.0f;   //経過時間
    Vector3 lastMousePosition;  //マウスの位置
    bool isPlayeng = false;     //再生中かどうかフラグ
    VideoPlayer player;         //動画プレイヤーコンポーネント


    private void Start()
    {
        player = GetComponent<VideoPlayer>();
        player.loopPointReached += Stop;        //動画が終わったらStopが呼ばれるようイベントを仕込む

        lastMousePosition = Input.mousePosition;    //初期マウス位置
    }

    // Update is called once per frame
    void Update()
    {
        //再生してない
        if (isPlayeng == false)
        {
            //時間計測
            elapsedTime += Time.deltaTime;

            //何か操作したら経過時間リセット
            if (Input.anyKeyDown || (Input.mousePosition != lastMousePosition))
            {
                elapsedTime = 0.0f;
                lastMousePosition = Input.mousePosition;
            }


            //指定した時間が経過したら再生
            if (elapsedTime > waitTime)
            {
                elapsedTime = 0.0f;
                Play();
            }
        }

        //再生中
        else
        {
            //何かキーが押されたらPV停止
            if (Input.anyKeyDown)
            {
                Stop(player);
            }
        }
    }


    /// <summary>
    /// 再生
    /// </summary>
    private void Play()
    {
        player.Play();
        isPlayeng = true;
    }


    /// <summary>
    /// 停止
    /// </summary>
    /// <param name="vp">プレイヤー（メンバ変数にしてるからいらないんだけど、動画終了時勝手に呼ばれるようにするために必要</param>
    private void Stop(VideoPlayer vp)
    {
        player.Stop();
        isPlayeng = false;
    }
}
