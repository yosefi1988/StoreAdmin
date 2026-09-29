$(function () {
    // ============ State ============
    var currentStep = 1;
    var totalSteps = 4;
    var allAttributes = [];
    var variantCounter = 0;

    // ============ Refresh Variants ============
    function refreshVariants() {
        if (allAttributes.length > 0) {
            renderVariantAttributes();
        }
    }

    // ============ Navigation ============
    function goToStep(step) {
        if (step < 1 || step > totalSteps) return;

        // قدم ۲: لود دسته‌بندی‌ها و صفت‌ها (اگه هنوز لود نشدن)
        if (step === 2) {
            if ($('#categoriesList').text().indexOf('در حال') > -1) {
                loadCategories();
            }
            if (allAttributes.length === 0) {
                loadAttributes();
            } else {
                refreshVariants();
            }
        }

        // قدم ۳: لود صفت‌ها (اگه لازم بود)
        if (step === 3 && allAttributes.length === 0) {
            loadAttributes();
        }

        // قدم ۴: ساخت خلاصه
        if (step === 4) {
            buildSummary();
        }

        // مخفی کردن همه
        $('.wizard-panel').hide();
        $('#step-' + step).show();

        // آپدیت Progress
        $('.wizard-step').each(function () {
            var s = parseInt($(this).data('step'));
            $(this).removeClass('active done');
            if (s < step) $(this).addClass('done');
            if (s === step) $(this).addClass('active');
        });

        // دکمه‌ها
        $('#btnPrev').prop('disabled', step === 1);
        if (step === totalSteps) {
            $('#btnNext').hide();
            $('#btnSave').show();
        } else {
            $('#btnNext').show();
            $('#btnSave').hide();
        }

        currentStep = step;
    }

    $('#btnNext').click(function () {
        if (!validateStep(currentStep)) return;
        goToStep(currentStep + 1);
    });

    $('#btnPrev').click(function () {
        goToStep(currentStep - 1);
    });

    // ============ Validation ============
    function validateStep(step) {
        if (step === 1) {
            if (!$('#NameFa').val().trim()) {
                alert('نام فارسی الزامی است');
                return false;
            }
        }
        if (step === 2) {
            var any = $('#categoriesList input:checked').length > 0;
            if (!any) {
                if (!confirm('هیچ دسته‌بندی انتخاب نشده. ادامه می‌دهید؟'))
                    return false;
            }
        }
        return true;
    }

    // ============ قدم ۱: Variants ============
    $('#btnAddVariant').click(function () {
        addVariantRow();
    });

    function addVariantRow(data) {
        data = data || {};
        var idx = variantCounter++;
        var row = '<tr data-idx="' + idx + '">' +
            '<td><input type="text" class="form-control v-sku" value="' + (data.SKU || '') + '" /></td>' +
            '<td><input type="number" class="form-control v-stock" value="' + (data.StockQuantity || 0) + '" /></td>' +
            '<td><input type="number" class="form-control v-price" value="' + (data.Price || '') + '" /></td>' +
            '<td><input type="checkbox" class="v-active" ' + (data.IsActive !== false ? 'checked' : '') + ' /></td>' +
            '<td><button type="button" class="btn btn-sm btn-danger btn-remove-variant">×</button></td>' +
        '</tr>';
        $('#tblVariants tbody').append(row);

        // ← بعد از اضافه کردن، اگه صفت‌ها لود شدن، رندر کن
        refreshVariants();
    }

    $(document).on('click', '.btn-remove-variant', function () {
        $(this).closest('tr').remove();
        refreshVariants();
    });

    // ============ قدم ۲: Categories ============
    function loadCategories() {
        ApiClient.get('productwizard/GetCategories', null, function (data) {
            var html = '';
            if (!data || data.length === 0) {
                html = '<em>دسته‌بندی‌ای یافت نشد</em>';
            } else {
                $.each(data, function (i, c) {
                    html += '<div class="checkbox">' +
                        '<label><input type="checkbox" class="cat-checkbox" value="' + c.CategoryId + '" /> ' +
                        c.NameFa + ' <small class="text-muted">(' + c.CategoryType + ')</small></label>' +
                    '</div>';
                });
            }
            $('#categoriesList').html(html);
        });
    }

    // ============ قدم ۲: Attributes ============
    function loadAttributes() {
        ApiClient.get('productwizard/GetAttributes', null, function (data) {
            allAttributes = data || [];
            renderVariantAttributes();
        });
    }

    function renderVariantAttributes() {
        var container = $('#variantAttributesContainer').empty();
        var variantRows = $('#tblVariants tbody tr');

        if (variantRows.length === 0) {
            container.html('<em class="text-muted">ابتدا در قدم ۱ واریانت اضافه کنید</em>');
            return;
        }

        variantRows.each(function (i) {
            var sku = $(this).find('.v-sku').val() || 'بدون SKU';
            var box = '<div class="panel panel-default">' +
                '<div class="panel-heading"><b>واریانت ' + (i + 1) + ':</b> ' + sku + '</div>' +
                '<div class="panel-body">' +
                    '<button type="button" class="btn btn-sm btn-success btn-add-var-attr" data-variant-idx="' + i + '">+ افزودن صفت</button>' +
                    '<table class="table table-condensed" style="margin-top:10px;">' +
                        '<thead><tr><th>صفت</th><th>مقدار</th><th></th></tr></thead>' +
                        '<tbody class="var-attr-body" data-variant-idx="' + i + '"></tbody>' +
                    '</table>' +
                '</div>' +
            '</div>';
            container.append(box);
        });
    }

    $(document).on('click', '.btn-add-var-attr', function () {
        var variantIdx = $(this).data('variant-idx');
        var tbody = $('.var-attr-body[data-variant-idx="' + variantIdx + '"]');

        var attrOptions = '<option value="">انتخاب صفت...</option>';
        $.each(allAttributes, function (i, a) {
            attrOptions += '<option value="' + a.AttributeId + '">' + a.NameFa + '</option>';
        });

        var row = '<tr>' +
            '<td><select class="form-control attr-select">' + attrOptions + '</select></td>' +
            '<td><select class="form-control attr-value-select"><option value="">ابتدا صفت را انتخاب کنید</option></select></td>' +
            '<td><button type="button" class="btn btn-sm btn-danger btn-remove-row">×</button></td>' +
        '</tr>';
        tbody.append(row);
    });

    $(document).on('change', '.attr-select', function () {
        var attrId = $(this).val();
        var valueSelect = $(this).closest('tr').find('.attr-value-select');

        if (!attrId) {
            valueSelect.html('<option value="">ابتدا صفت را انتخاب کنید</option>');
            return;
        }

        valueSelect.html('<option value="">در حال بارگذاری...</option>');
        ApiClient.get('productwizard/GetAttributeValues', { attributeId: attrId }, function (data) {
            var html = '<option value="">انتخاب مقدار...</option>';
            $.each(data, function (i, v) {
                html += '<option value="' + v.AttributeValueId + '">' + v.ValueFa + '</option>';
            });
            valueSelect.html(html);
        });
    });

    $(document).on('click', '.btn-remove-row', function () {
        $(this).closest('tr').remove();
    });

    // ============ قدم ۳: Resource Attributes ============
    $('#btnAddResourceAttr').click(function () {
        if (allAttributes.length === 0) {
            alert('ابتدا صفت‌ها را بارگذاری کنید');
            return;
        }
        var attrOptions = '<option value="">انتخاب صفت...</option>';
        $.each(allAttributes, function (i, a) {
            attrOptions += '<option value="' + a.AttributeId + '">' + a.NameFa + '</option>';
        });

        var row = '<tr>' +
            '<td><select class="form-control ra-attr">' + attrOptions + '</select></td>' +
            '<td><select class="form-control ra-value"><option value="">ابتدا صفت را انتخاب کنید</option></select></td>' +
            '<td><button type="button" class="btn btn-sm btn-danger btn-remove-row">×</button></td>' +
        '</tr>';
        $('#tblResourceAttrs tbody').append(row);
    });

    $(document).on('change', '.ra-attr', function () {
        var attrId = $(this).val();
        var valueSelect = $(this).closest('tr').find('.ra-value');

        if (!attrId) {
            valueSelect.html('<option value="">ابتدا صفت را انتخاب کنید</option>');
            return;
        }

        valueSelect.html('<option value="">در حال بارگذاری...</option>');
        ApiClient.get('productwizard/GetAttributeValues', { attributeId: attrId }, function (data) {
            var html = '<option value="">انتخاب مقدار...</option>';
            $.each(data, function (i, v) {
                html += '<option value="' + v.AttributeValueId + '">' + v.ValueFa + '</option>';
            });
            valueSelect.html(html);
        });
    });

    // ============ قدم ۳: Images ============
    $('#btnAddImage').click(function () {
        var row = '<tr>' +
            '<td><input type="text" class="form-control img-url" /></td>' +
            '<td><input type="text" class="form-control img-title" /></td>' +
            '<td><input type="number" class="form-control img-sort" value="0" /></td>' +
            '<td><input type="checkbox" class="img-main" /></td>' +
            '<td><button type="button" class="btn btn-sm btn-danger btn-remove-row">×</button></td>' +
        '</tr>';
        $('#tblImages tbody').append(row);
    });

    // ============ قدم ۴: Summary ============
    function collectStep1() {
        var variants = [];
        $('#tblVariants tbody tr').each(function () {
            variants.push({
                SKU: $(this).find('.v-sku').val(),
                StockQuantity: parseInt($(this).find('.v-stock').val() || 0),
                Price: parseFloat($(this).find('.v-price').val()) || null,
                IsActive: $(this).find('.v-active').is(':checked')
            });
        });

        return {
            NameFa: $('#NameFa').val(),
            NameEn: $('#NameEn').val(),
            Description: $('#Description').val(),
            ImageUrl: $('#ImageUrl').val(),
            ResourceIsActive: true,
            ProductCode: $('#ProductCode').val(),
            Barcode: $('#Barcode').val(),
            ProductIsActive: true,
            Variants: variants
        };
    }

    function collectStep2() {
        var categoryIds = [];
        $('#categoriesList input:checked').each(function () {
            categoryIds.push(parseInt($(this).val()));
        });

        var variantAttributes = [];
        $('.var-attr-body').each(function () {
            var variantIdx = parseInt($(this).data('variant-idx'));
            var attrs = [];
            $(this).find('tr').each(function () {
                var attrId = $(this).find('.attr-select').val();
                var valueId = $(this).find('.attr-value-select').val();
                if (attrId && valueId) {
                    attrs.push({
                        FK_AttributeId: parseInt(attrId),
                        FK_AttributeValueId: parseInt(valueId)
                    });
                }
            });
            if (attrs.length > 0) {
                variantAttributes.push({
                    VariantIndex: variantIdx,
                    Attributes: attrs
                });
            }
        });

        return {
            SelectedCategoryIds: categoryIds,
            VariantAttributes: variantAttributes
        };
    }

    function collectStep3() {
        var resourceAttrs = [];
        $('#tblResourceAttrs tbody tr').each(function () {
            var attrId = $(this).find('.ra-attr').val();
            var valueId = $(this).find('.ra-value').val();
            if (attrId) {
                resourceAttrs.push({
                    FK_AttributeId: parseInt(attrId),
                    FK_AttributeValueId: valueId ? parseInt(valueId) : null,
                    ValueText: null,
                    ValueNumber: null,
                    ValueDate: null,
                    ValueBoolean: null
                });
            }
        });

        var images = [];
        $('#tblImages tbody tr').each(function () {
            var url = $(this).find('.img-url').val();
            if (url) {
                images.push({
                    ImageUrl: url,
                    Title: $(this).find('.img-title').val(),
                    SortOrder: parseInt($(this).find('.img-sort').val() || 0),
                    IsMain: $(this).find('.img-main').is(':checked'),
                    IsActive: true
                });
            }
        });

        return {
            ResourceAttributes: resourceAttrs,
            Images: images
        };
    }

    function buildSummary() {
        var s1 = collectStep1();
        var s2 = collectStep2();
        var s3 = collectStep3();

        var html = '<div class="panel panel-default">' +
            '<div class="panel-heading"><b>اطلاعات پایه</b></div>' +
            '<div class="panel-body">' +
                '<p><b>نام فارسی:</b> ' + s1.NameFa + '</p>' +
                '<p><b>نام انگلیسی:</b> ' + (s1.NameEn || '-') + '</p>' +
                '<p><b>کد محصول:</b> ' + (s1.ProductCode || '-') + '</p>' +
                '<p><b>بارکد:</b> ' + (s1.Barcode || '-') + '</p>' +
                '<p><b>تعداد واریانت:</b> ' + s1.Variants.length + '</p>' +
            '</div>' +
        '</div>';

        html += '<div class="panel panel-default">' +
            '<div class="panel-heading"><b>دسته‌بندی‌ها</b></div>' +
            '<div class="panel-body">' +
                '<p>تعداد: ' + s2.SelectedCategoryIds.length + '</p>' +
            '</div>' +
        '</div>';

        html += '<div class="panel panel-default">' +
            '<div class="panel-heading"><b>صفت‌های واریانت</b></div>' +
            '<div class="panel-body">' +
                '<p>تعداد: ' + s2.VariantAttributes.length + ' واریانت دارای صفت</p>' +
            '</div>' +
        '</div>';

        html += '<div class="panel panel-default">' +
            '<div class="panel-heading"><b>ویژگی‌ها و تصاویر</b></div>' +
            '<div class="panel-body">' +
                '<p>ویژگی‌ها: ' + s3.ResourceAttributes.length + '</p>' +
                '<p>تصاویر: ' + s3.Images.length + '</p>' +
            '</div>' +
        '</div>';

        $('#summaryContainer').html(html);
    }

    // ============ Save ============
    $('#btnSave').click(function () {
        var model = {
            Step1: collectStep1(),
            Step2: collectStep2(),
            Step3: collectStep3()
        };

        $('#btnSave').prop('disabled', true).text('در حال ذخیره...');

        $.ajax({
            url: ApiClient.baseUrl + 'ProductWizard/Save',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(model),
            success: function (res) {
                if (res.success) {
                    alert(res.message);
                    window.location.href = ApiClient.baseUrl + 'Products/Index';
                } else {
                    alert('خطا: ' + res.message);
                    $('#btnSave').prop('disabled', false).text('ثبت نهایی');
                }
            },
            error: function (xhr) {
                alert('خطا در ارتباط با سرور');
                $('#btnSave').prop('disabled', false).text('ثبت نهایی');
            }
        });
    });

    // شروع
    goToStep(1);
});