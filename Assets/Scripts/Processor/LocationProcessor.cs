using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationProcessor : MonoBehaviour
{
    private Text lngText;
    private Text latText;
    private Text altText;

    public Text message;

    private void Start()
    {
        lngText = transform.Find("Longitude").GetComponent<Text>();
        latText = transform.Find("Latitude").GetComponent<Text>();
        altText = transform.Find("Altitude").GetComponent<Text>();
        StartCoroutine(EnableLocationService());
    }

    public IEnumerator EnableLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("User location info is disabled");
            message.text = "位置权限被禁止";
            yield break;
        }

        // 启动定位服务，服务所需的精度设置为5米，最小更新距离设置为5米
        Input.location.Start(5.0f, 5.0f);
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            // 暂停协同程序的执行(1秒)  
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            Debug.Log("Init GPS service time out");
            message.text = "初始化GPS服务超时";
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            message.text = "GPS服务启动失败";
            yield break;
        }
        else
        {
            Debug.Log("Location service started sussessfully");
            message.text = "GPS服务启动成功";
            // 每隔1秒更新位置信息
            StartCoroutine(AudoUploadLocationInfo());
        }
    }

    public void DisableLocationService()
    {
        Input.location.Stop();
        Debug.Log("Disable location service successfully");
        message.text = "GPS服务关闭成功";
    }

    private IEnumerator AudoUploadLocationInfo()
    {
        lngText.text = $"经度：{Input.location.lastData.longitude}";
        latText.text = $"纬度：{Input.location.lastData.latitude}";
        altText.text = $"高度：{Input.location.lastData.altitude}";
        yield return new WaitForSeconds(1f);
    }
}
