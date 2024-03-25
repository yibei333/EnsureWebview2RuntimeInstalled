using EnsureWebview2RuntimeInstalled;

if (RegistryUtil.GetRuntimeIsInstalled()) return;

WindowUtil.CreateWindow();
FileUtil.Download();
FileUtil.Run();