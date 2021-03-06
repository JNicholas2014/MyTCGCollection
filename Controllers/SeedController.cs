using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyTCGCollection.Data;
using OfficeOpenXml;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using MyTCGCollection.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security;
using Microsoft.AspNetCore.Identity;

namespace MyTCGCollection.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public SeedController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult> Import()
        {
            if (!_env.IsDevelopment())
                throw new SecurityException("Not allowed");

            var path = Path.Combine(_env.ContentRootPath, "Data/Source/cards_with_rarity.xlsx");

            using var stream = System.IO.File.OpenRead(path);
            using var excelPackage = new ExcelPackage(stream);

            var worksheet = excelPackage.Workbook.Worksheets[0];

            var nEndRow = worksheet.Dimension.End.Row;

            var numberOfGamesAdded = 0;
            var numberOfCardsAdded = 0;

            var gamesByName = _context.Games.AsNoTracking().ToDictionary(x => x.GameName, StringComparer.OrdinalIgnoreCase);

            for(int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];

                var gameName = row[nRow, 1].GetValue<string>();

                if (gamesByName.ContainsKey(gameName))
                    continue;

                var game = new Game
                {
                    GameName = gameName
                };

                await _context.Games.AddAsync(game);

                gamesByName.Add(gameName, game);

                numberOfGamesAdded++;
            }

            if (numberOfGamesAdded > 0)
                await _context.SaveChangesAsync();

            var cards = _context.Cards.AsNoTracking().ToDictionary(x => (CardNumber: x.CardNumber, CardName: x.CardName, CardRarity: x.CardRarity, Quantity: x.Quantity, CardValue: x.CardValue, GameId: x.GameId, Expansion: x.Expansion));

            for(int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];
                var cardNumber = row[nRow, 3].GetValue<string>();
                var cardName = row[nRow, 4].GetValue<string>();
                var cardRarity = row[nRow, 5].GetValue<string>();
                var quantity = row[nRow, 6].GetValue<int>();
                var cardValue = row[nRow, 7].GetValue<string>();
                var gameName = row[nRow, 1].GetValue<string>();
                var expansion = row[nRow, 2].GetValue<string>();

                var gameId = gamesByName[gameName].Id;

                if (cards.ContainsKey((
                    CardNumber: cardNumber,
                    CardName: cardName,
                    CardRarity: cardRarity,
                    Quantity: quantity,
                    CardValue: cardValue,
                    GameId: gameId,
                    Expansion: expansion)))
                    continue;

                var card = new Card
                {
                    CardNumber = cardNumber,
                    CardName = cardName,
                    CardRarity = cardRarity,
                    Quantity = quantity,
                    CardValue = cardValue,
                    GameId = gameId,
                    Expansion = expansion
                };

                _context.Cards.Add(card);

                numberOfCardsAdded++;
            }

            if (numberOfCardsAdded > 0)
                await _context.SaveChangesAsync();

            return new JsonResult(new { Cards = numberOfCardsAdded, Games = numberOfGamesAdded });
        }

        [HttpGet]
        public async Task<ActionResult> CreateDefaultUsers() 
        {
            string role_RegisteredUser = "RegisteredUser";
            string role_Administrator = "Administrator";

            if (await _roleManager.FindByNameAsync(role_RegisteredUser) == null)
                await _roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));

            if (await _roleManager.FindByNameAsync(role_Administrator) == null)
                await _roleManager.CreateAsync(new IdentityRole(role_Administrator));

            var addedUsersList = new List<ApplicationUser>();

            var email_Admin = "admin@email.com";
            if(await _userManager.FindByNameAsync(email_Admin) == null)
            {
                var user_Admin = new ApplicationUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_Admin,
                    Email = email_Admin,
                };

                await _userManager.CreateAsync(user_Admin, "MySecr3t$");

                await _userManager.AddToRoleAsync(user_Admin, role_RegisteredUser);
                await _userManager.AddToRoleAsync(user_Admin, role_Administrator);

                user_Admin.EmailConfirmed = true;
                user_Admin.LockoutEnabled = false;

                addedUsersList.Add(user_Admin);
            }

            var email_User = "user@email.com";
            if(await _userManager.FindByNameAsync(email_User) == null)
            {
                var user_User = new ApplicationUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_User,
                    Email = email_User
                };

                await _userManager.CreateAsync(user_User, "MySecr3t$");

                await _userManager.AddToRoleAsync(user_User, role_RegisteredUser);

                user_User.EmailConfirmed = true;
                user_User.LockoutEnabled = false;

                addedUsersList.Add(user_User);
            }

            if (addedUsersList.Count > 0)
                await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                Count = addedUsersList.Count,
                Users = addedUsersList
            });
        }
    }
}
