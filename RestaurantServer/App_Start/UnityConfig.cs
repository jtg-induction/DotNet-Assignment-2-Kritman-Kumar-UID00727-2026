using RestaurantServer.Helpers.Implementations;
using RestaurantServer.Helpers.Interfaces;
using RestaurantServer.Repositories.Implementations;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Implementations;
using RestaurantServer.Services.Interfaces;
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

            Container.RegisterType<ApplicationDbContext>(
                new HierarchicalLifetimeManager());

            // Auth
            Container.RegisterType<IAuthRepository, AuthRepository>(
                new HierarchicalLifetimeManager());

            Container.RegisterType<IAuthService, AuthService>(
                new HierarchicalLifetimeManager());

            // Account
            Container.RegisterType<IAccountRepository, AccountRepository>(
                new HierarchicalLifetimeManager());

            Container.RegisterType<IAccountService, AccountService>(
                new HierarchicalLifetimeManager());

            // Helpers
            Container.RegisterType<IPasswordHasher, PasswordHasher>(
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJwtTokenService, JwtTokenService>(
                new HierarchicalLifetimeManager());

            // Refresh Token
            Container.RegisterType<IRefreshTokenRepository, RefreshTokenRepository>(
                new HierarchicalLifetimeManager());
        }
    }
}