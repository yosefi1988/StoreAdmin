(function ($) {
    'use strict';

    // ========================
    // وضعیت ویزارد
    // ========================
    var state = {
        currentStep: 1,
        totalSteps: 4,
        otpTimerId: null,
        otpSecondsLeft: 0,
        otpResendEnabled: true
    };

    var OTP_RESEND_SECONDS = 120; // ۲ دقیقه

    // ========================
    // المان‌ها
    // ========================
    var $steps = $('.wizard-step');
    var $panels = $('.wizard-panel');
    var $btnPrev = $('#btnPrev');
    var $btnNext = $('#btnNext');
    var $btnSave = $('#btnSave');

    // ========================
    // ابزارها
    // ========================
    function showMessage(msg, isSuccess) {
        // اگر SweetAlert دارید، اینجا استفاده کنید. در غیر این صورت alert
        if (window.Swal) {
            Swal.fire({
                text: msg,
                icon: isSuccess ? 'success' : 'error',
                confirmButtonText: 'باشه'
            });
        } else {
            alert(msg);
        }
    }

    function setLoading($btn, loading) {
        if (loading) {
            $btn.data('original-text', $btn.text());
            $btn.prop('disabled', true).text('در حال ارسال...');
        } else {
            $btn.prop('disabled', false).text($btn.data('original-text') || $btn.text());
        }
    }

    function validateMobile(mobile) {
        return /^09\d{9}$/.test(mobile);
    }

    function maskMobile(mobile) {
        if (!mobile || mobile.length < 11) return mobile;
        return mobile.substring(0, 4) + '***' + mobile.substring(7);
    }

    // ========================
    // ناوبری ویزارد
    // ========================
    function goToStep(step) {
        if (step < 1 || step > state.totalSteps) return;

        state.currentStep = step;

        // پنل‌ها
        $panels.hide();
        $('#step-' + step).show();

        // دکمه‌ها
        $btnPrev.prop('disabled', step === 1);
        $btnNext.toggle(step < state.totalSteps);
        $btnSave.toggle(step === state.totalSteps);

        // نوار مراحل
        $steps.each(function () {
            var $s = $(this);
            var n = parseInt($s.data('step'), 10);
            $s.removeClass('active done');
            if (n === step) $s.addClass('active');
            else if (n < step) $s.addClass('done');
        });

        if (step === 4) buildSummary();
    }

    // ========================
    // اعتبارسنجی هر مرحله
    // ========================
    function validateStep1() {
        var mobile = ($('#MobileNumber').val() || '').trim();
        if (!validateMobile(mobile)) {
            showMessage('شماره موبایل معتبر نیست (مثال: 09123456789)', false);
            $('#MobileNumber').focus();
            return false;
        }
        return true;
    }

    function validateStep2() {
        var code = ($('#OtpCode').val() || '').trim();
        if (!/^\d{4,6}$/.test(code)) {
            showMessage('کد تأیید را به‌درستی وارد کنید', false);
            $('#OtpCode').focus();
            return false;
        }
        return true;
    }

    function validateStep3() {
        var pwd = $('#Password').val() || '';
        var confirm = $('#ConfirmPassword').val() || '';
        if (pwd.length < 6) {
            showMessage('رمز عبور حداقل ۶ کاراکتر باشد', false);
            $('#Password').focus();
            return false;
        }
        if (pwd !== confirm) {
            showMessage('تکرار رمز با رمز یکسان نیست', false);
            $('#ConfirmPassword').focus();
            return false;
        }
        return true;
    }

    // ========================
    // خلاصه مرحله ۴
    // ========================
    function buildSummary() {
        var mobile = $('#MobileNumber').val() || '';
        var email = $('#Email').val() || '—';
        var userName = $('#UserName').val() || mobile;

        var html = ''
            + '<table class="table table-bordered">'
            + '  <tbody>'
            + '    <tr><th style="width:160px;">موبایل</th><td>' + $('<div>').text(mobile).html() + '</td></tr>'
            + '    <tr><th>ایمیل</th><td>' + $('<div>').text(email).html() + '</td></tr>'
            + '    <tr><th>نام کاربری</th><td>' + $('<div>').text(userName).html() + '</td></tr>'
            + '  </tbody>'
            + '</table>';

        $('#summaryContainer').html(html);
    }

    // ========================
    // تماس با سرور
    // ========================
    function sendOtp() {
        var mobile = ($('#MobileNumber').val() || '').trim();
        var $btn = $('#btnResendOtp');

        setLoading($btn, true);

        window.ApiClient.post(
            'RegisterWizard/SendOtp',
            { mobile: mobile },
            function (res) {
                setLoading($btn, false);
                if (res && res.success) {
                    $('#maskedMobile').text(maskMobile(mobile));
                    startOtpTimer();
                    showMessage(res.message || 'کد ارسال شد', true);
                } else {
                    showMessage((res && res.message) || 'ارسال کد ناموفق بود', false);
                }
            },
            function () {
                setLoading($btn, false);
                showMessage('خطا در ارتباط با سرور', false);
            }
        );
    }

    function verifyOtp(callback) {
        var mobile = ($('#MobileNumber').val() || '').trim();
        var code = ($('#OtpCode').val() || '').trim();

        window.ApiClient.post(
            'RegisterWizard/VerifyOtp',
            { mobile: mobile, code: code },
            function (res) {
                if (res && res.success) {
                    callback(true);
                } else {
                    showMessage((res && res.message) || 'کد نامعتبر است', false);
                    callback(false);
                }
            },
            function () {
                showMessage('خطا در ارتباط با سرور', false);
                callback(false);
            }
        );
    }

    function saveUser() {
        var data = {
            MobileNumber: ($('#MobileNumber').val() || '').trim(),
            Email: ($('#Email').val() || '').trim(),
            UserName: ($('#UserName').val() || '').trim(),
            Password: $('#Password').val() || ''
        };

        setLoading($btnSave, true);

        window.ApiClient.post(
            'RegisterWizard/Save',
            data,
            function (res) {
                setLoading($btnSave, false);
                if (res && res.success) {
                    showMessage('ثبت‌نام با موفقیت انجام شد', true);
                    // در صورت نیاز، هدایت به صفحه ورود
                    setTimeout(function () {
						window.location.href = window.ApiClient.dashboardUrl;

                    }, 1500);
                } else {
                    showMessage((res && res.message) || 'ثبت‌نام ناموفق بود', false);
                }
            },
            function () {
                setLoading($btnSave, false);
                showMessage('خطا در ارتباط با سرور', false);
            }
        );
    }

    // ========================
    // تایمر ارسال مجدد OTP
    // ========================
    function startOtpTimer() {
        stopOtpTimer();
        state.otpSecondsLeft = OTP_RESEND_SECONDS;
        updateOtpTimerUi();

        state.otpTimerId = setInterval(function () {
            state.otpSecondsLeft--;
            if (state.otpSecondsLeft <= 0) {
                stopOtpTimer();
                $('#otpTimer').text('');
                $('#btnResendOtp').prop('disabled', false);
            } else {
                updateOtpTimerUi();
            }
        }, 1000);
    }

    function stopOtpTimer() {
        if (state.otpTimerId) {
            clearInterval(state.otpTimerId);
            state.otpTimerId = null;
        }
    }

    function updateOtpTimerUi() {
        var m = Math.floor(state.otpSecondsLeft / 60);
        var s = state.otpSecondsLeft % 60;
        $('#otpTimer').text('ارسال مجدد تا ' + m + ':' + (s < 10 ? '0' + s : s));
        $('#btnResendOtp').prop('disabled', state.otpSecondsLeft > 0);
    }

    // ========================
    // رویدادها
    // ========================
    $(function () {
        goToStep(1);

        // بعدی
        $btnNext.on('click', function () {
            var step = state.currentStep;

            if (step === 1) {
                if (!validateStep1()) return;
                // رفتن به قدم ۲ و ارسال OTP
                //goToStep(2);
                goToStep(3);
                //sendOtp();
            } else if (step === 2) {

                if (!validateStep2()) return;
                verifyOtp(function (ok) {
                    if (ok) goToStep(3);
                });
            } else if (step === 3) {
                if (!validateStep3()) return;
                goToStep(4);
            }
        });

        // قبلی
        $btnPrev.on('click', function () {
            if (state.currentStep > 1) {
                goToStep(state.currentStep - 1);
            }
        });

        // ارسال مجدد OTP
        $('#btnResendOtp').on('click', function () {
            if (state.otpSecondsLeft > 0) return;
            sendOtp();
        });

        // ثبت نهایی
        $btnSave.on('click', function () {
            if (!validateStep3()) {
                goToStep(3);
                return;
            }
            saveUser();
        });

        // فقط عدد در موبایل و OTP
        $('#MobileNumber, #OtpCode').on('input', function () {
            this.value = this.value.replace(/\D/g, '');
        });
    });

})(jQuery);