# windows 7 免配置免安装补丁全系支持TLs1.2 通讯

## 一、背景

win7默认不支持TLS1.2 协议，要解决这个问题需要打补丁或者做一系列很复杂的设置。

用C#实现的话需.net 4.5以上 ，在.net3.5中如果你用HttpwebRequest,那么肯定会有人告诉你这么设置解决问题

```c#
      private static bool RemoteCertificateValidate(
          object sender, X509Certificate cert,
           X509Chain chain, SslPolicyErrors error) {
             // trust any certificate!!!
             System.Console.WriteLine("Warning, trust any certificate");
             return true;
         }
```

* 实际上这样设置也没用，会报错 `不支持的协议`。

对于商业软件来说，为了使用你的产品而让用户更新Sp1补丁，费时费力，还可能影响用户体验。 国外有个chilkatsoft库可以解决这个问题，不过人家收费好贵！！！

![image-20240719233540722](https://www.qq7u.com/usr/uploads/softs/tlsTest/1.png)

## 二、 研究成果

经过大量研究测试，发现用C# .net3.5 的话，非常难斛决，甚至调用了winHttp、xmlHttp等系统api也无解。最终只能通过写dll来实现。

![image-20240719233757055](https://www.qq7u.com/usr/uploads/softs/tlsTest/1.png)

最终效果如图：

![image-202407](https://www.qq7u.com/usr/uploads/softs/tlsTest/1.png)

测试环境：

windows7及以上，包括32位，64位，C# .net framework 3.5理论上支持python、java、vb等语言。

无需修改系统设置，下载文件支持断点续传。

微信：root6819 

源码： 

https://github.com/root6819/tlsTest

https://www.qq7u.com    

