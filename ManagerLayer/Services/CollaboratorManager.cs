using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Models;
using ManagerLayer.Interface;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace ManagerLayer.Services
{
    public class CollaboratorManager :ICollaboratorManager
    {
        private readonly ICollaboratorRepository collab;
        private readonly FunDooDBContext context;

        public CollaboratorManager(ICollaboratorRepository collab, FunDooDBContext context)
        {
            this.collab = collab;
            this.context = context;

        }

        public bool MailExist(string email)
        {
            var checkMail = this.context.Users.FirstOrDefault(x => x.Email == email);

            if (checkMail != null)
            {
                return true;
            }
            return false;
        }

        public Collaborator AddCollaboration(int UserId, CollaboratorModel model)
        {
            return collab.AddCollaboration(UserId, model);
        }
        public List<Collaborator> GetAllCollaborators()
        {
            return collab.GetAllCollaborators();
        }
        public bool DeleteCollaborator(int UserId, int CollaboratorId, int NotesId)
        {
            return collab.DeleteCollaborator(UserId, CollaboratorId, NotesId);
        }
    }
}
