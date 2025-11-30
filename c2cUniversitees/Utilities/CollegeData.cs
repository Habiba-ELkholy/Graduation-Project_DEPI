namespace c2cUniversitees.Utilities
{
    public class CollegeData
    {
        // قائمة الكليات التي سيتم استخدامها في الفلاتر ونماذج التسجيل
        public static List<string> CollegeNames = new List<string>
        {
            "فنون",
            "آثار",
            "هندسة",
            "علوم",
            "تجارة",
            "آداب",
            "أخرى / غير محدد"
        };
        private readonly List<string> AllowedDomains = new List<string>
{
    ".edu.", // يغطي معظم الجامعات في مصر وأمريكا (مثل .edu.eg)
    ".ac.",  // يغطي الأكاديميات والمعاهد (مثل .ac.eg)
    ".uni.", // بعض الجامعات تستخدمه
};
    }
}
