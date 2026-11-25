using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;

#pragma warning disable CS0105 // The using directive for 'System.Threading.Tasks' appeared previously in this namespace
using System.Threading.Tasks;
#pragma warning restore CS0105 // The using directive for 'System.Threading.Tasks' appeared previously in this namespace

namespace WebApplicationCar.Services
{
    public enum ResetChannel { Sms, Email }

    public interface IPasswordResetSender
    {
        /// <summary>
        /// ارسال لینک یا کد بازیابی
        /// </summary>
        Task<bool> SendAsync(string destination, string token, ResetChannel channel);
    }
}