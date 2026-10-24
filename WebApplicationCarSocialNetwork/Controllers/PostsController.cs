using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplicationCar.Models;
using WebApplicationCar.Services;

namespace WebApplicationCar.Controllers
{
    [AllowAnonymous]
    public class PostsController : BaseController
    {
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 30;
        private const int DefaultCommentsPageSize = 20;

        // ============================================================
        // Helper: ساخت آدرس لاگین با returnUrl
        // ============================================================
        private string BuildLoginUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                returnUrl = Url.Action("Feed", "Posts");

            return Url.Action("Index", "Login", new { returnUrl = returnUrl });
        }

        // ============================================================
        // Helper: پاسخ استانداردِ «نیاز به لاگین»
        // ============================================================
        private JsonResult NeedLoginResult(string defaultReturnUrl = null)
        {
            string returnUrl = defaultReturnUrl;

            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
                returnUrl = Request.UrlReferrer.PathAndQuery;

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Feed", "Posts");

            return Json(new
            {
                success = false,
                requireLogin = true,
                loginUrl = BuildLoginUrl(returnUrl),
                message = "برای این عملیات باید وارد شوید."
            });
        }

        // ============================================================
        // صفحه فید
        // ============================================================
        public ActionResult Feed()
        {
            return View();
        }

        // ============================================================
        // AJAX: فید پیوسته
        // ============================================================
        [HttpGet]
        public JsonResult GetFeed(int skip = 0, int take = DefaultPageSize)
        {
            if (take < 1) take = DefaultPageSize;
            if (take > MaxPageSize) take = MaxPageSize;
            if (skip < 0) skip = 0;

            try
            {
                var currentUserId = GetCurrentUserId();
                int userIdValue = currentUserId.HasValue ? currentUserId.Value : -1;

                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var baseQuery = from r in db.X_Resources
                                    join u in db.X_Users on r.FK_UserId equals u.UserId
                                    where r.IsActive
                                    orderby r.CreatedAt descending
                                    select new
                                    {
                                        ResourceId = r.ResourceId,
                                        Title = r.NameFa,
                                        Content = r.Description,
                                        CreatedAt = r.CreatedAt,
                                        AuthorId = u.UserId,
                                        AuthorName = u.UserName,
                                        AuthorFirstName = u.FirstName,
                                        AuthorLastName = u.LastName,
                                        AuthorProfileImage = u.ProfileImageUrl,

                                        ImageUrl = db.X_ResourceImages
                                            .Where(i => i.FK_ResourceId == r.ResourceId
                                                     && i.IsActive
                                                     && i.IsMain == true)
                                            .Select(i => i.ImageUrl)
                                            .FirstOrDefault(),

                                        LikeCount = db.X_ResourceLikes
                                            .Count(l => l.FK_ResourceId == r.ResourceId),

                                        CommentCount = db.X_Reviews
                                            .Count(c => c.FK_ResourceId == r.ResourceId
                                                     && c.IsActive
                                                     && c.IsApproved),

                                        IsLikedByMe = currentUserId.HasValue &&
                                            db.X_ResourceLikes.Any(l =>
                                                l.FK_ResourceId == r.ResourceId &&
                                                l.FK_UserId == userIdValue)
                                    };

                    var total = baseQuery.Count();

                    var rawData = baseQuery
                        .Skip(skip)
                        .Take(take)
                        .ToList();

                    var data = rawData
                        .Select(p => new
                        {
                            p.ResourceId,
                            p.Title,
                            p.Content,
                            p.CreatedAt,
                            p.AuthorId,
                            p.AuthorName,
                            p.AuthorFirstName,
                            p.AuthorLastName,
                            p.AuthorProfileImage,
                            p.ImageUrl,
                            p.LikeCount,
                            p.CommentCount,
                            p.IsLikedByMe
                        })
                        .ToList();

                    return Json(new
                    {
                        success = true,
                        Data = data,
                        Total = total,
                        HasMore = (skip + take) < total,
                        Skip = skip + take,
                        CurrentUserId = currentUserId
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "خطا در بارگذاری فید: " + ex.Message,
                    detail = ex.ToString(),
                    Data = new object[0],
                    Total = 0,
                    HasMore = false,
                    Skip = skip
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // ============================================================
        // AJAX: جزئیات یک پست
        // ============================================================
        [HttpGet]
        public JsonResult GetPost(int resourceId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var post = (from r in db.X_Resources
                                join u in db.X_Users on r.FK_UserId equals u.UserId
                                where r.ResourceId == resourceId && r.IsActive
                                select new
                                {
                                    ResourceId = r.ResourceId,
                                    Title = r.NameFa,
                                    Content = r.Description,
                                    CreatedAt = r.CreatedAt,
                                    AuthorId = u.UserId,
                                    AuthorName = u.UserName,
                                    AuthorFirstName = u.FirstName,
                                    AuthorLastName = u.LastName,
                                    AuthorProfileImage = u.ProfileImageUrl,
                                    ImageUrl = db.X_ResourceImages
                                        .Where(i => i.FK_ResourceId == r.ResourceId
                                                 && i.IsActive && i.IsMain)
                                        .Select(i => i.ImageUrl).FirstOrDefault(),
                                    LikeCount = db.X_ResourceLikes
                                        .Count(l => l.FK_ResourceId == r.ResourceId),
                                    CommentCount = db.X_Reviews
                                        .Count(c => c.FK_ResourceId == r.ResourceId
                                                 && c.IsActive && c.IsApproved),
                                    IsLikedByMe = currentUserId.HasValue &&
                                        db.X_ResourceLikes.Any(l =>
                                            l.FK_ResourceId == r.ResourceId &&
                                            l.FK_UserId == currentUserId.Value)
                                }).FirstOrDefault();

                    if (post == null)
                        return Json(new { success = false, message = "پست یافت نشد." },
                                    JsonRequestBehavior.AllowGet);

                    return Json(new { success = true, post = post },
                                JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    detail = ex.StackTrace
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // ============================================================
        // POST: ایجاد پست جدید (چند تصویر)
        // ============================================================
        [HttpPost]
        public JsonResult Create(string title, string content, string imageUrls = null)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return NeedLoginResult();

            if (string.IsNullOrWhiteSpace(title))
                return Json(new { success = false, message = "عنوان الزامی است." });

            if (string.IsNullOrWhiteSpace(content))
                return Json(new { success = false, message = "متن الزامی است." });

            title = title.Trim();
            content = content.Trim();

            if (title.Length > 400)
                return Json(new { success = false, message = "عنوان حداکثر ۴۰۰ کاراکتر." });

            if (content.Length > 2000)
                return Json(new { success = false, message = "متن حداکثر ۲۰۰۰ کاراکتر." });

            var urlList = new List<string>();
            if (!string.IsNullOrWhiteSpace(imageUrls))
            {
                urlList = imageUrls
                    .Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(u => u.Trim())
                    .Where(u => !string.IsNullOrEmpty(u))
                    .Take(10)
                    .ToList();
            }

            try
            {
                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var resource = new X_Resource
                    {
                        NameFa = title,
                        Description = content,
                        FK_UserId = userId.Value,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };
                    db.X_Resources.InsertOnSubmit(resource);
                    db.SubmitChanges();

                    string mainImageUrl = null;
                    if (urlList.Count > 0)
                    {
                        mainImageUrl = urlList[0];

                        for (int i = 0; i < urlList.Count; i++)
                        {
                            db.X_ResourceImages.InsertOnSubmit(new X_ResourceImage
                            {
                                FK_ResourceId = resource.ResourceId,
                                ImageUrl = urlList[i],
                                IsMain = (i == 0),
                                SortOrder = i + 1,
                                IsActive = true,
                                CreatedAt = DateTime.Now
                            });
                        }
                        db.SubmitChanges();
                    }

                    var user = db.X_Users.First(u => u.UserId == userId.Value);

                    return Json(new
                    {
                        success = true,
                        message = "پست ارسال شد.",
                        post = new
                        {
                            ResourceId = resource.ResourceId,
                            Title = resource.NameFa,
                            Content = resource.Description,
                            CreatedAt = resource.CreatedAt,
                            AuthorId = user.UserId,
                            AuthorName = user.UserName,
                            AuthorFirstName = user.FirstName,
                            AuthorLastName = user.LastName,
                            AuthorProfileImage = user.ProfileImageUrl,
                            ImageUrl = mainImageUrl,
                            ImageUrls = urlList,
                            LikeCount = 0,
                            CommentCount = 0,
                            IsLikedByMe = false
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "خطا: " + ex.Message,
                    detail = ex.StackTrace
                });
            }
        }

        // ============================================================
        // POST: حذف پست
        // ============================================================
        [HttpPost]
        public JsonResult Delete(int resourceId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return NeedLoginResult();                 // ← اصلاح شد

            try
            {
                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var resource = db.X_Resources
                        .FirstOrDefault(r => r.ResourceId == resourceId);
                    if (resource == null)
                        return Json(new { success = false, message = "پست یافت نشد." });

                    if (resource.FK_UserId != userId.Value)
                        return Json(new { success = false, message = "دسترسی ندارید." });

                    resource.IsActive = false;
                    db.SubmitChanges();

                    return Json(new { success = true, message = "پست حذف شد." });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    detail = ex.StackTrace
                });
            }
        }

        // ============================================================
        // POST: لایک / آنلایک
        // ============================================================
        [HttpPost]
        public JsonResult ToggleLike(int resourceId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return NeedLoginResult();                 // ← اصلاح شد

            try
            {
                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var resource = db.X_Resources
                        .FirstOrDefault(r => r.ResourceId == resourceId && r.IsActive);
                    if (resource == null)
                        return Json(new { success = false, message = "پست یافت نشد." });

                    var existing = db.X_ResourceLikes
                        .FirstOrDefault(l => l.FK_ResourceId == resourceId
                                          && l.FK_UserId == userId.Value);

                    bool isLiked;
                    if (existing != null)
                    {
                        db.X_ResourceLikes.DeleteOnSubmit(existing);
                        isLiked = false;
                    }
                    else
                    {
                        db.X_ResourceLikes.InsertOnSubmit(new X_ResourceLike
                        {
                            FK_ResourceId = resourceId,
                            FK_UserId = userId.Value,
                            CreatedAt = DateTime.Now
                        });
                        isLiked = true;
                    }
                    db.SubmitChanges();

                    var likeCount = db.X_ResourceLikes
                        .Count(l => l.FK_ResourceId == resourceId);

                    return Json(new
                    {
                        success = true,
                        isLiked = isLiked,
                        likeCount = likeCount
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    detail = ex.StackTrace
                });
            }
        }

        [HttpGet]
        public JsonResult GetComments(int resourceId, int skip = 0, int take = DefaultCommentsPageSize)
        {
            if (take < 1) take = DefaultCommentsPageSize;
            if (take > 50) take = 50;
            if (skip < 0) skip = 0;

            try
            {
                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var query = from c in db.X_Reviews
                                join u in db.X_Users on c.FK_UserId equals u.UserId
                                where c.FK_ResourceId == resourceId
                                   && c.IsActive
                                   && c.IsApproved
                                orderby c.CreatedAt descending
                                select new
                                {
                                    CommentId = c.ReviewId,
                                    Comment = c.Comment,
                                    Rating = (int)c.Rating,
                                    CreatedAt = c.CreatedAt,
                                    AuthorId = u.UserId,
                                    AuthorName = u.UserName,
                                    AuthorFirstName = u.FirstName,
                                    AuthorLastName = u.LastName,
                                    AuthorProfileImage = u.ProfileImageUrl,

                                    // 👇 تصاویر کامنت
                                    ImageUrls = db.X_ReviewImages
                                        .Where(i => i.FK_ReviewId == c.ReviewId && i.IsActive)
                                        .OrderBy(i => i.SortOrder)
                                        .Select(i => i.ImageUrl)
                                        .ToList()
                                };

                    var total = query.Count();

                    var data = query
                        .Skip(skip)
                        .Take(take)
                        .ToList()
                        .Select(c => new
                        {
                            c.CommentId,
                            c.Comment,
                            c.Rating,
                            c.CreatedAt,
                            c.AuthorId,
                            c.AuthorName,
                            c.AuthorFirstName,
                            c.AuthorLastName,
                            c.AuthorProfileImage,
                            c.ImageUrls
                        })
                        .ToList();

                    return Json(new
                    {
                        success = true,
                        Data = data,
                        Total = total,
                        HasMore = (skip + take) < total
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    detail = ex.StackTrace,
                    Data = new object[0],
                    Total = 0,
                    HasMore = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddComment(int resourceId, string comment, string imageUrls = null)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return NeedLoginResult();

            comment = (comment ?? "").Trim();

            var urlList = new List<string>();
            if (!string.IsNullOrWhiteSpace(imageUrls))
            {
                urlList = imageUrls
                    .Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(u => u.Trim())
                    .Where(u => !string.IsNullOrEmpty(u))
                    .Take(6)
                    .ToList();
            }

            if (string.IsNullOrWhiteSpace(comment) && urlList.Count == 0)
                return Json(new { success = false, message = "متن یا تصویر کامنت را وارد کنید." });

            if (comment.Length > 4000)
                return Json(new { success = false, message = "حداکثر ۴۰۰۰ کاراکتر." });

            try
            {
                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var exists = db.X_Resources
                        .Any(r => r.ResourceId == resourceId && r.IsActive);
                    if (!exists)
                        return Json(new { success = false, message = "پست یافت نشد." });

                    var finalComment = string.IsNullOrWhiteSpace(comment)
                        ? "📷 تصویر"
                        : comment;

                    var review = new X_Review
                    {
                        FK_ResourceId = resourceId,
                        FK_UserId = userId.Value,
                        Comment = finalComment,
                        Rating = 5,
                        IsApproved = true,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };
                    db.X_Reviews.InsertOnSubmit(review);
                    db.SubmitChanges();

                    // 👇 ذخیره تصاویر کامنت
                    for (int i = 0; i < urlList.Count; i++)
                    {
                        db.X_ReviewImages.InsertOnSubmit(new X_ReviewImage
                        {
                            FK_ReviewId = review.ReviewId,
                            ImageUrl = urlList[i],
                            SortOrder = i + 1,
                            IsActive = true,
                            CreatedAt = DateTime.Now
                        });
                    }
                    db.SubmitChanges();

                    var user = db.X_Users.First(u => u.UserId == userId.Value);

                    var newCommentCount = db.X_Reviews
                        .Count(c => c.FK_ResourceId == resourceId
                                 && c.IsActive && c.IsApproved);

                    return Json(new
                    {
                        success = true,
                        message = "کامنت ثبت شد.",
                        comment = new
                        {
                            CommentId = review.ReviewId,
                            Comment = review.Comment,
                            CreatedAt = review.CreatedAt,
                            AuthorId = user.UserId,
                            AuthorName = user.UserName,
                            AuthorFirstName = user.FirstName,
                            AuthorLastName = user.LastName,
                            AuthorProfileImage = user.ProfileImageUrl,
                            ImageUrls = urlList
                        },
                        commentCount = newCommentCount
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    detail = ex.StackTrace
                });
            }
        }

        // ============================================================
        // POST: حذف کامنت
        // ============================================================
        [HttpPost]
        public JsonResult DeleteComment(int commentId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return NeedLoginResult();                 // ← اصلاح شد

            try
            {
                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var review = db.X_Reviews.FirstOrDefault(r => r.ReviewId == commentId);
                    if (review == null)
                        return Json(new { success = false, message = "کامنت یافت نشد." });

                    var post = db.X_Resources
                        .FirstOrDefault(r => r.ResourceId == review.FK_ResourceId);

                    if (review.FK_UserId != userId.Value &&
                        (post == null || post.FK_UserId != userId.Value))
                        return Json(new { success = false, message = "دسترسی ندارید." });


                    var images = db.X_ReviewImages
                        .Where(i => i.FK_ReviewId == review.ReviewId);
                    foreach (var img in images)
                    {
                        img.IsActive = false;
                    }

                    review.IsActive = false;
                    review.UpdatedAt = DateTime.Now;
                    db.SubmitChanges();

                    var commentCount = db.X_Reviews
                        .Count(c => c.FK_ResourceId == review.FK_ResourceId
                                 && c.IsActive && c.IsApproved);

                    return Json(new
                    {
                        success = true,
                        message = "کامنت حذف شد.",
                        commentCount = commentCount
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    detail = ex.StackTrace
                });
            }
        }

        // ============================================================
        // Helper: گرفتن UserId از cookie auth_token
        // ============================================================
        private int? GetCurrentUserId()
        {
            if (HttpContext != null && HttpContext.Items != null
                && HttpContext.Items["UserId"] != null)
            {
                object item = HttpContext.Items["UserId"];
                if (item is int) return (int)item;
            }

            if (Request != null && Request.Cookies != null
                && Request.Cookies["auth_token"] != null)
            {
                string authToken = Request.Cookies["auth_token"].Value;
                if (!string.IsNullOrEmpty(authToken))
                {
                    try
                    {
                        var tokenService = new SessionTokenService();
                        var currentUser = tokenService.ValidateToken(authToken);
                        if (currentUser != null) return currentUser.UserId;
                    }
                    catch { /* ignore */ }
                }
            }

            return null;
        }


        // ============================================================
        // POST: آپلود چند تصویر برای پست
        // ============================================================
        [HttpPost]
        public JsonResult UploadImage()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return NeedLoginResult();

            try
            {
                var files = Request.Files;
                if (files == null || files.Count == 0)
                    return Json(new { success = false, message = "فایلی انتخاب نشده" });

                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var folder = Server.MapPath("~/Uploads/Posts/");
                if (!System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);

                var results = new List<object>();
                var errors = new List<string>();

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    if (file == null || file.ContentLength == 0) continue;

                    var ext = System.IO.Path.GetExtension(file.FileName).ToLower();
                    if (!allowed.Contains(ext))
                    {
                        errors.Add(file.FileName + ": فرمت مجاز نیست");
                        continue;
                    }

                    if (file.ContentLength > 5 * 1024 * 1024)
                    {
                        errors.Add(file.FileName + ": حجم بیش از ۵ مگابایت");
                        continue;
                    }

                    var fileName = Guid.NewGuid().ToString("N") + ext;
                    var fullPath = System.IO.Path.Combine(folder, fileName);
                    file.SaveAs(fullPath);

                    results.Add(new
                    {
                        fileName = file.FileName,
                        url = Url.Content("~/Uploads/Posts/" + fileName)
                    });
                }

                return Json(new
                {
                    success = results.Count > 0,
                    files = results,
                    errors = errors,
                    message = results.Count > 0
                        ? results.Count + " فایل آپلود شد"
                        : "هیچ فایلی آپلود نشد"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا: " + ex.Message });
            }
        }


        // ============================================================
        // GET: صفحه جزئیات پست
        // ============================================================
        [HttpGet]
        public ActionResult Detail(int id)
        {
            using (var db = new DataClassesDatabaseDataContextDataContext())
            {
                var exists = db.X_Resources
                    .Any(r => r.ResourceId == id && r.IsActive);

                if (!exists)
                    return HttpNotFound("پست یافت نشد.");

                ViewBag.ResourceId = id;
                return View();
            }
        }

        // ============================================================
        // GET: یک پست کامل (برای صفحه Detail)
        // ============================================================
        [HttpGet]
        public JsonResult GetPostFull(int resourceId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                using (var db = new DataClassesDatabaseDataContextDataContext())
                {
                    var post = (from r in db.X_Resources
                                join u in db.X_Users on r.FK_UserId equals u.UserId
                                where r.ResourceId == resourceId && r.IsActive
                                select new
                                {
                                    ResourceId = r.ResourceId,
                                    Title = r.NameFa,
                                    Content = r.Description,
                                    CreatedAt = r.CreatedAt,
                                    AuthorId = u.UserId,
                                    AuthorName = u.UserName,
                                    AuthorFirstName = u.FirstName,
                                    AuthorLastName = u.LastName,
                                    AuthorProfileImage = u.ProfileImageUrl,

                                    ImageUrls = db.X_ResourceImages
                                        .Where(i => i.FK_ResourceId == r.ResourceId && i.IsActive)
                                        .OrderBy(i => i.SortOrder)
                                        .Select(i => i.ImageUrl)
                                        .ToList(),

                                    LikeCount = db.X_ResourceLikes
                                        .Count(l => l.FK_ResourceId == r.ResourceId),

                                    CommentCount = db.X_Reviews
                                        .Count(c => c.FK_ResourceId == r.ResourceId
                                                 && c.IsActive && c.IsApproved),

                                    IsLikedByMe = currentUserId.HasValue &&
                                        db.X_ResourceLikes.Any(l =>
                                            l.FK_ResourceId == r.ResourceId &&
                                            l.FK_UserId == currentUserId.Value)
                                }).FirstOrDefault();

                    if (post == null)
                        return Json(new { success = false, message = "پست یافت نشد." },
                                    JsonRequestBehavior.AllowGet);

                    return Json(new { success = true, post = post },
                                JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message },
                            JsonRequestBehavior.AllowGet);
            }
        }

        // ============================================================
        // POST: آپلود چند تصویر برای کامنت
        // ============================================================
        [HttpPost]
        public JsonResult UploadCommentImages()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return NeedLoginResult();

            try
            {
                var files = Request.Files;
                if (files == null || files.Count == 0)
                    return Json(new { success = false, message = "فایلی انتخاب نشده" });

                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var folder = Server.MapPath("~/Uploads/Comments/");
                if (!System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);

                var results = new List<object>();
                var errors = new List<string>();

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    if (file == null || file.ContentLength == 0) continue;

                    var ext = System.IO.Path.GetExtension(file.FileName).ToLower();
                    if (!allowed.Contains(ext))
                    {
                        errors.Add(file.FileName + ": فرمت مجاز نیست");
                        continue;
                    }

                    if (file.ContentLength > 5 * 1024 * 1024)
                    {
                        errors.Add(file.FileName + ": حجم بیش از ۵ مگابایت");
                        continue;
                    }

                    var fileName = Guid.NewGuid().ToString("N") + ext;
                    var fullPath = System.IO.Path.Combine(folder, fileName);
                    file.SaveAs(fullPath);

                    results.Add(new
                    {
                        fileName = file.FileName,
                        url = Url.Content("~/Uploads/Comments/" + fileName)
                    });
                }

                return Json(new
                {
                    success = results.Count > 0,
                    files = results,
                    errors = errors,
                    message = results.Count > 0
                        ? results.Count + " فایل آپلود شد"
                        : "هیچ فایلی آپلود نشد"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا: " + ex.Message });
            }
        }

    }
}