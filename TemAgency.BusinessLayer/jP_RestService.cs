using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using TemAgency.BusinessLayer.Models;

namespace TemAgency.BusinessLayer
{
    public class jP_RestService
    {
        public static string token = "";
        static string hostUrl = "89.19.10.211";
        static string port = "8080";
        string userName = "admin";
        string password = "net_123";

        public string GetBaseUrl()
        {
            return "http://" + hostUrl + ":" + port + "/logo/restservices/rest/v1.0/";
        }

        public string GetToken()
        {
            try
            {
                string FirmNr = "001";
                string HEADER = userName + ":" + password + ":1:" + Convert.ToInt16(FirmNr).ToString() + ":TRTR";
                string basicAuth = "Basic " + Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(HEADER));
                HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create("http://" + hostUrl + ":" + port + "/logo/restservices/rest/login");
                webrequest.Method = "POST";
                webrequest.Accept = "application/json";
                webrequest.ContentType = "application/json";
                webrequest.Headers.Add("Authorization", basicAuth);
                webrequest.Timeout = 1000000000;
                var response = webrequest.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var tokenResponse = JsonConvert.DeserializeObject<net_tokenResponse>(responseString);
                token = tokenResponse.authToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return token;
        }
        public string GetEncodedAuthToken()
        {
            string returnValue;
            try
            {
                string CLIENT_TOKEN_EN = "1";
                string AUTH_TOKEN_EN = token;
                string temp = CLIENT_TOKEN_EN + ":" + AUTH_TOKEN_EN + ":" + userName;
                returnValue = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(temp));
            }
            catch (Exception ex)
            {
                returnValue = ex.Message;
            }

            return returnValue;
        }
        public string PostProject(string data)
        {
            string resp_Reference = "";
            try
            {
                string postUrl = GetBaseUrl() + "pjprojects";
                var postData = Encoding.ASCII.GetBytes(data);
                WebRequest webrequest = WebRequest.Create(postUrl);
                webrequest.Method = "POST";
                webrequest.ContentType = "application/json";
                webrequest.Timeout = 1000000000;
                webrequest.Headers.Add("auth-token", GetEncodedAuthToken());
                var responseString = "";
                try
                {
                    using (var stream = webrequest.GetRequestStream())
                    {
                        stream.Write(postData, 0, postData.Length);
                    }
                    var response = (HttpWebResponse)webrequest.GetResponse();
                    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    if (response.StatusCode.ToString() == "Created")
                    {
                        string resp = JsonConvert.DeserializeObject(responseString).ToString();
                        resp_Reference = resp.Substring(resp.LastIndexOf('/') + 1);
                    }
                }
                catch (WebException webEx)
                {
                    var response = ((HttpWebResponse)webEx.Response);
                    StreamReader content = new StreamReader(response.GetResponseStream());
                    return content.ReadToEnd();
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            catch (Exception ex)
            {
                resp_Reference = ex.Message;
            }
            LogOut();
            return resp_Reference;
        }
        public string PostOrder(string data)
        {
            string resp_Reference = "";
            try
            {
                string postUrl = GetBaseUrl() + "orders";
                var postData = Encoding.ASCII.GetBytes(data);
                WebRequest webrequest = WebRequest.Create(postUrl);
                webrequest.Method = "POST";
                webrequest.ContentType = "application/json";
                webrequest.Headers.Add("auth-token", GetEncodedAuthToken());
                var responseString = "";
                try
                {
                    using (var stream = webrequest.GetRequestStream())
                    {
                        stream.Write(postData, 0, postData.Length);
                    }
                    var response = (HttpWebResponse)webrequest.GetResponse();
                    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    if (response.StatusCode.ToString() == "Created")
                    {
                        string resp = JsonConvert.DeserializeObject(responseString).ToString();
                        resp_Reference = resp.Substring(resp.LastIndexOf('/') + 1);
                    }
                }
                catch (WebException webEx)
                {
                    var response = ((HttpWebResponse)webEx.Response);
                    StreamReader content = new StreamReader(response.GetResponseStream());
                    return content.ReadToEnd();
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            catch (Exception ex)
            {
                resp_Reference = ex.Message;
            }
            LogOut();
            return resp_Reference;
        }
        public string LogOut()
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create("http://" + hostUrl + ":" + port + "/logo/restservices/rest/logout");
            string responseString;
            webrequest.Method = "POST";
            webrequest.ContentType = "application/json";
            webrequest.Headers.Add("auth-token", GetToken());
            try
            {
                var response = (HttpWebResponse)webrequest.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (WebException webEx)
            {
                var response = ((HttpWebResponse)webEx.Response);
                StreamReader content = new StreamReader(response.GetResponseStream());
                responseString = content.ReadToEnd();
            }
            return responseString;
        }
    }
}
