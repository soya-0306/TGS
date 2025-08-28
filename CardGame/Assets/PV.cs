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

    public Canvas canvas;       //Unity側でCanvasをアタッチ

    public AudioSource audioSource; //タイトル画面のサウンド


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
            if (Input.anyKeyDown || (Input.mousePosition != lastMousePosition) || PadCheck())
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
            if (Input.anyKeyDown || PadCheck())
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
        
        if (canvas != null)
        {
            canvas.GetComponent<Canvas>().enabled = false;  //動画再生時はUIを消す
        }
        if (audioSource != null)
        {
            audioSource.Stop(); //タイトル画面のサウンドを停止
        }
        player.Play();
        isPlayeng = true;

    }


    /// <summary>
    /// 停止
    /// </summary>
    /// <param name="vp">プレイヤー（メンバ変数にしてるからいらないんだけど、動画終了時勝手に呼ばれるようにするために必要</param>
    private void Stop(VideoPlayer vp)
    {
        if (canvas != null)
        {
            canvas.GetComponent<Canvas>().enabled = true;   //再生停止したらUIを復活
        }

        if (audioSource != null)
        {
            audioSource.Play(); //BGM復活
        }

        player.Stop();
        isPlayeng = false;

    }


    bool PadCheck()
    {
        // Unityの旧Inputで使えるジョイスティックボタンは 0〜19 まで
        for (int joyNum = 1; joyNum <= 8; joyNum++) // 最大8台まで想定
        {
            for (int button = 0; button <= 19; button++)
            {
                // "Joystick1Button0" のように文字列でKeyCodeを作る
                string keyName = $"Joystick{joyNum}Button{button}";
                KeyCode code = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyName);

                if (Input.GetKeyDown(code))
                {
                    return true;
                }
            }
        }

        // 特殊: Joystick番号を省略した「どのコントローラーでもボタンX」
        for (int button = 0; button <= 19; button++)
        {
            KeyCode code = (KeyCode)System.Enum.Parse(typeof(KeyCode), $"JoystickButton{button}");
            if (Input.GetKeyDown(code))
            {
                return true;
            }
        }

        return false;
    }
}


