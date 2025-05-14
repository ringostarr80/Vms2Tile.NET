using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Helpers
{
    public static class ViewHelpers
    {
        public static bool IsActive(ViewContext viewContext, string controller, string? action = null)
        {
            var currentController = viewContext.RouteData.Values["controller"]?.ToString();
            var currentAction = viewContext.RouteData.Values["action"]?.ToString();
            return currentController == controller && (action == null || currentAction == action);
        }
    }
}