using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;
using System.IO;
using System.Net;

/// <summary>
/// 网银接口的实现
/// </summary>
public class EbankPay
{
    /// <summary>
    /// 字符编码格式。 GBK、UTF-8
    /// </summary>
    private const string Charset = "UTF-8";

    #region 接口参数

    /// <summary>
    /// 商户ID
    /// </summary>
    private const string MerId = "198";
    /// <summary>
    /// 商户密钥
    /// </summary>
    private const string SecretKey = "12hi60ohgmp16nbev0gr8au69bodzguz";
    /// <summary>
    /// 对账用户名
    /// </summary>
    private const string UserId = "198";
    /// <summary>
    /// 对账密码
    /// </summary>
    private const string Pwd = "123!@#QAZ";
    /// <summary>
    /// 支付接口地址
    /// </summary>
    private const string PayUrl = "http://test.gnetpg.com:8089/GneteMerchantAPI/api/PayV36";
    /// <summary>
    /// 对账接口地址
    /// </summary>
    private const string ReconciliationUrl = "http://test.gnetpg.com:8089/GneteMerchantAPI/Trans.action";
    /// <summary>
    /// 退款接口地址
    /// </summary>
    private const string RefundUrl = "http://test.gnetpg.com:8089/GneteMerchantAPI/Trans.action";


    #endregion

    #region 支付响应码

    /// <summary>
    /// 支付响应码
    /// </summary>
    public static Dictionary<string, string> RespCode = new Dictionary<string, string>() {
        {"00", "交易成功" },//承兑或交易成功

        {"01", "交易失败，请联系发卡行" },//查发卡方
        {"02", "交易失败，请联系发卡行" },//查发卡方的特殊条件

        {"03", "交易失败，请联系发卡行" },//无效商户
        {"04", "交易失败，请联系发卡行" },//没收卡

        {"05", "交易失败，请联系发卡行" },//不予承兑
        {"06", "交易失败，请联系发卡行" },//出错
        {"07", "交易失败，请联系发卡行" },//特殊条件下没收卡
        {"09", "交易失败，请重试" },//请求正在处理中

        {"12", "交易失败，请联系发卡行" },//无效交易
        {"13", "金额有误，请重试" },//无效金额
        {"14", "无效卡号，请换卡重试" },//无效卡号（无此账号）
        {"15", "此卡不能受理" },//无此发卡方

        {"17", "交易失败，请联系发卡行" },//拒绝但不没收卡

        {"19", "交易失败，请重试" },//重新送入交易
        {"20", "交易失败，请联系发卡行" },//无效响应
        {"21", "交易失败，请联系发卡行" },//不能采取行动
        {"22", "操作有误，请重试" },//故障怀疑

        {"23", "交易失败，请联系发卡行" },//不可接受的交易费
        {"25", "交易失败，请联系发卡行" },//找不到原始交易

        {"30", "交易失败，请联系发卡行" },//格式错误
        {"31", "此卡不能受理" },//交换中心不支持的银行
        {"33", "卡片过期，请联系发卡行" },//过期的卡
        {"34", "交易失败，请联系发卡行" },//有作弊嫌疑

        {"35", "交易失败，请联系发卡行" },//受卡方与代理方联系（没收卡）
        {"36", "此卡有误，请换卡重试" },//受限制的卡

        {"37", "交易失败，请联系发卡行" },//受卡方电话通知代理方安全部门

        {"38", "密码错误次数超限" },//超过允许的PIN 试输入

        {"39", "交易失败，请联系发卡行" },//无贷记账户

        {"40", "交易失败，请联系发卡行" },//请求的功能尚不支持

        {"41", "交易失败，请联系发卡行" },//挂失卡

        {"42", "交易失败，请联系发卡行" },//无此账户
        {"43", "交易失败，请联系发卡行" },//被窃卡

        {"44", "交易失败，请联系发卡行" },//无此投资账户
        {"51", "余额不足，请查询" },//资金不足
        {"52", "交易失败，请联系发卡行" },//无此支票账户
        {"53", "交易失败，请联系发卡行" },//无此储蓄卡账户

        {"54", "卡片过期，请联系发卡行" },//过期的卡
        {"55", "密码错，请重试" },//不正确的PIN
        {"56", "交易失败，请联系发卡行" },//无此卡记录

        {"57", "该卡不支持此项服务，请联系发卡" },//不允许持卡人进行的交易

        {"58", "交易失败，请联系发卡行" },//不允许终端进行的交易
        {"59", "交易失败，请联系发卡行" },//有作弊嫌疑

        {"60", "交易失败，请联系发卡行" },//受卡方与代理方联系（不没收卡）

        {"61", "金额超限" },//超出金额限制
        {"62", "交易失败，请联系发卡行" },//受限制的卡

        {"63", "交易失败，请联系发卡行" },//侵犯安全
        {"64", "交易失败，请联系发卡行" },//原始金额错误
        {"65", "交易失败，请联系发卡行" },//超交易次数

        {"66", "交易失败，请联系发卡行" },//受卡方通知受理方安全部门

        {"67", "交易失败，请联系发卡行" },//强行受理（要求在自动会员机上没收此卡）

        {"68", "交易超时，请稍后重试" },//接收的响应超时

        {"75", "密码错误次数超限" },//允许的输入PIN 次数超限
        {"76", "交易失败，请联系发卡行" },//无效账户
        {"77", "交易失败，请联系发卡行" },//此卡需密码
        {"78", "交易失败，请联系发卡行" },//无效终端
        {"90", "交易失败，请稍后重试" },//正在日终处理（系统终止一天的活动，开始第二天的活动，交易在几分钟后可再次发送）
        {"91", "交易失败，请稍后重试" },//发卡方不能操作

        {"92", "交易失败，请稍后重试" },//金融机构或中间网络设施找不到或无法达到

        {"93", "交易失败，请联系发卡行" },//交易违法、不能完成

        {"94", "交易失败，请稍后重试" },//重复交易
        {"95", "交易失败，请稍后重试" },//核对差错
        {"96", "交易失败，请稍后重试" },//交换中心系统异常、失效

        {"97", "交易失败，请稍后重试" },//无此终端
        {"98", "交易超时，请稍后重试" },//交换中心收不到发卡方应答
        {"99", "交易失败，请稍后重试" },//PIN 格式错

        {"A0", "交易失败，请稍后重试" },//MAC 鉴别失败
        {"A2", "交易结果请查询发卡行" },//交换中心转发了原交易请求，但未收到发卡方应答时，交换中心直接向受理方应答为有缺陷的成功交易

        {"BF", "交易失败，请联系发卡行" },//深银联支付号/授权码与银行卡号不匹配

        {"XX", "交易失败，请稍后重试" }//超时已冲正

    };
    #endregion

