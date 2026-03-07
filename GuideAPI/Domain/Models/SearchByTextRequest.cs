namespace GuideAPI.Domain.Models
{
    public class SearchByTextRequest
    {
        public string TextQuery { get; set; } // обоязкове поле вводу тексту
        public string? IncludedType { get; set; } // необов'язкове
        public int? MaxResultCount { get; set; } // необов'язкове (1-20)
        public string? LanguageCode { get; set; } // необов'язкове (uk, en...)
        public locationBias? LocationBias { get; set; }
    }

    public class locationBias
    {
        public Circle Circle { get; set; }
    }
}
