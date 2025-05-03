using System.ComponentModel.DataAnnotations;

namespace mw_cw8_proj.Models.DTOs;

public class CreateClientDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string Pesel { get; set; }
}