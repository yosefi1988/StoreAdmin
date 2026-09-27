// ============================================
// ApiClient - ارتباط با WebApplicationApi
// ============================================

var ApiClient = (function () {

    // ⚠️ آدرس API رو با پورت واقعی جایگزین کن
    var BASE_URL = 'http://localhost:59658/';

    function request(method, endpoint, data, success, error) {
        var options = {
            url: BASE_URL + endpoint,
            type: method,
            dataType: 'json',
            success: success || function () {},
            error: error || function (xhr) {
                console.error('API Error:', method, endpoint, xhr);
                if (xhr.status === 0) {
                    alert('خطا: اتصال به سرور برقرار نشد. CORS یا آدرس API را چک کنید.');
                } else {
                    alert('خطا در ارتباط با سرور: ' + xhr.status);
                }
            }
        };

        if (data) {
            if (method === 'GET') {
                options.data = data;
            } else {
                options.contentType = 'application/json';
                options.data = JSON.stringify(data);
            }
        }

        $.ajax(options);
    }

    return {
        get: function (endpoint, params, success, error) {
            request('GET', endpoint, params, success, error);
        },
        post: function (endpoint, data, success, error) {
            request('POST', endpoint, data, success, error);
        },
        put: function (endpoint, data, success, error) {
            request('PUT', endpoint, data, success, error);
        },
        delete: function (endpoint, success, error) {
            request('DELETE', endpoint, null, success, error);
        },
        baseUrl: BASE_URL
    };
})();