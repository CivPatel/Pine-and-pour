using Npgsql;

namespace PinePour.Api.Data;

internal static class PostgresConnectionStringNormalizer
{
    public static string Normalize(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return connectionString;
        }

        var trimmed = StripWrappingQuotes(connectionString);
        if (!trimmed.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
            && !trimmed.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            // Npgsql expects the standard keyword/value format (Host=...;Username=...;...).
            // We'll still return the trimmed version to avoid issues when the entire value
            // was accidentally wrapped in quotes by copy/paste or .env syntax.
            return trimmed;
        }

        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri))
        {
            return trimmed;
        }

        var username = string.Empty;
        var password = string.Empty;
        if (!string.IsNullOrWhiteSpace(uri.UserInfo))
        {
            var parts = uri.UserInfo.Split(':', 2);
            username = Uri.UnescapeDataString(parts[0]);
            if (parts.Length > 1)
            {
                password = Uri.UnescapeDataString(parts[1]);
            }
        }

        var database = Uri.UnescapeDataString(uri.AbsolutePath.Trim('/'));
        if (string.IsNullOrWhiteSpace(database))
        {
            database = "postgres";
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort ? 5432 : uri.Port,
            Database = database,
            Username = username,
        };

        if (!string.IsNullOrWhiteSpace(password))
        {
            builder.Password = password;
        }

        // Best-effort querystring support (e.g. sslmode=require&sslrootcert=/path/to/cert)
        if (!string.IsNullOrWhiteSpace(uri.Query))
        {
            var query = uri.Query.TrimStart('?');
            foreach (var pair in query.Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var kv = pair.Split('=', 2);
                var key = Uri.UnescapeDataString(kv[0]);
                var value = kv.Length > 1 ? Uri.UnescapeDataString(kv[1]) : string.Empty;

                if (key.Equals("sslmode", StringComparison.OrdinalIgnoreCase)
                    && Enum.TryParse<SslMode>(value, ignoreCase: true, out var sslMode))
                {
                    builder.SslMode = sslMode;
                }
                else if (key.Equals("sslrootcert", StringComparison.OrdinalIgnoreCase))
                {
                    builder.RootCertificate = value;
                }
            }
        }

        return builder.ConnectionString;
    }

    private static string StripWrappingQuotes(string value)
    {
        var trimmed = value.Trim();
        while (trimmed.Length >= 2)
        {
            var first = trimmed[0];
            var last = trimmed[^1];
            if ((first == '"' && last == '"') || (first == '\'' && last == '\''))
            {
                trimmed = trimmed[1..^1].Trim();
                continue;
            }

            break;
        }

        return trimmed;
    }
}
