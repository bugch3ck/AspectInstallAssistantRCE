# AspectInstallAssistantRCE
POC for unauthenticated RCE in Aspect Unified Installation Assistant by Aspect Software 7.34.1000.003205 found in the end of 2021.

## Disclaimer
I did try to contant Aspect Software without any luck.

## The vulnerability
The service expose a WCF TCP.NET service accessible without auth. The WCF service has a public an Exec method (among others) 
that can be used to execute commands as the logged in user (or SYSTEM, if noone is logged in).

## The exploit
This can be exploited by implementing a WCF client calling the Exec method on the WCF endpoint.

## Building

```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe AspectInstallAssistantRCE.cs /reference:MonoTorrent.dll
```

## Running
```
AspectInstallAssistantRCE.exe 192.168.42.132 calc.exe
```
