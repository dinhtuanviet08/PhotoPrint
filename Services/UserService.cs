using MongoDB.Driver;
using PhotoPrintAPI.Models;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using MongoSettings = PhotoPrintAPI.Settings.MongoDbSettings;

namespace PhotoPrintAPI.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IOptions<MongoSettings> settings, IMongoClient client)
        {
            var databaseName = settings.Value.DatabaseName;
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentNullException(nameof(databaseName), "Database name is not configured.");
            }

            var db = client.GetDatabase(databaseName);
            _users = db.GetCollection<User>("Users");
        }

        public async Task<User?> GetUser(string username) =>
            await _users.Find(u => u.Username == username).FirstOrDefaultAsync();

        public async Task CreateUser(User user) =>
            await _users.InsertOneAsync(user);

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }

        public bool VerifyPassword(string password, string hash) =>
            HashPassword(password) == hash;
    }
}
