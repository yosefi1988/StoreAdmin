$(function () {
    var currentPage = 1;
    var pageSize = 20;

    // ============ لود محصولات ============
    function loadProducts(page) {
        currentPage = page || 1;
        pageSize = parseInt($('#ddlPageSize').val()) || 20;

        $('#loading').show();
        $('#error').hide();

        var params = {
            page: currentPage,
            pageSize: pageSize
        };

        // ← مستقیم از همین پروژه (بدون API جدا)
        $.get('/Products/GetProducts', params, function (res) {
            $('#loading').hide();

            if (!res || !res.Data) {
                $('#error').text('پاسخ نامعتبر از سرور').show();
                return;
            }

            renderProducts(res.Data);
            renderPagination(res.TotalPages, res.Page, res.Total);
        }).fail(function (xhr) {
            $('#loading').hide();
            $('#error').text('خطا در بارگذاری: ' + xhr.status).show();
        });
    }

    // ============ رندر جدول ============
    function renderProducts(products) {
        var tbody = $('#tblProducts tbody').empty();

        if (!products || products.length === 0) {
            tbody.append('<tr><td colspan="7" class="text-center">محصولی یافت نشد</td></tr>');
            return;
        }

        $.each(products, function (i, p) {
            var status = p.IsActive
                ? '<span class="label label-success">فعال</span>'
                : '<span class="label label-default">غیرفعال</span>';

            var row = '<tr>' +
                '<td>' + (p.ProductId || '-') + '</td>' +
                '<td>' + (p.NameFa || '-') + '</td>' +
                '<td>' + (p.NameEn || '-') + '</td>' +
                '<td>' + (p.ProductCode || '-') + '</td>' +
                '<td>' + (p.Barcode || '-') + '</td>' +
                '<td>' + status + '</td>' +
                '<td>' +
                    '<a href="/Products/Details/' + p.ProductId + '" class="btn btn-xs btn-info">جزئیات</a>' +
                '</td>' +
            '</tr>';
            tbody.append(row);
        });
    }

    // ============ صفحه‌بندی ============
    function renderPagination(totalPages, current, total) {
        var ul = $('#pagination').empty();

        $('#paginationInfo').text('کل: ' + total + ' محصول | صفحه ' + current + ' از ' + totalPages);

        if (totalPages <= 1) return;

        // قبلی
        ul.append('<li class="' + (current === 1 ? 'disabled' : '') + '">' +
            '<a href="#" data-page="' + (current - 1) + '">«</a></li>');

        // شماره‌ها
        var start = Math.max(1, current - 2);
        var end = Math.min(totalPages, current + 2);

        if (start > 1) {
            ul.append('<li><a href="#" data-page="1">1</a></li>');
            if (start > 2) ul.append('<li class="disabled"><a href="#">...</a></li>');
        }

        for (var i = start; i <= end; i++) {
            ul.append('<li class="' + (i === current ? 'active' : '') + '">' +
                '<a href="#" data-page="' + i + '">' + i + '</a></li>');
        }

        if (end < totalPages) {
            if (end < totalPages - 1) ul.append('<li class="disabled"><a href="#">...</a></li>');
            ul.append('<li><a href="#" data-page="' + totalPages + '">' + totalPages + '</a></li>');
        }

        // بعدی
        ul.append('<li class="' + (current === totalPages ? 'disabled' : '') + '">' +
            '<a href="#" data-page="' + (current + 1) + '">»</a></li>');
    }

    // ============ رویدادها ============
    $(document).on('click', '#pagination a', function (e) {
        e.preventDefault();
        var page = parseInt($(this).data('page'));
        if (page && page >= 1) {
            loadProducts(page);
        }
    });

    $('#ddlPageSize').change(function () {
        loadProducts(1);
    });

    // جستجو سمت کلاینت
    $('#txtSearch').on('keyup', function () {
        var value = $(this).val().toLowerCase().trim();
        $('#tblProducts tbody tr').each(function () {
            var text = $(this).text().toLowerCase();
            $(this).toggle(text.indexOf(value) > -1);
        });
    });

    // شروع
    loadProducts(1);
});