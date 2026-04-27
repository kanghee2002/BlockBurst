using System;
using UnityEngine;

/// <summary>
/// Critical 로그 한 곳에서 처리. Unity 콘솔 + Firebase Crashlytics 양쪽으로 보낸다.
/// 운영 빌드에서 발생 시 알아야 하는 데이터 정합성·핵심 흐름 실패에만 사용.
/// 단순 Inspector 검증·잘못된 호출자 인자·외부 시스템 일회성 오류는 Debug.LogError 로 둔다.
/// </summary>
public static class GameLog
{
    public static void Critical(string message, Exception exception = null)
    {
        Exception toReport;

        if (exception != null)
        {
            Debug.LogError($"{message}: {exception}");
            toReport = exception;
        }
        else
        {
            Debug.LogError(message);
            toReport = new Exception(message);
        }

        try
        {
            Firebase.Crashlytics.Crashlytics.LogException(toReport);
        }
        catch
        {
            // Firebase 가 아직 초기화되지 않았거나 사용 불가한 상태.
            // Debug.LogError 는 이미 출력됐으므로 추가 처리 불필요.
        }
    }
}
