using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface ICollaboratorRepository
    {
        public Collaborator AddCollaboration(int UserId, CollaboratorModel model);

        public List<Collaborator> GetAllCollaborators();
        public bool DeleteCollaborator(int UserId, int CollaboratorId, int NotesId);
    }
}
