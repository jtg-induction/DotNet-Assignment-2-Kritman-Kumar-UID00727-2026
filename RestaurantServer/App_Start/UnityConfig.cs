using RestaurantServer.Helpers;
using RestaurantServer.Helpers.Implementations;
using RestaurantServer.Helpers.Interfaces;
using RestaurantServer.Repositories.Implementations;
using RestaurantServer.Repositories.Interfaces;
using RestaurantServer.Services.Implementations;
using RestaurantServer.Services.Interfaces;
using RestaurantServer.validator.Interfaces;
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

            Container.RegisterType<ApplicationDbContext>(new HierarchicalLifetimeManager());
            Container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager());
            Container.RegisterType<IAuthRepository, AuthRepository>(new HierarchicalLifetimeManager());
            Container.RegisterType<IUsersRepository, UsersRepository>(new HierarchicalLifetimeManager());
            Container.RegisterType<IRefreshTokenRepository, RefreshTokenRepository>(new HierarchicalLifetimeManager());

            Container.RegisterType<IAuthService, AuthService>();
            Container.RegisterType<IUserUpdateService, UserUpdateService>(); 
            Container.RegisterType<IPasswordHasher, PasswordHasher>();
            Container.RegisterType<IJwtTokenService, JwtTokenService>();
            Container.RegisterType<IRefreshTokenHelper, RefreshTokenHelper>(); 
            Container.RegisterType<IAuthenticationValidator, AuthenticationValidator>();
            Container.RegisterType<IRefreshTokenValidator, RefreshTokenValidator>();
            Container.RegisterType<IUserValidator, UserValidator>();
            Container.RegisterType<ICookieHelper, CookieHelper>();
            Container.RegisterType<IRequestValidator, RequestValidator>();
        }
    }
}
