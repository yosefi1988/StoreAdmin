$(function () {
    var currentPage = 1;
    var pageSize = 20;
    var searchTerm = window.INITIAL_SEARCH || '';

    // ============ لود محصولات ============
    function loadProducts(page) {
        currentPage = page || 1;
        pageSize = parseInt($('#ddlPageSize').val()) || 20;

        $('#loading').show();
        $('#error').hide();

        var params = {
            page: currentPage,
            pageSize: pageSize,
            search: searchTerm
        };

        // ← از ApiClient.baseUrl استفاده می‌کنه
        $.get(ApiClient.baseUrl + 'Products/GetProducts', params, function (res) {
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

            var detailsUrl = ApiClient.baseUrl + 'Products/Details/' + p.ProductId;

            var row = '<tr style="cursor:pointer;" data-href="' + detailsUrl + '">' +
                '<td>' + (p.ProductId || '-') + '</td>' +
                '<td>' + (p.NameFa || '-') + '</td>' +
                '<td>' + (p.NameEn || '-') + '</td>' +
                '<td>' + (p.ProductCode || '-') + '</td>' +
                '<td>' + (p.Barcode || '-') + '</td>' +
                '<td>' + status + '</td>' +
                '<td>' +
                    '<a href="' + detailsUrl + '" class="btn btn-xs btn-info btn-details">جزئیات</a>' +
                '</td>' +
            '</tr>';
            tbody.append(row);
        });
    }

    // ============ کلیک روی ردیف → صفحه جزئیات ============
    $(document).on('click', '#tblProducts tbody tr', function (e) {
        if ($(e.target).closest('.btn-details').length) return;

        var href = $(this).data('href');
        if (href) {
            window.location.href = href;
        }
    });

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

    // ============ جستجو (سرور-محور با debounce) ============
    var searchTimer = null;
    $('#txtSearch').on('keyup', function () {
        clearTimeout(searchTimer);
        searchTimer = setTimeout(function () {
            searchTerm = $('#txtSearch').val().trim();
            loadProducts(1);
        }, 500);
    });

    // Enter هم فوری جستجو کنه
    $('#txtSearch').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            clearTimeout(searchTimer);
            searchTerm = $('#txtSearch').val().trim();
            loadProducts(1);
        }
    });

    // شروع
    loadProducts(1);
});