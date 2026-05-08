using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Models;

namespace SoulFoodAiBack.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<FoodPlan> FoodPlans { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Intolerance> Intolerances { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserData> UserDatas { get; set; }
        public DbSet<UserDiary> UserDiaries { get; set; }
        public DbSet<UserFoodPlanWeek> UserFoodPlansWeek { get; set; }
        public DbSet<UserFoodPlanDaily> UserFoodPlansDaily { get; set; }
        public DbSet<FoodPlanDailyRecipe> FoodPlanDailyRecipes { get; set; }
        public DbSet<RecipeUserIngredient> RecipeUserIngredients { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<UserIngredient> UserIngredients{ get; set; }
        public DbSet<UserIntolerance> UserIntolerances { get; set; }
        public DbSet<UserFoodPlanWeekIntolerance> UserFoodPlanWeekIntolerances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<UserFoodPlanDaily>()
                .HasOne(d => d.UserFoodPlanWeek)
                .WithMany(w => w.UserFoodPlanMeals)
                .HasForeignKey(d => d.IdUserFoodPlanWeek)
                .OnDelete(DeleteBehavior.NoAction);

            
            modelBuilder.Entity<FoodPlanDailyRecipe>()
                .HasOne(d => d.UserFoodPlanDaily)
                .WithMany(u => u.FoodPlanDailyRecipes)
                .HasForeignKey(d => d.IdUserFoodPlanDaily)
                .OnDelete(DeleteBehavior.NoAction);

           
            modelBuilder.Entity<FoodPlanDailyRecipe>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}



