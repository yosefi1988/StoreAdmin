$(function () {
    'use strict';

    var API = (window.PostDetailConfig && window.PostDetailConfig.baseUrl) || '/';
    var RESOURCE_ID = (window.PostDetailConfig && window.PostDetailConfig.resourceId) || 0;
    var COMMENTS_PAGE_SIZE = 20;

    var commentsState = {
        skip: 0,
        hasMore: true,
        isLoading: false
    };

    var commentImageUrls = [];

    // ============================================================
    // Helpers
    // ============================================================
    function escapeHtml(str) {
        if (str == null) return '';
        return String(str)
            .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;').replace(/'/g, '&#39;');
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

    function redirectToLogin() {
        var returnUrl = window.location.pathname + window.location.search;
        var loginBase = (window.PostDetailConfig && window.PostDetailConfig.loginUrl) || '/Login';
        var sep = loginBase.indexOf('?') >= 0 ? '&' : '?';
        window.location.href = loginBase + sep + 'returnUrl=' + encodeURIComponent(returnUrl);
    }

    function handleRequireLogin(res) {
        if (res && res.requireLogin) {
            if (res.loginUrl) window.location.href = res.loginUrl;
            else redirectToLogin();
            return true;
        }
        return false;
    }

    // ============================================================
    // رندر پست کامل
    // ============================================================
    function buildPostFull(p) {
        var name = authorDisplayName(p);
        var avatar = buildAvatar(p.AuthorProfileImage, name);

        var image = '';
        var urls = p.ImageUrls || [];
        if (urls.length === 1) {
            image = '<img class="post-image-single" src="' + escapeHtml(urls[0]) + '" alt="" ' +
                    'onerror="this.style.display=\'none\'">';
        } else if (urls.length > 1) {
            image = '<div class="post-image-grid">';
            urls.forEach(function (u) {
                image += '<img src="' + escapeHtml(u) + '" alt="" ' +
                         'data-full="' + escapeHtml(u) + '" ' +
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

        return '' +
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
            '<div class="post-actions">' +
                '<button type="button" class="post-action like-btn' + likedClass + '" ' +
                        'data-id="' + p.ResourceId + '">' +
                    '<i class="fa ' + heartIcon + '"></i>' +
                    '<span class="like-count">' + (p.LikeCount || 0) + '</span>' +
                '</button>' +
                '<button type="button" class="post-action" disabled>' +
                    '<i class="fa fa-comment-o"></i>' +
                    '<span class="comment-count">' + (p.CommentCount || 0) + '</span>' +
                '</button>' +
            '</div>';
    }

    // ============================================================
    // بارگذاری پست
    // ============================================================
    function loadPost() {
        $.ajax({
            url: API + 'Posts/GetPostFull',
            type: 'GET',
            data: { resourceId: RESOURCE_ID },
            dataType: 'json'
        })
        .done(function (res) {
            $('#postDetailLoading').hide();

            if (!res || !res.success || !res.post) {
                $('#postDetailError').text((res && res.message) || 'پست یافت نشد').show();
                return;
            }

            $('#postDetailCard').html(buildPostFull(res.post)).show();
            $('#commentsTotalCount').text(res.post.CommentCount || 0);

            // لود کامنت‌ها
            loadComments();
        })
        .fail(function (xhr) {
            $('#postDetailLoading').hide();
            $('#postDetailError').text('خطا در بارگذاری (' + xhr.status + ')').show();
        });
    }

    // ============================================================
    // رندر کامنت
    // ============================================================
    function buildCommentItem(c) {
        var name = authorDisplayName(c);
        var avatar = buildAvatar(c.AuthorProfileImage, name, 'comment-avatar');

        var images = '';
        if (c.ImageUrls && c.ImageUrls.length) {
            images = '<div class="comment-images">';
            c.ImageUrls.forEach(function (u) {
                images += '<img src="' + escapeHtml(u) + '" alt="" ' +
                          'data-full="' + escapeHtml(u) + '" ' +
                          'onerror="this.style.display=\'none\'">';
            });
            images += '</div>';
        }

        return '' +
            '<div class="comment-item" data-comment-id="' + c.CommentId + '">' +
                avatar +
                '<div class="comment-body">' +
                    '<div>' +
                        '<span class="comment-author">' + escapeHtml(name) + '</span>' +
                        '<span class="comment-time">' + relativeTime(c.CreatedAt) + '</span>' +
                    '</div>' +
                    '<div class="comment-text">' + escapeHtml(c.Comment) + '</div>' +
                    images +
                '</div>' +
            '</div>';
    }

    // ============================================================
    // بارگذاری کامنت‌ها (endless)
    // ============================================================
    function loadComments() {
        if (commentsState.isLoading || !commentsState.hasMore) return;

        commentsState.isLoading = true;
        $('#commentsLoading').show();

        $.ajax({
            url: API + 'Posts/GetComments',
            type: 'GET',
            data: {
                resourceId: RESOURCE_ID,
                skip: commentsState.skip,
                take: COMMENTS_PAGE_SIZE
            },
            dataType: 'json'
        })
        .done(function (res) {
            commentsState.isLoading = false;
            $('#commentsLoading').hide();

            if (!res || !res.success) return;

            commentsState.hasMore = res.HasMore;
            commentsState.skip += (res.Data || []).length;

            if ((res.Data || []).length === 0 && commentsState.skip === 0) {
                $('#commentsEmpty').show();
            } else {
                $('#commentsEmpty').hide();
            }

            var html = '';
            (res.Data || []).forEach(function (c) {
                html += buildCommentItem(c);
            });
            $('#commentsList').append(html);

            if (!commentsState.hasMore && commentsState.skip > 0) {
                $('#commentsEnd').show();
            }
        })
        .fail(function () {
            commentsState.isLoading = false;
            $('#commentsLoading').hide();
        });
    }

    // اسکرول بی‌نهایت
    $(window).on('scroll', function () {
        if (($(window).scrollTop() + $(window).height())
            >= ($(document).height() - 400)) {
            loadComments();
        }
    });

    // ============================================================
    // لایک
    // ============================================================
    $(document).on('click', '.like-btn', function () {
        var $btn = $(this);
        var postId = $btn.data('id');

        $.ajax({
            url: API + 'Posts/ToggleLike',
            type: 'POST',
            data: { resourceId: postId },
            dataType: 'json'
        })
        .done(function (res) {
            if (handleRequireLogin(res)) return;
            if (!res.success) { alert(res.message || 'خطا'); return; }

            $btn.toggleClass('liked', res.isLiked);
            $btn.find('i')
                .toggleClass('fa-heart', res.isLiked)
                .toggleClass('fa-heart-o', !res.isLiked);
            $btn.find('.like-count').text(res.likeCount);
        })
        .fail(function () { alert('خطا در ارتباط با سرور'); });
    });

    // ============================================================
    // آپلود تصاویر کامنت
    // ============================================================
    $('#commentImagesInput').on('change', function () {
        var files = this.files;
        var $preview = $('#commentSelectedPreview').empty();
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

    $('#btnUploadCommentImages').on('click', function () {
        var input = document.getElementById('commentImagesInput');
        var files = input.files;
        if (!files || files.length === 0) {
            showCommentAlert('لطفاً حداقل یک تصویر انتخاب کنید.');
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
            url: API + 'Posts/UploadCommentImages',
            type: 'POST',
            data: fd,
            processData: false,
            contentType: false,
            dataType: 'json'
        })
        .done(function (res) {
            if (handleRequireLogin(res)) return;

            if (!res || !res.success || !res.files || res.files.length === 0) {
                showCommentAlert((res && res.message) || 'آپلود ناموفق بود');
                return;
            }

            res.files.forEach(function (f) {
                addCommentImage(f.url, f.fileName || '');
            });

            if (res.errors && res.errors.length)
                showCommentAlert(res.message + ' | ' + res.errors.join('، '));
            else
                hideCommentAlert();

            input.value = '';
            $('#commentSelectedPreview').empty();
        })
        .fail(function (xhr) {
            showCommentAlert('خطا در ارتباط با سرور (' + xhr.status + ')');
        })
        .always(function () {
            $btn.prop('disabled', false)
                .html('<i class="fa fa-upload"></i> آپلود تصاویر');
        });
    });

    function addCommentImage(url, fileName) {
        if (!url) return;
        if (commentImageUrls.indexOf(url) >= 0) return;
        commentImageUrls.push(url);

        $('#commentUploadedList').append(
            '<div class="d-flex align-items-center mb-1 p-1 border rounded" ' +
                 'data-url="' + escapeHtml(url) + '" style="background:#fafafa;">' +
                '<img src="' + escapeHtml(url) + '" ' +
                     'style="width:44px;height:44px;object-fit:cover;' +
                            'border:1px solid #ddd;border-radius:4px;margin-left:6px;" />' +
                '<div class="flex-grow-1 small text-truncate">' +
                    escapeHtml(fileName || url) +
                '</div>' +
                '<button type="button" class="btn btn-danger btn-sm btn-remove-comment-image">' +
                    '<i class="fa fa-times"></i>' +
                '</button>' +
            '</div>'
        );
    }

    $(document).on('click', '.btn-remove-comment-image', function () {
        var $row = $(this).closest('[data-url]');
        var url = $row.data('url');
        var idx = commentImageUrls.indexOf(url);
        if (idx >= 0) commentImageUrls.splice(idx, 1);
        $row.remove();
    });

    // ============================================================
    // ارسال کامنت
    // ============================================================
    $('#newCommentText').on('input', function () {
        $('#commentCharCount').text(this.value.length + ' / 4000');
    });

    $('#btnSubmitComment').on('click', function () {
        var text = $('#newCommentText').val().trim();

        if (!text && commentImageUrls.length === 0) {
            showCommentAlert('متن یا تصویر کامنت را وارد کنید.');
            return;
        }

        var $btn = $(this);
        $btn.prop('disabled', true)
            .html('<i class="fa fa-spinner fa-spin"></i> ارسال...');

        $.ajax({
            url: API + 'Posts/AddComment',
            type: 'POST',
            data: {
                resourceId: RESOURCE_ID,
                comment: text,
                imageUrls: commentImageUrls.join(',')
            },
            dataType: 'json'
        })
        .done(function (res) {
            $btn.prop('disabled', false).html('<i class="fa fa-send"></i> ارسال نظر');

            if (handleRequireLogin(res)) return;

            if (!res.success) {
                showCommentAlert(res.message || 'خطا');
                return;
            }

            // پاک کردن فرم
            $('#newCommentText').val('');
            $('#commentCharCount').text('0 / 4000');
            $('#commentImagesInput').val('');
            $('#commentSelectedPreview').empty();
            $('#commentUploadedList').empty();
            commentImageUrls = [];
            hideCommentAlert();

            $('#commentsEmpty').hide();
            $('#commentsList').prepend(buildCommentItem(res.comment));
            $('#commentsTotalCount').text(res.commentCount);

            // اسکرول به بالا
            $('html, body').animate({ scrollTop: 0 }, 200);
        })
        .fail(function (xhr) {
            $btn.prop('disabled', false).html('<i class="fa fa-send"></i> ارسال نظر');
            showCommentAlert('خطا در ارتباط با سرور (' + xhr.status + ')');
        });
    });

    function showCommentAlert(msg) { $('#commentAlert').text(msg).show(); }
    function hideCommentAlert() { $('#commentAlert').hide(); }

    // ============================================================
    // شروع
    // ============================================================
    loadPost();
});