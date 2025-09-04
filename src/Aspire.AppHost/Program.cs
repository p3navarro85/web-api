IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<OllamaResource> ollama = builder
    .AddOllama("ollama")
    .WithDataVolume()
    //.WithHttpEndpoint(port: 11434, targetPort: 11434, name: "ollama")
    //.WithBindMount("../../.containers/ollama", "/root/.ollama")
    .WithOpenWebUI();

//ollama.WithUrlForEndpoint("https", url =>
//{
//    url.Url = "https://localhost:5002"; // Or your desired HTTPS URL
//    url.DisplayText = "Ollama (HTTPS)"; // Optional display text for the dashboard
//});

ollama.AddModel("llama3:8b");

IResourceBuilder<PostgresDatabaseResource> database = builder
    .AddPostgres("database")
    .WithImage("postgres:17")
    .WithBindMount("../../.containers/db", "/var/lib/postgresql/data")
    .WithPgAdmin()
    .AddDatabase("clean-architecture");

builder.AddProject<Projects.Web_Api>("web-api")
    .WithEnvironment("ConnectionStrings__Database", database)
    .WithReference(database)
    .WithReference(ollama)
    .WaitFor(database);

builder.Build().Run();
