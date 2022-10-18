using UnityEngine;

class MyDebug : UnityEngine.Debug
{
    private static string hr = "\n\n-------------------------------------------------------------------------------";

    public static new void Log(object message)
    {
        UnityEngine.Debug.Log(message.ToString() + hr);
    }

    public static new void Log(object message, Object context)
    {
        UnityEngine.Debug.Log(message.ToString() + hr, context);
    }

    public static new void LogError(object message)
    {
        UnityEngine.Debug.LogError(message.ToString() + hr);
    }

    public static new void LogError(object message, Object context)
    {
        UnityEngine.Debug.LogError(message.ToString() + hr, context);
    }

    public static new void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message.ToString() + hr);
    }

    public static new void LogWarning(object message, Object context)
    {
        UnityEngine.Debug.LogWarning(message.ToString() + hr, context);
    }
}