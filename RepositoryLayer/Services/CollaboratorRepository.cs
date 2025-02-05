using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Models;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Services
{
    public class CollaboratorRepository :ICollaboratorRepository
    {
        private readonly FunDooDBContext context;
        public CollaboratorRepository(FunDooDBContext context) 
        {
            this.context = context;
        }

        public Collaborator AddCollaboration(int UserId, CollaboratorModel model)
        {
            Collaborator collaborator = new Collaborator();
            collaborator.Email = model.Email;
            collaborator.NotesId = model.NotesId;
            collaborator.UserId= UserId;
            context.Collaborators.Add(collaborator);
            context.SaveChanges();

            return collaborator;
        }

        public List<Collaborator> GetAllCollaborators()
        {
            return context.Collaborators.ToList();
        }

        public bool DeleteCollaborator(int UserId, int CollaboratorId, int NotesId)
        {
            try
            {
                var collabs = context.Collaborators.FirstOrDefault(x => x.UserId == UserId && x.CollaboratorId == CollaboratorId && x.NotesId == NotesId );

                if (collabs == null)
                {
                    
                    return false;
                }
                else
                {
                    context.Remove(collabs);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        


    }
}
