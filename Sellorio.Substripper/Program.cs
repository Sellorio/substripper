using Microsoft.Extensions.DependencyInjection;
using Sellorio.Substripper.Services;

var services = new ServiceCollection();
services.AddSingleton<IAudioInfoService, AudioInfoService>();
services.AddSingleton<ISubtitleInfoService, SubtitleInfoService>();
services.AddSingleton<ISubtitleEditService, SubtitleEditService>();
services.AddSingleton<IHistoryService, HistoryService>();
services.AddSingleton<ISubtitleProcessingService, SubtitleProcessingService>();
services.AddSingleton<IMonitorService, MonitorService>();
var serviceProvider = services.BuildServiceProvider();

var monitoringService = serviceProvider.GetRequiredService<IMonitorService>();

await monitoringService.MonitorAsync();
