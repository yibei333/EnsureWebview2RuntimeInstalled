namespace EnsureWebview2RuntimeInstalled
{
    public static class StringUtil
    {
        public static string CombinePath(this string source, string path)
        {
            if (string.IsNullOrWhiteSpace(source)) return path;
            if (string.IsNullOrWhiteSpace(path)) return source;
            return $"{source.TrimEnd('/').TrimEnd('\\')}/{path.TrimStart('/').TrimStart('\\')}".Replace('\\', '/');
        }
    }
}


