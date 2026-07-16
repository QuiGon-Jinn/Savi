using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace Data.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        // Stored password hash (not serialized)
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;

        // Stored salt for the password hash (not serialized)
        [JsonIgnore]
        public string PasswordSalt { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        // Set a user's password by generating a random salt and hashing with PBKDF2.
        public void SetPassword(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            // 16 byte salt
            var salt = new byte[16];
            RandomNumberGenerator.Fill(salt);

            // derive a 32 byte subkey (SHA256)
            using Rfc2898DeriveBytes pbkdf2 = new (password, salt, 100_000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            PasswordSalt = Convert.ToBase64String(salt);
            PasswordHash = Convert.ToBase64String(hash);
        }

        // Verify a plaintext password against the stored hash/salt
        public bool VerifyPassword(string password)
        {
            if (string.IsNullOrEmpty(PasswordSalt) || string.IsNullOrEmpty(PasswordHash))
                return false;

            var salt = Convert.FromBase64String(PasswordSalt);
            var expectedHash = Convert.FromBase64String(PasswordHash);

            using Rfc2898DeriveBytes pbkdf2 = new(password, salt, 100_000, HashAlgorithmName.SHA256);
            var actualHash = pbkdf2.GetBytes(expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
