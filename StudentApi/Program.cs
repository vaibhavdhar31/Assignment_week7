using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Models;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StudentContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StudentContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();

app.MapGet("/api/students", async (StudentContext context) =>
{
    try
    {
        return Results.Ok(await context.Students.ToListAsync());
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error retrieving students: {ex.Message}");
    }
});

app.MapPost("/api/students", async (Student student, StudentContext context) =>
{
    try
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(student);
        
        if (!Validator.TryValidateObject(student, validationContext, validationResults, true))
        {
            return Results.BadRequest(validationResults.Select(v => v.ErrorMessage));
        }

        if (string.IsNullOrWhiteSpace(student.Name) || string.IsNullOrWhiteSpace(student.Batch))
        {
            return Results.BadRequest("Name and Batch are required");
        }

        context.Students.Add(student);
        await context.SaveChangesAsync();
        return Results.Created($"/api/students/{student.Rn}", student);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error creating student: {ex.Message}");
    }
});

app.MapPut("/api/students/{rn}", async (int rn, Student student, StudentContext context) =>
{
    try
    {
        var existingStudent = await context.Students.FindAsync(rn);
        if (existingStudent == null) return Results.NotFound($"Student with RN {rn} not found");
        
        if (string.IsNullOrWhiteSpace(student.Name) || string.IsNullOrWhiteSpace(student.Batch))
        {
            return Results.BadRequest("Name and Batch are required");
        }
        
        existingStudent.Name = student.Name;
        existingStudent.Batch = student.Batch;
        existingStudent.Marks = student.Marks;
        
        await context.SaveChangesAsync();
        return Results.Ok(existingStudent);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error updating student: {ex.Message}");
    }
});

app.MapDelete("/api/students/{rn}", async (int rn, StudentContext context) =>
{
    try
    {
        var student = await context.Students.FindAsync(rn);
        if (student == null) return Results.NotFound($"Student with RN {rn} not found");
        
        context.Students.Remove(student);
        await context.SaveChangesAsync();
        return Results.Ok($"Student with RN {rn} deleted successfully");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error deleting student: {ex.Message}");
    }
});

app.Run();
