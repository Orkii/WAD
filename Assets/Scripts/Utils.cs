



static class Utils{
    /// <summary>
    /// Used to check NULL value after JSON parse
    /// </summary>
    /// 
    public static bool isNull(object a) {
        if (a is string) {
            return ((a as string) == "null");
        }
        return (a == null);
    }
    public static string NULL_STR = "null";
}