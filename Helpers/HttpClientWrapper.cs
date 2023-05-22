using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


public interface IHttpClientWrapper
{
    RModel<T> Get<T>(string url);
    Task<RModel<T>> GetAsync<T>(string url);
    Task<RModel<T>> PostAsync<T>(string url, dynamic postModel);
    RModel<T> Post<T>(string url, dynamic postModel);
    Task<string> PostAsyncStr(string url, string apiUrl);
}
public class HttpClientWrapper : IHttpClientWrapper
{
    private readonly HttpClient _client;
    public HttpClient Client => _client;
    IConfiguration _IConfiguration;
    string apiUrl = "";
    AppSettings appSettings;
    IBaseSession _IBaseModel;

    public HttpClientWrapper(HttpClient _client, IConfiguration _IConfiguration, IBaseSession _IBaseModel)
    {
        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;

        this._client = new HttpClient(httpClientHandler);
        this._IConfiguration = _IConfiguration;

        var appSettingsSection = _IConfiguration.GetSection("AppSettings");
        appSettings = appSettingsSection.Get<AppSettings>();
        this._IBaseModel = _IBaseModel;

        this.apiUrl = appSettings.apiUrl;

    }


    void setConf()
    {
        appSettings.CreaUser = _IBaseModel?._BaseModel?.Id.ToStr();
        appSettings.LanguageId = _IBaseModel?._BaseModel?.LanguageId.ToStr();
    }


    public async Task<string> PostAsyncStr(string url, string apiUrl)
    {
        setConf();
        //appSettings.apiUrl = apiUrl;
        //var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(appSettings);
        //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), Convert.ToBase64String(Encoding.ASCII.GetBytes(jsonString)));
        var response = await _client.GetAsync(apiUrl);
        string respnoseText = response.Content.ReadAsStringAsync().Result;
        return respnoseText;
    }



    public async Task<RModel<T>> GetAsync<T>(string url)
    {
        setConf();
        RModel<T> result = new RModel<T>();
        try
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(appSettings);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), Convert.ToBase64String(Encoding.ASCII.GetBytes(jsonString)));

            var response = await _client.GetAsync(this.apiUrl + url);
            string respnoseText = response.Content.ReadAsStringAsync().Result;

            if (respnoseText == "" || response.ReasonPhrase != "OK")
            {
                result.Message = respnoseText;
                result.MessageListJson = response.RequestMessage.ToString();
                result.RType = RType.Error;
            }
            else
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<RModel<T>>(respnoseText);
        }
        catch (Exception ex)
        {
            result.Ex = ex;
            result.Message = ex.Message;
            result.RType = RType.Error;
        }
        return result;
    }


    public async Task<RModel<T>> PostAsync<T>(string url, dynamic postModel)
    {
        setConf();
        RModel<T> result = new RModel<T>();
        try
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(appSettings);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), Convert.ToBase64String(Encoding.ASCII.GetBytes(jsonString)));

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(postModel);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(this.apiUrl + url, httpContent);
            var respnoseText = await response.Content.ReadAsStringAsync();

            if (respnoseText == "" || response.ReasonPhrase != "OK")
            {
                result.Message = respnoseText;
                result.MessageListJson = response.RequestMessage.ToString();
                result.RType = RType.Error;
            }
            else
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<RModel<T>>(respnoseText);
        }
        catch (Exception ex)
        {
            result.Ex = ex;
            result.Message = ex.Message;
            result.RType = RType.Error;
        }
        return result;
    }

    public RModel<T> Post<T>(string url, dynamic postModel)
    {
        setConf();
        RModel<T> result = new RModel<T>();
        try
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(appSettings);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), Convert.ToBase64String(Encoding.ASCII.GetBytes(jsonString)));


            string json = Newtonsoft.Json.JsonConvert.SerializeObject(postModel);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = _client.PostAsync(this.apiUrl + url, httpContent);
            var respnoseText = response.Result.Content.ReadAsStringAsync().Result;

            if (respnoseText == "" || response.Result.ReasonPhrase != "OK")
            {
                result.Message = respnoseText;
                result.MessageListJson = response.Result.RequestMessage.ToString();
                result.RType = RType.Error;
            }
            else
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<RModel<T>>(respnoseText);
        }
        catch (Exception ex)
        {
            result.Ex = ex;
            result.Message = ex.Message;
            result.RType = RType.Error;
        }
        return result;
    }


    public RModel<T> Get<T>(string url)
    {
        setConf();
        RModel<T> result = new RModel<T>();
        try
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(appSettings);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), Convert.ToBase64String(Encoding.ASCII.GetBytes(jsonString)));

            var response = _client.GetAsync(this.apiUrl + url).Result;
            string respnoseText = response.Content.ReadAsStringAsync().Result;

            if (respnoseText == "" || response.ReasonPhrase != "OK")
            {
                result.Message = respnoseText;
                result.MessageListJson = response.RequestMessage.ToString();
                result.RType = RType.Error;
            }
            else
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<RModel<T>>(respnoseText);
        }
        catch (Exception ex)
        {
            result.Ex = ex;
            result.Message = ex.Message;
            result.RType = RType.Error;
        }
        return result;
    }




}

public class JsonContent : StringContent
{
    public JsonContent(object obj) :
        base(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json")
    { }
}





internal class WebApiClient : IDisposable
{

    public WebHeaderCollection Headers;

    private bool _isDispose;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
        if (!_isDispose)
        {

            if (disposing)
            {

            }
        }

        _isDispose = true;
    }

    private void SetHeaderParameters(WebClient client)
    {
        client.Headers.Clear();
        client.Headers.Add("Content-Type", "application/json");
        client.Encoding = Encoding.UTF8;
        //client.Credentials = new System.Net.NetworkCredential(appSettings.UserName, appSettings.Pass);
        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{"admin"}:{"123_*1"}"));
        client.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;


    }

    public async Task<T> PostJsonWithModelAsync<T>(string address, string data)
    {
        using (var client = new WebClient())
        {
            SetHeaderParameters(client);
            var result = await client.UploadStringTaskAsync(address, data); //  method
            var rs = JsonSerializer.Deserialize<T>(result);
            return rs;
        }
    }


    //     try
    //        {
    //            using (var client = new WebApiClient())
    //            {



    //                var response = await client.PostJsonWithModelAsync<RModel<T>>(this.apiUrl + url, json);
    //                return response;
    //            }

    //        }
    //        catch (Exception ex)
    //{

    //    throw;
    //}





}