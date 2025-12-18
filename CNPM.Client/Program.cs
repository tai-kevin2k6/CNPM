using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CNPM.Client;
using Blazored.LocalStorage; // Thư viện để lưu Token đăng nhập

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Đăng ký dịch vụ LocalStorage để lưu Token
builder.Services.AddBlazoredLocalStorage();

// 2. Cấu hình địa chỉ API Backend
// QUAN TRỌNG: Bạn cần thay đổi số cổng (7000, 7123,...) cho đúng với dự án API của bạn
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5024/")
});

await builder.Build().RunAsync();