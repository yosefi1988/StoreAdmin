$(function () {
    function showMsg(text, ok) {
        $('#msg').removeClass('alert-danger alert-success')
                  .addClass(ok ? 'alert-success' : 'alert-danger')
                  .text(text).show();
    }

    $('#btnReset').on('click', function () {
        var token = $('#Token').val();
        var np = $('#NewPassword').val();
        var cp = $('#ConfirmPassword').val();

        if (np.length < 6) return showMsg('رمز حداقل ۶ کاراکتر', false);
        if (np !== cp) return showMsg('رمزها یکسان نیستند', false);

        var $btn = $(this).prop('disabled', true).text('در حال ثبت...');

        $.post('/ForgotPassword/ResetPassword',
            { Token: token, NewPassword: np, ConfirmPassword: cp },
            function (res) {
                if (res.success) {
                    showMsg('رمز تغییر کرد! در حال انتقال...', true);
                    setTimeout(function () {
                        window.location.href = '/Login/Index';
                    }, 1500);
                } else {
                    showMsg(res.message, false);
                    $btn.prop('disabled', false).text('تغییر رمز');
                }
            }).fail(function () {
                showMsg('خطای ارتباط', false);
                $btn.prop('disabled', false).text('تغییر رمز');
            });
    });
});