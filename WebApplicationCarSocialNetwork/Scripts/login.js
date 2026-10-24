/* ============================================================
   login.js  —  نسخه‌ی نهایی
   منطبق با:
     - Login/Index.cshtml  (window.ApiClient, window.ReturnUrl)
     - LoginController.Login  (پارامتر returnUrl، پاسخ redirectUrl)
     - PostsController.NeedLoginResult  (loginUrl با returnUrl)
   ============================================================ */

(function ($) {
    'use strict';

    // ----------------------------------------------------------
    // تنظیمات پیش‌فرض (در صورت نبود در ویو)
    // ----------------------------------------------------------
    var DEFAULTS = {
        loginUrl: '/Login/Login',
        feedUrl: '/Posts/Feed',
        homeUrl: '/'
    };

    // ----------------------------------------------------------
    // ابزارهای کوچک
    // ----------------------------------------------------------
    function getLoginUrl() {
        return (window.ApiClient && window.ApiClient.loginUrl)
            ? window.ApiClient.loginUrl
            : DEFAULTS.loginUrl;
    }

    function getFallbackUrl() {
        if (window.ApiClient && window.ApiClient.feedUrl) return window.ApiClient.feedUrl;
        return DEFAULTS.feedUrl || DEFAULTS.homeUrl;
    }

    function showMessage(text, type) {
        type = type || 'danger';
        var $el = $('#loginMessage');
        $el
            .removeClass('alert-danger alert-success alert-warning alert-info')
            .addClass('alert-' + type)
            .text(text)
            .show();
    }

    function hideMessage() {
        $('#loginMessage').hide().text('');
    }

    function setLoading($btn, loading) {
        if (loading) {
            if (!$btn.data('original-text')) {
                $btn.data('original-text', $btn.text());
            }
            $btn.prop('disabled', true).text('در حال ورود...');
        } else {
            $btn
                .prop('disabled', false)
                .text($btn.data('original-text') || 'ورود');
        }
    }

    function isValidMobile(mobile) {
        // فرمت ایران: 09xxxxxxxxx  (۱۱ رقم)
        return /^09\d{9}$/.test(mobile);
    }

    // ----------------------------------------------------------
    // اعتبارسنجی
    // ----------------------------------------------------------
    function validate(mobile, password) {
        if (!mobile) {
            showMessage('شماره موبایل را وارد کنید.', 'warning');
            $('#Mobile').focus();
            return false;
        }
        if (!isValidMobile(mobile)) {
            showMessage('شماره موبایل معتبر نیست. مثال: 09123456789', 'warning');
            $('#Mobile').focus();
            return false;
        }
        if (!password) {
            showMessage('رمز عبور را وارد کنید.', 'warning');
            $('#Password').focus();
            return false;
        }
        if (password.length < 6) {
            showMessage('رمز عبور حداقل ۶ کاراکتر است.', 'warning');
            $('#Password').focus();
            return false;
        }
        return true;
    }

    // ----------------------------------------------------------
    // ارسال درخواست ورود
    // ----------------------------------------------------------
    function doLogin() {
        hideMessage();

        var $btn = $('#btnLogin');
        var mobile = ($('#Mobile').val() || '').trim();
        var password = $('#Password').val() || '';
        var remember = $('#RememberMe').is(':checked');

        if (!validate(mobile, password)) return;

        var payload = {
            Mobile: mobile,
            Password: password,
            RememberMe: remember,
            returnUrl: window.ReturnUrl || null
        };

        setLoading($btn, true);

        $.ajax({
            url: getLoginUrl(),
            type: 'POST',
            dataType: 'json',
            cache: false,
            data: payload
        })
        .done(function (res) {
            if (!res || !res.success) {
                showMessage(
                    (res && res.message) || 'ورود ناموفق بود.',
                    'danger'
                );
                setLoading($btn, false);
                return;
            }

            // ⚠️ توجه: auth_token در کوکی HttpOnly از سمت سرور ست می‌شود
            // و عمداً در localStorage ذخیره نمی‌شود (جلوگیری از XSS)
            showMessage(res.message || 'ورود موفق! در حال انتقال...', 'success');

            var target = res.redirectUrl || getFallbackUrl();

            // کمی تأخیر تا کاربر پیام موفقیت را ببیند
            setTimeout(function () {
                window.location.href = target;
            }, 700);
        })
        .fail(function (xhr) {
            var text = 'خطا در ارتباط با سرور.';

            if (xhr) {
                if (xhr.status === 0) {
                    text = 'اتصال به سرور قطع است.';
                } else if (xhr.responseJSON && xhr.responseJSON.message) {
                    text = xhr.responseJSON.message;
                } else if (xhr.status === 500) {
                    text = 'خطای داخلی سرور.';
                } else if (xhr.status === 404) {
                    text = 'سرویس ورود یافت نشد.';
                } else if (xhr.status === 403) {
                    text = 'دسترسی مجاز نیست.';
                }
            }

            showMessage(text, 'danger');
            setLoading($btn, false);
        });
    }

    // ----------------------------------------------------------
    // راه‌اندازی پس از لود DOM
    // ----------------------------------------------------------
    $(function () {
        var $mobile = $('#Mobile');
        var $password = $('#Password');
        var $btn = $('#btnLogin');

        if (!$btn.length) return; // این صفحه، صفحه‌ی لاگین نیست

        // کلیک روی دکمه
        $btn.on('click', function (e) {
            e.preventDefault();
            doLogin();
        });

        // Enter در فیلدها
        $mobile.add($password).on('keypress', function (e) {
            if (e.which === 13 || e.keyCode === 13) {
                e.preventDefault();
                doLogin();
            }
        });

        // پاک‌کردن پیام هنگام تایپ
        $mobile.add($password).on('input', hideMessage);

        // فقط رقم در موبایل
        $mobile.on('input', function () {
            this.value = this.value.replace(/[^\d]/g, '').slice(0, 11);
        });

        // فوکوس اولیه
        if (!$mobile.val()) {
            $mobile.focus();
        } else {
            $password.focus();
        }
    });

})(jQuery);