using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CertComm
{
    public delegate void SignProvider(List<ParmField> aparmList);
    public class HttpServer
    {
        private static byte[] getBody(List<ParmField> aparmList, string boundary)
        {
            if (aparmList == null)
                return new byte[0];
            List<byte> bodylist = new List<byte>();
            UTF8Encoding encoding = new UTF8Encoding();
            string crlf = "\r\n";
            string charset = "utf-8";
            byte[] temp;
            foreach (ParmField pf in aparmList)
            {
                string body = "";
                if (pf.GetType() == typeof(StringField))
                {
                    if (pf.Value == null)
                        continue;
                    body += "--" + boundary;
                    body += crlf;
                    body += "Content-Disposition:form-data;name=\"" + pf.Key + "\"";
                    body += crlf;
                    body += "Content-Type:text/plain;charset=" + charset;
                    body += crlf + crlf;
                    body += pf.Value;
                    body += crlf;
                    temp = encoding.GetBytes(body);
                    foreach (byte _b in temp)
                    {
                        bodylist.Add(_b);
                    }
                }
                else if (pf.GetType() == typeof(FileField) || pf.GetType() == typeof(StreamField))
                {
                    if (pf.StreamValue == null)
                        continue;
                    body += "--" + boundary;
                    body += crlf;
                    body += "Content-Disposition:form-data;name=\"" + pf.Key + "\";";
                    if (pf.GetType() == typeof(FileField))//是文件是有文件名
                    {
                        body += "filename=\"" + pf.Value + "\"";
                    }
                    body += crlf;
                    body += "Content-Type:application/octet-stream";
                    body += crlf;
                    body += "Content-Transfer-Encoding:binary";
                    body += crlf + crlf;
                    temp = encoding.GetBytes(body);
                    foreach (byte _b in temp)
                    {
                        bodylist.Add(_b);
                    }
                    byte[] bytefile = new byte[pf.StreamValue.Length];
                    pf.StreamValue.Read(bytefile, 0, Convert.ToInt32(pf.StreamValue.Length));
                    foreach (byte _b in bytefile)
                    {
                        bodylist.Add(_b);
                    }
                    temp = encoding.GetBytes(crlf);
                    foreach (byte _b in temp)
                    {
                        bodylist.Add(_b);
                    }
                }
            }
            temp = encoding.GetBytes("--" + boundary + "--" + crlf);
            foreach (byte _b in temp)
            {
                bodylist.Add(_b);
            }
            return bodylist.ToArray();
        }


        public static ClientResult InvokeApi(string url, List<ParmField> aparmList, SignProvider signpro)
        {
            if (signpro != null)
                signpro(aparmList);
            return InvokeApi(url, aparmList);
        }

        public static ClientResult InvokeApi(string url, List<ParmField> aparmList)
        {
          //  File.AppendAllText("D:\\crm.invoke.api.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + url +"   ==>start \r\n");
            if (!url.StartsWith("http"))
            {
                string tempt = System.Configuration.ConfigurationManager.AppSettings["appurl"];
                if (tempt.EndsWith("/"))
                {
                    tempt = tempt.Remove(tempt.Length - 1, 1);
                }
                if (!url.StartsWith("/"))
                {
                    url = "/" + url;
                }
                url = tempt + url;
            }

        

            ClientResult result = new ClientResult();
            HttpClient client = new HttpClient();
            Task<HttpResponseMessage> resultTask;
            try
            {
                List<ParmField> curParmList = new List<ParmField>();
                curParmList = aparmList;
                //curParmList.Add(new StringField("sign", sign));
                string boundary = "------------------------------7dXiaoXiao";
                byte[] bodybyte = getBody(curParmList, boundary);
                HttpContent content = new ByteArrayContent(bodybyte, 0, bodybyte.Length);
                content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("multipart/form-data;boundary=" + boundary);
                //string httpcode = WebLib.Sys.BytesToStr(bodybyte);
                resultTask = client.PostAsync(url, content);
                resultTask.Wait();
                result.statuscode = (int)resultTask.Result.StatusCode;
                if (result.statuscode == 404)
                {
                    result.code = -100;
                    result.msg = "您正在查找的资源不可用！";
                    result.resString = "您正在查找的资源不可用！";
                    return result;
                }
                try
                {
                    result.responseContentType = resultTask.Result.Content.Headers.ContentType.MediaType.ToLower();
                }
                catch
                {
                    result.responseContentType = "applicaiton/octet-stream";
                }
                if (result.responseContentType == "application/json" || result.responseContentType == "text/html")
                {
                    Task<string> strTask = resultTask.Result.Content.ReadAsStringAsync();
                    strTask.Wait();
                    result.resString = strTask.Result;
                    result.total = 0;
                    if (result.statuscode == 200)
                    {
                        try
                        {
                            result.repObject = JObject.Parse(result.resString);
                            result.code = result.repObject["code"].Value<int>();
                            result.msg = result.repObject["msg"] != null ? result.repObject["msg"].Value<string>() : "";
                            result.total = result.repObject["total"] != null ? result.repObject["total"].Value<long>() : 0L;
                        }
                        catch// (Exception ex)
                        {
                            result.code = -100;
                            result.msg = "返回json解析出错，源json请查看result.resString.";
                        }
                    }
                    else
                    {
                        result.code = -100;
                        result.msg = "请求出错，请查看result.statuscode";
                    }
                    return result;
                }
                else
                {
                    //文件流返回，
                    //result.code = -100;
                    //result.msg = "此外为文件下载，如需要此功能，请编写。如非文件下载，请检查 result.responseContentType ";
                    //   return result;
                    #region
                    result.code = 1;
                    Task<Stream> streamTask = resultTask.Result.Content.ReadAsStreamAsync();
                    streamTask.Wait();
                    result.responseStream = streamTask.Result;
                    result.responseContentType = resultTask.Result.Content.Headers.ContentType.MediaType;
                    if (result.responseStream == null)
                    {
                        result.code = -100;
                        result.statuscode = -1;
                        result.msg = "下载文件时发生错误！";
                        return result;
                    }
                    else
                    {
                        if (resultTask.Result.Content.Headers.ContentDisposition.FileName == null)
                            result.resString = resultTask.Result.Content.Headers.ContentDisposition.FileNameStar;
                        else
                            result.resString = resultTask.Result.Content.Headers.ContentDisposition.FileName;
                        return result;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result.code = -100;
                if (ex.InnerException != null)
                {
                    if (ex.InnerException is System.Net.Http.HttpRequestException)
                    {
                        result.msg = "连接服务器[" + url + "]时发生错误！";
                    }
                    else
                    {
                        result.msg = ex.InnerException.Message;
                    }
                }
                else
                {
                    result.msg = ex.Message;
                }
                return result;
            }
            finally
            {
                
               // File.AppendAllText("D:\\crm.invoke.api.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + url + "   ==>end\r\n");
                client.Dispose();
            }
        }

        /// <summary>
        /// all StringField will be signed and will add timespan,appid,appsecret; appid、appsecret from web.config or app.config
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static void DefaultSignProvider( List<ParmField> para)
        {
            if (para == null)
                para = new List<ParmField>();
            para.Add(new StringField("timespan", Authcomm.GetTimeSpan()));
            para.Add(new StringField("appid", Authcomm.GetAppConfig("appid")));
            string appsecret = Authcomm.GetAppConfig("appsecret");
            StringBuilder sb = new StringBuilder();
            foreach(ParmField f in para)
            {
                if (string.IsNullOrEmpty(f.Value))
                {
                    continue;
                }
                if (f.GetType() == typeof(StringField))
                {
                    sb.Append(f.Key.Trim() + "=" + f.Value.Trim() + "&");
                }
            }
            sb.Append("appsecret=" + appsecret);
            string sign = Authcomm.ToMD5String(sb.ToString());
            para.Add(new StringField("sign", sign));
        }

    }



}
