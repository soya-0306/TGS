using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TwoPlayerUISelector : MonoBehaviour
{
    [Header("UI Elements")]
    public List<Selectable> uiElements;

    [Header("Highlight Images (UI選択枠)")]
    public Image player1CursorHighlight;
    public Image player2CursorHighlight;

    [Header("操作カーソル Images (自由移動)")]
    public RectTransform player1Cursor;
    public RectTransform player2Cursor;

    [Header("Input Settings")]
    public float inputDelay = 0.3f;
    public float inputThreshold = 0.5f;

    [Header("イメージ透明化設定")]
    public Image targetImageForPlayer1;

    [Header("プレイヤー1用メッセージ表示UI")]
    public Text infoText;
    public Text infoText3;
    public Text infoText5;

    [Header("プレイヤー2用メッセージ表示UI")]
    public Text infoText2;
    public Text infoText4;
    public Text infoText6;

    [Header("プレイヤー1用メッセージリスト")]
    public List<string> elementMessages;
    public List<string> elementMessages3;
    public List<string> elementMessages5;

    [Header("プレイヤー2用メッセージリスト")]
    public List<string> elementMessages2;
    public List<string> elementMessages4;
    public List<string> elementMessages6;

    private int[] currentIndices = new int[2] { 0, 0 };
    private float[] lastInputTimes = new float[2] { 0f, 0f };
    private bool[] canMove = new bool[2] { false, false };

    private bool wasPlayer1OnTarget = false;

    void Start()
    {
        UpdateCursorPosition(0);
        UpdateCursorPosition(1);

        DisableText(infoText);
        DisableText(infoText2);
        DisableText(infoText3);
        DisableText(infoText4);
        DisableText(infoText5);
        DisableText(infoText6);
    }

    void DisableText(Text t)
    {
        if (t != null)
        {
            t.text = "";
            t.enabled = false;
        }
    }

    void Update()
    {
        HandlePlayerInput(0, "Joystick1Horizontal", "Joystick1Vertical", KeyCode.Joystick1Button0);
        HandlePlayerInput(1, "Joystick2Horizontal", "Joystick2Vertical", KeyCode.Joystick2Button0);

        HandleKeyboardInput(0, KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Return);
        HandleKeyboardInput(1, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.KeypadEnter);

        UpdateCursorPosition(0);
        UpdateCursorPosition(1);
    }

    void UpdateCursorPosition(int playerIndex)
    {
        if (uiElements.Count == 0) return;

        Image cursorHighlight = (playerIndex == 0) ? player1CursorHighlight : player2CursorHighlight;

        if (cursorHighlight != null && canMove[playerIndex])
        {
            Selectable selectedElement = uiElements[currentIndices[playerIndex]];
            cursorHighlight.transform.position = selectedElement.transform.position;
            cursorHighlight.enabled = true;

            if (playerIndex == 0 && targetImageForPlayer1 != null)
            {
                if (!wasPlayer1OnTarget)
                {
                    Color color = targetImageForPlayer1.color;
                    color.a = 0f;
                    targetImageForPlayer1.color = color;
                    wasPlayer1OnTarget = true;
                }
            }
        }
        else if (cursorHighlight != null)
        {
            cursorHighlight.enabled = false;

            if (playerIndex == 0 && targetImageForPlayer1 != null && wasPlayer1OnTarget)
            {
                Color color = targetImageForPlayer1.color;
                color.a = 1f;
                targetImageForPlayer1.color = color;
                wasPlayer1OnTarget = false;
            }
        }
    }

    void HandlePlayerInput(int playerIndex, string hAxis, string vAxis, KeyCode submitKey)
    {
        float h = Input.GetAxisRaw(hAxis);
        float v = Input.GetAxisRaw(vAxis);

        // 十字キーが動作しない場合の代替として DPad 軸を利用（例：Joystick1）
        if (Mathf.Approximately(h, 0f) && Mathf.Approximately(v, 0f))
        {
            if (playerIndex == 0)
            {
                h = Input.GetAxisRaw("P1_X");
                v = Input.GetAxisRaw("P1_Y"); // 反転なし
            }
            else if (playerIndex == 1)
            {
                h = Input.GetAxisRaw("P2_X");
                v = Input.GetAxisRaw("P2_Y");
            }
        }

        // 十字キーのボタン入力をチェックして軸に反映させる（上下反転）
        if (playerIndex == 0)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button7)) v = -1f;      // UpをDownに反転
            else if (Input.GetKeyDown(KeyCode.Joystick1Button6)) v = 1f;  // DownをUpに反転
            else if (Input.GetKeyDown(KeyCode.Joystick1Button4)) h = -1f; // Left
            else if (Input.GetKeyDown(KeyCode.Joystick1Button5)) h = 1f;  // Right
        }
        else if (playerIndex == 1)
        {
            if (Input.GetKeyDown(KeyCode.Joystick2Button7)) v = -1f;
            else if (Input.GetKeyDown(KeyCode.Joystick2Button6)) v = 1f;
            else if (Input.GetKeyDown(KeyCode.Joystick2Button4)) h = -1f;
            else if (Input.GetKeyDown(KeyCode.Joystick2Button5)) h = 1f;
        }

        // 操作開始したら canMove を true にする（カーソル表示のため）
        if (!canMove[playerIndex] && (Mathf.Abs(h) > inputThreshold || Mathf.Abs(v) > inputThreshold))
        {
            canMove[playerIndex] = true;
            Debug.Log($"Player {playerIndex + 1} started moving via joystick input");
        }

        if (Mathf.Abs(h) > inputThreshold || Mathf.Abs(v) > inputThreshold)
        {
            Debug.Log($"[PAD Input] Player {playerIndex + 1} H: {h}, V: {v}");
        }

        if (Time.time - lastInputTimes[playerIndex] < inputDelay) return;

        Vector2 inputDir = Vector2.zero;

        if (v > inputThreshold) inputDir = Vector2.up;
        else if (v < -inputThreshold) inputDir = Vector2.down;
        else if (h < -inputThreshold) inputDir = Vector2.left;
        else if (h > inputThreshold) inputDir = Vector2.right;

        if (inputDir != Vector2.zero)
        {
            currentIndices[playerIndex] = FindClosestInDirection(currentIndices[playerIndex], inputDir);
            lastInputTimes[playerIndex] = Time.time;
        }

        if (Input.GetKeyDown(submitKey))
        {
            Debug.Log($"[PAD] Player {playerIndex + 1} selected: {uiElements[currentIndices[playerIndex]].gameObject.name}");

            Button button = uiElements[currentIndices[playerIndex]] as Button;
            if (button != null)
            {
                button.onClick.Invoke();
            }

            ShowMessageForSelectedIndex(playerIndex, currentIndices[playerIndex]);
        }
    }

    void HandleKeyboardInput(int playerIndex, KeyCode up, KeyCode down, KeyCode left, KeyCode right, KeyCode submitKey)
    {
        if (!canMove[playerIndex])
        {
            if (Input.GetKey(up) || Input.GetKey(down) || Input.GetKey(left) || Input.GetKey(right))
            {
                canMove[playerIndex] = true;
                Debug.Log($"Player {playerIndex + 1} started moving via keyboard input");
            }
            else return;
        }

        bool upPressed = Input.GetKeyDown(up);
        bool downPressed = Input.GetKeyDown(down);
        bool leftPressed = Input.GetKeyDown(left);
        bool rightPressed = Input.GetKeyDown(right);

        if (upPressed) Debug.Log($"[Keyboard Input] Player {playerIndex + 1} Up pressed");
        if (downPressed) Debug.Log($"[Keyboard Input] Player {playerIndex + 1} Down pressed");
        if (leftPressed) Debug.Log($"[Keyboard Input] Player {playerIndex + 1} Left pressed");
        if (rightPressed) Debug.Log($"[Keyboard Input] Player {playerIndex + 1} Right pressed");

        if (Time.time - lastInputTimes[playerIndex] < inputDelay) return;

        Vector2 inputDir = Vector2.zero;

        if (upPressed) inputDir = Vector2.up;
        else if (downPressed) inputDir = Vector2.down;
        else if (leftPressed) inputDir = Vector2.left;
        else if (rightPressed) inputDir = Vector2.right;

        if (inputDir != Vector2.zero)
        {
            currentIndices[playerIndex] = FindClosestInDirection(currentIndices[playerIndex], inputDir);
            lastInputTimes[playerIndex] = Time.time;
        }

        if (Input.GetKeyDown(submitKey))
        {
            Debug.Log($"[Keyboard] Player {playerIndex + 1} selected: {uiElements[currentIndices[playerIndex]].gameObject.name}");

            Button button = uiElements[currentIndices[playerIndex]] as Button;
            if (button != null)
            {
                button.onClick.Invoke();
            }

            ShowMessageForSelectedIndex(playerIndex, currentIndices[playerIndex]);
        }
    }

    void ShowMessageForSelectedIndex(int playerIndex, int selectedIndex)
    {
        if (playerIndex == 0)
        {
            ShowText(infoText, elementMessages, selectedIndex);
            ShowText(infoText3, elementMessages3, selectedIndex);
            ShowText(infoText5, elementMessages5, selectedIndex);
        }
        else if (playerIndex == 1)
        {
            ShowText(infoText2, elementMessages2, selectedIndex);
            ShowText(infoText4, elementMessages4, selectedIndex);
            ShowText(infoText6, elementMessages6, selectedIndex);
        }
    }

    void ShowText(Text targetText, List<string> messages, int index)
    {
        if (targetText != null)
        {
            if (messages != null && index >= 0 && index < messages.Count)
            {
                targetText.text = messages[index];
                targetText.enabled = true;
            }
            else
            {
                targetText.text = "";
                targetText.enabled = false;
            }
        }
    }

    int FindClosestInDirection(int currentIndex, Vector2 direction)
    {
        Vector3 currentPos = uiElements[currentIndex].transform.position;
        float bestScore = -1f;
        float minDist = Mathf.Infinity;
        int bestIndex = currentIndex;

        for (int i = 0; i < uiElements.Count; i++)
        {
            if (i == currentIndex) continue;

            Vector3 targetPos = uiElements[i].transform.position;
            Vector2 toTarget = targetPos - currentPos;
            float dot = Vector2.Dot(direction.normalized, toTarget.normalized);

            if (dot > 0.5f)
            {
                float dist = toTarget.sqrMagnitude;
                if (dot > bestScore || (Mathf.Approximately(dot, bestScore) && dist < minDist))
                {
                    bestScore = dot;
                    minDist = dist;
                    bestIndex = i;
                }
            }
        }

        return bestIndex;
    }
}
