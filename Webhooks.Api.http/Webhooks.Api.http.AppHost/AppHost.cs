var builder = DistributedApplication.CreateBuilder(args);


var database = builder.AddPostgres("postgres")
                      .WithDataVolume("webhooks-data")
                      .WithPgAdmin(pg => pg.WithHostPort(5050))
                      .AddDatabase("webhooks");

builder.AddProject<Projects.Webhooks_Api_http>("webhooks-api-http")
       .WithReference(database)
       .WaitFor(database);

builder.Build().Run();
