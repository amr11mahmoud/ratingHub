using System.Threading.Tasks;
using Rating.Models.TokenAuth;
using Rating.Web.Controllers;
using Shouldly;
using Xunit;

namespace Rating.Web.Tests.Controllers
{
    public class HomeController_Tests: RatingWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}