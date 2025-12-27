using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace project.Attributes
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 1. Kiểm tra Session "Username" có tồn tại không?
            if (context.HttpContext.Session.GetString("Username") == null)
            {
                // Nếu chưa đăng nhập -> Đá về trang Login
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "Controller", "Account" },
                        { "Action", "Login" },
                        { "Area", "" } // Quan trọng: Quay về vùng Global (không phải Admin)
                    });
            }
            else
            {
                // 2. Nếu đã đăng nhập, kiểm tra xem có phải là Admin không?
                // Lưu ý: Lúc login bạn phải lưu Session["Role"] rồi nhé
                var role = context.HttpContext.Session.GetString("Role");

                if (role != "Admin")
                {
                    // Nếu không phải Admin (VD: Customer) -> Đá về trang chủ hoặc báo lỗi
                    context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "Controller", "Home" },
                        { "Action", "Index" },
                        { "Area", "" }
                    });
                }
            }
        }
    }
}
