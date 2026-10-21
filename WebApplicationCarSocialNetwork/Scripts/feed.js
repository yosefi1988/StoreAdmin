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
        seenIds: {}   // جلوگیری از رندر تکراری
    };

    // State کامنت‌ها
    var commentsState = {
        postId: null,
        skip: 0,
        hasMore: true,
        isLoading: false
    };

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
        // WCF format: "/Date(1699999999999)/"
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

        var name = authorDisplayName(p);
        var avatar = buildAvatar(p.AuthorProfileImage, name);
        var image = p.ImageUrl
            ? '<img class="post-image" src="' + escapeHtml(p.ImageUrl) +
              '" alt="" loading="lazy" onerror="this.style.display=\'none\'">'
            : '';
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

        return '' +
            '<div class="post-card" data-post-id="' + p.ResourceId + '">' +
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
            html += buildPostCard(p);
        });
        container.append(html);
    }

    // ============================================================
    // ارسال پست جدید
    // ============================================================
    function submitPost() {
        var title = $('#postTitle').val().trim();
        var content = $('#postContent').val().trim();
        var imageUrl = $('#postImageUrl').val().trim();

        if (!content) {
            showComposerAlert('متن پست خالی است.');
            return;
        }
        if (!window.FeedConfig.isLoggedIn) {
            showComposerAlert('برای ارسال پست باید وارد شوید.');
            return;
        }

        var $btn = $('#btnSubmitPost');
        $btn.prop('disabled', true)
            .html('<i class="fa fa-spinner fa-spin"></i> ارسال...');

        $.ajax({
            url: API + 'Posts/Create',
            type: 'POST',
            data: {
                title: title || content.substring(0, 100),  // اگر عنوان نبود، از متن
                content: content,
                imageUrl: imageUrl || null
            },
            dataType: 'json'
        })
        .done(function (res) {
            $btn.prop('disabled', false).html('<i class="fa fa-send"></i> ارسال');

            if (!res.success) {
                showComposerAlert(res.message || 'خطا');
                return;
            }

            // پاک کردن فرم
            $('#postTitle, #postContent, #postImageUrl').val('');
            $('#charCount').text('0 / 2000');
            hideComposerAlert();

            // اضافه کردن پست بالای فید
            var cardHtml = buildPostCard(res.post);
            if (feedState.seenIds[res.post.ResourceId]) {
                // قبلاً رندر شده — اسکرول کن بالا
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
        if (!window.FeedConfig.isLoggedIn) {
            alert('برای لایک باید وارد شوید.');
            return;
        }

        var postId = $btn.data('id');

        $.ajax({
            url: API + 'Posts/ToggleLike',
            type: 'POST',
            data: { resourceId: postId },
            dataType: 'json'
        })
        .done(function (res) {
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

        // نمایش پست بالا
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

        // لود کامنت‌ها
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

            if (!res.success) {
                return;
            }

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
        if (!window.FeedConfig.isLoggedIn) {
            alert('برای ثبت نظر باید وارد شوید.');
            return;
        }

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

            if (!res.success) {
                alert(res.message || 'خطا');
                return;
            }

            $('#newCommentText').val('');
            $('#commentsEmpty').hide();

            // افزودن کامنت به بالای لیست
            $('#commentsList').prepend(buildCommentItem(res.comment));

            // به‌روزرسانی شمارنده در کارت پشت
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
        // اگر مودال باز است، اسکرول فید را trigger نکن
        if ($('#commentsModal').hasClass('show')) return;
        if (isNearBottom()) loadFeed();
    });

    // اسکرول داخل مودال برای کامنت‌های بیشتر
    $('#commentsModal .modal-body').on('scroll', function () {
        var el = this;
        if (el.scrollTop + el.clientHeight >= el.scrollHeight - 100) {
            loadComments();
        }
    });

    // ============================================================
    // Event Binding
    // ============================================================

    // ارسال پست
    $('#btnSubmitPost').on('click', submitPost);

    // شمارنده کاراکتر
    $('#postContent').on('input', function () {
        $('#charCount').text(this.value.length + ' / 2000');
    });

    // لایک
    $(document).on('click', '.like-btn', function () {
        toggleLike($(this));
    });

    // باز کردن مودال کامنت
    $(document).on('click', '.comment-btn', function () {
        openCommentsModal($(this).data('id'));
    });

    // ارسال کامنت
    $('#btnAddComment').on('click', addComment);

    // Enter در textarea کامنت = ارسال (Ctrl+Enter)
    $('#newCommentText').on('keydown', function (e) {
        if (e.key === 'Enter' && (e.ctrlKey || e.metaKey)) {
            e.preventDefault();
            addComment();
        }
    });

    // ============================================================
    // شروع
    // ============================================================
    loadFeed();
});