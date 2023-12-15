﻿using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace WebAPI;

public static class DependencyInjection
{
    public static readonly string Local_Policy = "local_policy";

    public static IServiceCollection AddDependency(this IServiceCollection services, string databaseConnection = "")
    {
        //Db context 
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(databaseConnection));

        //Repository

        //Service

        return services;
    }

    public static IServiceCollection AddApiService(this IServiceCollection services, string jwtKey = "",
        string issuer = "", string audience = "")
    {
        services.AddCors(options =>
            {
                options.AddPolicy(name: Local_Policy,
                    //Define cors URL 
                    policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                );
            }
        );
        services.AddControllers()
            //allow enum string value in swagger and front-end instead of int value
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        //Add authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };
            }
        );

        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "Kid Pro API", Version = "v1", Description = "ASP NET core API for Kid Pro project."
                });
            string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            option.IncludeXmlComments(xmlPath);
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        services.AddDistributedMemoryCache();

        return services;
    }
}