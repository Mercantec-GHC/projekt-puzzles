class User
{
    int Id { get; set; }
    string Username { get; set; }
    string Password { get; set; }
    string Email { get; set; }
    string PhoneNumber { get; set; }
    string PassHash { get; set; }
    DateTime CreatedAt { get; set; }
}