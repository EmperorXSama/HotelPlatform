var builder = DistributedApplication.CreateBuilder(args);

// postgres
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();

var hotelDb = postgres.AddDatabase("hoteldb");

//redis
var redis = builder.AddRedis("redis")
    .WithDataVolume()
    .WithRedisInsight()
    .WithImage("redis", "7.4");

var keycloak = builder.AddKeycloak("keycloak", port:8080)
    .WithDataVolume();



var api = builder.AddProject<Projects.HotelPlatform_Api>("api")
    .WithReference(hotelDb)
    .WithReference(redis)
    .WithReference(keycloak);



builder.Build().Run();
