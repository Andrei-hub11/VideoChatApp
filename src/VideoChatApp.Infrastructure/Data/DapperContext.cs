﻿using System.Data;
using System.Data.SqlClient;

using Microsoft.Extensions.Configuration;

namespace VideoChatApp.Infrastructure.Data;

public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration, string? connectionString = null)
    {
        _connectionString = connectionString ?? configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException(nameof(configuration), 
            "Connection string 'DefaultConnection' not found.");
    }
    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);
}
