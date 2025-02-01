using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Services
{
    public class NoteRepository : INotesRepository
    {
        private readonly FunDooDBContext context;

        public NoteRepository(FunDooDBContext context)
        {
            this.context = context;
        }

        public Notes AddNote(int UserId, NoteModel model)
        {
            Notes notes = new Notes();
            notes.Title = model.Title;
            notes.Description = model.Description;
            notes.UserId = UserId;
            notes.CreatedAt = DateTime.Now;
          
            context.Add(notes);
            context.SaveChanges();
            return notes;

        }

        public List<Notes> GetNotes(int UserId)
        {
            var listOfNotes = context.Notes.Where(x => x.UserId == UserId).ToList();
            return listOfNotes;
        }


        public Notes UpdateNotes(int UserId, int NotesId, UpdateNotesModel model)
        {
            var notes = context.Notes.FirstOrDefault(x => x.UserId == UserId && x.NotesId == NotesId);

            if (notes == null)
            {
                return null;
            }

            notes.Title = model.Title;
            notes.Description = model.Description;
            notes.Reminder = model.Reminder;
            notes.Color = model.Color;
            notes.IsPin = model.IsPin;
            notes.IsArchive = model.IsArchive;
            notes.IsTrash = model.IsTrash;
            notes.UpdatedAt = DateTime.Now;

            context.SaveChanges();

            return notes;
        }

        public bool DeleteNotes(int NotesId, int UserId)
        {
            var notes = context.Notes.FirstOrDefault(x => x.NotesId == NotesId && x.UserId == UserId);
            if (notes == null)
            {
                return false;
            }
            context.Remove(notes);
            context.SaveChanges();
            return true;
        }

        public bool IsPinOrUnpin(int UserId, int NotesId)
        {
            var check = context.Notes.FirstOrDefault(x => x.UserId == UserId && x.NotesId == NotesId);

            if (check != null)
            {
                if (check.IsPin == true)
                {
                    check.IsPin = false;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    check.IsPin = true;
                    context.SaveChanges();
                    return true;
                }
            }
            else
            {
                return false;
            }
            
        }

        public bool IsArchiveOrUnArchive(int UserId, int NotesId)
        {
            var check = context.Notes.FirstOrDefault(x => x.UserId == UserId && x.NotesId == NotesId);

            if (check != null)
            {
                if (check.IsArchive == true)
                {
                    check.IsArchive = false;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    check.IsArchive = true;
                    context.SaveChanges();
                    return true;
                }
            }
            else
            {
                return false;
            }

        }

        public bool IsTrashOrUnTrash(int UserId, int NotesId)
        {
            var check = context.Notes.FirstOrDefault(x => x.UserId == UserId && x.NotesId == NotesId);

            if (check != null)
            {
                if (check.IsTrash == true)
                {
                    check.IsTrash = false;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    check.IsTrash = true;
                    context.SaveChanges();
                    return true;
                }
            }
            else
            {
                return false;
            }

        }



    }
}
