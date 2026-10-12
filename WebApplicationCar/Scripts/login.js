$(function () {
    function showMessage(text, isSuccess) {
        var $el = $('#loginMessage');
        $el.removeClass('alert-danger alert-success')
           .addClass(isSuccess ? 'alert-success' : 'alert-danger')
           .text(text)
           .show();
    }

    // Enter key
    $('#Mobile, #Password').on('keypress', function (e) {
        if (e.which === 13) $('#btnLogin').click();
    });

    $('#btnLogin').on('click', function () {
        var mobile = $('#Mobile').val().trim();
        var password = $('#Password').val();
        var remember = $('#RememberMe').is(':checked');

        if (!/^09\d{9}$/.test(mobile)) {
            return showMessage('شماره موبایل نامعتبر', false);
        }
        if (!password) {
            return showMessage('رمز عبور را وارد کنید', false);
        }

        var $btn = $(this).prop('disabled', true).text('در حال ورود...');

        ApiClient.post('Login/Login',
            { Mobile: mobile, Password: password, RememberMe: remember },
            function (res) {
                if (res.success) {
                    showMessage('ورود موفق! در حال انتقال...', true);
                    // ذخیره توکن برای درخواست‌های بعدی
                    localStorage.setItem('auth_token', res.token);
                    setTimeout(function () {
                        window.location.href = '/';
                    }, 800);
                } else {
                    showMessage(res.message, false);
                    $btn.prop('disabled', false).text('ورود');
                }
            },
            function () {
                showMessage('خطای ارتباط با سرور', false);
                $btn.prop('disabled', false).text('ورود');
            });
    });
});