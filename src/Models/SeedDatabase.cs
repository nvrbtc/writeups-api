using dotnetMVP.Models.Entities;
using dotnetMVP.Policies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace dotnetMVP.Models
{
    public class SeedDatabase
    {
        private readonly ApplicationDBcontext _context;
        private readonly UserManager<AppUser> _userManager;
        private AppUser? _adminUser;
        public SeedDatabase(ApplicationDBcontext context, UserManager<AppUser> manager)
        {
            _context = context;
            _userManager = manager;
        }
        //
        public async Task SeedDatabaseDemo()
        {
            List<Platform> platforms = new List<Platform>();
            if (!_context.Platforms.Any())
            {
                platforms = new List<Platform>()
                {
                    new Platform()
                    {
                        Id = Guid.NewGuid(),
                        Name = "HTB",
                        Description = "Hack The Box is an online platform that allows you to test your penetration testing skills.",
                        AvailableChallenges = new()
                    },
                    new Platform()
                    {
                        Id = Guid.NewGuid(),
                        Name = "THM",
                        Description = "TryHackMe is an online platform that allows you to learn and practice cybersecurity skills.",
                        AvailableChallenges = new()
                    },
                    new Platform()
                    {
                        Id = Guid.NewGuid(),
                        Name = "cryptohack",
                        Description = "Cryptohack is an online platform that allows you to learn and practice cryptography skills.",
                        AvailableChallenges = new()
                    }
                };
            }
            List<Challenge> tasks = new List<Challenge>();
            if (!_context.Challenges.Any())
            {
                tasks = new List<Challenge>()
                {
                    new Challenge()
                    {
                        Id = Guid.NewGuid(),
                        Name = "MakeSense",
                        Text = "MakeSense is a challenge on Hack The Box that requires you to find the flag hidden in a web application.",
                        Platform = platforms[0]
                    },
                    new Challenge()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Blue",
                        Text = "Blue is a challenge on THM that requires you to find the flag hidden in a web application.",
                        Platform = platforms[1]
                    },
                    new Challenge()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Crypto 101",
                        Text = "Crypto 101 is a challenge on Cryptohack that requires you to find the flag hidden in a web application.",
                        Platform = platforms[2]
                    }
                };
            }

            List<WriteUp> writeUps = new List<WriteUp>();
            if (!_context.WriteUps.Any())
            {
                writeUps = new List<WriteUp>()
                {
                    new WriteUp()
                    {
                        Id = Guid.NewGuid(),
                        Challenge = tasks[0],
                        ChallengeId = tasks[0].Id,
                        AppUserId = _adminUser.Id,
                        AppUser = _adminUser,
                        Name = "MakeSense WriteUp",
                        Text = "This is a write-up for the MakeSense challenge on Hack The Box.",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsHidden = false,
                        IsHiddenByAdmin = false
                    },
                    new WriteUp()
                    {
                        Id = Guid.NewGuid(),
                        Challenge = tasks[1],
                        ChallengeId = tasks[1].Id,

                        AppUserId = _adminUser.Id,
                        AppUser = _adminUser,
                        Name = "Blue WriteUp",
                        Text = "This is a write-up for the Blue challenge on THM.",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsHidden = false,
                        IsHiddenByAdmin = false
                    },
                    new WriteUp()
                    {
                        Id = Guid.NewGuid(),
                        Challenge = tasks[2],
                        ChallengeId = tasks[2].Id,
                        AppUserId = _adminUser.Id,
                        AppUser = _adminUser,
                        Name = "Crypto 101 WriteUp",
                        Text = "This is a write-up for the Crypto 101 challenge on Cryptohack.",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsHidden = false,
                        IsHiddenByAdmin = false
                    }
                };

            }
            await _context.Platforms.AddRangeAsync(platforms);
            await _context.WriteUps.AddRangeAsync(writeUps);
            await _context.Challenges.AddRangeAsync(tasks);
            await _context.SaveChangesAsync();
        }
        public async Task EnsureRolesAndAdmins()
        {
            // remove all active users and their data [on_cascade]
            var users = await _context.AppUser.Where(x => x.Id != default).ToListAsync();
            _context.RemoveRange(users);
            //Create admin role
            var adminRes = await _context.Roles.AddAsync(new Microsoft.AspNetCore.Identity.IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = nameof(AppPolicies.AdminAccess),
                NormalizedName = nameof(AppPolicies.AdminAccess).ToUpper()
            });
            //Create moderator role
            var moderRes = await _context.Roles.AddAsync(new Microsoft.AspNetCore.Identity.IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = nameof(AppPolicies.ModeratorAccess),
                NormalizedName = nameof(AppPolicies.ModeratorAccess).ToUpper()
            });

            //await _context.SaveChangesAsync(); [not necessary]


            var adminUser = new AppUser
            {
                Id = Guid.NewGuid(),
                Bio = "Admin User for testing.",
                UserName = "admin",
            };
            this._adminUser = adminUser;
            var createdUser = await _userManager.CreateAsync(adminUser, "Admin123!");
            if (!createdUser.Succeeded)
            {
                throw new Exception($"Failed to create admin user in seed database.\n{createdUser.Errors.FirstOrDefault()?.Description ?? "Unknown error"}");
            }
            var admin = await _userManager.AddToRoleAsync(adminUser, nameof(AppPolicies.AdminAccess));
            if (admin.Errors.Any())
            {
                throw new Exception($"Failed to add admin user to role in seed database.\n{admin.Errors.FirstOrDefault()?.Description ?? "Unknown error"}");

            }
        }
    }
}
