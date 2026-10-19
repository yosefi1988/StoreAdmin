$(function () {
    var currentStep = 1;
    var totalSteps = 4;
    var allAttributes = [];
    var variantCounter = 0;
    var imageCounter = 0;

 
     $(document).on('select2:opening', function () {
        var scrollY = window.scrollY || window.pageYOffset;
        $('html, body').css('overflow', 'hidden');
        setTimeout(function () {
            $('html, body').css('overflow', '');
            window.scrollTo(0, scrollY);
        }, 150);
    });


    function refreshVariants() {
        if (allAttributes.length > 0) renderVariantAttributes();
    }

function goToStep(step) {
    if (step < 1 || step > totalSteps) return;

    $('.wizard-panel').hide();
    $('#step-' + step).show();

    // کمی تاخیر برای اینکه مرورگر لایه‌بندی DOM را آپدیت کند
    setTimeout(function () {
        if (step === 2) {
            if ($('#categoriesDropdown option').length === 0) loadCategories();
            if (allAttributes.length === 0) loadAttributes();
            else refreshVariants();
            if (!$('#CountryId').data('initialized')) {
                initLocationSelectors();
                $('#CountryId').data('initialized', true);
            }
        }
        if (step === 3 && allAttributes.length === 0) loadAttributes();
        if (step === 4) buildSummary();
    }, 10);

    $('.wizard-step').each(function () {
        var s = parseInt($(this).data('step'));
        $(this).removeClass('active done');
        if (s < step) $(this).addClass('done');
        if (s === step) $(this).addClass('active');
    });

    $('#btnPrev').prop('disabled', step === 1);
    if (step === totalSteps) { $('#btnNext').hide(); $('#btnSave').show(); }
    else { $('#btnNext').show(); $('#btnSave').hide(); }

    currentStep = step;
}

    $('#btnNext').click(function () {
        if (!validateStep(currentStep)) return;
        goToStep(currentStep + 1);
    });
    $('#btnPrev').click(function () { goToStep(currentStep - 1); });

    function validateStep(step) {
        if (step === 1 && !$('#NameFa').val().trim()) {
            alert('نام فارسی الزامی است');
            return false;
        }
        if (step === 2) {
            var selected = $('#categoriesDropdown').val();
            if (!selected || selected === '') {
                if (!confirm('هیچ دسته‌بندی انتخاب نشده. ادامه می‌دهید؟')) return false;
            }
        }
        return true;
    }

    function uploadFile(file, onSuccess, onError) {
        if (!file) return;
        var allowed = /\.(jpg|jpeg|png|gif|webp)$/i;
        if (!allowed.test(file.name)) { alert('فرمت فایل مجاز نیست'); if (onError) onError(); return; }
        if (file.size > 5 * 1024 * 1024) { alert('حجم بیش از ۵ مگابایت'); if (onError) onError(); return; }

        var fd = new FormData();
        fd.append('file', file);

        $.ajax({
            url: ApiClient.baseUrl + 'ProductWizard/UploadImage',
            type: 'POST',
            data: fd,
            processData: false,
            contentType: false,
            success: function (res) {
                if (res && res.success) onSuccess(res.url);
                else { alert('خطا: ' + (res && res.message ? res.message : 'ناموفق')); if (onError) onError(); }
            },
            error: function () { alert('خطا در آپلود'); if (onError) onError(); }
        });
    }

    $('#mainImageFile').change(function () {
        var file = this.files[0];
        if (!file) return;
        var $area = $('#mainImageCard .image-upload-area');
        $area.addClass('uploading');

        uploadFile(file,
            function (url) {
                $('#ImageUrl').val(url);
                $('#mainImagePreview').html('<img src="' + url + '" alt="preview" />').show();
                $('#mainImageLabel').hide();
                $area.removeClass('uploading');
            },
            function () {
                $area.removeClass('uploading');
                $('#mainImageFile').val('');
            }
        );
    });

    $('#btnAddVariant').click(function () { addVariantRow(); });

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
        refreshVariants();
    }

    $(document).on('click', '.btn-remove-variant', function () {
        $(this).closest('tr').remove();
        refreshVariants();
    });

    function loadCategories() {
        ApiClient.get('ProductWizard/GetCategories', null, function (data) {
            var $select = $('#categoriesDropdown').empty();

            if (!data || data.length === 0) {
                $select.append('<option value="">دسته‌بندی‌ای یافت نشد</option>');
                return;
            }

            $.each(data, function (i, c) {
                if (c.Children && c.Children.length > 0) {
                    var $group = $('<optgroup>').attr('label', '📁 ' + c.NameFa + ' (' + (c.CategoryType || '') + ')');
                    $group.append($('<option>').attr('value', c.CategoryId).attr('data-parent', c.FK_ParentId || '').attr('data-is-parent', 'true').text('📁 ' + c.NameFa + ' (والد)'));

                    $.each(c.Children, function (j, child) {
                        $group.append($('<option>').attr('value', child.CategoryId).attr('data-parent', child.FK_ParentId || '').attr('data-is-parent', (child.Children && child.Children.length > 0) ? 'true' : 'false').text('└ ' + child.NameFa + ' (' + (child.CategoryType || '') + ')'));
                        if (child.Children && child.Children.length > 0) {
                            $.each(child.Children, function (k, sub) {
                                $group.append($('<option>').attr('value', sub.CategoryId).attr('data-parent', sub.FK_ParentId || '').attr('data-is-parent', 'false').text('└── ' + sub.NameFa + ' (' + (sub.CategoryType || '') + ')'));
                            });
                        }
                    });
                    $select.append($group);
                } else {
                    $select.append($('<option>').attr('value', c.CategoryId).attr('data-parent', c.FK_ParentId || '').attr('data-is-parent', 'false').text(c.NameFa + ' (' + (c.CategoryType || '') + ')'));
                }
            });

            $select.select2({
                placeholder: 'جستجوی دسته‌بندی...', allowClear: true, dir: 'rtl', width: '100%',
                dropdownParent: $('#step-2'),
                language: { noResults: function () { return 'نتیجه‌ای یافت نشد'; }, searching: function () { return 'در حال جستجو...'; } },
                templateResult: function (opt) {
                    if (!opt.id) return opt.text;
                    if ($(opt.element).attr('data-is-parent') === 'true') {
                        return $('<span style="font-weight:bold;color:#0056b3;">' + opt.text + '</span>');
                    }
                    return $('<span>' + opt.text + '</span>');
                }
            });
        });
    }

    // ====== کشور / استان / شهر ======
    function initLocationSelectors() {
        var $country  = $('#CountryId');
        var $province = $('#ProvinceId');
        var $city     = $('#CityId');
        var $parent   = $('#step-2');

        // تنظیمات مشترک
        var select2Options = {
            width: '100%', 
            dir: 'rtl', 
            allowClear: true, 
            dropdownParent: $parent
        };

        $country.select2($.extend({}, select2Options, { placeholder: 'کشور را انتخاب کنید' }));
        $province.select2($.extend({}, select2Options, { placeholder: 'ابتدا کشور را انتخاب کنید', disabled: true }));
        $city.select2($.extend({}, select2Options, { placeholder: 'ابتدا استان را انتخاب کنید', disabled: true }));

        ApiClient.get('ProductWizard/GetCountries', null, function (data) {
            $country.empty().append('<option value=""></option>'); // آپشن خالی برای پلیس‌هولدر
            $.each(data, function (i, item) { $country.append(new Option(item.text, item.id, false, false)); });
            $country.trigger('change.select2');
        });

        $country.on('change', function () {
            var countryId = $(this).val();
            $province.val(null).trigger('change.select2');
            $city.val(null).trigger('change.select2');

            if (!countryId) {
                $province.prop('disabled', true).empty().append('<option value=""></option>').trigger('change.select2');
                $city.prop('disabled', true).empty().append('<option value=""></option>').trigger('change.select2');
                return;
            }

            ApiClient.get('ProductWizard/GetProvinces', { countryId: countryId }, function (data) {
                $province.empty().append('<option value=""></option>'); // آپشن خالی برای پلیس‌هولدر
                $.each(data, function (i, item) { $province.append(new Option(item.text, item.id, false, false)); });
                $province.prop('disabled', false).trigger('change.select2');
            });
        });

        $province.on('change', function () {
            var provinceId = $(this).val();
            $city.val(null).trigger('change.select2');

            if (!provinceId) {
                $city.prop('disabled', true).empty().append('<option value=""></option>').trigger('change.select2');
                return;
            }

            ApiClient.get('ProductWizard/GetCities', { provinceId: provinceId }, function (data) {
                $city.empty().append('<option value=""></option>'); // آپشن خالی برای پلیس‌هولدر
                $.each(data, function (i, item) { $city.append(new Option(item.text, item.id, false, false)); });
                $city.prop('disabled', false).trigger('change.select2');
            });
        });
    }

    $(document).on('change', '#categoriesDropdown', function () { showParentChain($(this).val()); });

    function showParentChain(selectedId) {
        var $info = $('#parentCategoriesInfo').empty();
        if (!selectedId || selectedId === '') return;

        var chain = [];
        var $opt = $('#categoriesDropdown option[value="' + selectedId + '"]');
        var parentId = $opt.attr('data-parent');

        while (parentId && parentId !== '' && parentId != 1) {
            var $parentOpt = $('#categoriesDropdown option[value="' + parentId + '"]');
            if ($parentOpt.length === 0) break;
            chain.unshift($parentOpt.text().replace('📁 ', '').replace(' (والد)', ''));
            parentId = $parentOpt.attr('data-parent');
        }

        if (chain.length > 0) {
            var html = '<small class="text-muted">📁 والدها: </small>';
            $.each(chain, function (i, name) {
                if (i > 0) html += '<span class="parent-arrow">←</span>';
                html += '<span class="parent-chain">' + name + '</span>';
            });
            $info.html(html);
        }
    }

    function loadAttributes() {
        ApiClient.get('ProductWizard/GetAttributes', null, function (data) {
            allAttributes = data || [];
            renderVariantAttributes();
        });
    }

    function renderVariantAttributes() {
        var container = $('#variantAttributesContainer').empty();
        var variantRows = $('#tblVariants tbody tr');
        if (variantRows.length === 0) { container.html('<em class="text-muted">ابتدا در قدم ۱ واریانت اضافه کنید</em>'); return; }
        variantRows.each(function (i) {
            var sku = $(this).find('.v-sku').val() || 'بدون SKU';
            container.append('<div class="panel panel-default">' +
                '<div class="panel-heading"><b>واریانت ' + (i + 1) + ':</b> ' + sku + '</div>' +
                '<div class="panel-body">' +
                    '<button type="button" class="btn btn-sm btn-success btn-add-var-attr" data-variant-idx="' + i + '">+ افزودن صفت</button>' +
                    '<div class="table-responsive" style="margin-top:10px;">' +
                        '<table class="table table-condensed">' +
                            '<thead><tr><th>صفت</th><th>مقدار</th><th></th></tr></thead>' +
                            '<tbody class="var-attr-body" data-variant-idx="' + i + '"></tbody>' +
                        '</table>' +
                    '</div>' +
                '</div>' +
            '</div>');
        });
    }

    $(document).on('click', '.btn-add-var-attr', function () {
        var variantIdx = $(this).data('variant-idx');
        var tbody = $('.var-attr-body[data-variant-idx="' + variantIdx + '"]');
        var attrOptions = '<option value="">انتخاب صفت...</option>';
        $.each(allAttributes, function (i, a) { attrOptions += '<option value="' + a.AttributeId + '">' + a.NameFa + '</option>'; });
        tbody.append('<tr><td><select class="form-control attr-select">' + attrOptions + '</select></td><td><select class="form-control attr-value-select"><option value="">ابتدا صفت را انتخاب کنید</option></select></td><td><button type="button" class="btn btn-sm btn-danger btn-remove-row">×</button></td></tr>');
    });

    $(document).on('change', '.attr-select', function () {
        var attrId = $(this).val();
        var valueSelect = $(this).closest('tr').find('.attr-value-select');
        if (!attrId) { valueSelect.html('<option value="">ابتدا صفت را انتخاب کنید</option>'); return; }
        valueSelect.html('<option value="">در حال بارگذاری...</option>');
        ApiClient.get('ProductWizard/GetAttributeValues', { attributeId: attrId }, function (data) {
            var html = '<option value="">انتخاب مقدار...</option>';
            $.each(data, function (i, v) { html += '<option value="' + v.AttributeValueId + '">' + v.ValueFa + '</option>'; });
            valueSelect.html(html);
        });
    });

    $(document).on('click', '.btn-remove-row', function () { $(this).closest('tr').remove(); });

    $('#btnAddResourceAttr').click(function () {
        if (allAttributes.length === 0) { alert('ابتدا صفت‌ها را بارگذاری کنید'); return; }
        var attrOptions = '<option value="">انتخاب صفت...</option>';
        $.each(allAttributes, function (i, a) { attrOptions += '<option value="' + a.AttributeId + '">' + a.NameFa + '</option>'; });
        $('#tblResourceAttrs tbody').append('<tr><td><select class="form-control ra-attr">' + attrOptions + '</select></td><td><select class="form-control ra-value"><option value="">ابتدا صفت را انتخاب کنید</option></select></td><td><button type="button" class="btn btn-sm btn-danger btn-remove-row">×</button></td></tr>');
    });

    $(document).on('change', '.ra-attr', function () {
        var attrId = $(this).val();
        var valueSelect = $(this).closest('tr').find('.ra-value');
        if (!attrId) { valueSelect.html('<option value="">ابتدا صفت را انتخاب کنید</option>'); return; }
        valueSelect.html('<option value="">در حال بارگذاری...</option>');
        ApiClient.get('ProductWizard/GetAttributeValues', { attributeId: attrId }, function (data) {
            var html = '<option value="">انتخاب مقدار...</option>';
            $.each(data, function (i, v) { html += '<option value="' + v.AttributeValueId + '">' + v.ValueFa + '</option>'; });
            valueSelect.html(html);
        });
    });

    $('#btnAddImage').click(function () { addImageCard(); });

    function addImageCard(data) {
        data = data || {};
        var idx = imageCounter++;
        $('#noImagesMessage').hide();

        var card = '<div class="image-card" data-img-idx="' + idx + '">' +
            '<div class="image-card-header">' +
                '<span class="image-card-title"><i class="fa fa-image"></i> تصویر</span>' +
                '<button type="button" class="btn btn-sm btn-danger btn-remove-image" title="حذف">×</button>' +
            '</div>' +
            '<div class="image-card-body">' +
                '<div class="image-upload-area">' +
                    '<input type="file" class="img-file" accept="image/*" style="display:none;" id="img-file-' + idx + '" />' +
                    '<label for="img-file-' + idx + '" class="upload-label">' +
                        '<div class="upload-icon">📷</div>' +
                        '<div class="upload-text">کلیک کنید یا فایل را بکشید</div>' +
                        '<div class="upload-hint">JPG, PNG, GIF, WEBP — حداکثر ۵ مگابایت</div>' +
                    '</label>' +
                    '<div class="img-preview" style="display:none;"></div>' +
                    '<input type="text" class="form-control img-url" readonly placeholder="آدرس تصویر بعد از آپلود" style="margin-top:8px; font-size:12px;" />' +
                '</div>' +
                '<div class="image-meta">' +
                    '<div class="form-group" style="margin-bottom:10px;">' +
                        '<label style="font-size:13px;">عنوان</label>' +
                        '<input type="text" class="form-control img-title" value="' + (data.Title || '') + '" placeholder="مثلاً نمای جلو" />' +
                    '</div>' +
                    '<div class="row">' +
                        '<div class="col-6">' +
                            '<div class="form-group" style="margin-bottom:10px;">' +
                                '<label style="font-size:13px;">ترتیب</label>' +
                                '<input type="number" class="form-control img-sort" value="' + (data.SortOrder || 0) + '" min="0" />' +
                            '</div>' +
                        '</div>' +
                        '<div class="col-6">' +
                            '<div class="form-group" style="margin-bottom:10px;">' +
                                '<label style="font-size:13px;">تصویر اصلی</label>' +
                                '<div class="checkbox-wrapper">' +
                                    '<input type="checkbox" class="img-main" id="img-main-' + idx + '" ' + (data.IsMain ? 'checked' : '') + ' />' +
                                    '<label for="img-main-' + idx + '" class="main-checkbox-label">اصلی</label>' +
                                '</div>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</div>' +
        '</div>';

        $('#imagesContainer').prepend(card);
    }

    $(document).on('click', '.btn-remove-image', function () {
        $(this).closest('.image-card').remove();
        if ($('#imagesContainer .image-card').length === 0) $('#noImagesMessage').show();
    });

    $(document).on('change', '.img-file', function () {
        var file = this.files[0];
        if (!file) return;
        var $card = $(this).closest('.image-card');
        var $preview = $card.find('.img-preview');
        var $uploadArea = $card.find('.image-upload-area');

        $uploadArea.addClass('uploading');

        uploadFile(file,
            function (url) {
                $card.find('.img-url').val(url);
                $preview.html('<img src="' + url + '" alt="preview" />').show();
                $card.find('.upload-label').hide();
                $uploadArea.removeClass('uploading');
            },
            function () {
                $uploadArea.removeClass('uploading');
                $card.find('.img-file').val('');
            }
        );
    });

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
            NameFa: $('#NameFa').val(), NameEn: $('#NameEn').val(),
            Description: $('#Description').val(), ImageUrl: $('#ImageUrl').val(),
            ResourceIsActive: true, ProductCode: $('#ProductCode').val(),
            Barcode: $('#Barcode').val(), ProductIsActive: true, Variants: variants
        };
    }

    function collectStep2() {
        var categoryIds = [];
        var selected = $('#categoriesDropdown').val();
        if (selected && selected !== '') {
            categoryIds.push(parseInt(selected));
            var $opt = $('#categoriesDropdown option[value="' + selected + '"]');
            var parentId = $opt.attr('data-parent');
            while (parentId && parentId !== '' && parentId != 1) {
                categoryIds.push(parseInt(parentId));
                var $parentOpt = $('#categoriesDropdown option[value="' + parentId + '"]');
                if ($parentOpt.length === 0) break;
                parentId = $parentOpt.attr('data-parent');
            }
        }

        var variantAttributes = [];
        $('.var-attr-body').each(function () {
            var variantIdx = parseInt($(this).data('variant-idx'));
            var attrs = [];
            $(this).find('tr').each(function () {
                var attrId = $(this).find('.attr-select').val();
                var valueId = $(this).find('.attr-value-select').val();
                if (attrId && valueId) attrs.push({ FK_AttributeId: parseInt(attrId), FK_AttributeValueId: parseInt(valueId) });
            });
            if (attrs.length > 0) variantAttributes.push({ VariantIndex: variantIdx, Attributes: attrs });
        });

        return {
            SelectedCategoryIds: categoryIds,
            VariantAttributes: variantAttributes,
            CountryId:  $('#CountryId').val()  || null,
            ProvinceId: $('#ProvinceId').val() || null,
            CityId:     $('#CityId').val()     || null
        };
    }

    function collectStep3() {
        var resourceAttrs = [];
        $('#tblResourceAttrs tbody tr').each(function () {
            var attrId = $(this).find('.ra-attr').val();
            var valueId = $(this).find('.ra-value').val();
            if (attrId) resourceAttrs.push({
                FK_AttributeId: parseInt(attrId),
                FK_AttributeValueId: valueId ? parseInt(valueId) : null,
                ValueText: null, ValueNumber: null, ValueDate: null, ValueBoolean: null
            });
        });

        var images = [];
        $('#imagesContainer .image-card').each(function () {
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

        return { ResourceAttributes: resourceAttrs, Images: images };
    }

    function buildSummary() {
        var s1 = collectStep1(), s2 = collectStep2(), s3 = collectStep3();
        var html = '<div class="panel panel-default"><div class="panel-heading"><b>اطلاعات پایه</b></div><div class="panel-body">' +
            '<p><b>نام فارسی:</b> ' + s1.NameFa + '</p>' +
            '<p><b>نام انگلیسی:</b> ' + (s1.NameEn || '-') + '</p>' +
            '<p><b>کد محصول:</b> ' + (s1.ProductCode || '-') + '</p>' +
            '<p><b>بارکد:</b> ' + (s1.Barcode || '-') + '</p>' +
            '<p><b>تعداد واریانت:</b> ' + s1.Variants.length + '</p>' +
            '</div></div>' +
            '<div class="panel panel-default"><div class="panel-heading"><b>دسته‌بندی‌ها</b></div><div class="panel-body">' +
            '<p>تعداد: ' + s2.SelectedCategoryIds.length + ' (شامل والدها)</p>' +
            '<p><small>IDها: ' + s2.SelectedCategoryIds.join(', ') + '</small></p>' +
            '</div></div>' +
            '<div class="panel panel-default"><div class="panel-heading"><b>صفت‌های واریانت</b></div><div class="panel-body">' +
            '<p>تعداد: ' + s2.VariantAttributes.length + ' واریانت دارای صفت</p></div></div>' +
            '<div class="panel panel-default"><div class="panel-heading"><b>محل تولید / انبار</b></div><div class="panel-body">' +
            '<p><b>کشور:</b> ' + ($('#CountryId option:selected').text() || '-') + '</p>' +
            '<p><b>استان:</b> ' + ($('#ProvinceId option:selected').text() || '-') + '</p>' +
            '<p><b>شهر:</b> ' + ($('#CityId option:selected').text() || '-') + '</p>' +
            '</div></div>' +
            '<div class="panel panel-default"><div class="panel-heading"><b>ویژگی‌ها و تصاویر</b></div><div class="panel-body">' +
            '<p>ویژگی‌ها: ' + s3.ResourceAttributes.length + '</p>' +
            '<p>تصاویر: ' + s3.Images.length + '</p></div></div>';
        $('#summaryContainer').html(html);
    }

    $('#btnSave').click(function () {
        var model = { Step1: collectStep1(), Step2: collectStep2(), Step3: collectStep3() };
        $('#btnSave').prop('disabled', true).text('در حال ذخیره...');
        $.ajax({
            url: ApiClient.baseUrl + 'ProductWizard/Save',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(model),
            success: function (res) {
                if (res.success) { alert(res.message); window.location.href = ApiClient.baseUrl + 'Products/Index'; }
                else { alert('خطا: ' + res.message); $('#btnSave').prop('disabled', false).text('ثبت نهایی'); }
            },
            error: function () { alert('خطا در سرور'); $('#btnSave').prop('disabled', false).text('ثبت نهایی'); }
        });
    });

    goToStep(1);
});