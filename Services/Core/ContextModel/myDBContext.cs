using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

public partial class myDBContext : DbContext
{
    public myDBContext()
    {
    }

    public myDBContext(DbContextOptions<myDBContext> options)
        : base(options)
    {

    }

    public virtual DbSet<User> User { get; set; }
    public virtual DbSet<Partner> Company { get; set; }
    public virtual DbSet<SiteConfig> SiteConfig { get; set; }
    public virtual DbSet<Lang> Lang { get; set; }
    public virtual DbSet<LangDisplay> LangDisplay { get; set; }
    public virtual DbSet<BusinessType> BusinessType { get; set; }
    public virtual DbSet<BusinessTypeMemberShip> BusinessTypeMemberShip { get; set; }
    public virtual DbSet<BusinessTypePartner> BusinessTypePartner { get; set; }
    public virtual DbSet<Coupon> Coupon { get; set; }
    public virtual DbSet<CouponMemberShip> CouponMemberShip { get; set; }
    public virtual DbSet<MemberShip> MemberShip { get; set; }
    public virtual DbSet<UserCard> UserCard { get; set; }
    public virtual DbSet<Partner> Partner { get; set; }
    public virtual DbSet<PartnerCoupon> PartnerCoupon { get; set; }
    public virtual DbSet<PartnerDocument> PartnerDocument { get; set; }
    public virtual DbSet<UserCoupon> UserCoupon { get; set; }
    public virtual DbSet<UserMemberShip> UserMemberShip { get; set; }
    public virtual DbSet<Country> Country { get; set; }
    public virtual DbSet<City> City { get; set; }

    public virtual DbSet<ContentPage> ContentPage { get; set; }

    public virtual DbSet<Documents> Documents { get; set; }
    public virtual DbSet<LangText> LangText { get; set; }




    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = "Data Source=178.18.201.44,1533;Initial Catalog=FunAndSaveTest; User Id=sa_kaan;Password=Txp4L&QbCb;";
            optionsBuilder.UseSqlServer(connectionString, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "mySchema"));
        }
    }

    private static readonly MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(myDBContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            ConfigureGlobalFiltersMethodInfo
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(this, new object[] { modelBuilder, entityType });
        }

        modelBuilder.Entity<BusinessType>().HasMany(a => a.Childs).WithOne(b => b.Parent);

        modelBuilder.Entity<ContentPage>().HasOne(a => a.ThumbImage).WithOne(b => b.ThumbImage).HasForeignKey<Documents>(b => b.ThumbImageId);
        modelBuilder.Entity<ContentPage>().HasOne(a => a.Picture).WithOne(b => b.Picture).HasForeignKey<Documents>(b => b.PictureId);
        modelBuilder.Entity<ContentPage>().HasOne(a => a.BannerImage).WithOne(b => b.BannerImage).HasForeignKey<Documents>(b => b.BannerImageId);

        modelBuilder.Entity<ContentPage>().HasMany(a => a.Gallery).WithOne(b => b.Gallery);
        modelBuilder.Entity<ContentPage>().HasMany(a => a.Documents).WithOne(b => b.Document);

        modelBuilder.Entity<ContentPage>().HasMany(a => a.OrjChild).WithOne(b => b.Orj);




        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
        .SelectMany(t => t.GetForeignKeys())
        .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        base.OnModelCreating(modelBuilder);
    }

    protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType) where TEntity : class
    {
        if (entityType.BaseType != null || !ShouldFilterEntity<TEntity>(entityType)) return;
        var filterExpression = CreateFilterExpression<TEntity>();
        if (filterExpression == null) return;
        //if (entityType.GetType().IsInterface==true)
        //if (false)
        //    modelBuilder.Query<TEntity>().HasQueryFilter(filterExpression);
        //else
        modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
    }

    protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
    {
        return typeof(IBaseModel).IsAssignableFrom(typeof(TEntity));
    }

    protected Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>() where TEntity : class
    {
        Expression<Func<TEntity, bool>> expression = null;

        if (typeof(IBaseModel).IsAssignableFrom(typeof(TEntity)))
        {
            Expression<Func<TEntity, bool>> removedFilter = e => (DateTime)((IBaseModel)e).IsDeleted == null;
            expression = expression == null ? removedFilter : CombineExpressions(expression, removedFilter);
        }

        return expression;
    }

    protected Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
    {
        return Helpers.Combine(expression1, expression2);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