    /// <summary>
    /// MD5 加密
    /// </summary>
    /// <param name="charset">编码格式。GBK、UTF-8等</param>
    /// <param name="hashText">要加密的字符串</param>
    /// <returns></returns>
    private static string MD5(string charset, string hashText)
    {

        byte[] b = Encoding.GetEncoding(charset).GetBytes(hashText);
        b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
        StringBuilder ret = new StringBuilder();
        for (int i = 0; i < b.Length; i++)
        {
            ret.Append(b[i].ToString("x").PadLeft(2, '0'));
        }
        return ret.ToString();

    }
    /// <summary>
    /// 获取网站根目录。
    /// 网址（协议+IP+端口号）+虚拟应用程序根路径
    /// 如：http://192.168.0.125:8680/jQuery 、http://www.zlglpt.com
    /// </summary>
    /// <returns></returns>
    private static string GetWebsiteRootPath()
    {
        return HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority
            + (HttpContext.Current.Request.ApplicationPath == "/" ? "" : HttpContext.Current.Request.ApplicationPath);

    }

    /// <summary>
    /// 写日志
    /// </summary>
    /// <param name="text">内容</param>
    public static void Log(string text)
    {
        string strPath = HttpContext.Current.Server.MapPath("/e-bank_log.txt");
        System.IO.FileInfo fileInfo = new System.IO.FileInfo(strPath);
        System.IO.FileStream fs = null;
        System.IO.StreamWriter sw = null;
        if (!fileInfo.Exists)
        {
            fs = fileInfo.Create();
            sw = new System.IO.StreamWriter(fs);
        }
        else
        {
            fs = fileInfo.Open(System.IO.FileMode.Append, System.IO.FileAccess.Write);
            sw = new System.IO.StreamWriter(fs);
        }
        sw.WriteLine("[log_" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]: " + text);
        sw.Close();
        fs.Close();

    }
    /// <summary>
    /// 提交加支付订单
    /// </summary>
    /// <param name="orderNo">订单号,长度20位以内</param>
    /// <param name="orderAmount">支付金额。格式：x.xx（元.角分）</param>
    /// <param name="callBackUrl">页面跳转同步通知页面地址</param>
    public static void SubmitPayOrder(string orderNo, string orderAmount, string callBackUrl)
    {
        string OrderNo = orderNo;//商户支付流水号（订单号），20位以内字符串。

        //支付金额。格式：x.xx（元.角分）。经测试，最低支付金额限制为0.3元

        string OrderAmount = orderAmount;
        /*
         支付币种。非空

            CNY ：人民币
            HKD：港币

            TWD：台币

         */
        string CurrCode = "CNY";
        /*订单类型。可空，系统默认B2C
            B2C ：B2C 订单
            B2B ：B2B 订单
            PPA ：预授权订单
         */
        string OrderType = "B2C";

        if (!callBackUrl.StartsWith("/"))
        {
            callBackUrl = "/" + callBackUrl;
        }
        callBackUrl = GetWebsiteRootPath() + callBackUrl;
        string CallBackUrl = callBackUrl;//页面跳转同步通知页面地址。非空

        string BankCode = "";//银行代码（直通车）。可空

        string LangType = Charset;//编码语言 可空默认：GB2312。GB2312、GBK、UTF-8
        /*业务类型。

         可空，系统默认01
            01：普通支付

            02：分账支付

            03：UPOP 实名支付
            04：境外支付

            05：实名分账支付

            06：海关实名支付

            07：海关实名分账支付

         */
        string BuzType = "01";
        string Reserved01 = "";//保留域1。可空

        string Reserved02 = "";//保留域2。可空

        //签名数据。非空，32 位MD5加密 。SignMsg 生产算法：MD5(报文内容&MD5(商户密钥))
        string SignMsg = "";
        SignMsg = MerId + OrderNo + OrderAmount + CurrCode + OrderType + CallBackUrl
                + BankCode + LangType + BuzType + Reserved01 + Reserved02 + MD5(LangType, SecretKey);

        SignMsg = MD5(LangType, SignMsg);

        //构造form，提交

        StringBuilder Html = new StringBuilder();
        Html.Append(
            string.Format("<form id='e_banksubmit' name='e_banksubmit' method='post' action='{0}' accept-charset='{1}' >"
                , PayUrl, LangType));

        Html.Append(string.Format("<input type='hidden' name='MerId' value='{0}'/>", MerId));
        Html.Append(string.Format("<input type='hidden' name='OrderNo' value='{0}'/>", OrderNo));
        Html.Append(string.Format("<input type='hidden' name='OrderAmount' value='{0}'/>", OrderAmount));
        Html.Append(string.Format("<input type='hidden' name='CurrCode' value='{0}'/>", CurrCode));
        Html.Append(string.Format("<input type='hidden' name='OrderType' value='{0}'/>", OrderType));
        Html.Append(string.Format("<input type='hidden' name='CallBackUrl' value='{0}'/>", CallBackUrl));
        Html.Append(string.Format("<input type='hidden' name='BankCode' value='{0}'/>", BankCode));
        Html.Append(string.Format("<input type='hidden' name='LangType' value='{0}'/>", LangType));
        Html.Append(string.Format("<input type='hidden' name='BuzType' value='{0}'/>", BuzType));
        Html.Append(string.Format("<input type='hidden' name='Reserved01'value='{0}'/>", Reserved01));
        Html.Append(string.Format("<input type='hidden' name='Reserved02'value='{0}'/>", Reserved02));
        Html.Append(string.Format("<input type='hidden' name='SignMsg' value='{0}'/>", SignMsg));

        Html.Append("<input type='submit' value='提交' style='display:none;'></form>");
        Html.Append("<script>document.forms['e_banksubmit'].submit();</script>");

        HttpContext.Current.Response.ContentType = "text/html";
        HttpContext.Current.Response.Write(Html.ToString());
        HttpContext.Current.Response.Flush();
       
        //JsonHelper.WriteJson(true, "form", Html.ToString());

    }
    /// <summary>
    /// 支付页面跳转同步通知签名验证
    /// </summary>
    /// <returns>返回true，则验证成功</returns>
    public static bool VerifyPayCallBackSign()
    {
        try
        {
            HttpRequest Request = HttpContext.Current.Request;
            string OrderNo = Request["OrderNo"];
            string PayNo = Request["PayNo"];
            string PayAmount = Request["PayAmount"];
            string CurrCode = Request["CurrCode"];
            string SystemSSN = Request["SystemSSN"];
            string RespCode = Request["RespCode"];
            string SettDate = Request["SettDate"];
            string Reserved01 = Request["Reserved01"];
            string Reserved02 = Request["Reserved02"];
            string SignMsg = Request["SignMsg"];

            string SourceMsg = OrderNo + PayNo + PayAmount + CurrCode + SystemSSN + RespCode + SettDate + Reserved01 + Reserved02;

            SourceMsg = MD5(Charset, SourceMsg + MD5(Charset, SecretKey));
            //E_bank.Log("SourceMsgMD5:" + SourceMsg);
            return SignMsg.Equals(SourceMsg);

        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// 获取交易结果（实时对账）
    /// </summary>
    /// <param name="PayStatus">支付状态（0：失败订单；1：成功订单；2：全部订单；3：撤销订单；4：退款订单）,可空（默认2）</param>
    /// <param name="OrderNo">商户订单号,可空</param>
    /// <param name="ShoppingTime">交易时间,可空【ShoppingTime和(BeginTime、EndTime)不能同时为空】</param>
    /// <param name="BeginTime">交易开始时间,可空</param>
    /// <param name="EndTime">结束开始时间,可空</param>
    public static string GetTradeResult(string PayStatus, string OrderNo, string ShoppingTime, string BeginTime, string EndTime)
    {

        //string TranType = "100";//交易类型,非空。交易类型（固定）不同交易有不同定义。100：实时对账

        //string JavaCharset = "";//编码,非空  编码（固定）：UTF-8 或 GBK
        //string Version = "V60";//版本号,非空
        //string UserId = "";//用户ID,非空
        //string Pwd ="";//用户密码,非空
        //string MerId ="";//商户ID,非空
        //string PayStatus = "";//支付状态,可空，默认2
        //string ShoppingTime = "";//交易时间,可空
        //string BeginTime = "";//交易开始时间,可空
        //string EndTime = "";//结束开始时间,可空
        //string OrderNo = "";//商户订单号,可空

        //对账响应结果数据是指构造成对账结果的数据，每条记录用Chr(10)分隔，每列用\n 分隔
        //订单的格式：订单日期\n 支付金额\n 商户订单号\n 商户终端号\n 系统参考号\n 响应码\n 清算日期\n 保留域1\n 保留域2\n

        if (PayStatus == null || PayStatus == "")
        {
            PayStatus = "2";
        }
        if (OrderNo == null)
        {
            OrderNo = "";
        }
        if (ShoppingTime == null)
        {
            ShoppingTime = "";
        }
        if (BeginTime == null)
        {
            BeginTime = "";
        }
        if (EndTime == null)
        {
            EndTime = "";
        }

        //待请求参数数组字符串
        string Params = "TranType={0}&JavaCharset={1}&Version={2}&UserId={3}&Pwd={4}&MerId={5}&PayStatus={6}"
                + "&ShoppingTime={7}&BeginTime={8}&EndTime={9}&OrderNo={10}&ExtFields=";
        Params = string.Format(Params, "100", Charset, "V60", UserId, MD5(Charset, Pwd), MerId, PayStatus
                , ShoppingTime, BeginTime, EndTime, OrderNo);

        Encoding code = Encoding.GetEncoding(Charset);
        //把数组转换成流中所需字节数组类型
        byte[] bytesParams = code.GetBytes(Params);

        //请求远程HTTP
        StreamReader reader = null;
        try
        {
            //设置HttpWebRequest基本信息
            HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(ReconciliationUrl);
            myReq.Method = "post";
            myReq.ContentType = "application/x-www-form-urlencoded";
            myReq.Timeout = 1000 * 60 * 3;

            //填充POST数据
            myReq.ContentLength = bytesParams.Length;
            Stream requestStream = myReq.GetRequestStream();
            requestStream.Write(bytesParams, 0, bytesParams.Length);
            requestStream.Close();

            //发送POST数据请求服务器

            HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();

            //获取服务器返回信息

            reader = new StreamReader(HttpWResp.GetResponseStream(), code);

            //读取返回信息
            return reader.ReadToEnd();

        }
        catch (Exception exp)
        {
            throw new Exception("获取交易结果（实时对账）报错：" + exp.Message);
        }
        finally
        {
            reader.Close();
        }


    }
    /// <summary>
    /// 解析交易结果（实时对账）数据
    /// </summary>
    /// <param name="result">响应结果数据</param>
    /// <returns></returns>
    public static List<Dictionary<string, string>> ParseTradeResult(string result)
    {

        //对账响应结果数据是指构造成对账结果的数据，每条记录用Chr(10)分隔，每列用\n 分隔
        //订单的格式：订单日期\n 支付金额\n 商户订单号\n 商户终端号\n 系统参考号\n 响应码\n 清算日期\n 保留域1\n 保留域2\n

        //判断结果是否是错误的结果
        if (result.TrimStart().StartsWith("Code"))
        {
            throw new Exception(result.Replace("&", "，"));
        }

        List<Dictionary<string, string>> retList = new List<Dictionary<string, string>>();
        string[] table = result.Split(Convert.ToChar(10));
        foreach (string row in table)
        {
            if (row == "")
            {
                continue;
            }
            Dictionary<string, string> tr = new Dictionary<string, string>();
            string[] data = row.Split(new string[] { "\\n" }, StringSplitOptions.None);
            tr.Add("ShoppingDate", data[0]);//订单日期
            tr.Add("OrderAmount", data[1]);//支付金额
            tr.Add("OrderNo", data[2]);//商户订单号

            tr.Add("TermNo", data[3]);//商户终端号

            tr.Add("SystemSSN", data[4]);//系统参考号
            tr.Add("RespCode", data[5]);//响应码

            tr.Add("SettDate", data[6]);//清算日期
            tr.Add("Reserved01", data[7]);//保留域1
            tr.Add("Reserved02", data[8]);//保留域2

            retList.Add(tr);

        }
        return retList;
    }
    /// <summary>
    /// 查询交易结果（实时对账）
    /// </summary>
    /// <param name="PayStatus">支付状态（0：失败订单；1：成功订单；2：全部订单；3：撤销订单；4：退款订单）,可空（默认2）</param>
    /// <param name="OrderNo">商户订单号,可空</param>
    /// <param name="ShoppingTime">交易时间,可空【格式：yyyy-MM-dd HH:mm:ss，ShoppingTime和(BeginTime、EndTime)不能同时为空】</param>
    /// <param name="BeginTime">交易开始时间,可空【格式：yyyy-MM-dd HH:mm:ss】</param>
    /// <param name="EndTime">结束开始时间,可空【格式：yyyy-MM-dd HH:mm:ss】</param>
    /// <returns>字典集合：[{"ShoppingDate":"","OrderAmount":"","OrderNo":"","TermNo":"","SystemSSN":"","RespCode":"","SettDate":"","Reserved01":"","Reserved02":""}]</returns>
    public static List<Dictionary<string, string>> SearchTradeResult(string PayStatus, string OrderNo, string ShoppingTime, string BeginTime, string EndTime)
    {
        string result = GetTradeResult(PayStatus, OrderNo, ShoppingTime, BeginTime, EndTime);
        return ParseTradeResult(result);
    }
    /// <summary>
    /// 根据商户订单号，查询交易结果（实时对账）
    /// </summary>
    /// <param name="OrderNo">商户订单号</param>
    /// <returns>字典：{"ShoppingDate":"","OrderAmount":"","OrderNo":"","TermNo":"","SystemSSN":"","RespCode":"","SettDate":"","Reserved01":"","Reserved02":""}</returns>
    public static Dictionary<string, string> SearchTradeResultByOrderNo(string OrderNo)
    {
        if (OrderNo == "" || OrderNo == null)
        {
            throw new Exception("商户订单号不能为空");
        }

        string BeginTime = DateTime.Now.AddDays(-27).ToString("yyyy-MM-dd");
        string EndTime = DateTime.Now.ToString("yyyy-MM-dd");
        string result = GetTradeResult("2", OrderNo, "", BeginTime, EndTime);
        return ParseTradeResult(result).ToArray()[0];
    }
    /// <summary>
    /// 获取退款结果参数值
    /// </summary>
    /// <param name="result">结果字符串</param>
    /// <param name="name">参数名称</param>
    /// <returns></returns>
    private static string GetRefundResultParam(string result, string name)
    {
        System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[?&]*" + name + "=([^&]+)(&|$)");
        System.Text.RegularExpressions.Match m = reg.Match(result);
        return (m.Success ? m.Groups[1].Value : string.Empty);
    }
    /// <summary>
    /// 提交退款
    /// </summary>
    /// <param name="RefundNo">商户退款流水号</param>
    /// <param name="OrderNo">商户订单号</param>
    /// <param name="ShoppingDate">订单交易（支付）日期</param>
    /// <param name="PayAmount">交易（支付）金</param>
    /// <param name="RefundAmount">退款金额</param>
    public static void SubmitRefund(string RefundNo, string OrderNo, string ShoppingDate, string PayAmount, string RefundAmount)
    {

        //string TranType = "";//交易类型。String(3)；非空；交易类型（固定）。不同交易有不同定义。31：退款

        //string JavaCharset = "";//编码。String (6)；非空；编码（固定）：UTF-8 或 GBK
        //string Version = "";//版本号。String (3)；非空；版本号（固定）：V36
        //商户退款流水号。String (32)；可空；
        //1、如果商户需要自己生成退款流水，上送流水每次需要唯一；

        //2、商户如果不需要生成退款流水，系统自动分配退款流水号
        //string RefundNo = "";
        ////string MerId = "";//商户ID。String (3)；非空；商户ID
        //string OrderNo = "";//商户订单号。String(20)；非空；商户订单号

        //string ShoppingDate = "";//订单交易（支付）日期。String(8)；非空；
        //string PayAmount = "";//交易（支付）金。Number(10,2)；非空；
        //string RefundAmount = "";//退款金额。Number(10,2)；非空；
        string Reserved = "";//保留域。String(100)；可空；
        //签名。String(64)；非空；
        //1、本签名方式与支付接口签名方式不一样。

        //2、签名密钥与支付接口一样。

        string SignMsg = "";

        //签名方法：

        //   1、单个<key, value>对的表示方式为key=value。如果该key 对应的value 为空，则表示方式为key=
        //   2、多个<key, value>对的拼接方式为key1=value1&key2=&key3=value3
        //   3、合作密钥信息的拼接方式为key1=value1&key2=&key3=value3&md5（secret_key），
        //       密钥信息经过MD5 计算后拼接在<key, value>对的尾端。

        //   4、签名方法SignMsg= md5(key1=value1&key2=&key3=value3...&keyn=valuen&md5(secret_key))
        //   【说明0】secret_key 与交易的密钥一样

        //   【说明1】将签名中的<key, value>对（不包含合作密钥）根据key 值作升序排列。其中key 应包含报文格式

        //       中除“签名方法”和“签名信息”外的所有取值。若<key, value>对中含有&、@等特殊字符或者中文字符时，

        //       要保持原样计算摘要值。


        SignMsg = "JavaCharset={0}&MerId={1}&OrderNo={2}&PayAmount={3}&RefundAmount={4}"
            + "&RefundNo={5}&Reserved={6}&ShoppingDate={7}&TranType={8}&Version={9}&";
        SignMsg = string.Format(SignMsg, Charset, MerId, OrderNo, PayAmount, RefundAmount
            , RefundNo, Reserved, ShoppingDate, "31", "V36");
        SignMsg = MD5(Charset, (SignMsg + MD5(Charset, SecretKey)));

        //待请求参数数组字符串
        string Params = "Version={0}&TranType={1}&JavaCharset={2}&MerId={3}&OrderNo={4}"
                + "&ShoppingDate={5}&PayAmount={6}&RefundAmount={7}&Reserved={8}&SignMsg={9}";
        Params = string.Format(Params, "V36", "31", Charset, MerId, OrderNo
                , ShoppingDate, PayAmount, RefundAmount, Reserved, SignMsg);

        Encoding code = Encoding.GetEncoding(Charset);
        //把数组转换成流中所需字节数组类型
        byte[] bytesParams = code.GetBytes(Params);

        //请求远程HTTP

        try
        {
            //设置HttpWebRequest基本信息
            HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(ReconciliationUrl);
            myReq.Method = "post";
            myReq.ContentType = "application/x-www-form-urlencoded";
            myReq.Timeout = 1000 * 60 * 3;

            //填充POST数据
            myReq.ContentLength = bytesParams.Length;
            Stream requestStream = myReq.GetRequestStream();
            requestStream.Write(bytesParams, 0, bytesParams.Length);
            requestStream.Close();

            //发送POST数据请求服务器

            HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();

            //获取服务器返回信息

            StreamReader reader = new StreamReader(HttpWResp.GetResponseStream(), code);

            //读取返回信息
            //响应数据格式样例
            //    Code=0000&Message=退款成功&SignMsg=签名信息
            //    Code=非0000&Message=错误信息
            string result = reader.ReadToEnd(),
                 _Code = GetRefundResultParam(result, "Code"),
                 _Message = GetRefundResultParam(result, "Message"),
                 _SignMsg = GetRefundResultParam(result, "SignMsg");

            //Log(result);
            if (_Code.Equals("0000"))
            {
                string _SignMsg2 = string.Format("Code={0}&Message{1}&", _Code, _Message);
                _SignMsg2 = MD5(Charset, _SignMsg2 + MD5(Charset, SecretKey));
                if (!_SignMsg.Equals(_SignMsg2))
                {
                    throw new Exception("返回结果签名验证失败");
                }
            }
            else
            {
                throw new Exception(result.Replace("&", "，"));
            }

        }
        catch (Exception exp)
        {
            throw new Exception(string.Format("商户订单 {0} 退款失败：\r\n{1}", OrderNo, exp.Message));
        }


    }
    /// <summary>
    /// 获取退款结果
    /// </summary>
    /// <param name="OrderNo">商户交易订单号。String(20),非空</param>
    /// <param name="ShoppingDate">交易日期。非空,格式：YYYYMMDD</param>
    /// <returns>字典集合：[{"RefundNo":"","OrderNo":"","ShoppingDate":"","SubmitRefundDate":"","PayAmount":"","RefundAmount":"","RefundResult":"","ComeFrom":""}]</returns>
    public static List<Dictionary<string, string>> GetRefundResult(string OrderNo, string ShoppingDate)
    {

        //string TranType = "101";//交易类型。String(3),非空,交易类型（固定）。不同交易有不同定义。101：退款查询

        //string JavaCharset = "";//编码。String (6),非空,编码（固定）：UTF-8 或 GBK
        //string Version = "V60";//版本号。String (3),非空,版本号（固定）：V60
        //string UserId = "";//用户ID。String (128),非空,用户ID，银联业务人员分配。测试环境参数见3.2
        //string Pwd = "";//用户密码。String (64),非空,用户密码，银联业务人员分配，需要进行MD5（32位）后再上送

        //string MerId = "";//商户ID。String (3),非空,商户ID
        //string ShoppingDate = "";//交易日期。String (19),非空,格式：YYYYMMDD
        //string OrderNo = "";//商户交易订单号。String(20),非空,商户订单号

        //string RefundNo = "";//商户退款流水号。String(32),可空,如果退款时，退款流水是由商户生成。

        ////提交退款来源。String(1),可空,
        ////  0:文件批量上传
        ////  1:单笔提交接口(旧)
        ////  2:单笔后台提交
        ////  3:接口提交
        ////  4:界面（批量）提交
        //string ComeFrom = "";
        //string RefundStartDate = "";//提交退款开始日期。String(8),可空,格式：YYYYMMDD
        //string RefundEndDate = "";//提交退款结束日期。String(8),可空,格式：YYYYMMDD

        //待请求参数数组字符串
        string Params = "TranType={0}&JavaCharset={1}&Version={2}&UserId={3}&Pwd={4}&MerId={5}"
            + "&ShoppingDate={6}&OrderNo={7}";
        Params = string.Format(Params, "101", Charset, "V60", UserId, MD5(Charset, Pwd), MerId
            , ShoppingDate, OrderNo);

        Encoding code = Encoding.GetEncoding(Charset);
        //把数组转换成流中所需字节数组类型
        byte[] bytesParams = code.GetBytes(Params);

        //请求远程HTTP
        try
        {
            //设置HttpWebRequest基本信息
            HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(ReconciliationUrl);
            myReq.Method = "post";
            myReq.ContentType = "application/x-www-form-urlencoded";
            myReq.Timeout = 1000 * 60 * 3;

            //填充POST数据
            myReq.ContentLength = bytesParams.Length;
            Stream requestStream = myReq.GetRequestStream();
            requestStream.Write(bytesParams, 0, bytesParams.Length);
            requestStream.Close();

            //发送POST数据请求服务器

            HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();

            //获取服务器返回信息

            StreamReader reader = new StreamReader(HttpWResp.GetResponseStream(), code);

            //读取返回信息
            string result = reader.ReadToEnd();
            reader.Close();

            Log(result);

            //处理错误结果
            if (result.TrimStart().StartsWith("Code"))
            {
                throw new Exception(result.Replace("&", "，"));
            }

            //解析结果
            //查询响应结果数据是指构造成退款结果的数据，每条记录用Chr(10)分隔，每列用\n 分隔
            //结果数据格式：退款流水号\n 商户支付流水号\n 交易日期\n 提交退款日期\n 订单支付金额\n 退款金额\n 退款状态\n 提交退款来源


            List<Dictionary<string, string>> retList = new List<Dictionary<string, string>>();
            string[] table = result.Split(Convert.ToChar(10));
            foreach (string row in table)
            {
                if (row == "")
                {
                    continue;
                }
                Dictionary<string, string> tr = new Dictionary<string, string>();
                string[] data = row.Split(new string[] { "\\n" }, StringSplitOptions.None);

                tr.Add("RefundNo", data[0]);//退款流水号 商户ID+退款流水号
                tr.Add("OrderNo", data[1]);//商户支付流水号

                tr.Add("ShoppingDate", data[2]);//交易日期
                tr.Add("SubmitRefundDate", data[3]);//提交退款日期

                tr.Add("PayAmount", data[4]);//订单支付金额
                tr.Add("RefundAmount", data[5]);//退款金额

                tr.Add("RefundResult", data[6]);//退款状态

                tr.Add("ComeFrom", data[7]);//提交退款来源


                retList.Add(tr);

            }
            return retList;

        }
        catch (Exception exp)
        {
            throw new Exception("获取退款结果报错：" + exp.Message);
        }

    }
    /// <summary>
    /// 服务器后台异步通知签名验证
    /// </summary>
    /// <returns>返回true，则验证成功</returns>
    public static bool VerifyNotifySign()
    {
        try
        {
            //服务器后台异步通知参数
            HttpRequest Request = HttpContext.Current.Request;

            string OrderNo = Request["OrderNo"];//商户支付流水号  String(20)，非空

            string PayNo = Request["PayNo"];//银联支付单号  String(20)，非空

            string PayAmount = Request["PayAmount"];//支付金额  Number(10,2)，非空，格式：x.xx（元.角分）

            string CurrCode = Request["CurrCode"];//支付币种  String(3)，非空，[CNY ：人民币，HKD：港币，TWD：台币]
            string SystemSSN = Request["SystemSSN"];//银联系统参考号  String(15)，非空

            string RespCode = Request["RespCode"];//交易响应码  String(2)，非空

            string SettDate = Request["SettDate"];//清算日期，格式（MMDD）  String(8)，非空

            string Reserved01 = Request["Reserved01"];//保留域1  String(300)，可空

            string Reserved02 = Request["Reserved02"];//保留域2  String(300)，可空

            string SignMsg = Request["SignMsg"];//签名数据  String，非空


            //签名内容
            string SourceMsg = OrderNo + PayNo + PayAmount + CurrCode + SystemSSN
                    + RespCode + SettDate + Reserved01 + Reserved02;

            SourceMsg = MD5(Charset, SourceMsg + MD5(Charset, SecretKey));

            return SignMsg.Equals(SourceMsg);

        }
        catch (Exception exp)
        {
            return false;
        }

    }

}