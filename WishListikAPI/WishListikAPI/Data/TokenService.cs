using WishListik.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace WishListikAPI.Data
{
    public class TokenService
    {
        private readonly ApplicationDbContext _context; // Database context for accessing refresh tokens
        private readonly IConfiguration _configuration; // Configuration settings for JWT and encryption

        // Constructor to initialize context and configuration
        public TokenService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Generates a JWT access token for a given user
        public string GenerateAccessToken(int userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // Add user ID to claims
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Define token expiration (30 minutes from now)
            var currentEpoch = EpochTime.GetIntDate(DateTime.UtcNow);
            var expiryEpoch = currentEpoch + (30 * 60);

            // Create JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                notBefore: DateTimeOffset.FromUnixTimeSeconds(currentEpoch).UtcDateTime,
                expires: DateTimeOffset.FromUnixTimeSeconds(expiryEpoch).UtcDateTime,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token); // Return the token as a string
        }

        // Generates and saves a refresh token for a given user
        public string GenerateAndSaveRefreshToken(int userId)
        {
            var randomNumber = new byte[32];
            string refreshToken;

            // Generate a random refresh token
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken = Convert.ToBase64String(randomNumber); // Base64 encode the token
            }

            var encryptedRefreshToken = Encrypt(refreshToken); // Encrypt the refresh token
            SaveEncryptedRefreshToken(userId, encryptedRefreshToken); // Save the encrypted token to the database

            return refreshToken; // Return the unencrypted token to the client
        }

        // Saves the encrypted refresh token to the database
        public void SaveEncryptedRefreshToken(int userId, string encryptedRefreshToken)
        {
            // Check for an existing refresh token for the user
            var existingToken = _context.RefreshTokens.FirstOrDefault(rt => rt.UserId == userId);

            if (existingToken != null)
            {
                // Update the existing token if found
                existingToken.Token = encryptedRefreshToken;
                existingToken.Expiration = DateTime.UtcNow.AddDays(30); // Update expiration date
            }
            else
            {
                // Create a new refresh token if none exists
                var refreshToken = new RefreshToken
                {
                    UserId = userId,
                    Token = encryptedRefreshToken,
                    Expiration = DateTime.UtcNow.AddDays(30) // Set expiration date
                };

                _context.RefreshTokens.Add(refreshToken); // Add the new token to the database
            }

            _context.SaveChanges(); // Commit the changes to the database
        }

        // Encrypts a string
        public string Encrypt(string plainText)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretRefreshKey"]!);
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = new byte[16];

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText); // Write the plain text into the encryption stream
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray()); // Return the encrypted string as Base64
                }
            }
        }

        // Decrypts a Base64 encoded cipher text string
        public string Decrypt(string cipherText)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretRefreshKey"]!);
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = new byte[16]; 

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd(); // Return the decrypted plain text
                        }
                    }
                }
            }
        }
    }
}
