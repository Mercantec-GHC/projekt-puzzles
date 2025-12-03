namespace TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var advertService = new AdvertService();
            var userService = new UserService();

            var adverts = advertService.GetAllAdvertsAsync().Result;

            foreach (var advert in adverts)
            {
                Console.WriteLine($"Advert ID: {advert.AdvertId}, Title: {advert.Title}");
            }


            var advertById = advertService.GetAdvertByIdAsync(1).Result;

            Console.WriteLine($"Fetched Advert by ID 1: Title: {advertById.Title}, Description: {advertById.Description}");

            var getUserAdvert = advertService.GetAdvertsByUserIdAsync(2).Result;

            foreach (var advert in getUserAdvert)
            {
                Console.WriteLine($"User's Advert ID: {advert.AdvertId}, Title: {advert.Title}");
            }

            var loggedInUser = userService.LoginUserAsync("user1", "1234").Result;

            Console.WriteLine($"Logged in User: {loggedInUser.Username}, Email: {loggedInUser.Email}");


        }
    }
}
