using System;
using System.Collections;

public static class CoroutineExtensions
{
    /// <summary>
    /// コルーチンが終了したら onComplete を呼び出す拡張メソッド
    /// </summary>
    public static IEnumerator Wrap(this IEnumerator routine, Action onComplete)
    {
        yield return routine;
        onComplete?.Invoke();
    }
}
