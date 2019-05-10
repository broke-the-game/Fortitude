using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Text;

public static class Utility
{
    public enum App{
        Text, Mail, News, Bank, Team, Setting,
        COUNT
    }

    public static AppController[] GetAppControllers()
    {
        return AppController.GetAppControllers();
    }

    public static AppController GetAppController(App appType)
    {
        return AppController.GetAppController(appType);
    }

    public static AppInfo[] GetAppInfos()
    {
        return AppInfoModule.Instance.GetAppInfos();
    }

    public static AppInfo GetAppInfo(App app)
    {
        return AppInfoModule.Instance.GetAppInfo(app);
    }

    public static string ParseName(string identifiableName)
    {
        return identifiableName.Split('#')[0];
    }

    public static void SetScroll(ScrollRect scroll, float value)
    {
        ViewController.Instance.StartCoroutine(setScroll(scroll, value));
    }
    static IEnumerator setScroll(ScrollRect scroll, float value)
    {
        yield return null;
        scroll.verticalNormalizedPosition = value;
    }

    public static void ScrollTo(ScrollRect scroll, float value)
    {
        ViewController.Instance.StartCoroutine(scrollTo(scroll, value));
    }
    static IEnumerator scrollTo(ScrollRect scroll, float value)
    {
        yield return null;
        float defaultPos = scroll.verticalNormalizedPosition;
        float yVelocity = 0.0f;
        while (true)
        {
            scroll.verticalNormalizedPosition = Mathf.Lerp(scroll.verticalNormalizedPosition, value, Time.deltaTime * 15f);
            if (Mathf.Abs(scroll.verticalNormalizedPosition - value) < 0.002f)
            {
                Utility.SetScroll(scroll, value);
                yield break;
            }
            yield return null;
        }
    }

    public static string FormatData(string str)
    {
        Stack<string> stack = new Stack<string>();
        bool open = false;
        StringBuilder tag = new StringBuilder();
        StringBuilder newStr = new StringBuilder();
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '<')
            {
                open = true;
            }
            else if (str[i] == '>')
            {
                open = false;
                string t = tag.ToString();
                tag = new StringBuilder();
                if (t != "end")
                {
                    stack.Push(t);
                    if (t == "h")
                    {
                        newStr.Append("<b><i>");
                    }
                    if (t == "bold")
                    {
                        newStr.Append("<b>");
                    }
                    else if (t == "italic")
                    {
                        newStr.Append("<i>");
                    }
                    else if (t == "red")
                    {
                        newStr.Append("<color=red>");
                    }
                    else if (t == "green")
                    {
                        newStr.Append("<#1D8E00>");
                    }
                }
                else
                {
                    if (stack.Count > 0)
                    {
                        t = stack.Pop();
                    }
                    if (t == "h")
                    {
                        newStr.Append("</i></b>");
                    }
                    if (t == "bold")
                    {
                        newStr.Append("</b>");
                    }
                    else if (t == "italic")
                    {
                        newStr.Append("</i>");
                    }
                    else if (t == "red" || t == "green")
                    {
                        newStr.Append("</color>");
                    }
                }
            }
            else
            {
                if (open)
                {
                    tag.Append(str[i]);
                }
                else
                {
                    newStr.Append(str[i]);
                }
            }
        }
        return newStr.ToString();
    }
}
