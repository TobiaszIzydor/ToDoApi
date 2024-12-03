using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Application.Services;
using ToDo.Application.Validators;

namespace ToDo.Application.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IToDoService, ToDoService>();
            services.AddHttpContextAccessor(); //Add access to Http Context in validation
            services.AddValidatorsFromAssemblyContaining<ToDoValidator>() //Add validation of todo
                .AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
        }
        
    }
}
