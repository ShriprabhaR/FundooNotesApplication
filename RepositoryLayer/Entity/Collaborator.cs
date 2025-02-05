using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace RepositoryLayer.Entity
{
    public class Collaborator
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CollaboratorId {  get; set; }
        public string Email {  get; set; }
        [ForeignKey("CollaboratorNote")]
        public int NotesId {  get; set; }
        [ForeignKey("CollaboratorUser")]
        public int UserId {  get; set; }
        [JsonIgnore]
        public virtual Notes CollaboratorNote { get; set; }
        [JsonIgnore]
        public virtual Users CollaboratorUser { get; set; }

    }
}
