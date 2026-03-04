using Microsoft.EntityFrameworkCore;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Application.Services;
using SchoolSystem.Domain.Interfaces;
using SchoolSystem.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISchoolService, SchoolService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

// MINIMAL API ENDPOINTS

// --- Teachers ---
app.MapGet("/api/teachers", async (ISchoolService service) => await service.GetAllTeachersAsync());
app.MapGet("/api/teachers/{id}", async (int id, ISchoolService service) => 
    await service.GetTeacherByIdAsync(id) is { } t ? Results.Ok(t) : Results.NotFound());
app.MapPost("/api/teachers", async (CreateTeacherDto dto, ISchoolService service) => 
{
    var created = await service.CreateTeacherAsync(dto);
    return Results.Created($"/api/teachers/{created.Id}", created);
});
app.MapPut("/api/teachers/{id}", async (int id, CreateTeacherDto dto, ISchoolService service) => 
{
    await service.UpdateTeacherAsync(id, dto);
    return Results.NoContent();
});
app.MapDelete("/api/teachers/{id}", async (int id, ISchoolService service) => 
{
    await service.DeleteTeacherAsync(id);
    return Results.NoContent();
});

// --- Courses ---
app.MapGet("/api/courses", async (ISchoolService service) => await service.GetAllCoursesAsync());
app.MapGet("/api/courses/{id}", async (int id, ISchoolService service) => 
    await service.GetCourseByIdAsync(id) is { } c ? Results.Ok(c) : Results.NotFound());
app.MapPost("/api/courses", async (CreateCourseDto dto, ISchoolService service) => 
{
    var created = await service.CreateCourseAsync(dto);
    return Results.Created($"/api/courses/{created.Id}", created);
});
app.MapPut("/api/courses/{id}", async (int id, CreateCourseDto dto, ISchoolService service) => 
{
    await service.UpdateCourseAsync(id, dto);
    return Results.NoContent();
});
app.MapDelete("/api/courses/{id}", async (int id, ISchoolService service) => 
{
    await service.DeleteCourseAsync(id);
    return Results.NoContent();
});

// --- Course Instances ---
app.MapGet("/api/courseinstances", async (ISchoolService service) => await service.GetAllCourseInstancesAsync());
app.MapGet("/api/courseinstances/{id}", async (int id, ISchoolService service) => 
    await service.GetCourseInstanceByIdAsync(id) is { } c ? Results.Ok(c) : Results.NotFound());
app.MapPost("/api/courseinstances", async (CreateCourseInstanceDto dto, ISchoolService service) => 
{
    var created = await service.CreateCourseInstanceAsync(dto);
    return Results.Created($"/api/courseinstances/{created.Id}", created);
});
app.MapPut("/api/courseinstances/{id}", async (int id, CreateCourseInstanceDto dto, ISchoolService service) => 
{
    await service.UpdateCourseInstanceAsync(id, dto);
    return Results.NoContent();
});
app.MapDelete("/api/courseinstances/{id}", async (int id, ISchoolService service) => 
{
    await service.DeleteCourseInstanceAsync(id);
    return Results.NoContent();
});

// --- Participants ---
app.MapGet("/api/participants", async (ISchoolService service) => await service.GetAllParticipantsAsync());
app.MapGet("/api/participants/{id}", async (int id, ISchoolService service) => 
    await service.GetParticipantByIdAsync(id) is { } p ? Results.Ok(p) : Results.NotFound());
app.MapPost("/api/participants", async (CreateParticipantDto dto, ISchoolService service) => 
{
    var created = await service.CreateParticipantAsync(dto);
    return Results.Created($"/api/participants/{created.Id}", created);
});
app.MapPut("/api/participants/{id}", async (int id, CreateParticipantDto dto, ISchoolService service) => 
{
    await service.UpdateParticipantAsync(id, dto);
    return Results.NoContent();
});
app.MapDelete("/api/participants/{id}", async (int id, ISchoolService service) => 
{
    await service.DeleteParticipantAsync(id);
    return Results.NoContent();
});

// --- Registrations ---
app.MapGet("/api/registrations", async (ISchoolService service) => await service.GetAllRegistrationsAsync());
app.MapGet("/api/registrations/{id}", async (int id, ISchoolService service) => 
    await service.GetRegistrationByIdAsync(id) is { } r ? Results.Ok(r) : Results.NotFound());
app.MapPost("/api/registrations", async (CreateCourseRegistrationDto dto, ISchoolService service) => 
{
    var created = await service.CreateRegistrationAsync(dto);
    return Results.Created($"/api/registrations/{created.Id}", created);
});
app.MapDelete("/api/registrations/{id}", async (int id, ISchoolService service) => 
{
    await service.DeleteRegistrationAsync(id);
    return Results.NoContent();
});

app.MapGet("/api/registrations/counts", async (ISchoolService service) => await service.GetRegistrationCountsAsync());

app.Run();
