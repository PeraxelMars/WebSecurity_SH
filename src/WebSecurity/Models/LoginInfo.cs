using System.ComponentModel.DataAnnotations;
//using SH_WebSecurity.Views.Shared.Resources;
namespace SH_WebSecurity.Models
{
    public class LoginInfo
    {
        [Required]
        //[Display(ResourceType = typeof(SharedResources), Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        //[Display(ResourceType = typeof(SharedResources), Name = "Password")]
        public string Password { get; set; }

        [ScaffoldColumn(false)]
        public string PW_MD5 { get; set; }
    }
}