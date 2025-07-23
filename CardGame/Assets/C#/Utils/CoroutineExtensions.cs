using System;
using System.Collections;

public static class CoroutineExtensions
{
    /// <summary>
    /// �R���[�`�����I�������� onComplete ���Ăяo���g�����\�b�h
    /// </summary>
    public static IEnumerator Wrap(this IEnumerator routine, Action onComplete)
    {
        yield return routine;
        onComplete?.Invoke();
    }
}
