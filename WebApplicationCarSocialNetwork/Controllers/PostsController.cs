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
                                                l.FK_UserId == userIdValue)   // ← اینجا
                                    };

                    var total = baseQuery.Count();   // ← خط 79 — الان OK

                    var rawData = baseQuery
                        .Skip(skip)
                        .Take(take)
                        .ToList();

                    var data = rawData
                        .Select(p => new
                        {
                            ResourceId = p.ResourceId,
                            Title = p.Title,
                            Content = p.Content,
                            CreatedAt = p.CreatedAt,
                            AuthorId = p.AuthorId,
                            AuthorName = p.AuthorName,
                            AuthorFirstName = p.AuthorFirstName,
                            AuthorLastName = p.AuthorLastName,
                            AuthorProfileImage = p.AuthorProfileImage,
                            ImageUrl = p.ImageUrl,
                            LikeCount = p.LikeCount,
                            CommentCount = p.CommentCount,
                            IsLikedByMe = p.IsLikedByMe
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
        // POST: ایجاد پست جدید
        // ============================================================
        [HttpPost]
        public JsonResult Create(string title, string content, string imageUrl = null)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Json(new { success = false, message = "لطفاً وارد شوید." });

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

                    string savedImageUrl = null;
                    if (!string.IsNullOrWhiteSpace(imageUrl))
                    {
                        savedImageUrl = imageUrl.Trim();
                        db.X_ResourceImages.InsertOnSubmit(new X_ResourceImage
                        {
                            FK_ResourceId = resource.ResourceId,
                            ImageUrl = savedImageUrl,
                            IsMain = true,
                            IsActive = true,
                            CreatedAt = DateTime.Now
                        });
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
                            ImageUrl = savedImageUrl,
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
        // POST: حذف پست (soft delete)
        // ============================================================
        [HttpPost]
        public JsonResult Delete(int resourceId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Json(new { success = false, message = "لطفاً وارد شوید." });

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
                return Json(new { success = false, message = "لطفاً وارد شوید." });

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

        // ============================================================
        // GET: کامنت‌های یک پست (paginated)
        // ============================================================
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
                                    // ⬇️ محافظت: اگر Rating نال‌پذیر است
                                    Rating = (int)c.Rating,
                                    
                                    CreatedAt = c.CreatedAt,
                                    AuthorId = u.UserId,
                                    AuthorName = u.UserName,
                                    AuthorFirstName = u.FirstName,
                                    AuthorLastName = u.LastName,
                                    AuthorProfileImage = u.ProfileImageUrl
                                };

                    var total = query.Count();

                    var data = query
                        .Skip(skip)
                        .Take(take)
                        .ToList()
                        .Select(c => new
                        {
                            CommentId = c.CommentId,
                            Comment = c.Comment,
                            Rating = c.Rating,
                            CreatedAt = c.CreatedAt,
                            AuthorId = c.AuthorId,
                            AuthorName = c.AuthorName,
                            AuthorFirstName = c.AuthorFirstName,
                            AuthorLastName = c.AuthorLastName,
                            AuthorProfileImage = c.AuthorProfileImage
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

        // ============================================================
        // POST: افزودن کامنت
        // ============================================================
        [HttpPost]
        public JsonResult AddComment(int resourceId, string comment)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Json(new { success = false, message = "لطفاً وارد شوید." });

            if (string.IsNullOrWhiteSpace(comment))
                return Json(new { success = false, message = "متن کامنت خالی است." });

            comment = comment.Trim();

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

                    var review = new X_Review
                    {
                        FK_ResourceId = resourceId,
                        FK_UserId = userId.Value,
                        Comment = comment,
                        Rating = 5,
                        IsApproved = true,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };
                    db.X_Reviews.InsertOnSubmit(review);
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
                            Rating = (int?)review.Rating,
                            CreatedAt = review.CreatedAt,
                            AuthorId = user.UserId,
                            AuthorName = user.UserName,
                            AuthorFirstName = user.FirstName,
                            AuthorLastName = user.LastName,
                            AuthorProfileImage = user.ProfileImageUrl
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
        // POST: حذف کامنت (soft delete)
        // ============================================================
        [HttpPost]
        public JsonResult DeleteComment(int commentId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Json(new { success = false, message = "لطفاً وارد شوید." });

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
    }
}