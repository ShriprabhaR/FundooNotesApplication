using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using ManagerLayer.Interface;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace ManagerLayer.Services
{
    public class NoteManager : INoteManager
    {
        private readonly INotesRepository note;
        private readonly FunDooDBContext context;

        public NoteManager(INotesRepository note, FunDooDBContext context)
        {
            this.note = note;
            this.context = context;
        }

        public Notes AddNote(int UserId, NoteModel model)
        {
            return note.AddNote(UserId, model);
        }

        public List<Notes> GetNotes(int UserId)
        {
            return note.GetNotes(UserId);
        }

        public Notes UpdateNotes(int UserId, int NotesId, UpdateNotesModel model)
        {
            return note.UpdateNotes( UserId, NotesId, model);
        }

        public bool DeleteNotes(int NotesId, int UserId)
        {
            return note.DeleteNotes( NotesId, UserId);
        }

    }
}
