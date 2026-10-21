using System;
using System.Threading.Tasks;
using System.Web;
using WebApplicationCar.Services;

namespace WebApplicationCar.Services
{
    /// <summary>
    /// ارسال کد بازیابی رمز عبور با کاوه‌نگار
    /// </summary>
    public class KavenegarPasswordResetSender : IPasswordResetSender
    {
        private readonly string _apiKey;
        private readonly string _senderLine;

        public KavenegarPasswordResetSender(string apiKey, string senderLine)
        {
            _apiKey = apiKey;
            _senderLine = senderLine;
        }

        public Task<bool> SendAsync(string dest, string token, ResetChannel ch)
        {
            // کاوه‌نگار فقط پیامک
            if (ch != ResetChannel.Sms)
                return Task.FromResult(false);

            //try
            //{
            //    var api = new KavenegarApi(_apiKey);
            //    var message = $"کد بازیابی: {token}";

            //    var result = api.Send(_senderLine, dest, message);

            //    return Task.FromResult(result != null && result.Count > 0);
            //}
            ////catch (ApiException ex)
            ////{
            ////    System.Diagnostics.Debug.WriteLine("[KAVENEGAR-API] " + ex.Message);
            ////    return Task.FromResult(false);
            ////}
            //catch (HttpException ex)
            //{
            //    System.Diagnostics.Debug.WriteLine("[KAVENEGAR-HTTP] " + ex.Message);
            //    return Task.FromResult(false);
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine("[KAVENEGAR] " + ex.Message);
            //    return Task.FromResult(false);
            //}
            return Task.FromResult(false);
        }
    }
}