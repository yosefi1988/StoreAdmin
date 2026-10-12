using System;
using System.Security.Cryptography;

namespace WebApplicationCar.Services
{
    /// <summary>
    /// هش و بررسی رمز عبور با PBKDF2 (Rfc2898DeriveBytes)
    /// سازگار با .NET Framework 4.5.2 — بدون وابستگی خارجی
    /// </summary>
    public static class PasswordHasher
    {
        // ====== تنظیمات ======
        private const int SaltSize = 16;        // 128-bit salt
        private const int HashSize = 32;        // 256-bit hash
        private const int Iterations = 100000;  // تعداد تکرار (بالاتر = امن‌تر ولی کندتر)
        private const int Version = 1;          // نسخه الگوریتم (برای مهاجرت آینده)

        /// <summary>
        /// تبدیل رمز خام به هش امن
        /// </summary>
        public static string Hash(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("رمز نمی‌تواند خالی باشد", nameof(password));

            // 1. تولید Salt تصادفی
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // 2. استخراج کلید از رمز + Salt
            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                hash = pbkdf2.GetBytes(HashSize);
            }

            // 3. ساختار نهایی: [Version(1)] + [Salt(16)] + [Hash(32)]
            byte[] result = new byte[1 + SaltSize + HashSize];
            result[0] = (byte)Version;
            Array.Copy(salt, 0, result, 1, SaltSize);
            Array.Copy(hash, 0, result, 1 + SaltSize, HashSize);

            // 4. Base64 برای ذخیره در دیتابیس
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// بررسی رمز ورودی با هش ذخیره‌شده
        /// </summary>
        public static bool Verify(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            try
            {
                byte[] data = Convert.FromBase64String(hashedPassword);

                // حداقل طول: 1 + 16 + 32 = 49
                if (data.Length < 1 + SaltSize + HashSize)
                    return false;

                // 1. خواندن نسخه
                int version = data[0];
                if (version != Version)
                    return false; // نسخه ناشناخته

                // 2. جدا کردن Salt
                byte[] salt = new byte[SaltSize];
                Array.Copy(data, 1, salt, 0, SaltSize);

                // 3. جدا کردن Hash اصلی
                byte[] storedHash = new byte[HashSize];
                Array.Copy(data, 1 + SaltSize, storedHash, 0, HashSize);

                // 4. محاسبه مجدد هش با همان Salt
                byte[] computedHash;
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
                {
                    computedHash = pbkdf2.GetBytes(HashSize);
                }

                // 5. مقایسه زمان-ثابت (مقاوم به Timing Attack)
                return FixedTimeEquals(storedHash, computedHash);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// مقایسه زمان-ثابت برای جلوگیری از Timing Attack
        /// </summary>
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        /// <summary>
        /// بررسی نیاز به Rehash (اگه Iterations تغییر کرده)
        /// </summary>
        public static bool NeedsRehash(string hashedPassword)
        {
            // در این نسخه ساده، همیشه false — ولی برای مهاجرت آینده آماده است
            return false;
        }
    }
}