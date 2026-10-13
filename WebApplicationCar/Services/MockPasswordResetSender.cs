using System.Diagnostics;
using System.Threading.Tasks;

namespace WebApplicationCar.Services
{
    /// <summary>
    /// پیاده‌سازی موقت — بدون ارسال واقعی
    /// فقط توی Debug لاگ می‌کنه
    /// </summary>
    public class MockPasswordResetSender : IPasswordResetSender
    {
        public Task<bool> SendAsync(string destination, string token, ResetChannel channel)
        {
            Debug.WriteLine($"[RESET-MOCK] {channel} → {destination}");
            Debug.WriteLine($"[RESET-MOCK] Token: {token}");
            Debug.WriteLine($"[RESET-MOCK] Link: /ForgotPassword/Reset?token={token}");
            return Task.FromResult(true);
        }
    }
}