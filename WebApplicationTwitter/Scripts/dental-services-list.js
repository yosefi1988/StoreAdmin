(function () {
    var state = {
        page: 1,
        pageSize: 20,
        search: window.INITIAL_SEARCH || ''
    };

    function formatPrice(v) {
        if (!v) return 'تماس بگیرید';
        return Number(v).toLocaleString('fa-IR') + ' تومان';
    }

    function escapeHtml(s) {
        if (!s) return '';
        return String(s).replace(/[&<>"']/g, function (m) {
            return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m];
        });
    }

    function load() {
        $('#loading').show();
        $('#error').hide();
        $.ajax({
            url: window.ApiClient.baseUrl + 'DentalServices/GetDentalServices',
            data: {
                page: state.page,
                pageSize: state.pageSize,
                search: state.search
            },
            dataType: 'json',
            success: function (res) {
                $('#loading').hide();
                renderCards(res.Data || []);
                renderPagination(res);
            },
            error: function (xhr) {
                $('#loading').hide();
                $('#error').show().text('خطا در بارگذاری: ' + xhr.statusText);
            }
        });
    }

    function renderCards(items) {
        var $c = $('#cardsContainer').empty();
        if (!items.length) {
            $c.append('<div class="col-12 text-center text-muted">خدمتی یافت نشد.</div>');
            return;
        }
        items.forEach(function (it) {
            var img = it.ImageUrl || '/Content/images/no-image.png';
            var html =
                '<div class="col-md-4 col-sm-6" style="margin-bottom:20px;">' +
                '  <div class="card h-100">' +
                '    <img src="' + escapeHtml(img) + '" class="card-img-top" alt="' + escapeHtml(it.NameFa) + '" style="height:180px;object-fit:cover;">' +
                '    <div class="card-body">' +
                '      <h5 class="card-title">' + escapeHtml(it.NameFa) + '</h5>' +
                '      <p class="card-text text-muted" style="min-height:40px;">' + escapeHtml(it.Description || '') + '</p>' +
                '      <p><strong>قیمت:</strong> ' + formatPrice(it.Price) + ' / ' + escapeHtml(it.UnitName || '') + '</p>' +
                '      <p><strong>نوبت‌های آزاد:</strong> ' + (it.AvailableSlots || 0) + '</p>' +
                '      <a href="' + window.ApiClient.baseUrl + 'DentalServices/Details/' + it.ServiceId + '" class="btn btn-primary btn-sm">مشاهده و رزرو</a>' +
                '    </div>' +
                '  </div>' +
                '</div>';
            $c.append(html);
        });
    }

    function renderPagination(res) {
        var $p = $('#pagination').empty();
        var total = res.TotalPages || 0;
        if (total <= 1) {
            $('#paginationInfo').text('مجموع: ' + (res.Total || 0) + ' خدمت');
            return;
        }
        function addLi(page, label, disabled, active) {
            var cls = 'page-item' + (disabled ? ' disabled' : '') + (active ? ' active' : '');
            $p.append('<li class="' + cls + '"><a class="page-link" href="#" data-page="' + page + '">' + label + '</a></li>');
        }
        addLi(state.page - 1, 'قبلی', state.page <= 1, false);
        for (var i = 1; i <= total; i++) {
            if (i === 1 || i === total || Math.abs(i - state.page) <= 2) {
                addLi(i, i, false, i === state.page);
            } else if (i === 2 || i === total - 1) {
                $p.append('<li class="page-item disabled"><span class="page-link">...</span></li>');
            }
        }
        addLi(state.page + 1, 'بعدی', state.page >= total, false);
        $('#paginationInfo').text('صفحه ' + state.page + ' از ' + total + ' — مجموع ' + res.Total + ' خدمت');
    }

    $(function () {
        $('#btnSearch').on('click', function () {
            state.search = $('#txtSearch').val();
            state.page = 1;
            load();
        });
        $('#txtSearch').on('keypress', function (e) {
            if (e.which === 13) { $('#btnSearch').click(); }
        });
        $('#ddlPageSize').on('change', function () {
            state.pageSize = parseInt($(this).val(), 10);
            state.page = 1;
            load();
        });
        $('#pagination').on('click', 'a.page-link', function (e) {
            e.preventDefault();
            var p = parseInt($(this).data('page'), 10);
            if (isNaN(p) || p < 1) return;
            state.page = p;
            load();
        });
        load();
    });
})();