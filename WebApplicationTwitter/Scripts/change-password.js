$(function () {
    function showMsg(text, ok) {
        $('#msg').removeClass('alert-danger alert-success')
                  .addClass(ok ? 'alert-success' : 'alert-danger')
                  .text(text).show();
    }

    $('#btnChange').on('click', function () {
        var cur = $('#CurrentPassword').val();
        var np = $('#NewPassword').val();
        var cp = $('#ConfirmPassword').val();

        if (!cur) return showMsg('رمز فعلی را وارد کنید', false);
        if (np.length < 6) return showMsg('رمز جدید حداقل ۶ کاراکتر', false);
        if (np !== cp) return showMsg('رمزها یکسان نیستند', false);

        var $btn = $(this).prop('disabled', true).text('در حال ثبت...');

        $.post('/ChangePassword/Save',
            { CurrentPassword: cur, NewPassword: np, ConfirmPassword: cp },
            function (res) {
                if (res.success) {
                    showMsg(res.message, true);
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