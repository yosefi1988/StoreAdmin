$(function () {
    var currentStep = 1;
    var otpVerified = false;
    var timerInterval = null;

    function goToStep(step) {
        $('.wizard-panel').hide();
        $('#step-' + step).show();

        $('.wizard-step').removeClass('active done');
        $('.wizard-step').each(function () {
            var s = parseInt($(this).data('step'));
            if (s < step) $(this).addClass('done');
            else if (s === step) $(this).addClass('active');
        });

        $('#btnPrev').prop('disabled', step === 1);
        $('#btnNext').toggle(step < 4);
        $('#btnSave').toggle(step === 4);

        if (step === 4) buildSummary();
        currentStep = step;
    }

    // ---- قدم ۱ → ۲ ----
    $('#btnNext').on('click', function () {
        if (currentStep === 1) {
            var mobile = $('#MobileNumber').val().trim();
            if (!/^09\d{9}$/.test(mobile)) return alert('شماره موبایل نامعتبر');

            $.post(ApiClient.baseUrl + 'RegisterWizard/SendOtp', { mobile: mobile }, function (res) {
                if (!res.success) return alert(res.message);
                $('#maskedMobile').text(mobile.substring(0, 4) + '****' + mobile.substring(8));
                startTimer(120);
                goToStep(2);
            });
            return;
        }

        if (currentStep === 2) {
            if (!otpVerified) return alert('ابتدا کد را تأیید کنید');
            goToStep(3);
            return;
        }

        if (currentStep === 3) {
            var p = $('#Password').val();
            var cp = $('#ConfirmPassword').val();
            if (p.length < 6) return alert('رمز حداقل ۶ کاراکتر');
            if (p !== cp) return alert('رمزها یکسان نیستند');
            goToStep(4);
            return;
        }
    });

    $('#btnPrev').on('click', function () { goToStep(currentStep - 1); });

    // ---- تأیید OTP ----
    $('#OtpCode').on('input', function () {
        if (this.value.length === 6) {
            $.post(ApiClient.baseUrl + 'RegisterWizard/VerifyOtp',
                { mobile: $('#MobileNumber').val(), code: this.value },
                function (res) {
                    if (res.success) {
                        otpVerified = true;
                        stopTimer();
                        alert('تأیید شد');
                    } else {
                        alert(res.message);
                    }
                });
        }
    });

    // ---- ارسال مجدد ----
    $('#btnResendOtp').on('click', function () {
        $.post(ApiClient.baseUrl + 'RegisterWizard/SendOtp',
            { mobile: $('#MobileNumber').val() },
            function (res) {
                if (!res.success) return alert(res.message);
                startTimer(120);
                alert('کد مجدداً ارسال شد');
            });
    });

    // ---- تایمر ----
    function startTimer(seconds) {
        stopTimer();
        var remaining = seconds;
        $('#otpTimer').text('(' + remaining + ' ثانیه)');
        timerInterval = setInterval(function () {
            remaining--;
            $('#otpTimer').text('(' + remaining + ' ثانیه)');
            if (remaining <= 0) {
                stopTimer();
                $('#otpTimer').text('کد منقضی شد');
            }
        }, 1000);
    }
    function stopTimer() {
        if (timerInterval) { clearInterval(timerInterval); timerInterval = null; }
    }

    // ---- خلاصه ----
    function buildSummary() {
        var html =
            '<ul class="list-group">' +
            '<li class="list-group-item">موبایل: ' + $('#MobileNumber').val() + '</li>' +
            '<li class="list-group-item">ایمیل: ' + ($('#Email').val() || '-') + '</li>' +
            '<li class="list-group-item">نام کاربری: ' + ($('#UserName').val() || '-') + '</li>' +
            '</ul>';
        $('#summaryContainer').html(html);
    }

    // ---- ثبت نهایی ----
    $('#btnSave').on('click', function () {
        var data = {
            MobileNumber: $('#MobileNumber').val(),
            Email: $('#Email').val(),
            UserName: $('#UserName').val(),
            Password: $('#Password').val()
        };
        $.post(ApiClient.baseUrl + 'RegisterWizard/Save', data, function (res) {
            if (res.success) {
                alert('ثبت‌نام موفق');
                window.location.href = '/';
            } else {
                alert(res.message);
            }
        });
    });
});