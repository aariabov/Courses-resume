using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace Devpull.Common;

public class AppConfig
{
    public ConnectionStringsConfig ConnectionStrings { get; } = new();
    public SmtpConfig Smtp { get; } = new();
    public JwtConfig Jwt { get; } = new();
    public UserConfig User { get; } = new();
    public ClientCodeConfig ClientCode { get; } = new();
    public ExerciseConfig Exercise { get; } = new();
    public PriceConfig Price { get; } = new();
    public YookassaConfig Yookassa { get; } = new();
    public bool UseCache { get; }

    public AppConfig() { }

    public AppConfig(IConfiguration configuration)
    {
        ConnectionStrings = new ConnectionStringsConfig();
        Smtp = new SmtpConfig();
        Jwt = new JwtConfig();
        User = new UserConfig();
        ClientCode = new ClientCodeConfig();
        Exercise = new ExerciseConfig();
        Price = new PriceConfig();
        Yookassa = new YookassaConfig();

        // Bind sections
        configuration.Bind("ConnectionStrings", ConnectionStrings);
        configuration.Bind("Smtp", Smtp);
        configuration.Bind("Jwt", Jwt);
        configuration.Bind("User", User);
        configuration.Bind("ClientCode", ClientCode);
        configuration.Bind("Exercise", Exercise);
        configuration.Bind("Price", Price);
        configuration.Bind("Yookassa", Yookassa);
        UseCache = configuration.GetValue<bool>("UseCache");

        // Validate all
        var errors = new List<string>();
        ValidateObject(ConnectionStrings, errors);
        ValidateObject(Smtp, errors);
        ValidateObject(Jwt, errors);
        ValidateObject(User, errors);
        ValidateObject(ClientCode, errors);
        ValidateObject(Exercise, errors);
        ValidateObject(Price, errors);
        ValidateObject(Yookassa, errors);

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                "AppConfig validation failed: " + string.Join("; ", errors)
            );
        }
    }

    private static void ValidateObject(object obj, List<string> errors)
    {
        var ctx = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(obj, ctx, results, true))
        {
            foreach (var r in results)
            {
                errors.Add(r.ErrorMessage ?? r.ToString());
            }
        }
    }
}

public class ConnectionStringsConfig
{
    [Required]
    public string? Devpull { get; set; }
}

public class SmtpConfig
{
    [Required]
    public string? Host { get; set; }

    [Required]
    public int Port { get; set; }

    [Required]
    public string? Username { get; set; }

    [Required]
    public string? Password { get; set; }

    public string? DisplayName { get; set; }
}

public class JwtConfig
{
    [Required]
    public string? Key { get; set; }

    public string? Issuer { get; set; }
    public string? Audience { get; set; }

    [Range(1, int.MaxValue)]
    public int AccessTokenExpirationInSeconds { get; set; }

    [Range(1, int.MaxValue)]
    public int ShortRefreshTokenExpirationInSeconds { get; set; }

    [Range(1, int.MaxValue)]
    public int RefreshTokenExpirationInSeconds { get; set; }

    [Range(0, int.MaxValue)]
    public int RefreshTimeoutInSeconds { get; set; }
}

public class UserConfig
{
    [Range(1, int.MaxValue)]
    public int ValidCodeTimeInSeconds { get; set; }

    [Range(0, int.MaxValue)]
    public int FreeRunsPerDay { get; set; }
}

public class ClientCodeConfig
{
    [Range(1, int.MaxValue)]
    public int TimeoutInSeconds { get; set; }

    [Range(1, int.MaxValue)]
    public int RamLimitInMb { get; set; }

    [Range(1, 100)]
    public int CpuInPercent { get; set; }
}

public class ExerciseConfig
{
    [Range(1, int.MaxValue)]
    public int TimeoutInSeconds { get; set; }

    [Range(1, int.MaxValue)]
    public int RamLimitInMb { get; set; }

    [Range(1, 100)]
    public int CpuInPercent { get; set; }
}

public class PriceConfig
{
    [Required]
    public decimal PerMonth { get; set; }

    [Required]
    public decimal PerYear { get; set; }
}

public class YookassaConfig
{
    [Required]
    public string? ShopId { get; set; }

    [Required]
    public string? SecretKey { get; set; }

    [Required]
    public string? ReturnBaseUrl { get; set; }
}
