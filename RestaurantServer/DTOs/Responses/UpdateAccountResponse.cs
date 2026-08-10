namespace RestaurantServer.DTOs.Responses
{
    public class UpdateAccountResponse
    {
        public long UserId {  get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber {  get; set; }
        public string Message {  get; set; }
    }
}
