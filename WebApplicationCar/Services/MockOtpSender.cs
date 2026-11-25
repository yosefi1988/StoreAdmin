using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;

using System.Diagnostics;
#pragma warning disable CS0105 // The using directive for 'System.Threading.Tasks' appeared previously in this namespace
using System.Threading.Tasks;
#pragma warning restore CS0105 // The using directive for 'System.Threading.Tasks' appeared previously in this namespace

namespace WebApplicationCar.Services
{
    public class MockOtpSender : IOtpSender
    {
        public Task<bool> SendAsync(string destination, string code, OtpChannel channel)
        {
            Debug.WriteLine($"[OTP-MOCK] {channel} → {destination} : {code}");
            // TODO: اینجا در لاگ فایل هم بنویس تا در حالت ریلیز ببینی
            return Task.FromResult(true);
        }
    }
}