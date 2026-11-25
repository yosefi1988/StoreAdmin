$(function () {
    'use strict';

    // ============================================================
    // تنظیمات
    // ============================================================
    var API = (window.FeedConfig && window.FeedConfig.baseUrl) || '/';
    var PAGE_SIZE = 10;
    var COMMENTS_PAGE_SIZE = 20;

    // State فید
    var feedState = {
        skip: 0,
        hasMore: true,
        isLoading: false,
        seenIds: {}
    };

    // State کامنت‌ها
    var commentsState = {
        postId: null,
        skip: 0,
        hasMore: true,
        isLoading: false
    };

    // State تصاویر پست (چند فایل)
    var postImageUrls = [];

    // State فروشگاه‌ها
    var storeState = {
        activeId: null,
        stores: [],
        isSwitching: false
    };

    // ============================================================
    // Helper: هدایت به لاگین با returnUrl
    // ============================================================
    function redirectToLogin(returnUrl) {
        if (!returnUrl) {
            returnUrl = window.location.pathname + window.location.search;
        }
        var loginBase = (window.FeedConfig && window.FeedConfig.loginUrl) || '/Login';
        var sep = loginBase.indexOf('?') >= 0 ? '&' : '?';
        window.location.href = loginBase + sep + 'returnUrl=' + encodeURIComponent(returnUrl);
    }

    function handleRequireLogin(res) {
        if (res && res.requireLogin) {
            if (res.loginUrl) window.location.href = res.loginUrl;
            else redirectToLogin(null);
            return true;
        }
        return false;
    }

    // ============================================================
    // Helpers
    // ============================================================
    function escapeHtml(str) {
        if (str == null) return '';
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    function parseDate(iso) {
        if (!iso) return null;
        var m = /\/Date\((-?\d+)/.exec(iso);
        if (m) return new Date(parseInt(m[1], 10));
        var d = new Date(iso);
        return isNaN(d.getTime()) ? null : d;
    }

    function relativeTime(iso) {
        var d = parseDate(iso);
        if (!d) return '';
        var diff = (Date.now() - d.getTime()) / 1000;
        if (diff < 0) diff = 0;
        if (diff < 60) return 'الان';
        if (diff < 3600) return Math.floor(diff / 60) + ' دقیقه پیش';
        if (diff < 86400) return Math.floor(diff / 3600) + ' ساعت پیش';
        if (diff < 2592000) return Math.floor(diff / 86400) + ' روز پیش';
        return d.toLocaleDateString('fa-IR');
    }

    function authorDisplayName(p) {
        var full = ((p.AuthorFirstName || '') + ' ' + (p.AuthorLastName || '')).trim();
        return full || p.AuthorName || 'کاربر';
    }

    function buildAvatar(profileImage, displayName, cssClass) {
        cssClass = cssClass || 'post-avatar';
        if (profileImage) {
            return '<img class="' + cssClass + '" src="' + escapeHtml(profileImage) +
                '" alt="" onerror="this.style.display=\'none\'">';
        }
        var initial = (displayName || '?').charAt(0).toUpperCase();
        return '<div class="' + cssClass + '">' + escapeHtml(initial) + '</div>';
    }

    // ============================================================
    // رندر کارت پست
    // ============================================================
    function buildPostCard(p, options) {
        options = options || {};
        var isPreview = options.isPreview === true;
        var clickable = options.clickable === true;

        var name = authorDisplayName(p);
        var avatar = buildAvatar(p.AuthorProfileImage, name);

        var urls = (p.ImageUrls && p.ImageUrls.length)
            ? p.ImageUrls
            : (p.ImageUrl ? [p.ImageUrl] : []);

        var image = '';
        if (urls.length === 1) {
            image = '<img class="post-image" src="' + escapeHtml(urls[0]) + '" alt="" ' +
                'loading="lazy" onerror="this.style.display=\'none\'">';
        } else if (urls.length > 1) {
            image = '<div class="post-image-grid">';
            urls.forEach(function (u) {
                image += '<img src="' + escapeHtml(u) + '" alt="" loading="lazy" ' +
                    'onerror="this.style.display=\'none\'">';
            });
            image += '</div>';
        }

        var title = p.Title
            ? '<div class="post-title">' + escapeHtml(p.Title) + '</div>'
            : '';

        var content = p.Content
            ? '<div class="post-content">' + escapeHtml(p.Content) + '</div>'
            : '';

        var likedClass = p.IsLikedByMe ? ' liked' : '';
        var heartIcon = p.IsLikedByMe ? 'fa-heart' : 'fa-heart-o';

        var actionsHtml = '';
        if (!isPreview) {
            actionsHtml =
                '<div class="post-actions">' +
                '<button type="button" class="post-action like-btn' + likedClass + '" ' +
                'data-id="' + p.ResourceId + '">' +
                '<i class="fa ' + heartIcon + '"></i>' +
                '<span class="like-count">' + (p.LikeCount || 0) + '</span>' +
                '</button>' +
                '<button type="button" class="post-action comment-btn" ' +
                'data-id="' + p.ResourceId + '">' +
                '<i class="fa fa-comment-o"></i>' +
                '<span class="comment-count">' + (p.CommentCount || 0) + '</span>' +
                '</button>' +
                '</div>';
        }

        var cardClass = 'post-card' + (clickable ? ' post-card-clickable' : '');

        return '' +
            '<div class="' + cardClass + '" data-post-id="' + p.ResourceId + '">' +
            '<div class="post-header">' +
            avatar +
            '<div class="post-author-info">' +
            '<div class="post-author">' + escapeHtml(name) + '</div>' +
            '<div class="post-username">@' + escapeHtml(p.AuthorName || 'user') + '</div>' +
            '</div>' +
            '<div class="post-time">' + relativeTime(p.CreatedAt) + '</div>' +
            '</div>' +
            title +
            content +
            image +
            actionsHtml +
            '</div>';
    }

    // ============================================================
    // رندر کامنت
    // ============================================================
    function buildCommentItem(c) {
        var name = authorDisplayName(c);
        var avatar = buildAvatar(c.AuthorProfileImage, name, 'comment-avatar');

        return '' +
            '<div class="comment-item" data-comment-id="' + c.CommentId + '">' +
            avatar +
            '<div class="comment-body">' +
            '<div>' +
            '<span class="comment-author">' + escapeHtml(name) + '</span>' +
            '<span class="comment-time">' + relativeTime(c.CreatedAt) + '</span>' +
            '</div>' +
            '<div class="comment-text">' + escapeHtml(c.Comment) + '</div>' +
            '</div>' +
            '</div>';
    }

    // ============================================================
    // Store Switcher
    // ============================================================
    function loadMyStores() {
        $('#storeLoading').show();
        $('#storeError').hide();

        $.ajax({
            url: API + 'Posts/GetMyStores',
            type: 'GET',
            dataType: 'json'
        })
            .done(function (res) {
                $('#storeLoading').hide();

                if (handleRequireLogin(res)) return;

                if (!res || !res.success) {
                    $('#storeError').text((res && res.message) || 'خطا در بارگذاری فروشگاه‌ها').show();
                    return;
                }

                storeState.activeId = res.ActiveStoreId;
                storeState.stores = res.Data || [];

                renderStoreSwitcher();
            })
            .fail(function (xhr) {
                $('#storeLoading').hide();
                $('#storeError').text('خطا در ارتباط با سرور (' + xhr.status + ')').show();
            });
    }

    function renderStoreSwitcher() {
        var $current = $('#storeCurrent').empty();
        var $select = $('#storeSelect').empty();

        if (!storeState.stores.length) {
            $current.html('<div class="text-muted small">هیچ فروشگاهی یافت نشد.</div>');
            $select.hide();
            return;
        }

        var active = storeState.stores.filter(function (s) {
            return s.StoreSettingId === storeState.activeId;
        })[0] || storeState.stores[0];

        var logo = active.LogoUrl
            ? '<img class="store-logo" src="' + escapeHtml(active.LogoUrl) + '" alt="" ' +
            'onerror="this.style.display=\'none\'">'
            : '';

        var star = active.IsPrimary
            ? '<i class="fa fa-star store-primary-star" title="فروشگاه اصلی"></i>'
            : '';

        // ۱. نمایش فروشگاه فعال در باکس سبز
        $current.html(
            '<div class="current-store-row">' +
            '<i class="fa fa-check-circle"></i>' +
            logo +
            '<strong class="flex-grow-1 text-truncate">' +
            escapeHtml(active.StoreNameFa || active.StoreNameEn || '-') +
            '</strong>' +
            star +
            '</div>'
        );

        // ۲. پر کردن منوی کشویی
        storeState.stores.forEach(function (s) {
            var label = (s.StoreNameFa || s.StoreNameEn || ('#' + s.StoreSettingId));
            if (s.IsPrimary) label = '★ ' + label;

            $select.append(
                '<option value="' + s.StoreSettingId + '"' +
                (s.StoreSettingId === storeState.activeId ? ' selected' : '') + '>' +
                escapeHtml(label) +
                '</option>'
            );
        });

        // ۳. نمایش منو فقط در صورتی که بیش از یک فروشگاه وجود داشته باشد
        if (storeState.stores.length > 1) {
            // 🔴 یک فاصله (margin) به منو می‌دهیم تا زیر باکس سبز قرار بگیرد
            $select.css('margin-top', '10px').show();
        } else {
            // اگر فقط یک فروشگاه بود، منو را کاملاً مخفی کن
            $select.hide();
        }
    }

    // ============================================================
    // تغییر فروشگاه به صورت خودکار با انتخاب از منوی کشویی
    // ============================================================
    // ============================================================
    // تغییر فروشگاه به صورت خودکار با انتخاب از منوی کشویی
    // ============================================================
    $(document).on('change', '#storeSelect', function (e) {
        e.preventDefault();

        if (storeState.isSwitching) return;

        var newId = parseInt($(this).val(), 10);
        if (!newId || newId === storeState.activeId) return;

        var $select = $(this);
        storeState.isSwitching = true;
        $select.prop('disabled', true);
        $('#storeError').hide();

        // 🔴 ذخیره موقعیت اسکرول فعلی
        var scrollPosition = $(window).scrollTop();

        // دریافت توکن امنیتی (AntiForgeryToken) از صفحه
        var token = $('input[name="__RequestVerificationToken"]').first().val();

        $.ajax({
            url: API + 'Posts/SwitchStore',
            type: 'POST',
            data: {
                storeSettingId: newId,
                __RequestVerificationToken: token
            },
            dataType: 'json'
        })
            .done(function (res) {
                storeState.isSwitching = false;
                $select.prop('disabled', false);

                if (handleRequireLogin(res)) return;

                if (!res || !res.success) {
                    $('#storeError').text((res && res.message) || 'خطا').show();
                    $select.val(storeState.activeId);
                    return;
                }

                storeState.activeId = newId;
                renderStoreSwitcher();

                // ریست فید و بارگذاری مجدد بدون ریلود صفحه
                feedState = { skip: 0, hasMore: true, isLoading: false, seenIds: {} };
                $('#feedContainer').empty();
                $('#feedEnd').hide();
                $('#feedError').hide();

                loadFeed();

                // 🔴 بازگرداندن موقعیت اسکرول پس از بارگذاری
                // از setTimeout استفاده می‌کنیم تا فید جدید فرصت رندر پیدا کند
                setTimeout(function () {
                    // اگر می‌خواهید دقیقاً به همان موقعیت قبلی برگردد:
                    $(window).scrollTop(scrollPosition);

                    // اگر می‌خواهید به بالای کارت ارسال پست (بالای فید) برود:
                    // $('html, body').animate({ scrollTop: $('.post-composer').offset().top - 20 }, 300);
                }, 100);
            })
            .fail(function (xhr) {
                storeState.isSwitching = false;
                $select.prop('disabled', false);

                var errorMsg = 'خطا در ارتباط با سرور';
                if (xhr.status === 500) {
                    errorMsg = 'خطای سرور (احتمالاً توکن امنیتی نامعتبر است)';
                } else if (xhr.status === 400) {
                    errorMsg = 'درخواست نامعتبر است';
                } else if (xhr.status === 403) {
                    errorMsg = 'دسترسی غیرمجاز';
                }
                $('#storeError').text(errorMsg + ' (' + xhr.status + ')').show();

                $select.val(storeState.activeId);

                // 🔴 بازگرداندن اسکرول در صورت خطا
                $(window).scrollTop(scrollPosition);
            });
    });

    // ============================================================
    // بارگذاری فید
    // ============================================================
    function loadFeed() {
        if (feedState.isLoading || !feedState.hasMore) return;

        feedState.isLoading = true;
        $('#feedLoading').show();
        $('#feedError').hide();

        $.ajax({
            url: API + 'Posts/GetFeed',
            type: 'GET',
            data: {
                skip: feedState.skip,
                take: PAGE_SIZE
            },
            dataType: 'json'
        })
            .done(function (res) {
                feedState.isLoading = false;
                $('#feedLoading').hide();

                if (!res || !res.success) {
                    $('#feedError').text((res && res.message) || 'خطا در بارگذاری').show();
                    return;
                }

                feedState.hasMore = res.HasMore;
                feedState.skip = res.Skip;

                renderFeed(res.Data);

                if (!feedState.hasMore && feedState.skip > 0) {
                    $('#feedEnd').show();
                }
            })
            .fail(function (xhr) {
                feedState.isLoading = false;
                $('#feedLoading').hide();
                $('#feedError').text('خطا در ارتباط با سرور (' + xhr.status + ')').show();
            });
    }

    function renderFeed(posts) {
        var container = $('#feedContainer');

        if (!posts || posts.length === 0) {
            if (feedState.skip === 0) {
                container.html(
                    '<div class="text-center text-muted py-5">' +
                    '<i class="fa fa-inbox fa-3x mb-3" style="color:#ccc;"></i>' +
                    '<p>هنوز پستی ثبت نشده. اولین نفر باشید!</p>' +
                    '</div>'
                );
            }
            return;
        }

        var html = '';
        posts.forEach(function (p) {
            if (feedState.seenIds[p.ResourceId]) return;
            feedState.seenIds[p.ResourceId] = true;
            html += buildPostCard(p, { clickable: true });
        });
        container.append(html);
    }

    // ============================================================
    // کلیک روی کارت پست → صفحه جزئیات
    // ============================================================
    $(document).on('click', '.post-card-clickable', function (e) {
        if ($(e.target).closest('button, a').length) return;
        var id = $(this).data('post-id');
        if (!id) return;
        var base = API.replace(/\/$/, '');
        window.location.href = base + '/Posts/Detail/' + id;
    });

    // ============================================================
    // آپلود چند تصویر برای پست
    // ============================================================
    $('#postImagesInput').on('change', function () {
        var files = this.files;
        var $preview = $('#selectedFilesPreview').empty();

        if (!files || files.length === 0) return;

        Array.prototype.forEach.call(files, function (file) {
            if (!file.type || file.type.indexOf('image/') !== 0) return;

            var reader = new FileReader();
            reader.onload = function (e) {
                $preview.append(
                    '<div style="width:70px;height:70px;">' +
                    '<img src="' + e.target.result + '" ' +
                    'style="width:100%;height:100%;object-fit:cover;' +
                    'border:1px solid #ddd;border-radius:6px;" />' +
                    '</div>'
                );
            };
            reader.readAsDataURL(file);
        });
    });

    $('#btnUploadPostImages').on('click', function () {
        var input = document.getElementById('postImagesInput');
        var files = input.files;

        if (!files || files.length === 0) {
            showComposerAlert('لطفاً حداقل یک تصویر انتخاب کنید.');
            return;
        }

        var $btn = $(this);
        $btn.prop('disabled', true)
            .html('<i class="fa fa-spinner fa-spin"></i> در حال آپلود...');

        var fd = new FormData();
        Array.prototype.forEach.call(files, function (file) {
            fd.append('files', file);
        });

        $.ajax({
            url: API + 'Posts/UploadImage',
            type: 'POST',
            data: fd,
            processData: false,
            contentType: false,
            dataType: 'json'
        })
            .done(function (res) {
                if (handleRequireLogin(res)) return;

                if (!res || !res.success || !res.files || res.files.length === 0) {
                    showComposerAlert((res && res.message) || 'آپلود ناموفق بود');
                    if (res && res.errors && res.errors.length)
                        console.warn('خطاها:', res.errors);
                    return;
                }

                res.files.forEach(function (f) {
                    addUploadedPostImage(f.url, f.fileName || '');
                });

                if (res.errors && res.errors.length)
                    showComposerAlert(res.message + ' | خطاها: ' + res.errors.join('، '));
                else
                    hideComposerAlert();

                input.value = '';
                $('#selectedFilesPreview').empty();
            })
            .fail(function (xhr) {
                showComposerAlert('خطا در ارتباط با سرور (' + xhr.status + ')');
            })
            .always(function () {
                $btn.prop('disabled', false)
                    .html('<i class="fa fa-upload"></i> آپلود تصاویر');
            });
    });

    function addUploadedPostImage(url, fileName) {
        if (!url) return;
        if (postImageUrls.indexOf(url) >= 0) return;
        postImageUrls.push(url);

        $('#uploadedImagesList').append(
            '<div class="d-flex align-items-center mb-1 p-1 border rounded" ' +
            'data-url="' + escapeHtml(url) + '" style="background:#fafafa;">' +
            '<img src="' + escapeHtml(url) + '" ' +
            'style="width:50px;height:50px;object-fit:cover;' +
            'border:1px solid #ddd;border-radius:4px;margin-left:6px;" />' +
            '<div class="flex-grow-1 small text-truncate" title="' + escapeHtml(url) + '">' +
            escapeHtml(fileName || url) +
            '</div>' +
            '<button type="button" class="btn btn-danger btn-sm btn-remove-post-image">' +
            '<i class="fa fa-times"></i>' +
            '</button>' +
            '</div>'
        );

        syncPostImageUrls();
    }

    $(document).on('click', '.btn-remove-post-image', function () {
        var $row = $(this).closest('[data-url]');
        var url = $row.data('url');

        var idx = postImageUrls.indexOf(url);
        if (idx >= 0) postImageUrls.splice(idx, 1);

        $row.remove();
        syncPostImageUrls();
    });

    function syncPostImageUrls() {
        $('#postImageUrls').val(postImageUrls.join(','));
    }

    // ============================================================
    // ارسال پست جدید
    // ============================================================
    function submitPost() {
        var title = $('#postTitle').val().trim();
        var content = $('#postContent').val().trim();

        if (!content) {
            showComposerAlert('متن پست خالی است.');
            return;
        }

        var $btn = $('#btnSubmitPost');
        $btn.prop('disabled', true)
            .html('<i class="fa fa-spinner fa-spin"></i> ارسال...');

        $.ajax({
            url: API + 'Posts/Create',
            type: 'POST',
            data: {
                title: title || content.substring(0, 100),
                content: content,
                imageUrls: postImageUrls.join(',')
            },
            dataType: 'json'
        })
            .done(function (res) {
                $btn.prop('disabled', false).html('<i class="fa fa-send"></i> ارسال');

                if (handleRequireLogin(res)) return;

                if (!res.success) {
                    showComposerAlert(res.message || 'خطا');
                    return;
                }

                $('#postTitle, #postContent').val('');
                $('#charCount').text('0 / 2000');
                $('#postImagesInput').val('');
                $('#selectedFilesPreview').empty();
                $('#uploadedImagesList').empty();
                postImageUrls = [];
                $('#postImageUrls').val('');
                hideComposerAlert();

                var cardHtml = buildPostCard(res.post, { clickable: true });
                if (feedState.seenIds[res.post.ResourceId]) {
                    $('html, body').animate({ scrollTop: 0 }, 200);
                    return;
                }
                feedState.seenIds[res.post.ResourceId] = true;
                $('#feedContainer').prepend(cardHtml);
                $('html, body').animate({ scrollTop: 0 }, 200);
            })
            .fail(function (xhr) {
                $btn.prop('disabled', false).html('<i class="fa fa-send"></i> ارسال');
                showComposerAlert('خطا در ارتباط با سرور (' + xhr.status + ')');
            });
    }

    function showComposerAlert(msg) {
        $('#composerAlert').text(msg).show();
    }
    function hideComposerAlert() {
        $('#composerAlert').hide();
    }

    // ============================================================
    // لایک
    // ============================================================
    function toggleLike($btn) {
        var postId = $btn.data('id');

        $.ajax({
            url: API + 'Posts/ToggleLike',
            type: 'POST',
            data: { resourceId: postId },
            dataType: 'json'
        })
            .done(function (res) {
                if (handleRequireLogin(res)) return;

                if (!res.success) {
                    alert(res.message || 'خطا');
                    return;
                }

                $btn.toggleClass('liked', res.isLiked);
                $btn.find('i')
                    .toggleClass('fa-heart', res.isLiked)
                    .toggleClass('fa-heart-o', !res.isLiked);
                $btn.find('.like-count').text(res.likeCount);
            })
            .fail(function () {
                alert('خطا در ارتباط با سرور');
            });
    }

    // ============================================================
    // Modal کامنت‌ها
    // ============================================================
    function openCommentsModal(postId) {
        commentsState.postId = postId;
        commentsState.skip = 0;
        commentsState.hasMore = true;
        commentsState.isLoading = false;

        $('#commentsList').empty();
        $('#commentsEmpty').hide();
        $('#commentsEnd').hide();
        $('#newCommentText').val('');

        $.ajax({
            url: API + 'Posts/GetPost',
            type: 'GET',
            data: { resourceId: postId },
            dataType: 'json'
        })
            .done(function (res) {
                if (res.success && res.post) {
                    $('#modalPostPreview').html(buildPostCard(res.post, { isPreview: true }));
                }
            });

        loadComments();
        $('#commentsModal').modal('show');
    }

    function loadComments() {
        if (commentsState.isLoading || !commentsState.hasMore) return;

        commentsState.isLoading = true;
        $('#commentsLoading').show();

        $.ajax({
            url: API + 'Posts/GetComments',
            type: 'GET',
            data: {
                resourceId: commentsState.postId,
                skip: commentsState.skip,
                take: COMMENTS_PAGE_SIZE
            },
            dataType: 'json'
        })
            .done(function (res) {
                commentsState.isLoading = false;
                $('#commentsLoading').hide();

                if (!res.success) return;

                commentsState.hasMore = res.HasMore;
                commentsState.skip += COMMENTS_PAGE_SIZE;

                if (res.Data.length === 0 && commentsState.skip === COMMENTS_PAGE_SIZE) {
                    $('#commentsEmpty').show();
                } else {
                    $('#commentsEmpty').hide();
                }

                var html = '';
                res.Data.forEach(function (c) {
                    html += buildCommentItem(c);
                });
                $('#commentsList').append(html);

                if (!commentsState.hasMore && commentsState.skip > COMMENTS_PAGE_SIZE) {
                    $('#commentsEnd').show();
                }
            })
            .fail(function () {
                commentsState.isLoading = false;
                $('#commentsLoading').hide();
            });
    }

    function addComment() {
        var text = $('#newCommentText').val().trim();
        if (!text) {
            alert('متن نظر خالی است.');
            return;
        }

        var $btn = $('#btnAddComment');
        $btn.prop('disabled', true)
            .html('<i class="fa fa-spinner fa-spin"></i> ارسال...');

        $.ajax({
            url: API + 'Posts/AddComment',
            type: 'POST',
            data: {
                resourceId: commentsState.postId,
                comment: text
            },
            dataType: 'json'
        })
            .done(function (res) {
                $btn.prop('disabled', false).html('<i class="fa fa-send"></i> ارسال نظر');

                if (handleRequireLogin(res)) return;

                if (!res.success) {
                    alert(res.message || 'خطا');
                    return;
                }

                $('#newCommentText').val('');
                $('#commentsEmpty').hide();
                $('#commentsList').prepend(buildCommentItem(res.comment));

                $('.post-card[data-post-id="' + commentsState.postId + '"] .comment-count')
                    .text(res.commentCount);
            })
            .fail(function () {
                $btn.prop('disabled', false).html('<i class="fa fa-send"></i> ارسال نظر');
                alert('خطا در ارتباط با سرور');
            });
    }

    // ============================================================
    // اسکرول بی‌نهایت
    // ============================================================
    function isNearBottom() {
        return ($(window).scrollTop() + $(window).height())
            >= ($(document).height() - 400);
    }

    $(window).on('scroll', function () {
        if ($('#commentsModal').hasClass('show')) return;
        if (isNearBottom()) loadFeed();
    });

    $('#commentsModal .modal-body').on('scroll', function () {
        var el = this;
        if (el.scrollTop + el.clientHeight >= el.scrollHeight - 100) {
            loadComments();
        }
    });

    // ============================================================
    // Event Binding
    // ============================================================
    $('#btnSubmitPost').on('click', submitPost);

    $('#postContent').on('input', function () {
        $('#charCount').text(this.value.length + ' / 2000');
    });

    $(document).on('click', '.like-btn', function () {
        toggleLike($(this));
    });

    $(document).on('click', '.comment-btn', function (e) {
        e.stopPropagation();
        var id = $(this).data('id');
        if (!id) return;
        var base = API.replace(/\/$/, '');
        window.location.href = base + '/Posts/Detail/' + id;
    });

    $('#btnAddComment').on('click', addComment);

    $('#newCommentText').on('keydown', function (e) {
        if (e.key === 'Enter' && (e.ctrlKey || e.metaKey)) {
            e.preventDefault();
            addComment();
        }
    });

    // جلوگیری از هرگونه ارسال فرم در صفحه فید
    $('form').on('submit', function (e) {
        if ($(e.target).find('#storeSelect').length > 0) {
            e.preventDefault();
            return false;
        }
    });

    // ============================================================
    // شروع
    // ============================================================
    loadMyStores();
    loadFeed();
});