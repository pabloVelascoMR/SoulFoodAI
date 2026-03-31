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
        public DbSet<FoodPlanMeal> FoodPlanMeals { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Intolerance> Intolerances { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserData> UserDatas { get; set; }
        public DbSet<UserDiary> UserDiaries { get; set; }
        public DbSet<UserFoodPlanWeek> UserFoodPlansWeek { get; set; }
        public DbSet<UserFoodPlanDaily> UserFoodPlansDaily { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<UserIngredient> UserIngredients{ get; set; }
        




    }
}



