using StudentWebApi.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory collection
var students = new List<Student>();

app.MapGet("/students", () => students);

app.MapGet("/students/{rn:int}", (int rn) =>
{
    var s = students.FirstOrDefault(x => x.Rn == rn);
    return s is null ? Results.NotFound() : Results.Ok(s);
});

app.MapPost("/students", (Student s) =>
{
    // Upsert behavior for simplicity
    students.RemoveAll(x => x.Rn == s.Rn);
    students.Add(s);
    return Results.Created($"/students/{s.Rn}", s);
});

app.MapPut("/students/{rn:int}", (int rn, Student update) =>
{
    var s = students.FirstOrDefault(x => x.Rn == rn);
    if (s is null) return Results.NotFound();
    s.Name = update.Name;
    s.Batch = update.Batch;
    s.Marks = update.Marks;
    return Results.NoContent();
});

app.MapDelete("/students/{rn:int}", (int rn) =>
{
    var removed = students.RemoveAll(x => x.Rn == rn) > 0;
    return removed ? Results.NoContent() : Results.NotFound();
});

app.Run();
