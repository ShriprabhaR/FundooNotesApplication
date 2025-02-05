using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entity;

namespace ManagerLayer.Interface
{
    public interface ICollaboratorManager
    {
        public bool MailExist(string email);
        public Collaborator AddCollaboration(int UserId, CollaboratorModel model);
        public List<Collaborator> GetAllCollaborators();
        public bool DeleteCollaborator(int UserId, int CollaboratorId, int NotesId);
    }
}
