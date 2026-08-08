using RestaurantServer.Helpers.Implementations;
using RestaurantServer.Helpers.Interfaces;
using RestaurantServer.Repositories.Implementations;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Implementations; 
using Unity;
using Unity.Lifetime;
 
namespace RestaurantServer.App_Start
{
    public static class UnityConfig
    {
        public static IUnityContainer Container { get; private set; }

        public static void RegisterComponents()
        {
            Container = new UnityContainer();

            Container.RegisterType<ApplicationDbContext>(new HierarchicalLifetimeManager());
            Container.RegisterType<IAuthRepository, AuthRepository>(new HierarchicalLifetimeManager());
            Container.RegisterType<IAuthService, AuthService>(new HierarchicalLifetimeManager());
            Container.RegisterType<IPasswordHasher, PasswordHasher>(new HierarchicalLifetimeManager());
            Container.RegisterType<IJwtTokenService, JwtTokenService>(new HierarchicalLifetimeManager());
            Container.RegisterType<IRefreshTokenRepository, RefreshTokenRepository>(new HierarchicalLifetimeManager());
        }
    }
}
