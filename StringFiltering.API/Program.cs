using FluentValidation;
using StringFiltering.API.BackgroundServices;
using StringFiltering.API.Middlewares;
using StringFiltering.Application.Factories;
using StringFiltering.Application.Implementations;
using StringFiltering.Application.Services;
using StringFiltering.Application.Utilities;
using StringFiltering.Application.Validators;
using StringFiltering.Infrastructure.Factories;
using StringFiltering.Infrastructure.Utilities.Compression;
using StringFiltering.Infrastructure.Utilities.Filtering;
using StringFiltering.Infrastructure.Utilities.Queueing;
using StringFiltering.Infrastructure.Utilities.Storage;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

#region Registration of validators

builder.Services.AddValidatorsFromAssemblyContaining<UploadRequestValidator>(ServiceLifetime.Singleton);

#endregion

#region Registration of services

builder.Services.AddSingleton<IUploadService, UploadService>();

#endregion

#region Registration of utilities

builder.Services.AddSingleton<IUploadStorage, InMemoryUploadStorage>();
builder.Services.AddSingleton<IBackgroundQueue, BackgroundQueue>();
builder.Services.AddSingleton<IStringArchiver, GZipStringArchiver>();

#endregion

#region Registration of strategies

builder.Services.AddSingleton<IFilteringHelper, LevenshteinFilteringHelper>();
builder.Services.AddSingleton<IFilteringHelper, JaroWinklerFilteringHelper>();

#endregion

#region Registration of factories

builder.Services.AddSingleton<IFilteringHelperFactory, FilteringHelperFactory>();

#endregion

#region Registration of background services

builder.Services.AddHostedService<FilteringService>();

#endregion


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();