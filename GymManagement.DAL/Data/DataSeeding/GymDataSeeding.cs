using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GymManagement.DAL.Data.DataSeeding
{
    //helper class
    public static class GymDataSeeding
    {
        public static async Task SeedAsync(GymDbContext dbContext, string seedFolderPath, ILogger logger, CancellationToken ct = default)
        {
            try
            {
                if(!await dbContext.Plans.AnyAsync(ct))
                {
                    var plans = LoadDataFromJsonFile<Plan>(seedFolderPath, "plans.json");
                    if (plans.Any())
                    {
                        dbContext.Plans.AddRange(plans);
                        logger.LogInformation($"Plans Seeded With Count = {plans.Count}");
                    }
                    if (dbContext.ChangeTracker.HasChanges())
                        await dbContext.SaveChangesAsync();
                    else
                        logger.LogInformation($"Plans Already Seeded");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Gym Data Seding Failed");
                throw;
            }
        }
        private static List<T> LoadDataFromJsonFile<T>(string folderPath, string fileName)
        {
            var filepath = Path.Combine(folderPath, fileName);
            if (!File.Exists(filepath))
                throw new FileNotFoundException($"Seed Data File Not Found : {filepath}");

            var data = File.ReadAllText(filepath);
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };

           return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];
        }
    }
}
