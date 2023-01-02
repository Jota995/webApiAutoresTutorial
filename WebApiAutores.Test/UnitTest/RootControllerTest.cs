using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiAutores.Controllers;
using WebApiAutores.Test.Moks;

namespace WebApiAutores.Test.UnitTest
{
    [TestClass]
    public class RootControllerTest
    {
        [TestMethod]
        public async Task SiUsuarioEsAdmin_Obtenemos4Links()
        {
            //preparacion
            var authorizationService = new AuthorizationService();
            authorizationService.Result = AuthorizationResult.Success();
            var rootController = new RootController(authorizationService);
            rootController.Url = new UrlHelperMock();

            //ejecuccion

            var result = await rootController.Get();

            //verificacion

            Assert.AreEqual(4,result.Value.Count());
        }

        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Obtenemos2Links()
        {
            //preparacion
            var authorizationService = new AuthorizationService();
            authorizationService.Result = AuthorizationResult.Failed();
            var rootController = new RootController(authorizationService);
            rootController.Url = new UrlHelperMock();

            //ejecuccion

            var result = await rootController.Get();

            //verificacion

            Assert.AreEqual(2, result.Value.Count());
        }

        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Obtenemos2LinksMoq()
        {
            //preparacion
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .Returns(Task.FromResult(AuthorizationResult.Failed()));

            mockAuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(AuthorizationResult.Failed()));

            var mockURlHelper = new Mock<IUrlHelper>();
            mockURlHelper.Setup(x => x.Link(It.IsAny<string>(),It.IsAny<object>()))
                .Returns(string.Empty);

            var rootController = new RootController(mockAuthorizationService.Object);
            rootController.Url = mockURlHelper.Object;

            //ejecuccion

            var result = await rootController.Get();

            //verificacion

            Assert.AreEqual(2, result.Value.Count());
        }
    }
}
