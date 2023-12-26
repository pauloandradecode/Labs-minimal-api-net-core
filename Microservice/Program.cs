var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Microservice is running!");

app.MapGet("/test", async () => {
    
    try
    {
        var client = new HttpClient();
        var token = new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token;

        var response = await client.GetAsync("https://jsonplaceholder.typicode.com/users", token);
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return Results.Text(content, contentType: "application/json");
        }
        else
        {
            return Results.BadRequest(content);
        }
    }
    catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
    {
        // Handle timeout.
        return Results.BadRequest("Timeout: " + ex.Message);
    }
    catch (TaskCanceledException ex)
    {
        // Handle cancellation.
        return Results.BadRequest("Cancelled: " + ex.Message);
    }
    catch (Exception ex)
    {
        // Handle other exceptions.
        return Results.BadRequest(ex.Message);
    }
    finally
    {
        // Clean up.
    }
});

app.Run();
