using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class FirebaseManager : SingletonMonoDontDestroy<FirebaseManager>
{
    private bool isInit = false;

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void Start()
    {
        if (isInit) return;
        StopAllCoroutines();
        StartCoroutine(IEStart());
    }

    // Start is called before the first frame update
    private IEnumerator IEStart()
    {
        yield return new WaitForEndOfFrame();
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                var app = Firebase.FirebaseApp.DefaultInstance;
                isInit = true;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("Start_FireBase");
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public void LogEvent(string event_name, Hashtable hash)
    {
        if (isInit == false)
        {
            StopAllCoroutines();
            StartCoroutine(IEWaitInit(event_name, hash));
            return;
        }
        Log(event_name, hash);
    }

    private IEnumerator IEWaitInit(string event_name, Hashtable hash)
    {
        yield return new WaitUntil(() => isInit);
        Log(event_name, hash);
    }

    private static void Log(string event_name, Hashtable hash)
    {
        Firebase.Analytics.Parameter[] parameter = new Firebase.Analytics.Parameter[hash.Count];
        //List<Firebase.Analytics.Parameter> parameters = new List<Firebase.Analytics.Parameter>();
        if (hash != null && hash.Count > 0)
        {
            int i = 0;
            foreach (DictionaryEntry item in hash)
            {
                if (item.Equals((DictionaryEntry)default)) continue;
                parameter[i] = (new Firebase.Analytics.Parameter(item.Key.ToString(), item.Value.ToString()));
                i++;
            }

            Firebase.Analytics.FirebaseAnalytics.LogEvent(
                       event_name,
                       parameter);
        }
    }

    public void LogEvent(string event_name)
    {
        if (isInit == false)
        {
            StopAllCoroutines();
            StartCoroutine(IEWaitInit(event_name));
            return;
        }
        Log(event_name);
    }

    private IEnumerator IEWaitInit(string event_name)
    {
        yield return new WaitUntil(() => isInit);
        Log(event_name);
    }

    private static void Log(string event_name)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent(event_name);
    }
}