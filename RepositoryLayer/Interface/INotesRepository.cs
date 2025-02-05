using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using Microsoft.AspNetCore.Http;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface INotesRepository
    {
        public Notes AddNote(int UserId, NoteModel model);

        public List<Notes> GetNotes(int UserId);

        public Notes UpdateNotes(int UserId, int NotesId, UpdateNotesModel model);

        public bool DeleteNotes(int NotesId, int UserId);

        public bool IsPinOrUnpin(int UserId, int NotesId);

        public bool IsArchiveOrUnArchive(int UserId, int NotesId);

        public bool IsTrashOrUnTrash(int UserId, int NotesId);

        public Notes DeleteForever(int UserId, int NotesId);

        public Notes SetRemainder(int userId, int notesId, DateTime reminderTime);

        public Notes Color(int UserId, int NotesId, string color);

        public string UploadImage(IFormFile ImagePath, int NotesId, int UserId);
    }
}
