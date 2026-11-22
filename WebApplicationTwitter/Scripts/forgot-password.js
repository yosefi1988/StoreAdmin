$(function () {
    function showMsg(text, ok) {
        $('#msg').removeClass('alert-danger alert-success')
                  .addClass(ok ? 'alert-success' : 'alert-danger')
                  .text(text).show();
    }

    $('#Mobile').on('keypress', function (e) {
        if (e.which === 13) $('#btnSendReset').click();
    });

    $('#btnSendReset').on('click', function () {
        var mobile = $('#Mobile').val().trim();
        if (!/^09\d{9}$/.test(mobile))
            return showMsg('شماره موبایل نامعتبر', false);

        var $btn = $(this).prop('disabled', true).text('در حال ارسال...');

        $.post('/ForgotPassword/RequestReset', { Mobile: mobile }, function (res) {
            showMsg(res.message, res.success);
            $btn.prop('disabled', false).text('ارسال لینک بازیابی');
        }).fail(function () {
            showMsg('خطای ارتباط', false);
            $btn.prop('disabled', false).text('ارسال لینک بازیابی');
        });
    });
});