using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantServer;
using RestaurantServer.Controllers;
using System.Web.Mvc;

namespace RestaurantServer.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Home Page", result.ViewBag.Title);
        }
    }
}
