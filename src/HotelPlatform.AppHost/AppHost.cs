var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();

var hotelDb = postgres.AddDatabase("hoteldb");

var redis = builder.AddRedis("redis")
    .WithDataVolume()
    .WithRedisInsight()
    .WithImage("redis", "7.4");



var keycloak = builder.AddKeycloak("keycloak", port: 10800)
    .WithDataVolume()
    .WithBindMount("../../keycloak/themes/hotel-theme", "/opt/keycloak/themes/hotel-theme");

var api = builder.AddProject<Projects.HotelPlatform_Api>("api")
    .WithReference(hotelDb)
    .WithReference(redis)
    .WithReference(keycloak)
    .WaitFor(hotelDb)
    .WithEnvironment("KeyVault__Url", "https://shopflow-kv.vault.azure.net/");

builder.Build().Run();