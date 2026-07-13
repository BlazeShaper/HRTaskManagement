using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HRTaskManagement.Persistence.Context
{
    public class WorkSphereDbContextFactory : IDesignTimeDbContextFactory<WorkSphereDbContext>
    {
        public WorkSphereDbContext CreateDbContext(string[] args)
        {
            // Komutun çalıştırıldığı mevcut dizinden başlayarak yukarı doğru .sln dosyasını arıyoruz
            var currentDir = Directory.GetCurrentDirectory();
            var dir = new DirectoryInfo(currentDir);
            
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "HRTaskManagement.sln")))
            {
                dir = dir.Parent;
            }

            // Eğer solution klasörünü bulduysak oradaki HRTaskManagement.Api yolunu kullanıyoruz, aksi halde fallback
            var apiProjectPath = dir != null 
                ? Path.Combine(dir.FullName, "HRTaskManagement.Api") 
                : Path.Combine(currentDir, "../HRTaskManagement.Api");

            // Config builder ile appsettings dosyalarını yüklüyoruz
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.Exists(apiProjectPath) ? apiProjectPath : currentDir)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            // appsettings.json içindeki DefaultConnection değerini okuyoruz
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                // Eğer okunamadıysa veya bulunamadıysa uyarı amaçlı localhost fallback
                connectionString = "Server=localhost;Database=WorkSphereDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;";
            }

            var optionsBuilder = new DbContextOptionsBuilder<WorkSphereDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            return new WorkSphereDbContext(optionsBuilder.Options);
        }
    }
}
