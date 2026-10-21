using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WebApplicationCar.Services
{
    public enum OtpChannel { Sms, Email }

    public interface IOtpSender
    {
        Task<bool> SendAsync(string destination, string code, OtpChannel channel);
    }
}