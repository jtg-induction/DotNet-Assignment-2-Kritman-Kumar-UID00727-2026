namespace RestaurantServer.Validators.Interfaces
{
    public interface IRequestValidator
    {
        void IsRequestNull<T>(T request);
    }
}
