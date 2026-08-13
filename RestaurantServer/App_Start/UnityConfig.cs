using RestaurantServer.Helpers;
using RestaurantServer.Helpers.Implementations;
using RestaurantServer.Helpers.Interfaces;
using RestaurantServer.Repositories.Implementations;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Implementations;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.Validators.Implementations;
using RestaurantServer.Validators.Interfaces;
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

            Container.RegisterType<IUnitOfWork, UnitOfWork>(
                new HierarchicalLifetimeManager());

            Container.RegisterType<IAuthRepository, AuthRepository>(
                new HierarchicalLifetimeManager());

            Container.RegisterType<IUsersRepository, UsersRepository>(
                new HierarchicalLifetimeManager());

            Container.RegisterType<IRefreshTokenRepository, RefreshTokenRepository>(
                new HierarchicalLifetimeManager());

            Container.RegisterType<IAuthService, AuthService>(
                new HierarchicalLifetimeManager());

            Container.RegisterType<IUserUpdateService, UserUpdateService>(
                new HierarchicalLifetimeManager());

            Container.RegisterType<IPasswordHasher, PasswordHasher>(
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJwtTokenService, JwtTokenService>(
                new HierarchicalLifetimeManager());

            Container.RegisterType<IAuthenticationValidator, AuthenticationValidator>(
                new HierarchicalLifetimeManager());

            Container.RegisterType<ICookieHelper, CookieHelper>(
                new HierarchicalLifetimeManager());
        }
    }
}
