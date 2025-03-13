using UnityEngine;

public class ExitHandler : MonoBehaviour
{
    void OnApplicationQuit()
    {
        RevokeMicrophonePermission();
    }

    void RevokeMicrophonePermission()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (AndroidJavaObject packageManager = activity.Call<AndroidJavaObject>("getPackageManager"))
                {
                    string packageName = activity.Call<string>("getPackageName");
                    using (AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, 0))
                    {
                        using (AndroidJavaObject permissionsArray = packageInfo.Get<AndroidJavaObject>("requestedPermissions"))
                        {
                            if (permissionsArray != null)
                            {
                                int length = permissionsArray.Call<int>("length");
                                for (int i = 0; i < length; i++)
                                {
                                    string permission = permissionsArray.Call<string>("get", i);
                                    if (permission == "android.permission.RECORD_AUDIO")
                                    {
                                        string command = "pm revoke " + packageName + " " + permission;
                                        RunCommand(command);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endif
    }

    void RunCommand(string command)
    {
        using (AndroidJavaClass processClass = new AndroidJavaClass("java.lang.Runtime"))
        {
            using (AndroidJavaObject process = processClass.CallStatic<AndroidJavaObject>("getRuntime"))
            {
                process.Call<AndroidJavaObject>("exec", command);
            }
        }
    }
}
