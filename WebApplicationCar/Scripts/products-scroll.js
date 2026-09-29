$(function () {
    var skip = 0;
    var take = 12;
    var isLoading = false;
    var hasMore = true;
    var total = 0;

    // ============ لود محصولات ============
    function loadMore() {
        if (isLoading || !hasMore) return;

        isLoading = true;
        $('#loading').show();
        $('#error').hide();

        $.get(ApiClient.baseUrl + 'Products/GetProductsScroll', { skip: skip, take: take }, function (res) {
            isLoading = false;
            $('#loading').hide();

            if (!res || !res.Data) {
                $('#error').text('پاسخ نامعتبر از سرور').show();
                return;
            }

            total = res.Total;
            hasMore = res.HasMore;

            renderProducts(res.Data);
            skip += take;

            if (!hasMore) {
                $('#endMessage').show();
            }
        }).fail(function (xhr) {
            isLoading = false;
            $('#loading').hide();
            $('#error').text('خطا در بارگذاری: ' + xhr.status).show();
        });
    }

    // ============ رندر کارت‌ها ============
    function renderProducts(products) {
        var container = $('#productsContainer');

        if (!products || products.length === 0) {
            if (skip === 0) {
                container.html('<div class="col-12 text-center"><p>محصولی یافت نشد</p></div>');
            }
            return;
        }

        $.each(products, function (i, p) {
            var imageHtml = p.ImageUrl
                ? '<img src="' + p.ImageUrl + '" class="card-img" alt="' + (p.NameFa || '') + '" onerror="this.src=\'/Content/no-image.png\'" />'
                : '<div class="card-img no-image">بدون تصویر</div>';

            var statusBadge = p.IsActive
                ? '<span class="badge badge-success">فعال</span>'
                : '<span class="badge badge-secondary">غیرفعال</span>';

            var detailsUrl = ApiClient.baseUrl + 'Products/Details/' + p.ProductId;

            var card =
                '<div class="col-md-4 col-sm-6">' +
                    '<div class="product-card" style="cursor:pointer;" data-href="' + detailsUrl + '">' +
                        imageHtml +
                        '<div class="card-body">' +
                            '<h5 class="card-title">' + (p.NameFa || '-') + '</h5>' +
                            '<p class="card-subtitle">' + (p.NameEn || '') + '</p>' +
                            '<div class="card-meta">کد محصول: ' + (p.ProductCode || '-') + '</div>' +
                            '<div class="card-meta">بارکد: ' + (p.Barcode || '-') + '</div>' +
                        '</div>' +
                        '<div class="card-footer">' +
                            statusBadge +
                            '<a href="' + detailsUrl + '" class="btn btn-sm btn-primary float-right btn-details">جزئیات</a>' +
                        '</div>' +
                    '</div>' +
                '</div>';

            container.append(card);
        });
    }

    // ============ کلیک روی کارت → صفحه جزئیات ============
    $(document).on('click', '.product-card', function (e) {
        // اگه روی دکمه جزئیات کلیک شد، نذار دوبار redirect بشه
        if ($(e.target).closest('.btn-details').length) return;

        var href = $(this).data('href');
        if (href) {
            window.location.href = href;
        }
    });

    // ============ Infinite Scroll ============
    function isNearBottom() {
        return $(window).scrollTop() + $(window).height() >= $(document).height() - 300;
    }

    $(window).on('scroll', function () {
        if (isNearBottom()) {
            loadMore();
        }
    });

    // ============ شروع ============
    loadMore();
});