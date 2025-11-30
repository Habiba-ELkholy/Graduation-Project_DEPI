namespace c2cUniversitees.Utilities
{
    public class CollegeData
    {
        
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
    ".edu.", 
    ".ac.", 
    ".uni.", 
};
    }
}
