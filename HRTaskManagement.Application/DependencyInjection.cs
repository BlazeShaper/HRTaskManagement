// HRTaskManagement.Application/DependencyInjection.cs
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using System.Reflection;

namespace HRTaskManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Mevcut servis kayıtların (varsa) burada kalmaya devam edecek...

            // AutoMapper'ı bu assembly (Application) içindeki tüm Profile sınıflarını tarayacak şekilde kaydediyoruz.
            services.AddAutoMapper(cfg => {}, Assembly.GetExecutingAssembly());

            return services;
        }
    }
}