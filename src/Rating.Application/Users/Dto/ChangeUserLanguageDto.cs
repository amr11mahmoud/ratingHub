using System.ComponentModel.DataAnnotations;

namespace Rating.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}