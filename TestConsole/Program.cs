namespace TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var advertService = new AdvertService();
            var userService = new UserService();

            var adverts = advertService.GetAllAdvertsAsync(searchTerm: "Puzz", limit: 4).Result;

            foreach (var advert in adverts)
            {
                Console.WriteLine($"Advert ID: {advert.AdvertId}, Title: {advert.Title}, User: {advert.User.UserId}");
            }


            var advertById = advertService.GetAdvertByIdAsync(1).Result;

            Console.WriteLine($"Fetched Advert by ID 1: Title: {advertById.Title}, Description: {advertById.Description} By User: {advertById.User.UserId}");

            var getUserAdvert = advertService.GetAdvertsByUserAsync("user1").Result;

            foreach (var advert in getUserAdvert)
            {
                Console.WriteLine($"User's Advert ID: {advert.AdvertId}, Title: {advert.Title}");
            }

            var loggedInUser = userService.LoginUserAsync("user1", "1234").Result;

            Console.WriteLine($"Logged in User: {loggedInUser.Username}, Email: {loggedInUser.Email}");

            loggedInUser = userService.LoginUserAsync("user0", "1234").Result;

            if (loggedInUser == null)
            {
                Console.WriteLine("Login failed for user0");
            }

            var advertCount = advertService.GetAdvertCountAsync().Result;

            Console.WriteLine($"Total Adverts Available: {advertCount}");

        }
    }
}
