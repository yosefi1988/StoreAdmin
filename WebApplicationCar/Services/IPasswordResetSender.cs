using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;

using System.Threading.Tasks;

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