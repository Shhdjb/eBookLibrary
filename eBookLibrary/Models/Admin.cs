using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBookLibrary.Models
{
    public class Admin
    {
        [Key] // Specifies that this property is the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Ensures the Id is auto-incremented
        public int Id { get; set; } // Use "Id" to match the database schema

        [Required] // Makes the field mandatory
        [MaxLength(100)] // Sets the maximum length for the username
        public string Username { get; set; }

        [Required] // Makes the field mandatory
        [MaxLength(100)] // Sets the maximum length for the email
        [EmailAddress] // Validates that the email is in a valid email format
        public string Email { get; set; }

        [Required] // Makes the field mandatory
        public string PasswordHash { get; set; }

        [MaxLength(50)] // Sets a maximum length for the role
        public string Role { get; set; }

        [Required] // Ensures CreatedAt is always populated
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] // Automatically sets the value on insert
        public DateTime CreatedAt { get; set; }
    }
}
