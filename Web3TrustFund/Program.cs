

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureServices();

var app = builder.Build();
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjMxMzkzQDMyMzAyZTMxMmUzMGtiT3piKzNBRXQzM2RsM1VXSjFnR2t1bDh3M3dGaGZkVTRRcmthYVFxbHc9");


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
