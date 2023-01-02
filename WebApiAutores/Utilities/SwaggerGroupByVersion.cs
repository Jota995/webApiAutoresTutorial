using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAutores.Utilities
{
    public class SwaggerGroupByVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var nameSpaceController = controller.ControllerType.Namespace;
            var ApiVersion = nameSpaceController.Split('.').Last().ToLower();
            controller.ApiExplorer.GroupName = ApiVersion;
        }
    }
}
