DataMailbox to Azure IoT Hub
============================

This tool copies messages from DataMailbox to Azure IoT Hub. The tool supports multiple eWONs in the same DataMailbox and automatically creates new eWONs as devices in the Azure IoT Hub.

To retrieve messages pushed to DataMailbox, the tool leverages Talk2M's RESTful API. Please refer to the documentation provided by eWON: https://developer.ewon.biz/content/datamailbox

To push messages to Azure IoT Hub, the tool uses Azure IoT Hub's RESTful API over HTTPS. Learn more here: https://msdn.microsoft.com/en-us/library/azure/mt548492.aspx

Platforms
---------

DataMailbox2Azure has been developed using .NET Core 1.0.1 and .NET Core SDK 1.0.0 Preview 2. Thus, the tool runs on Windows, Mac and Linux. Please download the right version of .NET Core for your platform before running DataMailbox2Azure.

Find .NET Core downloads here: https://www.microsoft.com/net/core

Settings
--------

The tool requires that a `settings.json` file exist in the same directory as the executing assembly. The `settings.json` must contain the following information:

```json
{
    "Talk2M":
    {
        "Username": "TALK2M-USERNAME",
        "Password": "TALK2M-PASSWORD",
        "Account": "TALK2M-ACCOUNT",
        "DeveloperId": "TALK2M-DEVELOPER-ID"
    },
    "IoTHub":
    {
        "KeyName": "IOTHUB-OWNER-POLICY-NAME", // E.g., iothubowner
        "KeyValue": "IOTHUB-OWNER-POLICY-KEY",
        "HostName": "IOTHUB-HOSTNAME", // E.g., {IOTHUB-NAME}.azure-devices.net
        "TimeToLive": 365 // Number of days the SAS token should be valid
    },
    "LastTransactionId": null, // Last transaction ID used in the syncdata service offered by Talk2M
    "IntervalInSeconds": 5 // The number of seconds between each cycle
}
```

Below is an example of what the `settings.json` file could look like:

```json
{
    "Talk2M":
    {
        "Username": "MyUsername",
        "Password": "MyPassw0rd!",
        "Account": "MyAccount",
        "DeveloperId": "4a4783ab-1619-49a0-b3cc-8ab046bd7b9b"
    },
    "IoTHub":
    {
        "KeyName": "iothubowner",
        "KeyValue": "AhqCOuHbm13xzH6kq7BL3463x1cm9dnqiKrFmZ+kTtY=",
        "HostName": "MyIoTHub.azure-devices.net",
        "TimeToLive": 365
    },
    "LastTransactionId": null,
    "IntervalInSeconds": 5
}
```

Support
-------

DataMailbox2Azure is developed by Sebastian Brandes ([sbrand@microsoft.com](mailto:sbrand@microsoft.com)), Tech Evangelist at Microsoft. If you run into any issues with the tool, please feel free to reach out via email.