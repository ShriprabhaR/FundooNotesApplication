using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface INotesRepository
    {
        public Notes AddNote(int UserId, NoteModel model);

        public List<Notes> GetNotes(int UserId);

        public Notes UpdateNotes(int UserId, int NotesId, UpdateNotesModel model);

        public bool DeleteNotes(int NotesId, int UserId);
    }
}
